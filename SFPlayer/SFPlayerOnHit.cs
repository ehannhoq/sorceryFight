using System;
using CalamityMod;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (blackFlashTimeLeft >= 60 && blackFlashTime <= 120) // 20 frame interval (1/3 second)
            {
                if (hit.DamageType != DamageClass.Magic && hit.DamageType != DamageClass.Melee && hit.DamageType != DamageClass.Ranged && 
                    hit.DamageType != DamageClass.Summon && hit.DamageType != RogueDamageClass.Throwing) return; // Ignore if damage done by a Cursed Technique.

                blackFlashTimeLeft = 0;
                sfUI.BlackFlashImpactFrames(target.Center);
                int additionalDamage = (int)Math.Pow(damageDone, 2);
                additionalDamage -= damageDone;

                Player.ApplyDamageToNPC(target, additionalDamage, hit.Knockback, hit.HitDirection, false);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}
