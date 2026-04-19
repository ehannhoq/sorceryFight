using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Vessel
{
    public class LineDevestationProjectile : ModProjectile
    {
        public static Texture2D texture;
        public static Texture2D textureBlood;
        public static readonly int FRAME_COUNT = 6;
        public static readonly int TICKS_PER_FRAME = 4;

        private static float animScale = 10f; 

        public override void SetDefaults()
        {
            Projectile.width = 500;
            Projectile.height = 500;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = FRAME_COUNT * TICKS_PER_FRAME;
        }

        public override void AI()
        {
            Projectile.alpha = (int)(255 * (1f - Projectile.timeLeft / (float)(FRAME_COUNT * TICKS_PER_FRAME)));
            Projectile.rotation = Projectile.ai[0] == 1f ? MathHelper.PiOver2 : 0f;

            if (++Projectile.frameCounter >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= FRAME_COUNT)
                    Projectile.frame = FRAME_COUNT - 1; // hold on last frame
            }

            float r = Projectile.ai[1] == 1f ? 1f : 1f;
            float g = Projectile.ai[1] == 1f ? 0.1f : 1f;
            float b = Projectile.ai[1] == 1f ? 0.1f : 1f;

            int lightSpacing = 16; // one light per tile
            for (int x = (int)Projectile.Left.X; x < (int)Projectile.Right.X; x += lightSpacing)
                for (int y = (int)Projectile.Top.Y; y < (int)Projectile.Bottom.Y; y += lightSpacing)
                    Lighting.AddLight(new Vector2(x, y), r, g, b);

        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DefenseEffectiveness *= 0;
            base.ModifyHitNPC(target, ref modifiers);
        } 

        public override bool PreDraw(ref Color lightColor)
        {

            if (!Main.dedServ)
            {
                if (Projectile.ai[2] > 0f)
                {
                    if (textureBlood == null)
                        textureBlood = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Vessel/LineDevestationProjectileBlood", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                }
                else
                {
                    if (texture == null)
                        texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Vessel/LineDevestationProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                }
            }

            Texture2D activeTex = Projectile.ai[2] > 0f ? textureBlood : texture;

            float alpha = 1f - (float)Projectile.alpha / 255f;
            int frameHeight = activeTex.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;
            Rectangle sourceRect = new Rectangle(0, frameY, activeTex.Width, frameHeight);
            Vector2 origin = new Vector2(activeTex.Width / 2, frameHeight / 2);

            Main.spriteBatch.Draw(activeTex, Projectile.Center - Main.screenPosition, sourceRect, Color.White * alpha, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
