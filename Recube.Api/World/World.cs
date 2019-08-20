using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using fNbt;
using Recube.Api.Entities.DataStructures;
using Recube.Api.Network.Extensions;

namespace Recube.Api.World
{
	public abstract class World
	{
		//TODO: Rewrite this interface
		public enum Difficulties
		{
			Peaceful = 0,
			Easy = 1,
			Normal = 2,
			Hard = 3
		}

		public enum Dimensions
		{
			End = -1,
			Overworld = 0,
			Nether = 1
		}

		public enum GameTypes
		{
			Survival = 0,
			Creative = 1,
			Adventure = 2,
			Spectator = 3
		}

		public enum Generators
		{
			Default,
			Flat
		}

		public bool AllowCommands;
		public BlockPosition BorderCenter;
		public double BorderDamagePerBlock = 0.2D;
		public double BorderSafeZone = 5D;
		public double BorderSize = 60000000D;
		public double BorderSizeLerpTarget = 60000000D;
		public long BorderSizeLerpTime;
		public double BorderWarningBlocks = 5D;
		public double BorderWarningTime = 15;
		public int ClearWeatherTime;

		//Todo: Change Version
		public int DataVersion = 512;
		public long DayTime;
		public Difficulties Difficulty = Difficulties.Easy;
		public bool DificultyLocked;
		public Dictionary<string, string> GameRules = new Dictionary<string, string>();
		public GameTypes GameType = GameTypes.Survival;
		public Generators Generator = Generators.Default;
		public string GeneratorOptions = "";
		public int GeneratorVersion;
		public bool Hardcore;
		public bool Initialized = true;
		public long LastPlayed;
		public bool MapFeatures = true;
		public bool Raining;
		public int RainTime;
		public long RandomSeed;
		public BlockPosition SpawnPoint = new BlockPosition(0, 0, 0);
		public bool Thundering;
		public int ThunderTime;
		public long Time;

		//TODO: Change Version
		public int Version = 19133;
		public string WorldName;

		// var RegionX = (int) MathF.Floor(ChunkX / 32f);
		// var RegionZ = (int) MathF.Floor(ChunkZ / 32f);
		// var path = "./" + WorldName + "/region/" + $"r.{RegionX}.{RegionZ}.mca";

		protected World(string worldName)
		{
			WorldName = worldName;
			GameRules["announceAdvancements"] = "true";
			GameRules["commandBlockOutput"] = "true";
			GameRules["disableElytraMovementCheck"] = "false";
			GameRules["doDaylightCycle"] = "true";
			GameRules["doEntityDrops"] = "true";
			GameRules["doFireTick"] = "true";
			GameRules["doLimitedCrafting"] = "false";
			GameRules["doMobLoot"] = "true";
			GameRules["doMobSpawning"] = "true";
			GameRules["doTileDrops"] = "true";
			GameRules["doWeatherCycle"] = "true";
			GameRules["keepInventory"] = "false";
			GameRules["logAdminCommands"] = "true";
			GameRules["maxCommandChainLength"] = "65536";
			GameRules["maxEntityCramming"] = "24";
			GameRules["mobGriefing"] = "true";
			GameRules["naturalRegeneration"] = "true";
			GameRules["randomTickSpeed"] = "3";
			GameRules["reducedDebugInfo"] = "false";
			GameRules["sendCommandFeedback"] = "true";
			GameRules["showDeathMessages"] = "true";
			GameRules["spawnRadius"] = "10";
			GameRules["spectatorsGenerateChunks"] = "true";
		}

