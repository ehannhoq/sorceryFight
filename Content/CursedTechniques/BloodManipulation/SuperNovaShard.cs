using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class SuperNovaShard : ModProjectile
    {

        private Texture2D texture;
        private const int FRAME_COUNT = 7;
        private const int TICKS_PER_FRAME = 5;

        public bool animating;
        public float animScale;

        private const float LifeTime = 60f;

        private Color projColor = new Color(255, 0, 0);


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 65;
            Projectile.height = 65;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            animating = false;
            Projectile.penetrate = -1;
            animScale = 1.25f;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0] += 1;
            float beginAnimTime = 30f;
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] > LifeTime + beginAnimTime)
            {
                Projectile.Kill();
            }

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.ai[0] < beginAnimTime)
            {
                //clean this up by removing animating checks
                if (!animating)
                {
                    Projectile.Center += new Vector2(0, -30);
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);
                }

                //Vector2 behindOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10f, 40f);
                //Vector2 particleOffset = Projectile.Center + behindOffset;
                //Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                //LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 20, 1f, projColor);
                //particle.Color *= 0.5f;
                //GeneralParticleHandler.SpawnParticle(particle);

                Vector2 behindOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10f, 40f);
                Vector2 particleOffset = Projectile.Center + behindOffset;

                Dust dust = Dust.NewDustPerfect(particleOffset, DustID.RedTorch, Vector2.Zero);
                dust.noGravity = true;
                dust.scale = Main.rand.NextFloat(0.8f, 1.5f);
                dust.alpha = 150; // 0 = opaque, 255 = invisible
                dust.velocity = particleOffset.DirectionTo(Projectile.Center) * 3f;
                return;
            }

            if (animating)
            {
                Projectile.tileCollide = true;
                animating = false;
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/SuperNovaShard").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            //Main.NewText("Hit NPC" + target);
            base.OnHitNPC(target, hit, damageDone);
            Projectile.penetrate = 0;

            target.AddBuff(BuffID.Poisoned, 300);

            for (int i = 0; i < 6; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LineParticle particle = new LineParticle(target.Center, Projectile.velocity + variation, false, 30, 1, projColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }
        }

    }
}