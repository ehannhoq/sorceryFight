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

        //previous max size was 2752, this hit on the event horizon but that's not really the point of the horizon
        private const int MaxSize = 2352;


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

            #region Shader Startup
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
            #endregion

            #region Hitbox
            //resize hitbox based on progress
            int currentSize = (int)MathHelper.Lerp(MinSize, MaxSize, expandProgress);
            Projectile.position += new Vector2(
                (Projectile.width - currentSize) / 2f,
                (Projectile.height - currentSize) / 2f
            );
            Projectile.width = currentSize;
            Projectile.height = currentSize;


            // pull strengths that scale with size
            float pullRadius = MathHelper.Lerp(MinSize + 10f, MaxSize + 1000f, expandProgress);
            float pullStrength = MathHelper.Lerp(0f, 15f, expandProgress); 

            foreach (NPC npc in Main.npc)
            {
                if (!npc.active) continue;

                float dist = Vector2.Distance(npc.Center, Projectile.Center);
                if (dist < pullRadius && dist > 0f)
                {
                    // make the pull stronger closer
                    float falloff = 1f - (dist / pullRadius);
                    Vector2 pull = Projectile.Center - npc.Center;
                    pull.Normalize();
                    npc.velocity += pull * pullStrength * falloff;

                    // cap velocity
                    if (npc.velocity.Length() > 20f)
                        npc.velocity = Vector2.Normalize(npc.velocity) * 20f;
                }
            }

            foreach (Player p in Main.player)
            {
                if (!p.active || p.dead) continue;

                float dist = Vector2.Distance(p.Center, Projectile.Center);
                if (dist < pullRadius && dist > 0f)
                {
                    float falloff = 1f - (dist / pullRadius);
                    Vector2 pull = Projectile.Center - p.Center;
                    pull.Normalize();
                    p.velocity += pull * pullStrength * falloff;

                    if (p.velocity.Length() > 20f)
                        p.velocity = Vector2.Normalize(p.velocity) * 20f;
                }
            }




            #endregion

            #region debugging prints
            //code to get an idea of the blackhole size
            //if ((int)Projectile.ai[0] % 30 == 0)
            //{

            //    // shader radius is in UV
            //    // use Y since UV Y goes 0->1
            //    float shaderRadiusPixels = expandProgress * Main.screenHeight;

            //    Vector2 shaderEdge = Projectile.Center + new Vector2(shaderRadiusPixels, 0f);
            //    Main.NewText($"Progress: {expandProgress:F2} | Radius px: {shaderRadiusPixels:F1} | Edge: {shaderEdge}", Color.Cyan);
            //}

            //get min size
            //if ((int)Projectile.ai[0] == 1)
            //{
            //    float progressAtTick1 = 1f / expandTime;
            //    float shaderRadiusPixels = progressAtTick1 * Main.screenHeight;
            //    Main.NewText($"Tick 1 radius: {shaderRadiusPixels:F1}px | Progress: {progressAtTick1:F4}", Color.Yellow);
            //}
            #endregion
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Main.NewText("Hit NPC" + target);
            float dist = Vector2.Distance(target.Center, Projectile.Center);
            float maxDist = Projectile.width / 2f; // edge of hitbox

            // 1.0 at center, 0.0 at edge
            float proximity = 1f - Math.Clamp(dist / maxDist, 0f, 1f);

            // scale between 25% and 100% damage based on proximity
            float damageScale = MathHelper.Lerp(0.25f, 1f, proximity);
            modifiers.SourceDamage *= damageScale;
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            float dist = Vector2.Distance(target.Center, Projectile.Center);
            float maxDist = Projectile.width / 2f;

            float proximity = 1f - Math.Clamp(dist / maxDist, 0f, 1f);
            float damageScale = MathHelper.Lerp(0.25f, 1f, proximity);
            modifiers.SourceDamage *= damageScale;
        }

    }
}