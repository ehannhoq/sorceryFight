using Microsoft.Build.Tasks;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.NPCDomains;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.SFPlayer;
using System.Collections.Generic;
using System.IO;
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
		PlayerCastingDomain
	}
	public class SorceryFight : Mod
	{
		private static List<string> DevModeNames =
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
			"Perseus",
			"TheRealCriky"
		];

		public static bool IsDevMode()
		{
			return DevModeNames.Contains(Main.LocalPlayer.name);
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
			}
		}

		private void HandleBossDefeatedPacket(BinaryReader reader)
		{
			int targetPlayer = reader.ReadInt32();
			int bossType = reader.ReadInt32();

			if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == targetPlayer)
			{
				Main.player[targetPlayer].GetModPlayer<SorceryFightPlayer>().AddDefeatedBoss(bossType);
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
	}
}
