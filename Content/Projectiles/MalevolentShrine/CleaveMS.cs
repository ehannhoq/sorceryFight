using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.MalevolentShrine
{
    public class CleaveMS : ModProjectile
    {
        public static Texture2D texture => ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/MalevolentShrine/CleaveMS").Value;
        public static readonly int FRAMES = 8;
        public static readonly int TICKS_PER_FRAME = 1;
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.timeLeft = TICKS_PER_FRAME * FRAMES;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame++ >= FRAMES)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAMES;
            int frameY = Projectile.frame * frameHeight;
            Rectangle src = new Rectangle(0, frameY, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.ai[0], src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
