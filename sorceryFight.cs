using Microsoft.Xna.Framework;
using sorceryFight.Content.DomainExpansions;
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

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			byte messageType = reader.ReadByte();
			switch (messageType)
			{
				case 1:
					int targetPlayer = reader.ReadInt32();
					int bossType = reader.ReadInt32();

					if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == targetPlayer)
					{
						Main.player[targetPlayer].GetModPlayer<SorceryFightPlayer>().AddDefeatedBoss(bossType);
					}
					break;
			}
		}
	}
}
