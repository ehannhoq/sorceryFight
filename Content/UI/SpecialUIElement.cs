using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class SpecialUIElement : UIElement
    {
        public Texture2D texture;
        private float timeCounter;
        private string hoverText;
        private float rotationAngle;
        private float rotationSpeed;
        private float scaleOscillate;
        private float baseScale;
        private bool transparentPixels;

        public SpecialUIElement(Texture2D texture, string hoverText = "", float rotationSpeed = 0f, float scaleOscillate = 0f, float baseScale = 1f, bool transparentPixels = false)
        {
            timeCounter = 0;
            this.texture = texture;
            this.hoverText = hoverText;
            rotationAngle = 0f;
            this.rotationSpeed = MathHelper.ToRadians(rotationSpeed);
            this.scaleOscillate = scaleOscillate;
            this.baseScale = baseScale;
            this.transparentPixels = transparentPixels;

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (SorceryFightUI.MouseHovering(this, texture))
            {
                Main.hoverItemName = hoverText;
            }

            timeCounter += 0.1f;
            if (timeCounter > 2 * MathF.PI)
                timeCounter = 0;

            float scale = baseScale + scaleOscillate * MathF.Sin(timeCounter);

            Vector2 pos = GetDimensions().Position();
            Vector2 center = new Vector2(texture.Width / 2f, texture.Height / 2f);
            pos += center;

            if (transparentPixels)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    null,
                    Main.UIScaleMatrix
                );

                spriteBatch.Draw(
                    texture,
                    pos,
                    null,
                    Color.White,
                    rotationAngle,
                    center,
                    scale,
                    SpriteEffects.None,
                    0f
                );

                spriteBatch.End();
                spriteBatch.Begin(
                    SpriteSortMode.Deferred, 
                    BlendState.AlphaBlend, 
                    SamplerState.LinearClamp, 
                    DepthStencilState.None, 
                    RasterizerState.CullCounterClockwise, 
                    null,
                    Main.UIScaleMatrix
                );
            }
            else
            {
                spriteBatch.Draw(
                    texture,
                    pos,
                    null,
                    Color.White,
                    rotationAngle,
                    center,
                    scale,
                    SpriteEffects.None,
                    0f
                );
            }


            rotationAngle += rotationSpeed;

        }
    }
}
