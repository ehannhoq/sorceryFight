using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.PassiveTechniques.Limitless
{
    public class AmplifiedAuraProjectile : ModProjectile
    {
        public virtual int FrameCount { get; set; } = 3;
        public virtual int TicksPerFrame { get; set; } = 5;
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
        }

        public override void AI()
        {
            Projectile.ai[0]++;


            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            if (Projectile.frameCounter++ >= TicksPerFrame)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FrameCount - 1)
                    Projectile.frame = 0;
            }

            if (Projectile.timeLeft <= 2)
                Projectile.timeLeft = 10;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"sorceryFight/Content/PassiveTechniques/Limitless/AmplifiedAuraProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int frameHeight = texture.Height / FrameCount;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(0f, 15f), sourceRectangle, Color.White, Projectile.rotation, origin, 0.8f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