		public void SaveLevelData()
		{
			Directory.CreateDirectory("./" + WorldName);
			var levelData = new NbtFile();
			var data = new NbtCompound("Data");

			var gameRulesData = new NbtCompound("GameRules");
			foreach (var rule in GameRules)
			{
				gameRulesData.AddString(rule.Key, rule.Value);
			}

			data.AddNbtCompound(gameRulesData);

			//TODO: Change Version
			data.AddNbtCompound(new NbtCompound("Version", new List<NbtTag>
			{
				new NbtInt("Id", 1631),
				new NbtString("Name", "1.13.2"),
				new NbtByte("Snapshot", 0)
			}));
			data.AddByte("allowCommands", AllowCommands ? (byte) 1 : (byte) 0)
				.AddDouble("BorderCenterX", BorderCenter.X)
				.AddDouble("BorderCenterZ", BorderCenter.Z)
				.AddDouble("BorderDamagePerBlock", BorderDamagePerBlock)
				.AddDouble("BorderSafeZone", BorderSafeZone)
				.AddDouble("BorderSize", BorderSize)
				.AddDouble("BorderSizeLerpTarget", BorderSizeLerpTarget)
				.AddLong("BorderSizeLerpTime", BorderSizeLerpTime)
				.AddDouble("BorderWarningBlocks", BorderWarningBlocks)
				.AddDouble("BorderWarningTime", BorderWarningTime)
				.AddInt("clearWeatherTime", ClearWeatherTime)
				.AddInt("DataVersion", DataVersion)
				.AddLong("DayTime", DayTime)
				.AddByte("Difficulty", (byte) Difficulty)
				.AddByte("DifficultyLocked", DificultyLocked ? (byte) 1 : (byte) 0)
				.AddInt("GameType", (byte) GameType)
				.AddString("generatorName", Generator.ToString().ToLower())
				.AddString("generatorOptions", GeneratorOptions)
				.AddInt("generatorVersion", GeneratorVersion)
				.AddByte("hardcore", Hardcore ? (byte) 1 : (byte) 0)
				.AddByte("initialized", Initialized ? (byte) 1 : (byte) 0)
				.AddLong("LastPlayed", LastPlayed)
				.AddString("LevelName", WorldName)
				.AddByte("MapFeatures", MapFeatures ? (byte) 1 : (byte) 0)
				.AddByte("raining", Raining ? (byte) 1 : (byte) 0)
				.AddInt("rainTime", RainTime)
				.AddLong("RandomSeed", RandomSeed)
				.AddLong("SizeOnDisk", 0)
				.AddInt("SpawnX", SpawnPoint.X)
				.AddInt("SpawnY", SpawnPoint.Y)
				.AddInt("SpawnZ", SpawnPoint.Z)
				.AddByte("thundering", Thundering ? (byte) 1 : (byte) 0)
				.AddInt("thunderTime", ThunderTime)
				.AddLong("Time", Time)
				.AddInt("version", Version);
			levelData.RootTag["Data"] = data;
			levelData.SaveToFile("./" + WorldName + "/level.dat", NbtCompression.None);
		}

		public void LoadLevelData()
		{
			var levelData = new NbtFile();
			levelData.LoadFromFile("./" + WorldName + "/level.dat");
			var data = levelData.RootTag.GetNbtCompound("Data");
			if (data == null)
				throw new NoNullAllowedException("Data is null while trying to load world");
			GameRules.Clear();
			foreach (var nbtTag in data.GetNbtCompound("GameRules"))
			{
				GameRules.Add(nbtTag.Name, nbtTag.StringValue);
			}

			AllowCommands = data.GetBoolean("allowCommands");
			BorderCenter = new BlockPosition((int) data.GetDouble("BorderCenterX"), 0,
				(int) data.GetDouble("BorderCenterZ"));
			BorderDamagePerBlock = data.GetDouble("BorderDamagePerBlock");
			BorderSafeZone = data.GetDouble("BorderSafeZone");
			BorderSize = data.GetDouble("BorderSize");
			BorderSizeLerpTarget = data.GetDouble("BorderSizeLerpTarget");
			BorderSizeLerpTime = data.GetLong("BorderSizeLerpTime");
			BorderWarningBlocks = data.GetDouble("BorderWarningBlocks");
			BorderWarningTime = data.GetDouble("BorderWarningTime");
			ClearWeatherTime = data.GetInt("clearWeatherTime");
			DataVersion = data.GetInt("DataVersion");
			DayTime = data.GetLong("DayTime");
			Difficulty = (Difficulties) data.GetByte("Difficulty");
			DificultyLocked = data.GetBoolean("DifficultyLocked");
			GameType = (GameTypes) data.GetInt("GameType");
			Generators nbtGenerator;
			Enum.TryParse(data.GetString("generatorName"), true, out nbtGenerator);
			Generator = nbtGenerator;
			GeneratorOptions = data.GetString("generatorOptions");
			GeneratorVersion = data.GetInt("generatorVersion");
			Hardcore = data.GetBoolean("hardcore");
			Initialized = data.GetBoolean("initialized");
			LastPlayed = data.GetLong("LastPlayed");
			WorldName = data.GetString("LevelName");
			MapFeatures = data.GetBoolean("MapFeatures");
			Raining = data.GetBoolean("raining");
			RainTime = data.GetInt("rainTime");
			RandomSeed = data.GetLong("RandomSeed");
			SpawnPoint = new BlockPosition(data.GetInt("SpawnX"), data.GetInt("SpawnY"), data.GetInt("SpawnZ"));
			Thundering = data.GetBoolean("thundering");
			ThunderTime = data.GetInt("thunderTime");
			Time = data.GetLong("Time");
			Version = data.GetInt("version");
		}
	}
}