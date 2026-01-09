using System;
using System.Linq;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.Melee
{
    public class InvertedSpearCharge : ModProjectile
    {
        private static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Melee/InvertedSpearCharge", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        private static Texture2D overlayTexture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/Melee/InvertedSpearChargeOverlay", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        private static readonly int chargeUpMax = 180;
        private static readonly int minimumCharge = 60;
        public override void SetDefaults()
        {
            Projectile.width = (int)(texture.Width * 3.5f);
            Projectile.height = (int)(texture.Height * 3.5f);
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.frameCounter = 0;
        }

        ref float charge => ref Projectile.ai[0];
        ref float rotation => ref Projectile.ai[1];
        ref float previousRotation => ref Projectile.ai[2];

        ref float initialDirection => ref Projectile.localAI[0];
        bool cameraShakeApplied => Projectile.localAI[1] == 0f;


        public override void AI()
        {
            if (charge++ >= chargeUpMax)
                charge = chargeUpMax;


            Player player = Main.player[Projectile.owner];
            Projectile.velocity = Vector2.Zero;
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + Projectile.velocity;

            if (Main.myPlayer == Projectile.owner)
            {
                if (charge == 1)
                {
                    Vector2 offsetVector = player.Center + new Vector2(10f * -player.direction, 0f);
                    rotation = player.Center.AngleTo(offsetVector);
                    initialDirection = player.direction;
                }

                if (player.CantUseSword(Projectile))
                {
                    Projectile.Kill();
                }

                if (charge >= minimumCharge && cameraShakeApplied)
                {
                    float progress = MathHelper.Clamp((charge - minimumCharge) / (chargeUpMax - minimumCharge), 0f, 1f);

                    Vector2 cameraOffset = new Vector2(Main.rand.NextFloat(-5 * progress, 5 * progress), Main.rand.NextFloat(-5 * progress, 5 * progress));
                    CameraController.SetCameraPosition(player.Center + cameraOffset);
                }

            }


            rotation += MathHelper.Lerp(0.1f, 0.33f, charge / chargeUpMax) * initialDirection;
            rotation = MathHelper.WrapAngle(rotation);

            float threshold = MathHelper.PiOver2;

            bool crossed =
                Math.Abs(previousRotation) < threshold &&
                Math.Abs(rotation) >= threshold;

            if (crossed)
            {
                SoundEngine.PlaySound(
                    SorceryFightSounds.InvertedSpearOfHeavenSpin,
                    Projectile.Center
                );
            }

            previousRotation = rotation;


            int minIFrameIgnore = 8;
            int maxIFrameIgnore = 45;

            int iFrameIgnoreDifference = maxIFrameIgnore - minIFrameIgnore;

            Projectile.idStaticNPCHitCooldown = (int)(maxIFrameIgnore - (iFrameIgnoreDifference * (charge / chargeUpMax)));
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(0, texture.Height);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, src, Color.White, rotation + MathHelper.PiOver4, origin, 1.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            Rectangle overlaySrc = new Rectangle(0, 0, overlayTexture.Width, overlayTexture.Height);
            SpriteEffects flipped = initialDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;

            float rotationOffset = initialDirection == -1 ? -MathHelper.PiOver4 : MathHelper.PiOver4;
            float opacity = MathHelper.Clamp((charge - minimumCharge) / (chargeUpMax - minimumCharge), 0f, 1f);

            Main.spriteBatch.Draw(overlayTexture, Main.player[Projectile.owner].Center - Main.screenPosition, overlaySrc, new Color(1, 1, 1, opacity), rotation + rotationOffset, overlaySrc.Size() * 0.5f, 1.5f, flipped, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();


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
            SoundEngine.PlaySound(SorceryFightSounds.InvertedSpearOfHeavenImpact, Projectile.Center);

            for (int i = 0; i < 3; i++)
            {
                Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                int colVariation = Main.rand.Next(-38, 100);
                float scale = Main.rand.NextFloat(1f, 1.25f);
                float scalar = Main.rand.NextFloat(5f, 15f);
                SparkParticle particle = new SparkParticle(target.Center, (Projectile.velocity * scalar) + veloVariation, false, 30, scale, new Color(225 + colVariation, 242 + colVariation, 97 + colVariation));
                GeneralParticleHandler.SpawnParticle(particle);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 veloVariation = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                int colVariation = Main.rand.Next(-38, 100);
                float scale = Main.rand.NextFloat(1f, 1.25f);
                float scalar = Main.rand.NextFloat(5f, 15f);
                LineParticle particle = new LineParticle(target.Center, (Projectile.velocity * scalar) + veloVariation, false, 30, scale, new Color(225 + colVariation, 242 + colVariation, 97 + colVariation));
                GeneralParticleHandler.SpawnParticle(particle);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 posVariation = new Vector2(Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));
                SparkleParticle particle = new SparkleParticle(target.Center + posVariation, Vector2.Zero, new Color(225, 242, 97), Color.White, 1f, 10, 0.75f, 0.2f);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer != Projectile.owner) return;
            if (charge < minimumCharge) return;

            if (Main.projectile.TryGet(proj => proj.type == ModContent.ProjectileType<InvertedSpearProjectile>() && proj.owner == Projectile.owner, out Projectile proj))
            {
                proj.Kill();
            }

            Player player = Main.player[Projectile.owner];
            Vector2 velocity = player.Center.DirectionTo(Main.MouseWorld) * (30f * (charge / chargeUpMax));
            int index = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + new Vector2(0f, 1.5f), velocity, ModContent.ProjectileType<InvertedSpearProjectile>(), Projectile.damage, 0f, player.whoAmI, (int)(charge / 60) * 2);

            Projectile spear = Main.projectile[index];
            spear.rotation = spear.velocity.ToRotation();


            if (spear.ModProjectile is InvertedSpearProjectile modSpear)
            {
                modSpear.bouncePositions.Add(player.Center);
            }

            spear.netUpdate = true;

            Projectile.localAI[1] = 1f;
            CameraController.ResetCameraPosition();
        }
    }
}
