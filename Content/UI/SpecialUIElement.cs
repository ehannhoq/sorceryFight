using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.UI.InnateTechniqueSelector;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class SpecialUIElement : UIElement
    {
        private float timeCounter;
        private Texture2D texture;
        private float rotationAngle;
        private float rotationSpeed;
        private float scaleOscillate;

        public SpecialUIElement(Texture2D texture, float rotationSpeed = 0f, float scaleOscillate = 0f)
        {
            timeCounter = 0;
            this.texture = texture;
            rotationAngle = 0f;
            this.rotationSpeed = MathHelper.ToRadians(rotationSpeed);
            this.scaleOscillate = scaleOscillate;

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            timeCounter += 0.1f;
            if (timeCounter > 2 * MathF.PI)
                timeCounter = 0;

            float baseScale = 1f;
            float scale = baseScale + scaleOscillate * MathF.Sin(timeCounter);

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
                scale,
                SpriteEffects.None,
                0f
            );
            
            rotationAngle += rotationSpeed;
            
        }
    }
}
