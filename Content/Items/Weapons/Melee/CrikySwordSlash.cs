using System;
using CalamityMod;
using CalamityMod.Particles;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class CrikySwordSlash : ModProjectile
    {
        private static Texture2D texture;
        private const int FRAME_COUNT = 20;
        private const int TICKS_PER_FRAME = 1;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Items/Weapons/Melee/CrikySwordSlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 270;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.frameCounter = 0;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame++ >= FRAME_COUNT - 1)
                    Projectile.frame = 0;
            }

            Player player = Main.player[Projectile.owner];
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            float velocityAngle = Projectile.velocity.ToRotation();
            float offset = 120f * Projectile.scale;

            Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
            player.ChangeDir(Projectile.direction);

            if (Main.myPlayer == Projectile.owner)
            {
                if (player.CantUseSword(Projectile))
                {
                    Projectile.Kill();
                }
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, -32).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation, projOrigin, 1f, spriteEffects, 0f);
            return false;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += target.defense;
            modifiers.ScalingArmorPenetration += 1f;
            modifiers.Defense *= 0;
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // for (int i = 0; i < 3; i++)
            // {
            //     Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
            //     int colVariation = Main.rand.Next(-38, 100);
            //     float scale = Main.rand.NextFloat(1f, 1.25f);
            //     float scalar = Main.rand.NextFloat(15f, 30f);
            //     SparkParticle particle = new SparkParticle(target.Center, (Projectile.velocity * scalar) + veloVariation, false, 30, scale, new Color(109 + colVariation, 38 + colVariation, 115 + colVariation));
            //     GeneralParticleHandler.SpawnParticle(particle);
            // }

            for (int i = 0; i < 2; i++)
            {
                Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                int colVariation = Main.rand.Next(-38, 100);
                float scale = Main.rand.NextFloat(0.9f, 1.2f);
                LinearParticle particle = new LinearParticle(target.Center, (Projectile.velocity * 30) + veloVariation, new Color(109 + colVariation, 38 + colVariation, 115 + colVariation), default, 0.9f, scale, 45);
                ParticleController.SpawnParticle(particle);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 posVariation = new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));
                SparkleParticle particle = new SparkleParticle(target.Center + posVariation, Vector2.Zero, new Color(109, 38, 115), Color.White, 1f, 10, 0.75f, 0.2f);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }
    }
}
