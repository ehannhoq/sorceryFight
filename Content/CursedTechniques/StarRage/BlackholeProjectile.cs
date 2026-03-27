using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static tModPorter.ProgressUpdate;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class BlackholeProjectile : ModProjectile
    {

        private Texture2D texture;
        private const int FRAME_COUNT = 8;
        private const int TICKS_PER_FRAME = 5;

        public bool animating;
        public float animScale;

        private const float LifeTime = 1680f;

        //The time at which the blackhole reaches it's maximum size, it then stats for the rest of Lifetime
        private const float expandTime = 480f;

        //private Color projColor = new Color(255, 0, 0);

        //radius * 2 values at 1 tick and 100% progress
        private const int MinSize = 6;
        private const int MaxSize = 2752;


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 65;
            Projectile.height = 65;

            //Hits both players and enemies
            Projectile.friendly = true;
            Projectile.hostile = true;

            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            animScale = 1f;

        }
        public override void AI()
        {
            Projectile.ai[0] += 1;
            Player player = Main.player[Projectile.owner];

            float expandProgress = Math.Clamp(Projectile.ai[0] / expandTime, 0f, 1f);

            if (Projectile.ai[0] > LifeTime)
            {
                if (!Main.dedServ)
                {
                    //Main.NewText("Removing Blackhole shader");
                    Filters.Scene["SF:Blackhole"].GetShader().UseOpacity(0f);
                    Filters.Scene["SF:Blackhole"].Deactivate();
                }
                Projectile.Kill();
                //return is important or it will recall the shader later in the code
                return;
            }

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

            SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);
            if (!Main.dedServ)
            {
                if (!Filters.Scene["SF:Blackhole"].IsActive())
                {
                    Filters.Scene.Activate("SF:Blackhole")
                        .GetShader()
                        .UseTargetPosition(Projectile.Center)
                        .UseOpacity(1f);
                }
            }
            else
            {
                //add code for drawing sprite of blackhole as a fallback
            }

            Filters.Scene["SF:Blackhole"].GetShader().UseProgress(expandProgress);

            //resize hitbox based on progress
            int currentSize = (int)MathHelper.Lerp(MinSize, MaxSize, expandProgress);
            Projectile.position += new Vector2(
                (Projectile.width - currentSize) / 2f,
                (Projectile.height - currentSize) / 2f
            );
            Projectile.width = currentSize;
            Projectile.height = currentSize;



            //code to get an idea of the blackhole size
            if ((int)Projectile.ai[0] % 30 == 0)
            {

                // shader radius is in UV
                // use Y since UV Y goes 0->1
                float shaderRadiusPixels = expandProgress * Main.screenHeight;

                Vector2 shaderEdge = Projectile.Center + new Vector2(shaderRadiusPixels, 0f);
                Main.NewText($"Progress: {expandProgress:F2} | Radius px: {shaderRadiusPixels:F1} | Edge: {shaderEdge}", Color.Cyan);
            }

            if ((int)Projectile.ai[0] == 1)
            {
                float progressAtTick1 = 1f / expandTime;
                float shaderRadiusPixels = progressAtTick1 * Main.screenHeight;
                Main.NewText($"Tick 1 radius: {shaderRadiusPixels:F1}px | Progress: {progressAtTick1:F4}", Color.Yellow);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/StarRage/BlackholeProjectile").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Main.NewText("Hit NPC" + target);

        }

    }
}