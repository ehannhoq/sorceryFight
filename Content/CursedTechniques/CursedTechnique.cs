using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.Content.SFPlayer;

namespace sorceryFight.Content.CursedTechniques
{
    public abstract class CursedTechnique : ModProjectile
    {
        public abstract string Description { get; }
        public abstract string LockedDescription { get; } 
        public abstract float Cost { get; }
        public abstract float MasteryNeeded { get; }
        public abstract Color textColor { get; }
        public abstract bool DisplayNameInGame { get; }
        public abstract int BaseDamage { get; }
        public abstract int MaxDamage { get; }
        public abstract float MaxMastery { get; }
        public abstract float Speed { get; }
        public abstract float LifeTime { get; }
        public abstract bool Unlocked(SorceryFightPlayer sf);
        public abstract int GetProjectileType();
        public virtual string GetStats(SorceryFightPlayer sf) 
        {
            return $"Damage: {CalculateTrueDamage(sf)}\n" 
                + $"Cost: {Cost} CE\n";
        }
        public virtual int CalculateTrueDamage(SorceryFightPlayer sf)
        {
            float slope = (MaxDamage - BaseDamage) / 100;
            return (int)(slope * sf.mastery + BaseDamage);
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