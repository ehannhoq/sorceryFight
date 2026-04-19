using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class FallingBlossomEmotionProjectile : ModProjectile
    {
        private Texture2D texture;
        private const int frames = 6;
        private const int ticksPerFrame = 3;
        private float alpha = 0;

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Buffs/Limitless/FallingBlossomEmotionProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            if (Projectile.frameCounter++ >= ticksPerFrame)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= frames - 1)
                    Projectile.frame = 0;
            }

            if (Projectile.timeLeft <= 2)
                Projectile.timeLeft = 10;


            if (player.velocity != Vector2.Zero)
            {
                alpha = 0;
            }
            else if (alpha != 1)
            {
                alpha += 0.01f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / frames;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(0f, 20f), sourceRectangle, Color.White * alpha, Projectile.rotation, origin, 1.5f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
