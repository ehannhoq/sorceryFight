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
        public bool immune;
        public override bool ImmuneTo(PlayerDeathReason damageSource, int cooldownCounter, bool dodgeable)
        {
            if (innateTechnique == null) return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);

            if (Player == Main.LocalPlayer)
            {
                return infinity || hollowWickerBasket || immune;
            }

            return base.ImmuneTo(damageSource, cooldownCounter, dodgeable);
        }

        public override void PostUpdateBuffs()
        {
            if (innateTechnique == null) return;

            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                if (Player.HasBuff<BurntTechnique>() || (!beerHat && cursedEnergy < 2))
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
        }

        public void ResetBuffs()
        {
            if (innateTechnique == null) return;
            foreach (PassiveTechnique passiveTechnique in innateTechnique.PassiveTechniques)
            {
                passiveTechnique.isActive = false;
            }
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (innateTechnique == null) return;

            if (domainAmp)
            {
                modifiers.FinalDamage *= 0.5f;
            }

            base.ModifyHitByNPC(npc, ref modifiers);
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (innateTechnique == null) return;

            if (domainAmp)
            {
                modifiers.FinalDamage *= 0.5f;
            }

            base.ModifyHitByProjectile(proj, ref modifiers);
        }

        public void AddDeductableDebuff(int debuffType, float duration)
        {
            float percentReduction = 0f;

            if (celestialAmulet)
            {
                percentReduction += 0.2f;
            }

            duration -= duration * percentReduction;
            Player.AddBuff(debuffType, SFUtils.BuffSecondsToTicks(duration));
        }

        public void DisablePTBooleans()
        {
            infinity = false;
            hollowWickerBasket = false;
            domainAmp = false;
        }
    }
}
