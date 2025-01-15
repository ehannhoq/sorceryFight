using System;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public bool infinity;
        public bool domainAmp;
        public bool hollowWickerBasket;
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            bool immune = infinity || hollowWickerBasket;
            if (Player == Main.LocalPlayer && immune)
            {
                return true;
            }

            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }

        public override void PostUpdateBuffs()
        {
            if (innateTechnique == null) return;

            if (cursedEnergy < 1)
            {
                cursedEnergy = 1;
                int duration = SorceryFight.BuffSecondsToTicks(defaultBurntTechniqueDuration - (defaultBurntTechniqueDuration * percentBurntTechnqiueReduction));
                Player.AddBuff(ModContent.BuffType<BurntTechnique>(), duration);
            }


            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                if (cursedEnergy <= 1 || Player.HasBuff<BurntTechnique>())
                {
                    passiveTechnique.isActive = false;
                }

                if (passiveTechnique.isActive)
                {
                    passiveTechnique.Apply(Player);
                }
                else
                {
                    passiveTechnique.Remove(Player);
                }
            }

            percentBurntTechnqiueReduction = 0f;
        }

        public void ResetBuffs()
        {
            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                passiveTechnique.isActive = false;
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (domainAmp)
            {
                modifiers.FinalDamage *= 0.5f;
            }

            base.ModifyHitByNPC(npc, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (domainAmp)
            {
                modifiers.FinalDamage *= 0.5f;
            }

            base.ModifyHitByProjectile(proj, ref modifiers);
        }
    }
}
