using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool infinity;
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (Player == Main.LocalPlayer && infinity)
                return true;

            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }

        public override void OnHurt(Player.HurtInfo info)
        {
            if (Player == Main.LocalPlayer && infinity) // Handles players hit by PvP swords - no clue if this works
            {
                cursedEnergy -= 3 * info.Damage / (float)Math.Sqrt(info.Damage + 1);
            }

            base.OnHurt(info);
        }
    }
}
