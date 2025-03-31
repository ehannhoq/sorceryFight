using System;
using CalamityMod;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        private const int MAX_BLACK_FLASH_HITS = 5;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (blackFlashTimeLeft >= 0)
            {
                bool guaranteedBlackFlash = blackFlashCounter >= MAX_BLACK_FLASH_HITS - 1;
                bool validBlackFlashWindow = blackFlashTimeLeft >= lowerWindowTime && blackFlashTimeLeft <= upperWindowTime;

                if (guaranteedBlackFlash || validBlackFlashWindow)
                {
                    BlackFlash(target, hit, damageDone);

                    if (blackFlashCounter >= MAX_BLACK_FLASH_HITS)
                    {
                        ResetBlackFlashState();
                    }
                }
                else if (blackFlashTimeLeft < lowerWindowTime || blackFlashTimeLeft > upperWindowTime)
                {
                    ResetBlackFlashState();
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            base.OnHitByNPC(npc, hurtInfo);
            ResetBlackFlashState();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            base.OnHitByProjectile(proj, hurtInfo);
            ResetBlackFlashState();
        }

        private void BlackFlash(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (hit.DamageType != DamageClass.Magic && hit.DamageType != DamageClass.Melee &&
                hit.DamageType != DamageClass.Ranged && hit.DamageType != DamageClass.Summon &&
                hit.DamageType.CountsAsClass(new CalamityMod.RogueDamageClass()) && hit.DamageType.CountsAsClass(new CalamityMod.TrueMeleeDamageClass()))
                return;


            blackFlashTimeLeft = -60; // 1 second cooldown between black flashes

            Vector2 direction = Player.Center.DirectionTo(target.Center) * 30f;

            for (int i = 0; i < 15; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                LineParticle redParticle = new LineParticle(target.Center, direction + variation, false, 60, 6f, Color.Red);
                LineParticle blackParticle = new LineParticle(target.Center, direction + variation, false, 60, 5f, Color.White);
                GeneralParticleHandler.SpawnParticle(redParticle);
                GeneralParticleHandler.SpawnParticle(blackParticle);
            }

            if (!infinity && !hollowWickerBasket)
            {
                blackFlashCounter++;
            }
            else
            {
                ResetBlackFlashState();
            }

            bool showFlowState = blackFlashCounter == 1;
            sfUI.InitiateBlackFlashUI(target.Center, showFlowState);

            int additionalDamage = (int)Math.Pow(damageDone, 2) - damageDone;

            Player.ApplyDamageToNPC(target, additionalDamage, hit.Knockback, hit.HitDirection, false);

            if (blackFlashCounter > MAX_BLACK_FLASH_HITS)
            {
                ResetBlackFlashState();
            }

            cursedEnergy = maxCursedEnergy;
            if (Player.HasBuff<BurntTechnique>())
            {
                int index = -1;
                for (int i = 0; i < Player.buffType.Length; i++)
                {
                    if (Player.buffType[i] == ModContent.BuffType<BurntTechnique>())
                    {
                        index = i;
                    }
                }
                if (index != -1)
                    Player.DelBuff(index);
            }
        }

        private void ResetBlackFlashState()
        {
            blackFlashCounter = 0;
            blackFlashTimeLeft = 0;
            sfUI.ClearBlackFlashUI();
        }
    }
}
