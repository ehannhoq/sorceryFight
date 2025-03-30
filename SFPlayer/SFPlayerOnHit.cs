using System;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
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
                BlackFlash(target, hit, damageDone);
            }
            base.OnHitNPC(target, hit, damageDone);
        }
        

        private void BlackFlash(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.DamageType != DamageClass.Magic && hit.DamageType != DamageClass.Melee && hit.DamageType != DamageClass.Ranged &&
                hit.DamageType != DamageClass.Summon && hit.DamageType != RogueDamageClass.Throwing) return; // Ignore if damage done by a Cursed Technique.

            blackFlashTimeLeft = 0;
            int additionalDamage = (int)Math.Pow(damageDone, 2);
            additionalDamage -= damageDone;

            Player.ApplyDamageToNPC(target, additionalDamage, hit.Knockback, hit.HitDirection, false);

            
            Vector2 direction = Player.Center.DirectionTo(target.Center);
            direction *= 30f;

            for (int i = 0; i < 15; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                LineParticle redParticle = new LineParticle(target.Center, direction + variation, false, 60, 6f, Color.Red);
                LineParticle blackParticle = new LineParticle(target.Center, direction + variation, false, 60, 5f, Color.White);
                GeneralParticleHandler.SpawnParticle(redParticle);
                GeneralParticleHandler.SpawnParticle(blackParticle);
            }


            if (ModContent.GetInstance<ClientConfig>().BlackFlashScreenEffects)
                sfUI.BlackFlashImpactFrames(target.Center);
        }
    }
}
