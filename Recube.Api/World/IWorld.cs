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

		public enum Generators
		{
			Default,
			Flat
		}

		public readonly Dimensions Dimension;
		public readonly Generators Generator;
		public readonly string worldName;
		public int BorderSize = 60000000;
		public float DayTime;

		public Difficulties Difficulty;
		public Dictionary<string, string> GameRules = new Dictionary<string, string>();
		public long Seed;
		public BlockPosition SpawnBlockPosition = new BlockPosition(0, 0, 0);

		protected IWorld(string worldName)
		{
			this.worldName = worldName;
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
			Directory.CreateDirectory("./" + worldName);
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
			Data.AddByte("allowCommands", 0)
				.AddDouble("BorderCenterX", 0)
				.AddDouble("BorderCenterZ", 0)
				.AddDouble("BorderDamagePerBlock", 0.2D)
				.AddDouble("BorderSafeZone", 5)
				.AddDouble("BorderSize", 60000000)
				.AddDouble("BorderSizeLerpTarget", 60000000)
				.AddLong("BorderSizeLerpTime", 0)
				.AddDouble("BorderWarningBlocks", 5)
				.AddDouble("BorderWarningTime", 15)
				.AddInt("clearWeatherTime", 0)
				.AddInt("DataVersion", 1631)
				.AddLong("DayTime", 24570)
				.AddByte("Difficulty", (byte) Difficulty)
				.AddByte("DifficultyLocked", 0)
				.AddInt("GameType", 0)
				.AddString("generatorName", Generator.ToString().ToLower())
				.AddInt("generatorVersion", 1)
				.AddByte("hardcore", 0)
				.AddByte("initialized", 1)
				.AddLong("LastPlayed", 0)
				.AddString("LevelName", worldName)
				.AddByte("MapFeatures", 1)
				.AddByte("raining", 0)
				.AddInt("rainTime", 0)
				.AddLong("RandomSeed", Seed)
				.AddLong("SizeOnDisk", 0)
				.AddInt("SpawnX", SpawnBlockPosition.x)
				.AddInt("SpawnY", SpawnBlockPosition.y)
				.AddInt("SpawnZ", SpawnBlockPosition.z)
				.AddByte("thundering", 0)
				.AddInt("thunderTime", 44821)
				.AddLong("Time", 24570)
				.AddInt("version", 19133);
			levelData.RootTag["Data"] = Data;
			levelData.SaveToFile("./" + worldName + "/level.dat", NbtCompression.None);
		}
	}
}