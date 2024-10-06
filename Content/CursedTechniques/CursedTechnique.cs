using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques
{
    public class CursedTechnique : ModProjectile
    {
        public virtual new string Name { get; set; } = "None Selected.";
        public virtual float Cost { get; set; } = 0;
        public virtual float CostPercentage { get; set; } = -1;
        public virtual float MasteryNeeded { get; set; } = 0f;
        public virtual Color textColor { get; set; } = new Color(255, 255, 255);

        public virtual int Damage { get; set; } = 0;
        public virtual float Speed { get; set; } = 0f;
        public virtual float LifeTime { get; set; } = 30f;
        public virtual int GetProjectileType()
        {
            return ModContent.ProjectileType<CursedTechnique>();
        }
        public virtual float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Projectile.Kill();
        }
    }
}