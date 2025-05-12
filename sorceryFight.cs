using sorceryFight.Content.DomainExpansions;
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
		SyncDomain
	}
	public class SorceryFight : Mod
	{
		public static List<string> DevModeNames =
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

		public override void HandlePacket(BinaryReader reader, int _)
		{
			byte messageType = reader.ReadByte();
			switch (messageType)
			{
				case 0:
					HandleBossDefeatedPacket(reader);
					break;

				case 1:
					HandleDomainSyncingPacket(reader);
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

			DomainNetMessage msg;

			DomainExpansionFactory.DomainExpansionType playerDomainType = (DomainExpansionFactory.DomainExpansionType)reader.ReadByte();
			DomainExpansion playerDomain = DomainExpansionFactory.Create(playerDomainType);

			msg = (DomainNetMessage)reader.ReadByte();
			if (Main.netMode == NetmodeID.Server)
			{
				switch (msg)
				{
					case DomainNetMessage.ExpandDomain:
						Logger.Info($"Activating domain for player {whoAmI} on client {Main.myPlayer}");
						ExpandDomain(whoAmI, playerDomain);
						break;

					case DomainNetMessage.CloseDomain:
						Logger.Info($"Removing domain from player {whoAmI} on client {Main.myPlayer}");
						CloseDomain(whoAmI);
						break;
				}

				ModPacket packet = ModContent.GetInstance<SorceryFight>().GetPacket();
				packet.Write((byte)MessageType.SyncDomain);
				packet.Write(whoAmI);
				packet.Write((byte)DomainExpansionFactory.GetDomainExpansionType(playerDomain));
				packet.Write((byte)msg);
				packet.Send(-1, whoAmI);
			}

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				switch (msg)
				{
					case DomainNetMessage.ExpandDomain:
						Logger.Info($"Activating domain for player {whoAmI} on client {Main.myPlayer}");
						ExpandDomain(whoAmI, playerDomain);
						break;

					case DomainNetMessage.CloseDomain:
						Logger.Info($"Removing domain from player {whoAmI} on client {Main.myPlayer}");
						CloseDomain(whoAmI);
						break;
				}
			}

		}
	}
}
