// using Microsoft.Xna.Framework;
// using Terraria;
// using Terraria.ModLoader;
// using sorceryFight.SFPlayer;
// using System;
// using System.IO;
// using Microsoft.Build.Evaluation;
// using System.Reflection;

// namespace sorceryFight.Content.CursedTechniques
// {
//     public abstract class CursedTechnique : ModProjectile
//     {
//         /// <summary>
//         /// Self reference, allows for better readability for child objects.
//         /// </summary>
//         public CursedTechnique Technique => this;


//         public int damage = 0;
//         public float cost = 0f;
//         public float speed = 1f;
//         public int lifetime = 300;


//         /// <summary>
//         /// Used to retrieve the correct display name and descriptions from localization.
//         /// </summary>
//         public abstract string InternalName { get; }

//         public new string DisplayName => SFUtils.GetLocalizationValue($"Mods.sorceryFight.CursedTechniques.{InternalName}.DisplayName");
//         public string Description
//         {
//             get
//             {
//                 return Unlocked(Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>()) ? SFUtils.GetLocalizationValue($"Mods.sorceryFight.CursedTechniques.{InternalName}.Description") : SFUtils.GetLocalizationValue($"Mods.sorceryFight.CursedTechniques.{InternalName}.LockedDescription");
//             }
//         }


//         /// <summary>
//         /// Unlocking condition for this cursed technique.
//         /// </summary>
//         /// <param name="sf"></param>
//         /// <returns></returns>
//         public abstract bool Unlocked(SorceryFightPlayer sf);


//         /// <summary>
//         /// Calculates the final damage of the cursed technique before spawning it in the world. Takes player mastery, accessories, armors, and buffs into account.
//         /// </summary>
//         /// <param name="sf"></param>
//         /// <returns></returns>
//         public virtual int CalculateFinalDamage(SorceryFightPlayer sf)
//         {
//             // int finalDamage = damage + (int)(sf.numberBossesDefeated * 4); TODO
//             int finalDamage = (int)sf.Player.GetTotalDamage(CursedTechniqueDamageClass.Instance).ApplyTo(damage);
//             return finalDamage;
//         }


//         /// <summary>
//         /// Calculates the final cost of the cursed technique. Takes player mastery, accessories, armors, and buffs into account.
//         /// </summary>
//         /// <param name="sf"></param>
//         /// <returns></returns>
//         public virtual float CalculateFinalCost(SorceryFightPlayer sf)
//         {
//             float finalCost = cost * (sf.numberBossesDefeated / SorceryFight.totalBosses);
//             cost *= 1 - sf.ctCostReduction;
//             return finalCost;
//         }


//         /// <summary>
//         /// Main access point for summoning a cursed technique. Handles cursed energy decrementation, as well as spawning the
//         /// projectile with the correct ProjectileType, damage, owner, and velocity (direction defaults to 'towards cursor', magnitude defaults to 'speed').
//         /// </summary>
//         /// <param name="sf"></param>
//         /// <returns></returns>
//         public virtual int UseTechnique(SorceryFightPlayer sf)
//         {
//             Player player = sf.Player;

//             if (player.whoAmI == Main.myPlayer)
//             {
//                 Vector2 playerPos = player.MountedCenter;
//                 Vector2 mousePos = Main.MouseWorld;
//                 Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * speed;
//                 var entitySource = player.GetSource_FromThis();

//                 sf.cursedEnergy -= CalculateFinalCost(sf);

//                 return Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), (int)CalculateFinalDamage(sf), 0, player.whoAmI);
//             }
//             return -1;
//         }


//         /// <summary>
//         /// Retrieves the ProjectileType at runtime. !! CHECK FOR PERFORMANCE ISSUES !!
//         /// </summary>
//         /// <returns></returns>
//         public int GetProjectileType()
//         {
//             var type = GetType();
//             var generic = SorceryFight.ModContentProjectileType.MakeGenericMethod(type);
//             return (int)generic.Invoke(null, null);
//         }
//     }
// }



using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System;
using System.IO;

namespace sorceryFight.Content.CursedTechniques
{
    public abstract class CursedTechnique : ModProjectile
    {
        public abstract string Description { get; }
        public abstract string LockedDescription { get; }
        public abstract float Cost { get; }
        public abstract Color textColor { get; }
        public abstract bool DisplayNameInGame { get; }
        public abstract int Damage { get; }
        public abstract int MasteryDamageMultiplier { get; }
        public abstract float Speed { get; }
        public abstract float LifeTime { get; }
        public abstract bool Unlocked(SorceryFightPlayer sf);
        public abstract int GetProjectileType();
        public virtual string GetStats(SorceryFightPlayer sf)
        {
            return $"Damage: {Math.Round(CalculateTrueDamage(sf), 2)}\n"
                + $"Cost: {Math.Round(CalculateTrueCost(sf), 2)} CE\n";
        }
        public virtual float CalculateTrueDamage(SorceryFightPlayer sf)
        { 
            int baseDamage = Damage + (sf.bossesDefeated.Count * MasteryDamageMultiplier);
            int finalDamage = (int)sf.Player.GetTotalDamage(CursedTechniqueDamageClass.Instance).ApplyTo(baseDamage);
            return finalDamage;
        }

        public virtual float CalculateTrueCost(SorceryFightPlayer sf)
        {
            float finalCost =  Cost - (Cost * (sf.bossesDefeated.Count / 100f));
            finalCost *= 1 - sf.ctCostReduction;
            return finalCost;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.Kill();
        }

        public virtual int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * Speed;
                var entitySource = player.GetSource_FromThis();

                sf.cursedEnergy -= CalculateTrueCost(sf);

                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }

                return Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), (int)CalculateTrueDamage(sf), 0, player.whoAmI);
            }
            return -1;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Projectile.ai[0]);
            writer.Write(Projectile.ai[1]);
            writer.Write(Projectile.ai[2]);
            writer.Write(Projectile.velocity.X);
            writer.Write(Projectile.velocity.Y);
            writer.Write(Projectile.rotation);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Projectile.ai[0] = reader.ReadSingle();
            Projectile.ai[1] = reader.ReadSingle();
            Projectile.ai[2] = reader.ReadSingle();
            Projectile.velocity.X = reader.ReadSingle();
            Projectile.velocity.Y = reader.ReadSingle();
            Projectile.rotation = reader.ReadSingle();
        }

        public override void OnKill(int timeLeft)
        {
            if (Projectile.owner == Main.myPlayer)
            {
                Main.player[Projectile.owner].SorceryFight().disableRegenFromProjectiles = false;
            }
            base.OnKill(timeLeft);
        }
    }
}