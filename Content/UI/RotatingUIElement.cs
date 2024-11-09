using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.UI.InnateTechniqueSelector;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class RotatingUIElement : UIElement
    {
        private Texture2D texture;
        private float rotationAngle;
        private float rotationSpeed;

        public RotatingUIElement(Texture2D texture, float rotationSpeed = 1f)
        {
            this.texture = texture;
            rotationAngle = 0f;
            this.rotationSpeed = MathHelper.ToRadians(rotationSpeed);

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Vector2 pos = GetDimensions().Position();
            Vector2 center = new Vector2(texture.Width / 2f, texture.Height / 2f);
            pos += center;

            spriteBatch.Draw(
                texture,
                pos,
                null,
                Color.White,
                rotationAngle,
                center,
                1f,
                SpriteEffects.None,
                0f
            );
            
            rotationAngle += rotationSpeed;
            
        }
    }
}
