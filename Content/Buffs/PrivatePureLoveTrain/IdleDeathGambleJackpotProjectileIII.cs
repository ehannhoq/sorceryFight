using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PrivatePureLoveTrain
{
    public class IdleDeathGambleJackpotProjectileIII : ModProjectile
    {
        public int FrameCount { get; set; } = 9;
        public int TicksPerFrame { get; set; } = 5;
        public Texture2D texture;
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.Hitbox = new Rectangle(0, 0, 0, 0);

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Buffs/PrivatePureLoveTrain/IdleDeathGambleJackpotProjectileIII", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            if (!player.buffType.ToList<int>().Contains(ModContent.BuffType<IdleDeathGambleJackpotBuffIII>()))
            {
                Projectile.Kill();
                return;
            }

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
            int frameHeight = texture.Height / FrameCount;
            int frameY = Projectile.frame * frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Vector2 offset = new Vector2(0, frameHeight / 2);
            
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - offset, sourceRectangle, Color.White, Projectile.rotation, origin, 2f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
