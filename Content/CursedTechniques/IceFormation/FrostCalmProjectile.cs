using Microsoft.Xna.Framework;
using sorceryFight.Content.Particles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.IceFormation
{
    public class FrostCalmProjectile : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/CursedTechniques/IceFormation/FrostCalmProjectile";

        public static int Lifetime => 96;
        public ref float Time => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.penetrate = 5;
            Projectile.MaxUpdates = 4;
            Projectile.timeLeft = Lifetime;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void AI()
        {
            Color frostColor = Main.rand.Next(3) switch
            {
                0 => new Color(100, 180, 255),
                1 => new Color(150, 220, 255),
                _ => new Color(200, 240, 255),
            };

            Time++;

            if (Time >= 1f)
                Projectile.scale = 1.8f * Utils.GetLerpValue(5f, 30f, Time, true);
            else
                return;

            if (Time == 1)
            {
                for (int i = 0; i < 12; i++)
                {
                    float rotMulti = Main.rand.NextFloat(0.7f, 1.1f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.IceTorch);
                    dust.scale = Main.rand.NextFloat(1.8f, 2.5f) - rotMulti;
                    dust.noGravity = true;
                    dust.velocity = Projectile.velocity.RotatedByRandom(0.5f * rotMulti) * Main.rand.NextFloat(0.5f, 1.8f) * rotMulti;
                    dust.alpha = Main.rand.Next(90, 150);
                    dust.color = frostColor;
                }
            }

            if (Time > 9)
            {
                float dustArea = Main.rand.NextFloat(0.1f, 1.7f);
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(9, 9) + Projectile.velocity * Main.rand.NextFloat(-1.8f, 1.8f),
                    DustID.IceTorch
                );
                dust.scale = (1.8f - dustArea) * 0.65f;
                dust.noGravity = true;
                dust.velocity = new Vector2(4, 4).RotatedByRandom(100) * dustArea;
                dust.alpha = Main.rand.Next(90, 150);
                dust.color = frostColor;
            }

            // Frost smoke particles
            float smokeRot = MathHelper.ToRadians(3f);
            Color smokeColor = Color.Lerp(new Color(100, 180, 255), new Color(200, 240, 255), 0.5f * MathF.Sin(Main.GlobalTimeWrappedHourly * 5f) + 0.5f);
            Particle smoke = new HeavySmokeParticle(
                Projectile.Center,
                Projectile.velocity * 0.5f,
                smokeColor,
                12,
                Projectile.scale * Main.rand.NextFloat(0.6f, 1.2f),
                0.45f,
                smokeRot,
                true,
                required: true
            );
            GeneralParticleHandler.SpawnParticle(smoke);

            if (Main.rand.NextBool(5))
            {
                Color glowColor = Color.Lerp(smokeColor, Color.White, 0.3f);
                Particle smokeGlow = new HeavySmokeParticle(
                    Projectile.Center,
                    Projectile.velocity * 0.5f,
                    glowColor,
                    9,
                    Projectile.scale * Main.rand.NextFloat(0.4f, 0.7f),
                    0.2f,
                    smokeRot,
                    true,
                    0.005f
                );
                GeneralParticleHandler.SpawnParticle(smokeGlow);
            }

            Lighting.AddLight(Projectile.Center, smokeColor.ToVector3() * Projectile.scale * 0.3f);
        }


        //need to implement this

        //public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        //{
        //    return CalamityMod.CalamityUtils.CircularHitboxCollision(Projectile.Center, 52 * Projectile.scale * 0.5f, targetHitbox);
        //}

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}