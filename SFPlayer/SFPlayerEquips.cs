using System;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool celestialAmulet;
        public bool pictureLocket;
        public bool cursedOfuda;

        public override void ResetEffects()
        {
            celestialAmulet = false;
            pictureLocket = false;
            cursedOfuda = false;
        }
    }
}