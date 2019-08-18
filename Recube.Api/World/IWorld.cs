using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using fNbt;
using Recube.Api.Entities.DataStructures;
using Recube.Api.Network.Extensions;

namespace Recube.Api.World
{
	public abstract class IWorld
	{
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

		public bool allowCommands;
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
		public bool initialized = true;
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


		protected IWorld(string worldName)
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

		public void SaveWorld()
		{
			Directory.CreateDirectory("./" + WorldName);
			var levelData = new NbtFile();
			var Data = new NbtCompound("Data");

			var GameRulesData = new NbtCompound("GameRules");
			foreach (var rule in GameRules)
			{
				GameRulesData.AddString(rule.Key, rule.Value);
			}

			Data.AddNbtCompound(GameRulesData);

			//TODO: Change Version
			Data.AddNbtCompound(new NbtCompound("Version", new List<NbtTag>
			{
				new NbtInt("Id", 1631),
				new NbtString("Name", "1.13.2"),
				new NbtByte("Snapshot", 0)
			}));
			Data.AddByte("allowCommands", allowCommands ? (byte) 1 : (byte) 0)
				.AddDouble("BorderCenterX", BorderCenter.x)
				.AddDouble("BorderCenterZ", BorderCenter.z)
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
				.AddByte("initialized", initialized ? (byte) 1 : (byte) 0)
				.AddLong("LastPlayed", LastPlayed)
				.AddString("LevelName", WorldName)
				.AddByte("MapFeatures", MapFeatures ? (byte) 1 : (byte) 0)
				.AddByte("raining", Raining ? (byte) 1 : (byte) 0)
				.AddInt("rainTime", RainTime)
				.AddLong("RandomSeed", RandomSeed)
				.AddLong("SizeOnDisk", 0)
				.AddInt("SpawnX", SpawnPoint.x)
				.AddInt("SpawnY", SpawnPoint.y)
				.AddInt("SpawnZ", SpawnPoint.z)
				.AddByte("thundering", Thundering ? (byte) 1 : (byte) 0)
				.AddInt("thunderTime", ThunderTime)
				.AddLong("Time", Time)
				.AddInt("version", Version);
			levelData.RootTag["Data"] = Data;
			levelData.SaveToFile("./" + WorldName + "/level.dat", NbtCompression.None);
		}

		public void LoadWorld()
		{
			var levelData = new NbtFile();
			levelData.LoadFromFile("./" + WorldName + "/level.dat");
			var Data = levelData.RootTag.GetNbtCompound("Data");
			if (Data == null)
				throw new NoNullAllowedException("Data is null while trying to load world");
			GameRules.Clear();
			foreach (var nbtTag in Data.GetNbtCompound("GameRules"))
			{
				GameRules.Add(nbtTag.Name, nbtTag.StringValue);
			}

			allowCommands = Data.GetBoolean("allowCommands");
			BorderCenter = new BlockPosition((int) Data.GetDouble("BorderCenterX"), 0,
				(int) Data.GetDouble("BorderCenterZ"));
			BorderDamagePerBlock = Data.GetDouble("BorderDamagePerBlock");
			BorderSafeZone = Data.GetDouble("BorderSafeZone");
			BorderSize = Data.GetDouble("BorderSize");
			BorderSizeLerpTarget = Data.GetDouble("BorderSizeLerpTarget");
			BorderSizeLerpTime = Data.GetLong("BorderSizeLerpTime");
			BorderWarningBlocks = Data.GetDouble("BorderWarningBlocks");
			BorderWarningTime = Data.GetDouble("BorderWarningTime");
			ClearWeatherTime = Data.GetInt("clearWeatherTime");
			DataVersion = Data.GetInt("DataVersion");
			DayTime = Data.GetLong("DayTime");
			Difficulty = (Difficulties) Data.GetByte("Difficulty");
			DificultyLocked = Data.GetBoolean("DifficultyLocked");
			GameType = (GameTypes) Data.GetInt("GameType");
			Generators nbtGenerator;
			Enum.TryParse(Data.GetString("generatorName"), true, out nbtGenerator);
			Generator = nbtGenerator;
			GeneratorOptions = Data.GetString("generatorOptions");
			GeneratorVersion = Data.GetInt("generatorVersion");
			Hardcore = Data.GetBoolean("hardcore");
			initialized = Data.GetBoolean("initialized");
			LastPlayed = Data.GetLong("LastPlayed");
			WorldName = Data.GetString("LevelName");
			MapFeatures = Data.GetBoolean("MapFeatures");
			Raining = Data.GetBoolean("raining");
			RainTime = Data.GetInt("rainTime");
			RandomSeed = Data.GetLong("RandomSeed");
			SpawnPoint = new BlockPosition(Data.GetInt("SpawnX"), Data.GetInt("SpawnY"), Data.GetInt("SpawnZ"));
			Thundering = Data.GetBoolean("thundering");
			ThunderTime = Data.GetInt("thunderTime");
			Time = Data.GetLong("Time");
			Version = Data.GetInt("version");
		}
	}
}