using System;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool celestialAmulet;
        private void PreAccessoryUpdate()
        {
            celestialAmulet = false;
        }
    }
}