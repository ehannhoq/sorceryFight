using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques
{
    public abstract class CursedTechnique : ModProjectile
    {
        public virtual string Stats 
        {
            get
            {
                return $"Damage: {Damage}\n" 
                    + $"Cost: {Cost} CE\n";
            }
        }
        public abstract string Description { get; }
        public abstract string LockedDescription { get; } 
        public abstract float Cost { get; }
        public abstract float MasteryNeeded { get; }
        public abstract Color textColor { get; }
        public abstract bool DisplayNameInGame { get; }

        public abstract int Damage { get; }
        public abstract float Speed { get; }
        public abstract float LifeTime { get; }
        public abstract bool Unlocked(SorceryFightPlayer sf);
        public abstract int GetProjectileType();

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