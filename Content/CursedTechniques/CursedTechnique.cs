using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System;
using System.IO;
using JetBrains.Annotations;
using Terraria.Cinematics;
using Terraria.Localization;
using System.Security.Policy;
using System.Collections.Generic;
namespace sorceryFight.Content.CursedTechniques
{
    public abstract class CursedTechnique : ModProjectile
    {
        /// <summary>
        /// The internal name for this technique. Used to retrieve DisplayName and Description localizations.
        /// </summary>
        public abstract string InternalName { get; }


        /// <summary>
        /// The display name of the cursed technique. Based off of it's InternalName.
        /// </summary>
        public string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.{parentTechnique}.{InternalName}.DisplayName");


        /// <summary>
        /// The description of this cursed technique. Based off of it's InternalName..
        /// </summary>
        public string Description => SFUtils.GetLocalizationValue($"Mods.sorceryFight.{parentTechnique}.{InternalName}.Description");


        /// Variables below are set from methods that are used in each innate technique when adding a new technique to it.
        private Predicate<SorceryFightPlayer> unlocked;
        private int bossType = -1;
        private string lockedDescriptionLocalizationKey;
        public string parentTechnique;


        /// <summary>
        /// Self-reference variable for better readability.
        /// </summary>
        public CursedTechnique Technique => this;


        /// <summary>
        /// Base damage of the cursed technique assuming 0 bosses killed and no class buffs.
        /// </summary>
        public int baseDamage = 0;


        /// <summary>
        /// How much additional damage each boss defeated adds to the base damage.
        /// </summary>
        public int damagePerBoss = 0;


        /// <summary>
        /// Cost of the cursed technique before any modifiers are applied.
        /// </summary>
        public float cost = 0;


        /// <summary>
        /// Initial speed the cursed technique is fired at.
        /// </summary>
        public float speed = 0;


        /// <summary>
        /// Lifetime of the cursed technique in game ticks. Defaults to 300.
        /// </summary>
        public int lifetime = 300;


        /// <summary>
        /// Sets the unlock requirement to any predicate. Use boss type to set to unlock at a boss defeat.
        /// </summary>
        public CursedTechnique SetUnlock(Predicate<SorceryFightPlayer> predicate)
        {
            this.unlocked = predicate;
            return this;
        }


        /// <summary>
        /// Sets the unlock requirement to a boss defeated. Automatically sets the unlock requirement description.
        /// </summary>
        public CursedTechnique SetUnlock(int bossType)
        {
            this.bossType = bossType;
            return this;
        }


        /// <summary>
        /// Sets the unlock requirement description. This is already set if SetUnlock(int bossType) is used.
        /// </summary>
        public CursedTechnique SetUnlockRequirement(string localizationKey)
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
        /// Sets the parent innate technique. Not to be used by developer, as this is automaticaly set for each technique.
        /// </summary>
        public void SetParentTechnique(string parentTechnique)
        {
            this.parentTechnique = parentTechnique;
        }


        /// <summary>
        /// Returns the parent technique of this cursed technique.
        /// CursedTechniqueSummon.cs:
        /// Cleans up the minions if state switches
        /// Important to set in sub classes unless it's a multi tech summon
        /// </summary>
        public string GetParentTechnique()
        {
            return parentTechnique;
        }


        /// <summary>
        /// Gets the damage and cost stats for this technique.
        /// </summary>
        public virtual string GetStats(SorceryFightPlayer sf)
        {
            string localizationCategoryKey = "Mods.sorceryFight.Misc.CursedTechniques";

            string damage = SFUtils.GetLocalization(localizationCategoryKey + ".Damage")
                .WithFormatArgs(CalculateTrueDamage(sf)).Value;

            string ceCost = SFUtils.GetLocalization(localizationCategoryKey + ".Cost")
                .WithFormatArgs((int)MathF.Round(CalculateTrueCost(sf))).Value;

            string stats = damage + "\n" + ceCost;

            return stats;
        }



        /// <summary>
        /// Returns the final damage of the cursed technique, after applying boss multiplier and damage class modifiers.
        /// </summary>
        public virtual int CalculateTrueDamage(SorceryFightPlayer sf)
        {
            int damage = baseDamage + (sf.bossesDefeated.Count * damagePerBoss);
            return (int)sf.Player.GetTotalDamage(CursedTechniqueDamageClass.Instance).ApplyTo(damage);
        }


        /// <summary>
        /// Returns the total cost of the cursed technique, after applying boss defeated discount and accessory-related deductions.
        /// </summary>
        public virtual float CalculateTrueCost(SorceryFightPlayer sf)
        {
            float finalCost = cost - (cost * (sf.bossesDefeated.Count / 100f));
            finalCost *= 1 - sf.ctCostReduction;
            return finalCost;
        }


        public override void SetDefaults()
        {
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.timeLeft = lifetime;
        }


        public virtual int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * speed;
                var entitySource = player.GetSource_FromThis();

                int index = Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), CalculateTrueDamage(sf), 0, player.whoAmI);
                SyncCursedTechniqueInfo(index);
                return index;
            }
            return -1;
        }

        
        public virtual void ApplyCosts(SorceryFightPlayer sfPlayer)
        {
            sfPlayer.cursedEnergy -= CalculateTrueCost(sfPlayer);
        }

        public void SyncCursedTechniqueInfo(int index)
        {
            CursedTechnique self = Main.projectile[index].ModProjectile as CursedTechnique;

            self.baseDamage = this.baseDamage;
            self.damagePerBoss = this.damagePerBoss;
            self.speed = this.speed;
            self.cost = this.cost;
            self.lifetime = this.lifetime;
            self.parentTechnique = this.parentTechnique;
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Main.player[Projectile.owner].SorceryFight().disableRegenFromProjectiles = false;
            }
            base.OnKill(timeLeft);
        }


        /// <summary>
        /// Whether or not the current technique can be usable at the moment.
        /// </summary>
        public virtual bool CanUse(SorceryFightPlayer sf)
        {
            return true;
        }


        /// <summary>
        /// Retrieves the ProjectileType at runtime. !! CHECK FOR PERFORMANCE ISSUES !!
        /// </summary>
        /// <returns>The ModContent.ProjectileType<> of this cursed technique.</returns>
        public int GetProjectileType()
        {
            var type = GetType();
            var generic = SorceryFightMod.ModContentProjectileType.MakeGenericMethod(type);
            return (int)generic.Invoke(null, null);
        }
    }
}
