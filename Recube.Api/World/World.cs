using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using fNbt;
using Recube.Api.Entities.DataStructures;
using Recube.Api.Network.Extensions;

namespace Recube.Api.World
{
	public class World
	{
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
		public GameDifficulty Difficulty = GameDifficulty.Easy;
		public bool DifficultyLocked;
		public Dictionary<string, string> GameRules = new Dictionary<string, string>();
		public GameType GameType = GameType.Survival;
		public WorldGenerator Generator = WorldGenerator.Default;
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
				.AddByte("DifficultyLocked", DifficultyLocked ? (byte) 1 : (byte) 0)
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

		/// <summary>
		/// 	Loads a world from it's level.dat file
		/// </summary>
		/// <exception cref="WorldParseException">When a required NBT tag is missing</exception>
		public void LoadLevelData()
		{
			var levelData = new NbtFile();
			levelData.LoadFromFile("./" + WorldName + "/level.dat");
			var data = levelData.RootTag.GetNbtCompound("Data");
			if (data == null)
				throw new WorldParseException("Data is null while trying to load world");
			GameRules.Clear();
			foreach (var nbtTag in data.GetNbtCompound("GameRules"))
			{
				GameRules.Add(nbtTag.Name ?? throw new WorldParseException("GameRule tag name is null"),
					nbtTag.StringValue);
			}

			AllowCommands = data.GetBoolean("allowCommands") ?? throw new WorldParseException("allowCommands is null");
			// BORDER CENTER
			var borderCenterX = data.GetInt("BorderCenterX");
			if (!borderCenterX.HasValue) throw new WorldParseException("BorderCenterX is null");

			var borderCenterZ = data.GetInt("BorderCenterZ");
			if (!borderCenterZ.HasValue) throw new WorldParseException("BorderCenterZ is null");

			BorderCenter = new BlockPosition(borderCenterX.Value, 0, borderCenterZ.Value);
			////
			BorderDamagePerBlock = data.GetDouble("BorderDamagePerBlock") ??
			                       throw new WorldParseException("BorderDamagePerBlock is null");
			BorderSafeZone = data.GetDouble("BorderSafeZone") ??
			                 throw new WorldParseException("BorderSafeZone is null");
			BorderSize = data.GetDouble("BorderSize") ?? throw new WorldParseException("BorderSize is null");
			BorderSizeLerpTarget = data.GetDouble("BorderSizeLerpTarget") ??
			                       throw new WorldParseException("BorderSizeLerpTarget is null");
			BorderSizeLerpTime = data.GetLong("BorderSizeLerpTime") ??
			                     throw new WorldParseException("BorderSizeLerpTime is null");
			BorderWarningBlocks = data.GetDouble("BorderWarningBlocks") ??
			                      throw new WorldParseException("BorderWarningBlocks is null");
			BorderWarningTime = data.GetDouble("BorderWarningTime") ??
			                    throw new WorldParseException("BorderWarningTime is null");
			ClearWeatherTime = data.GetInt("clearWeatherTime") ??
			                   throw new WorldParseException("clearWeatherTime is null");
			DataVersion = data.GetInt("DataVersion") ?? throw new WorldParseException("DataVersion is null");
			DayTime = data.GetLong("DayTime") ?? throw new WorldParseException("DayTime is null");
			Difficulty =
				(GameDifficulty) (data.GetByte("Difficulty") ?? throw new WorldParseException("Difficulty is null"));
			DifficultyLocked = data.GetBoolean("DifficultyLocked") ??
			                   throw new WorldParseException("DifficultyLocked is null");
			// GAME TYPE
			var gameType = data.GetInt("GameType") ?? throw new WorldParseException("GameType is null");
			GameType = (GameType) gameType;
			var generatorName = data.GetString("generatorName") ??
			                    throw new WorldParseException("generatorName is null");
			Enum.TryParse(generatorName, true, out WorldGenerator nbtGenerator);
			////
			Generator = nbtGenerator;
			GeneratorOptions = data.GetString("generatorOptions") ??
			                   throw new WorldParseException("generatorOptions is null");
			GeneratorVersion = data.GetInt("generatorVersion") ??
			                   throw new WorldParseException("generatorVersion is null");
			Hardcore = data.GetBoolean("hardcore") ?? throw new WorldParseException("hardcore is null");
			Initialized = data.GetBoolean("initialized") ?? throw new WorldParseException("initialized is null");
			LastPlayed = data.GetLong("LastPlayed") ?? throw new WorldParseException("LastPlayed is null");
			WorldName = data.GetString("LevelName") ?? throw new WorldParseException("LevelName is null");
			MapFeatures = data.GetBoolean("MapFeatures") ?? throw new WorldParseException("MapFeatures is null");
			Raining = data.GetBoolean("raining") ?? throw new WorldParseException("raining is null");
			RainTime = data.GetInt("rainTime") ?? throw new WorldParseException("rainTime is null");
			RandomSeed = data.GetLong("RandomSeed") ?? throw new WorldParseException("RandomSeed is null");
			// SPAWN POSITION
			var spawnX = data.GetInt("SpawnX") ?? throw new WorldParseException("SpawnX is null");
			var spawnY = data.GetInt("SpawnY") ?? throw new WorldParseException("SpawnY is null");
			var spawnZ = data.GetInt("SpawnZ") ?? throw new WorldParseException("SpawnZ is null");
			SpawnPoint = new BlockPosition(spawnX, spawnY, spawnZ);
			/////
			Thundering = data.GetBoolean("thundering") ?? throw new WorldParseException("thundering is null");
			ThunderTime = data.GetInt("thunderTime") ?? throw new WorldParseException("thunderTime is null");
			Time = data.GetLong("Time") ?? throw new WorldParseException("Time is null");
			Version = data.GetInt("version") ?? throw new WorldParseException("version is null");
		}
	}
}