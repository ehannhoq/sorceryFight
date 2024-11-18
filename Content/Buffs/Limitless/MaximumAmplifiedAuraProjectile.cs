using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class MaximumAmplifiedAuraProjectile : AmplifiedAuraProjectile
    {
        public override int TicksPerFrame { get; set; } = 3;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>($"sorceryFight/Content/Buffs/Limitless/MaximumAmplifiedAuraProjectile", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int frameHeight = texture.Height / FrameCount;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition - new Vector2(0f, 25f), sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
