using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class FingerBearerBall : ModProjectile
    {
        public static readonly int FRAMES = 8;
        public static readonly int TICKS_PER_FRAME = 3;
        public static Texture2D texture;

        public ref float Timer => ref Projectile.ai[0];
        public ref float ChargeUpTime => ref Projectile.ai[1];
        public ref float Target => ref Projectile.ai[2];

        public const float projectileSpeed = 20f;
        public float scale;

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;

            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Projectiles/FingerBearerBall", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.tileCollide = false;
            Projectile.hostile = true;
            Projectile.damage = 75;
            Projectile.ArmorPenetration = 7;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= FRAMES)
                    Projectile.frame = 0;
            }

            if (Timer <= ChargeUpTime)
            {
                Timer++;

                scale += 1f / ChargeUpTime;
            }

            if (Timer == ChargeUpTime - 1)
            {
                int targetPlayer = (int)Target;
                if (Main.player[targetPlayer].active)
                {
                    Vector2 targetDirection = Projectile.Center.DirectionTo(Main.player[targetPlayer].Center);
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetDirection * projectileSpeed, 0.5f);
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAMES;
            int framey = Projectile.frame * frameHeight;
            Rectangle sourceRectangle = new Rectangle(0, framey, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, sourceRectangle.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
