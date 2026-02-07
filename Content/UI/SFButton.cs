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
        public Action ClickAction;

        private const int InitialDelayTicks = 30;
        private const int RepeatIntervalTicks = 6;

        private int holdTimer = 0;
        private bool wasHolding = false;

        public SFButton(Texture2D texture, string hoverText)
        {
            this.texture = texture;
            this.hoverText = hoverText;

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);


            CalculatedStyle dimensions = GetDimensions();
            bool hovering = SorceryFightUI.MouseHovering(this, texture);
            bool holding = hovering && Main.mouseLeft;

            if (hovering && hoverText != "")
                Main.hoverItemName = hoverText;

            if (holding)
            {
                holdTimer++;

                if (!wasHolding)
                {
                    OnClick();
                    holdTimer = 0;
                }
                else if (holdTimer >= InitialDelayTicks &&
                         (holdTimer - InitialDelayTicks) % RepeatIntervalTicks == 0)
                {
                    OnClick();
                }
            }
            else
            {
                holdTimer = 0;
            }

            wasHolding = holding;

            spriteBatch.Draw(texture, new Vector2(dimensions.X, dimensions.Y), Color.White);
        }

        public virtual void OnClick()
        {
            ClickAction?.Invoke();
        }
    }
}