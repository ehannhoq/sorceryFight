using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using sorceryFight.Content.Buffs;
using sorceryFight; // or wherever CursedTechniqueDamageClass lives

namespace sorceryFight.Content.Projectiles.Abilities
{
    public class CursedLightningStrike : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 120;
            Projectile.height = 600;

            Projectile.friendly = true;
            Projectile.hostile = false;

            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;

            // Prevent frame-by-frame damage abuse
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;

            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
        }

        public override void AI()
        {
            // First-frame impact effects
            if (Projectile.timeLeft == 60)
            {
                for (int i = 0; i < 40; i++)
                {
                    Dust.NewDust(
                        Projectile.Bottom,
                        10,
                        10,
                        DustID.Electric,
                        Main.rand.NextFloat(-3f, 3f),
                        Main.rand.NextFloat(-3f, -1f),
                        0,
                        default,
                        1.5f
                    );

                    Dust.NewDust(
                        Projectile.Bottom,
                        10,
                        10,
                        DustID.Stone,
                        Main.rand.NextFloat(-2f, 2f),
                        Main.rand.NextFloat(-2f, 0f),
                        0,
                        default,
                        1.2f
                    );
                }
            }

            // Fade out safely
            Projectile.alpha = Utils.Clamp(Projectile.alpha + 4, 0, 255);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(ModContent.BuffType<ElectroStunned>(), 180);
        }
    }
}
