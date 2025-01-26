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
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class SorceryFight : Mod
	{
		/// <summary>
		/// Converts seconds into buff time.
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns>The number of ticks in a second.</returns>
		public static int BuffSecondsToTicks(float seconds)
		{
			return (int)(seconds * 60);
		}

		/// <summary>
		/// Converts x/second into x/ticks. Usually used for CE regen and CE consumption.
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns>The rate per tick.</returns>
		public static float RateSecondsToTicks(float ticks)
		{
			return ticks / 60;
		}

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
