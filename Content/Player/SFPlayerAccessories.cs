using System;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool celestialAmulet;

        public float percentBurntTechnqiueReduction;

        public override void PreUpdate()
        {
            if (celestialAmulet)
            {
                percentBurntTechnqiueReduction += 0.2f;
            }
        }

        private void ResetAccessories()
        {
            celestialAmulet = false;
        }
    }
}