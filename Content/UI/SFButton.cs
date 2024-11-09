using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class SFButton : UIElement
    {
        public Texture2D texture;
        public string hoverText;
        
        public SFButton(Texture2D texture, string hoverText)
        {
            this.texture = texture;
            this.hoverText = hoverText;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
            CalculatedStyle dimensions = GetDimensions();

            if (SorceryFightUI.MouseHovering(this, texture))
            {
                if (hoverText != "")
                    Main.hoverItemName = hoverText;

                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    OnClick();
                }
            }

            spriteBatch.Draw(texture, new Vector2(dimensions.X, dimensions.Y), Color.White);
        }
        public virtual void OnClick() {}
    }
}
