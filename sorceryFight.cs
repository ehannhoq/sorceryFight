using Microsoft.Xna.Framework;
using sorceryFight.Content.DomainExpansions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class SorceryFight : Mod
	{
		/// <summary>
		/// Converts given seconds into ticks.
		/// </summary>
		/// <param name="seconds"></param>
		/// <returns>The number of ticks in a given amount of seconds.</returns>
        public static int SecondsToTicks(float seconds)
		{
			return (int)(seconds * 60);
		}

		/// <summary>
		/// Converts given ticks into seconds.
		/// </summary>
		/// <param name="ticks"></param>
		/// <returns>The number of seconds in a given amount of ticks.</returns>
		public static int TicksToSeconds(float ticks)
		{
			return (int)ticks / 60;
		}
	}
}
