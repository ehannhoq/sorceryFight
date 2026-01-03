using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System;
using System.IO;
using Microsoft.Build.Evaluation;
using System.Reflection;

namespace sorceryFight.Content.CursedTechniques
{
    public abstract class CursedTechnique : ModProjectile
    {
        /// <summary>
        /// Self reference, allows for better readability for child objects.
        /// </summary>
        public CursedTechnique Technique => this;


        public int damage = 0;
        public float cost = 0f;
        public float speed = 1f;
        public int lifetime = 300;


        /// <summary>
        /// Used to retrieve the correct display name and descriptions from localization.
        /// </summary>
        public abstract string InternalName { get; }

        public new string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.CursedTechniques.{InternalName}.DisplayName");
        public string Description
        {
            get
            {
                return Unlocked(Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>()) ? SFUtils.GetLocalizationValue($"Mods.sorceryFight.CursedTechniques.{InternalName}.Description") : SFUtils.GetLocalizationValue($"Mods.sorceryFight.CursedTechniques.{InternalName}.LockedDescription");
            }
        }


        /// <summary>
        /// Unlocking condition for this cursed technique.
        /// </summary>
        /// <param name="sf"></param>
        /// <returns></returns>
        public abstract bool Unlocked(SorceryFightPlayer sf);


        /// <summary>
        /// Calculates the final damage of the cursed technique before spawning it in the world. Takes player mastery, accessories, armors, and buffs into account.
        /// </summary>
        /// <param name="sf"></param>
        /// <returns></returns>
        public virtual int CalculateFinalDamage(SorceryFightPlayer sf)
        {
            // int finalDamage = damage + (int)(sf.numberBossesDefeated * 4); TODO
            int finalDamage = (int)sf.Player.GetTotalDamage(CursedTechniqueDamageClass.Instance).ApplyTo(damage);
            return finalDamage;
        }


        /// <summary>
        /// Calculates the final cost of the cursed technique. Takes player mastery, accessories, armors, and buffs into account.
        /// </summary>
        /// <param name="sf"></param>
        /// <returns></returns>
        public virtual float CalculateFinalCost(SorceryFightPlayer sf)
        {
            float finalCost = cost * (sf.numberBossesDefeated / SorceryFight.totalBosses);
            cost *= 1 - sf.ctCostReduction;
            return finalCost;
        }


        /// <summary>
        /// Main access point for summoning a cursed technique. Handles cursed energy decrementation, as well as spawning the
        /// projectile with the correct ProjectileType, damage, owner, and velocity (direction defaults to 'towards cursor', magnitude defaults to 'speed').
        /// </summary>
        /// <param name="sf"></param>
        /// <returns></returns>
        public virtual int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * speed;
                var entitySource = player.GetSource_FromThis();

                sf.cursedEnergy -= CalculateFinalCost(sf);

                return Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), (int)CalculateFinalDamage(sf), 0, player.whoAmI);
            }
            return -1;
        }


        /// <summary>
        /// Retrieves the ProjectileType at runtime. !! CHECK FOR PERFORMANCE ISSUES !!
        /// </summary>
        /// <returns></returns>
        public int GetProjectileType()
        {
            var type = GetType();
            var generic = SorceryFight.ModContentProjectileType.MakeGenericMethod(type);
            return (int)generic.Invoke(null, null);
        }
    }
}