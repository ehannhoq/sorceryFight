using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class MaximumAmplifiedAuraProjectile : AmplifiedAuraProjectile
    {
        public override int TicksPerFrame { get; set; } = 3;

        public override void SetDefaults()
        {
            Projectile.damage = 0;
            Projectile.tileCollide = false;
            Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Buffs/Limitless/MaximumAmplifiedAuraProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FrameCount;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(0f, 25f), sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
