using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Shrine
{
    public class HollowWickerBasketProjectile : ModProjectile
    {
        public virtual int FrameCount { get; set; } = 5;
        public virtual int TicksPerFrame { get; set; } = 12;
        public Texture2D texture;
        public Texture2D underTexture;
        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.Hitbox = new Rectangle(0, 0, 0, 0);

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Buffs/Shrine/HollowWickerBasketProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            underTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/Buffs/Shrine/HollowWickerBasketProjectile_Underlay", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value; 
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
            int frameHeight = texture.Height / FrameCount;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Main.spriteBatch.Draw(underTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White * 0.25f, Projectile.rotation, origin, 0.8f, SpriteEffects.None, 0f);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 0.8f, SpriteEffects.None, 0f);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

    }
}
