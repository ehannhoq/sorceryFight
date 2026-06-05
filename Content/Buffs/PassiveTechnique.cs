using System;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.PlayerAttributes;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public abstract class PassiveTechnique : ModBuff
    {
        public abstract string Stats { get; }
        public virtual bool isAura { get; } = false;

        public abstract bool isActive { get; set; }
        public abstract float CostPerSecond { get; set; }

        public virtual float BloodRegenPerSecond { get; set; }

        public virtual Color selectorBGColor { get; set; }
        public virtual Color selectorBorderColor { get; set; }

        private Predicate<SorceryFightPlayer> unlocked;
        private int bossType;
        private string lockedDescriptionLocalizationKey;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public abstract void Apply(Player player);
        public abstract void Remove(Player player);

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sf = player.SorceryFight();

            float finalCostPerSecond = CostPerSecond;

            float finalBloodRegenPerSecond = BloodRegenPerSecond;

            if (sf.uniqueBodyStructure)
                finalCostPerSecond *= 1 - UniqueBodyStructureBuff.passiveTechniqueCostReduction;

            sf.cursedEnergyUsagePerSecond += finalCostPerSecond;

            sf.bloodEnergyRegenPerSecond += finalBloodRegenPerSecond;
        }

        public virtual bool UseCondition(Player player)
        {
            return true;
        }


        public PassiveTechnique() { }

        public PassiveTechnique SetUnlock(Predicate<SorceryFightPlayer> predicate)
        {
            this.unlocked = predicate;
            return this;
        }

        public PassiveTechnique SetUnlock(int bossType)
        {
            this.bossType = bossType;
            return this;
        }

        public PassiveTechnique SetUnlockRequirement(string localizationKey)
        {
            this.lockedDescriptionLocalizationKey = localizationKey;
            return this;
        }

        public bool IsUnlocked(SorceryFightPlayer sfPlayer)
        {
            return unlocked != null ? unlocked(sfPlayer) : sfPlayer.HasDefeatedBoss(bossType);
        }

        public string GetUnlockRequirement()
        {
            if (lockedDescriptionLocalizationKey == null)
            {
                return SFUtils.GetUnlockRequirementFromBossID(bossType);
            }

            return SFUtils.GetLocalizationValue(lockedDescriptionLocalizationKey);
        }
    }
}
