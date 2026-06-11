using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.Content.Particles.UIParticles;

namespace sorceryFight.Content.CursedTechniques.IceFormation
{
    public class IceShard : CursedTechnique
    {
        public override string InternalName => "IceShard";


        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 8;
        // public override float Cost => 20f;

        // public override int Damage => 18;
        // public override int MasteryDamageMultiplier => 50;

        // public override float Speed => 25f;
        // public override float LifeTime => 300f;

        public static Texture2D texture;

        public bool animating;
        public float animScale;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 35;
            Projectile.height = 15;
            Projectile.tileCollide = true;
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

            if (Projectile.ai[0] > lifetime + beginAnimTime)
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
                if (!animating)
                {
                    Projectile.Center += new Vector2(0, -30);
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);
                }

                // Code that was for expanding the spread of the particles based on height of shooting, but rotating the projectile itself looked better 
                // float verticalness = 1f - Math.Abs(Projectile.velocity.SafeNormalize(Vector2.Zero).X);
                // float spreadWidth = MathHelper.Lerp(8f, 60f, verticalness);
                // Vector2 behindOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(30f, 120f);
                // Vector2 perpendicular = Projectile.velocity.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(-spreadWidth, spreadWidth);
                // Vector2 particleOffset = Projectile.Center + behindOffset + perpendicular;
                // Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                // LinearParticle particle = new LinearParticle(particleOffset, particleVelocity * 3, textColor, false, 0.9f, 1f, 20);
                // ParticleController.SpawnParticle(particle);

                Vector2 behindOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10f, 40f);
                Vector2 particleOffset = Projectile.Center + behindOffset;
                Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
                LinearParticle particle = new LinearParticle(particleOffset, particleVelocity * 3, new Color(149, 237, 214), false, 0.9f, 1f, 20);
                ParticleController.SpawnParticle(particle);
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
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/IceFormation/IceShard").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 6; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, new Color(149, 237, 214), false, 0.9f, 1f, 30);
                ParticleController.SpawnParticle(particle);
            }
        }

    }
}