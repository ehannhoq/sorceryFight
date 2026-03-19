using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class BloodDaggerStormProjectile : ModProjectile
    {
        private Texture2D texture;
        private const int FRAME_COUNT = 7;
        private const int TICKS_PER_FRAME = 5;
        private int target = -1;
        private float trackingRadius = 800f;
        public float animScale;
        private int noTargetCounter = 0;

        private Color textColor => new Color(255, 0, 0);

        public override void SetDefaults()
        {
            Projectile.width = 65;
            Projectile.height = 65;
            Projectile.penetrate = -1;
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

            Main.NewText("Target State" + target);

            if (target == -1)
                FindTarget();
                if(100 < noTargetCounter++)
                    Projectile.Kill();
            else if (target >= 0 && !Main.npc[target].active)
                FindTarget();
                Vector2 mouseVelocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, mouseVelocity, 0.25f);
            if (100 < noTargetCounter++)
                    Projectile.Kill();
            else if (target >= 0)
            {
                Vector2 targetVelocity = (Main.npc[target].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20f;
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

        void FindTarget()
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.CanBeChasedBy() && Vector2.DistanceSquared(npc.Center, Projectile.Center) < trackingRadius.Squared())
                {
                    target = npc.whoAmI;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            this.target = -2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/BloodDaggerStormProjectile").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}