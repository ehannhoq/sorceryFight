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

	}
}
