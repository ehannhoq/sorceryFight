using Microsoft.Xna.Framework;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.InnateTechniques;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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

			int caster = reader.ReadInt32();
			SorceryFightPlayer sfPlayer = Main.player[caster].GetModPlayer<SorceryFightPlayer>();

			InnateTechniqueType innateTechniqueType = (InnateTechniqueType)reader.ReadByte();
			sfPlayer.innateTechnique = InnateTechniqueFactory.Create(innateTechniqueType);

			DomainNetMessage msg = (DomainNetMessage)reader.ReadByte();


			if (Main.netMode == NetmodeID.Server)
			{
				switch (msg)
				{
					case DomainNetMessage.ExpandDomain:
						Logger.Info($"Removing domain from {caster} on client {Main.myPlayer}");
						DomainExpansionController.ActiveDomains[caster].CloseDomain(sfPlayer, true);
						break;

					case DomainNetMessage.CloseDomain:
						Logger.Info($"Activating domain for {caster} on client {Main.myPlayer}");
						DomainExpansionController.ActivateDomain(sfPlayer);
						break;
				}

				ModPacket packet = GetPacket();
				packet.Write((byte)MessageType.SyncDomain);

				packet.Write(caster);
				packet.Write((byte)innateTechniqueType);
				packet.Write((byte)msg);
				packet.Send(-1, caster);
			}


			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				switch (msg)
				{
					case DomainNetMessage.ExpandDomain:
						Logger.Info($"Removing {sfPlayer.innateTechnique.DomainExpansion.DisplayName} from {caster} on client {Main.myPlayer}");
						DomainExpansionController.ActiveDomains[caster].CloseDomain(sfPlayer, true);
						break;

					case DomainNetMessage.CloseDomain:
						Logger.Info($"Activating {sfPlayer.innateTechnique.DomainExpansion.DisplayName} for {caster} on client {Main.myPlayer}");
						DomainExpansionController.ActivateDomain(sfPlayer);
						break;
				}
			}
		}
	}
}
