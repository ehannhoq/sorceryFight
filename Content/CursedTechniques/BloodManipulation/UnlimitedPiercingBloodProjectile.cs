using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class UnlimitedPiercingBloodProjectile : ModProjectile
    {
        private Texture2D texture;
        private const int FRAME_COUNT = 5;
        private const int TICKS_PER_FRAME = 5;
        private float trackingRadius = 2000f;
        public float animScale;

        private Color textColor => new Color(255, 0, 0);

        public override void SetDefaults()
        {
            Projectile.width = 150;
            Projectile.height = 50;
            Projectile.penetrate = 1;
            Projectile.knockBack = 0;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 300;
            animScale = 1.25f;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            if (Projectile.ai[1] < 0 || !Main.npc[(int)Projectile.ai[1]].active || Main.npc[(int)Projectile.ai[1]].Distance(Projectile.Center) > trackingRadius)
            {
                Main.NewText("Couldn't find target!!!");
                Projectile.Kill();
            }
            else
            {
                Vector2 targetVelocity = (Main.npc[(int)Projectile.ai[1]].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.25f);
            }

            Projectile.rotation = Projectile.velocity.ToRotation();

            Vector2 behindOffset = -Projectile.velocity.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(10f, 40f);
            Vector2 particleOffset = Projectile.Center + behindOffset;
            Vector2 particleVelocity = particleOffset.DirectionTo(Projectile.Center);
            LineParticle particle = new LineParticle(particleOffset, particleVelocity * 3, false, 20, 1f, textColor);
            GeneralParticleHandler.SpawnParticle(particle);
            return;

        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/PiercingBloodCollision").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}