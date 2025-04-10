using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using Microsoft.Build.Tasks;
using System;
using System.IO;
using CalamityMod;

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
            return $"Damage: {CalculateTrueDamage(sf)}\n"
                + $"Cost: {CalculateTrueCost(sf)} CE\n";
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
            
            if (sf.sixEyes)
                finalCost *= 0.7f;

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
                Main.player[Projectile.owner].GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
            }
            base.OnKill(timeLeft);
        }
    }
}