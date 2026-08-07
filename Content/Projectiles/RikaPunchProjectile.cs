using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class RikaPunchProjectile : ModProjectile
    {
        private const int FRAMES = 13;
        private const int TICKS_PER_FRAME = 2;

        private Vector2 offset;

        private ref float targetWhoAmI => ref Projectile.ai[0];

        public override void SetDefaults()
        {
            Projectile.width = 64;
            Projectile.height = 42;
            Projectile.friendly = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = FRAMES * TICKS_PER_FRAME;
            Projectile.scale = 2f;

        }

        public override void OnSpawn(IEntitySource source)
        {
            offset = Projectile.Center - Main.npc[(int)targetWhoAmI].Center;
        }

        public override void AI()
        {
            if (++Projectile.frameCounter >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= FRAMES)
                    Projectile.frame = 0;
            }

            if (Main.npc[(int)targetWhoAmI] != null)
            {
                Projectile.Center = Main.npc[(int)targetWhoAmI].Center + offset;
                Projectile.rotation = (Main.npc[(int)targetWhoAmI].Center - Projectile.Center).SafeNormalize(Vector2.UnitX).ToRotation();
            }
        }

        public override bool? CanHitNPC(NPC target)
        {
            return Projectile.frame == 7;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 center = Projectile.Center;
            Vector2 offset = (Vector2.UnitX * (Projectile.width / 2f)).RotatedBy(Projectile.rotation);

            Vector2 start = center - offset;
            Vector2 end = center + offset;

            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.height, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int frameHeight = texture.Height / FRAMES;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}