using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles.VFX
{
    public class HollowPurpleCollision : ModProjectile
    {
        public static Texture2D texture => ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/VFX/HollowPurpleCollision", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public static readonly int FRAMES = 20;
        public static readonly int TICKS_PER_FRAME = 3;
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.friendly = true;
            Projectile.timeLeft = TICKS_PER_FRAME * FRAMES;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame++ >= FRAMES - 1)
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
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, src, Color.White, -MathF.PI / 2, src.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
