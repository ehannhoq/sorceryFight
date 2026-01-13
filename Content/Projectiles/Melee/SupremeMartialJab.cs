using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.Melee
{
    public class SupremeMartialJab : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;

            Projectile.timeLeft = 12; // very fast jab
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;

            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.active || player.dead)
            {
                Projectile.Kill();
                return;
            }

            Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.UnitX);
            Projectile.Center = player.MountedCenter + direction * 32f;

            Projectile.rotation = direction.ToRotation();

            player.itemTime = 2;
            player.itemAnimation = 2;
            player.ChangeDir(direction.X > 0f ? 1 : -1);
        }

        public override bool? CanHitNPC(NPC target)
        {
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 6; i++)
            {
                Dust.NewDust(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Electric,
                    Main.rand.NextFloat(-1.5f, 1.5f),
                    Main.rand.NextFloat(-1.5f, 1.5f),
                    0,
                    default,
                    0.9f
                );
            }
        }
    }
}