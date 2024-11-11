using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight
{
	// Please read https://github.com/tModLoader/tModLoader/wiki/Basic-tModLoader-Modding-Guide#mod-skeleton-contents for more information about the various files in a mod.
	public class SorceryFight : Mod
	{

		public static float SecondsToTicks(float seconds)
		{
			return seconds * 60;
		}

		public static float TicksToSeconds(float ticks)
		{
			return ticks / 60;
		}

     
    }
}
