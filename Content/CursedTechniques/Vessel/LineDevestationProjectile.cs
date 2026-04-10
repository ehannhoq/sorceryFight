using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 3;

        public override void SetDefaults()
        {
            Projectile.width = 200;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = FRAME_COUNT * TICKS_PER_FRAME;
            Projectile.damage = 40;
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
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Vessel/LineDevestationProjectile").Value;

            float alpha = 1f - (float)Projectile.alpha / 255f;
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;
            Rectangle sourceRect = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Main.spriteBatch.Draw(
                texture,
                Projectile.Center - Main.screenPosition,
                sourceRect,
                Color.White * alpha,
                Projectile.rotation,
                origin,
                1f,
                SpriteEffects.None,
                0f
            );

            return false;
        }
    }
}
