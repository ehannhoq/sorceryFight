using System;
using sorceryFight.Content.Buffs.PlayerAttributes;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public abstract class PassiveTechnique : ModBuff
    {
        /// <summary>
        /// The internal name for this technique. Used to retrieve DisplayName and Description localizations.
        /// </summary>
        public abstract string InternalName { get; }


        /// <summary>
        /// The display name of the cursed technique. Based off of it's InternalName.
        /// </summary>
        public new string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.PassiveTechniques.{InternalName}.DisplayName");


        /// <summary>
        /// The description of this cursed technique. Based off of it's InternalName.
        /// </summary>
        public new string Description => SFUtils.GetLocalizationValue($"Mods.sorceryFight.PassiveTechniques.{InternalName}.Description");


        /// Variables below are set from methods that are used in each innate technique when adding a new technique to it.
        private Predicate<SorceryFightPlayer> unlocked;
        private int bossType;
        private string lockedDescriptionLocalizationKey;


        /// <summary>
        /// Self-reference variable for better readability.
        /// </summary>
        public PassiveTechnique Technique => this;


        /// <summary>
        /// Flag of whether this passive is active.
        /// </summary>
        public bool active = false;


        /// <summary>
        /// Passive Techniques with this set will auto-disable any other passive that has this set as well.
        /// </summary>
        public bool isAura = false;


        /// <summary>
        /// The cost in CE/s this passive uses before any modifiers are applied.
        /// </summary>
        public float cost = 0;


        /// <summary>
        /// Whether or not the current technique can be usable at the moment.
        /// </summary>
        public virtual bool CanUse(Player player)
        {
            return true;
        }


        public PassiveTechnique() { }


        /// <summary>
        /// Sets the unlock requirement to any predicate. Use boss type to set to unlock at a boss defeat.
        /// </summary>
        public PassiveTechnique SetUnlock(Predicate<SorceryFightPlayer> predicate)
        {
            this.unlocked = predicate;
            return this;
        }

        /// <summary>
        /// Sets the unlock requirement to a boss defeated. Automatically sets the unlock requirement description.
        /// </summary>
        public PassiveTechnique SetUnlock(int bossType)
        {
            this.bossType = bossType;
            return this;
        }

        /// <summary>
        /// Sets the unlock requirement description. This is already set if SetUnlock(int bossType) is used.
        /// </summary>
        public PassiveTechnique SetUnlockRequirement(string localizationKey)
        {
            this.lockedDescriptionLocalizationKey = localizationKey;
            return this;
        }


        /// <summary>
        /// Retrieves the appropriate unlock requirement based off whether one was provided or based off of boss type.
        /// </summary>
        public string GetUnlockRequirement()
        {
            if (lockedDescriptionLocalizationKey == null)
            {
                return SFUtils.GetUnlockRequirementFromBossID(bossType);
            }

            return SFUtils.GetLocalizationValue(lockedDescriptionLocalizationKey);
        }


        /// <summary>
        /// Checks if the cursed technique based off of if a predicate was used, and if not if the player defeated the set boss.
        /// </summary>
        public bool IsUnlocked(SorceryFightPlayer sfPlayer)
        {
            return unlocked != null ? unlocked(sfPlayer) : sfPlayer.HasDefeatedBoss(bossType);
        }


        /// <summary>
        /// Gets the damage and cost stats for this technique.
        /// </summary>
        public virtual string GetStats(SorceryFightPlayer sf)
        {
            string localizationCategoryKey = "Mods.sorceryFight.Misc.CursedTechniques";

            string ceCost = SFUtils.GetLocalization(localizationCategoryKey + ".ContinuousCost")
                .WithFormatArgs(CalculateTrueCost(sf)).Value;

            return ceCost;
        }


        public virtual float CalculateTrueCost(SorceryFightPlayer sf)
        {
            return sf.uniqueBodyStructure ? cost * 1 - UniqueBodyStructureBuff.passiveTechniqueCostReduction : cost;
        }


        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
        }


        public void Apply(Player player)
        {
            active = true;
            OnApply(player);
        }


        public void Remove(Player player)
        {
            active = false;
            OnRemove(player);
        }


        /// <summary>
        /// Called once when the technique is activated.
        /// </summary>
        public virtual void OnApply(Player player) { }


        /// <summary>
        /// Called once when the technique is deactivated.
        /// </summary>
        public virtual void OnRemove(Player player) { }


        /// <summary>
        /// Called continuously; automatically applies ce cost/second to the player.
        /// </summary>
        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sf = player.SorceryFight();
            sf.cursedEnergyUsagePerSecond += CalculateTrueCost(sf);
        }

        /// <summary>
        /// Retrieves the ModBuffType at runtime. !! CHECK FOR PERFORMANCE ISSUES !!
        /// </summary>
        public int GetBuffType()
        {
            var type = GetType();
            var generic = SorceryFightMod.ModContentBuffType.MakeGenericMethod(type);
            return (int)generic.Invoke(null, null);
        }
    }
}
