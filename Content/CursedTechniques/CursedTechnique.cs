using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using Microsoft.Build.Tasks;

namespace sorceryFight.Content.CursedTechniques
{
    public abstract class CursedTechnique : ModProjectile
    {
        public abstract string Description { get; }
        public abstract string LockedDescription { get; }
        public abstract float Cost { get; }
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

        public virtual void UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * Speed;
                var entitySource = player.GetSource_FromThis();

                sf.cursedEnergy -= Cost;

                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }
                Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), CalculateTrueDamage(sf), 0, player.whoAmI);
            }
        }
    }
}