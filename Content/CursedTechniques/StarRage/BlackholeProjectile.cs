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
using Terraria.Graphics.Effects;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class BlackholeProjectile : ModProjectile
    {

        private Texture2D texture;
        private const int FRAME_COUNT = 8;
        private const int TICKS_PER_FRAME = 5;

        public bool animating;
        public float animScale;

        private const float LifeTime = 240f;

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

            //Hits both players and enemies
            Projectile.friendly = true;
            Projectile.hostile = true;

            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            animScale = 1f;
            //Projectile.timeLeft = (int)LifeTime;

        }
        public override void AI()
        {
            Projectile.ai[0] += 1;
            Player player = Main.player[Projectile.owner];

            if (Projectile.ai[0] > LifeTime)
            {
                Projectile.Kill();
                Filters.Scene["SF:DivineFlame"].GetShader().UseOpacity(0f);
                Filters.Scene["SF:DivineFlame"].Deactivate();
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
            if (!Main.dedServ && Main.myPlayer == Projectile.owner)
            {
                if (!Filters.Scene["SF:Blackhole"].IsActive()){
                    Main.NewText("blackhole active this player is" + Main.player[Main.myPlayer].name);
                    Filters.Scene.Activate("SF:Blackhole").GetShader().UseTargetPosition(Projectile.Center).UseOpacity(1f);
                }
                //Use this formula to scale width and height of the projectile for hitboxes: Projectile.ai[0] / LifeTime (needs adjustment to be somewhat accurate with average screensizes)
                Filters.Scene["SF:Blackhole"].GetShader().UseProgress(Projectile.ai[0] / LifeTime);
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