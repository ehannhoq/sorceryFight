using System;
using Terraria.ModLoader;

namespace sorceryFight.Content.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool celestialAmulet;

        public float burntDurationReduction;

        public override void UpdateEquips()
        {
            if (celestialAmulet)
            {
                burntDurationReduction += defaultBurntTechniqueDuration * 0.10f;
            }
        }

        private void ResetAccessories()
        {
            celestialAmulet = false;
            burntDurationReduction = 0f;
        }
    }
}