using System.Collections.Generic;
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

		private readonly bool allowCommands = false;
		private readonly double BorderDamagePerBlock = 0.2D;
		private readonly double BorderSafeZone = 5D;
		private readonly double BorderSize = 60000000D;
		private readonly double BorderSizeLerpTarget = 60000000D;
		private readonly long BorderSizeLerpTime = 0L;
		private readonly double BorderWarningBlocks = 5D;
		private readonly double BorderWarningTime = 15;

		private readonly int ClearWeatherTime = 0;

		//Todo: Change Version
		private readonly int DataVersion = 512;
		private readonly int DayTime = 0;
		private readonly Difficulties Difficulty = Difficulties.Easy;
		private readonly bool DificultyLocked = false;

		private readonly Dictionary<string, string> GameRules = new Dictionary<string, string>();
		private readonly GameTypes GameType = GameTypes.Survival;
		private readonly Generators Generator = Generators.Default;
		private readonly string GeneratorOptions = "";
		private readonly bool Hardcore = false;
		private readonly bool initialized = true;
		private readonly bool MapFeatures = true;
		private readonly bool Raining = false;
		private readonly int RainTime = 0;
		private readonly BlockPosition SpawnPoint = new BlockPosition(0, 0, 0);
		private readonly bool Thundering = false;
		private readonly int ThunderTime = 0;

		//TODO: Change Version
		private readonly int Version = 19133;
		private readonly string WorldName;
		private BlockPosition BorderCenter;
		private int GeneratorVersion;
		private long LastPlayed;
		private long RandomSeed;

		private long Time;


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
	}
}