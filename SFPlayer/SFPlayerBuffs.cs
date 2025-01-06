using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool infinity;
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (Player == Main.LocalPlayer && infinity)
            {
                return true;
            }

            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }
    }
}
