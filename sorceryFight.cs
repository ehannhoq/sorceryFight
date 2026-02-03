using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.NPCDomains;
using sorceryFight.SFPlayer;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static sorceryFight.Content.DomainExpansions.DomainExpansionController;

namespace sorceryFight
{
	public enum MessageType : byte
	{
		DefeatedBoss,
		SyncDomain,
		PlayerCastingDomain,
		KilledNPC
	}
	public class SorceryFight : Mod
	{
		/// <summary>
		/// The total number of bosses loaded across all mods.
		/// </summary>
		public static int totalBosses;

		/// <summary>
		/// A reflection method allowing to retrieve ModContent.ProjectileTypes at runtime.
		/// </summary>
		public static MethodInfo ModContentProjectileType;

		public static NPC strongestBoss;

		private static readonly int vanillaBossesCount = 32;
		/// <summary>
		/// Whether or not the player's name is a developer name (grants developer powers).
		/// </summary>
		/// <returns></returns>
		public static bool IsDevMode()
		{
			List<string> devModeNames =
			[
				"The Honored One",
				"ehann",
				"gooloohoodoo",
				"gooloohoodoo1",
				"gooloohoodoo2",
				"gooloohoodoo3",
				"gooloohoodoo4",
				"gooloohoodoo5",
				"gooloohoodoo6",
				"gooloohoodoo7",
				"TheRealCriky",
				"prowler",
				"rend",
				"KaiTheExaminer",
				"Ryomen"
			];
			return devModeNames.Contains(Main.LocalPlayer.name);
		}
		public override void PostSetupContent()
		{
			ModContentProjectileType = typeof(ModContent).GetMethod("ProjectileType");
			CountBosses();
		}

		private void CountBosses()
		{
			totalBosses = vanillaBossesCount;

			foreach (ModNPC npc in ModContent.GetContent<ModNPC>())
			{
				var sample = ContentSamples.NpcsByNetId[npc.Type];
				if (sample.boss && !sample.dontCountMe)
				{
					totalBosses++;
				}
			}
		}

		private void DetermineStrongestBoss()
		{
			List<NPC> bosses = new();

			for (short i = 0; i < NPCID.Count; i++)
			{
				NPC npc = ContentSamples.NpcsByNetId[i];
				if (!npc.boss) continue;

				bosses.Add(npc);
			}

			foreach (ModNPC modNPC in ModContent.GetContent<ModNPC>())
			{
				NPC npc = ContentSamples.NpcsByNetId[modNPC.Type];
				if (!npc.boss) continue;

				bosses.Add(npc);
			}

			float largestDistance = 0;

			foreach (NPC boss in bosses)
			{
				float health = boss.lifeMax;
				float damage = boss.damage;

				float distance = new Vector2(health, damage).Length();
				if (distance > largestDistance)
				{
					largestDistance = distance;
					strongestBoss = boss;
				}
			}

			Logger.Debug($"Strongest Boss: {strongestBoss.FullName}");
		}

		public override void Unload()
		{
			totalBosses = 0;
			ModContentProjectileType = null;
		}
		public override void HandlePacket(BinaryReader reader, int _)
		{
			byte messageType = reader.ReadByte();
			switch (messageType)
			{
				case (byte)MessageType.DefeatedBoss:
					HandleBossDefeatedPacket(reader);
					break;

				case (byte)MessageType.SyncDomain:
					HandleDomainSyncingPacket(reader);
					break;

				case (byte)MessageType.PlayerCastingDomain:
					HandlePlayerCastingDomainPacket(reader);
					break;

				case (byte)MessageType.KilledNPC:
					HandleKilledNPCPacket(reader);
					break;
			}
		}
		private void HandleBossDefeatedPacket(BinaryReader reader)
		{
			int targetPlayer = reader.ReadInt32();
			int bossType = reader.ReadInt32();

			if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == targetPlayer)
			{
				Main.player[targetPlayer].SorceryFight().AddDefeatedBoss(bossType);
			}
		}
		private void HandleDomainSyncingPacket(BinaryReader reader)
		{
			int whoAmI = reader.ReadInt32();
			DomainNetMessage msg = (DomainNetMessage)reader.ReadByte();

			DomainExpansionFactory.DomainExpansionType domainType = (DomainExpansionFactory.DomainExpansionType)reader.ReadByte();
			DomainExpansion de = DomainExpansionFactory.Create(domainType);

			int id = reader.ReadInt32();
			int clashingWith = reader.ReadInt32();

			switch (msg)
			{
				case DomainNetMessage.ExpandDomain:
					ExpandDomain(whoAmI, de);
					break;

				case DomainNetMessage.CloseDomain:
					CloseDomain(id);
					break;

				case DomainNetMessage.ClashingDomains:
					DomainExpansions[id].clashingWith = clashingWith;
					DomainExpansions[clashingWith].clashingWith = id;
					break;


			}

			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = GetPacket();

				packet.Write((byte)MessageType.SyncDomain);
				packet.Write(whoAmI);
				packet.Write((byte)msg);
				packet.Write((byte)domainType);
				packet.Write(id);
				packet.Write(clashingWith);
				packet.Send(-1, whoAmI);
			}
		}
		private void HandlePlayerCastingDomainPacket(BinaryReader reader)
		{
			int sentFrom = reader.ReadInt32();

			NPCDomainController.playerCastedDomain = true;

			if (Main.netMode == NetmodeID.Server)
			{
				ModPacket packet = GetPacket();
				packet.Write((byte)MessageType.PlayerCastingDomain);
				packet.Send(-1, sentFrom);
			}
		}
		private void HandleKilledNPCPacket(BinaryReader reader)
		{
			int targetPlayer = reader.ReadInt32();
			int npcType = reader.ReadInt32();

			if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == targetPlayer)
			{
				Main.player[targetPlayer].SorceryFight().OnKilledNPC(npcType);
			}
		}
	}
}
