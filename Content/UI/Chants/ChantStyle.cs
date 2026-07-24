using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.UI.Chat;

namespace sorceryFight.Content.UI.Chants
{
    public interface ChantStyle
    {
        public void ApplyCharacterStyle(ref SpriteBatch spriteBatch, ref DynamicSpriteFont font, ref string currentCharacter, ref Vector2 drawPos, ref Color currentColor, ref float rotation, ref float scale, int index, int tick);
        public void DrawCharacterStyle(SpriteBatch spriteBatch, DynamicSpriteFont font, string currentCharacter, Vector2 drawPos, Color currentColor, float rotation, float scale, int index, int tick);
    }

    public class CharacterShake : ChantStyle
    {
        private float strength;
        public CharacterShake(float strength = 1f)
        {
            this.strength = strength;
        }

        public void ApplyCharacterStyle(ref SpriteBatch spriteBatch, ref DynamicSpriteFont font, ref string currentCharacter, ref Vector2 drawPos, ref Color currentColor, ref float rotation, ref float scale, int index, int tick)
        {
            Vector2 offset = new Vector2(strength, strength).RotateRandom(MathHelper.TwoPi);
            drawPos += offset;
        }

        public void DrawCharacterStyle(SpriteBatch spriteBatch, DynamicSpriteFont font, string currentCharacter, Vector2 drawPos, Color currentColor, float rotation, float scale, int index, int tick) { }
    }

    public class CharacterStroke : ChantStyle
    {
        private Color borderColor;
        private int passes;
        private float borderWidth;
        
        public CharacterStroke(Color borderColor, int passes = 12, float borderWidth = 1.0f)
        {
            this.borderColor = borderColor;
            this.passes = passes;
            this.borderWidth = borderWidth;
        }
        
        public void ApplyCharacterStyle(ref SpriteBatch spriteBatch, ref DynamicSpriteFont font, ref string currentCharacter, ref Vector2 drawPos, ref Color currentColor, ref float rotation, ref float scale, int index, int tick) { }

        public void DrawCharacterStyle(SpriteBatch spriteBatch, DynamicSpriteFont font, string currentCharacter, Vector2 drawPos, Color currentColor, float rotation, float scale, int index, int tick)
        {
            for (int j = 0; j < passes; j++)
            {
                float angle = MathHelper.TwoPi * j / passes;
                Vector2 borderOffset = angle.ToRotationVector2() * borderWidth;
                ChatManager.DrawColorCodedString(
                    spriteBatch,
                    font,
                    currentCharacter,
                    drawPos + borderOffset,
                    borderColor,
                    0f,
                    Vector2.Zero,
                    new Vector2(scale)
                );
            }
        }
    }


    public class CharacterGlow : ChantStyle
    {
        private Color glowColor;
        private int passes;
        private float glowRadius;
        
        public CharacterGlow(Color glowColor, int passes = 12, float glowRadius = 1.0f)
        {
            this.glowColor = glowColor;
            this.passes = passes;
            this.glowRadius = glowRadius;
        }
        
        public void ApplyCharacterStyle(ref SpriteBatch spriteBatch, ref DynamicSpriteFont font, ref string currentCharacter, ref Vector2 drawPos, ref Color currentColor, ref float rotation, ref float scale, int index, int tick) { }

        public void DrawCharacterStyle(SpriteBatch spriteBatch, DynamicSpriteFont font, string currentCharacter, Vector2 drawPos, Color currentColor, float rotation, float scale, int index, int tick)
        {
            for (int j = 0; j < passes; j++)
            {
                float angle = MathHelper.TwoPi * j / passes;
                Vector2 glowOffset = angle.ToRotationVector2() * glowRadius;
                Color fadedGlow = new Color(glowColor.R, glowColor.G, glowColor.B, (byte)(glowColor.A * 0.25f));

                ChatManager.DrawColorCodedString(
                    spriteBatch,
                    font,
                    currentCharacter,
                    drawPos + glowOffset,
                    fadedGlow,
                    0f,
                    Vector2.Zero,
                    new Vector2(scale)
                );
            }
        }
    }

    public class CharacterWave : ChantStyle
    {
        private float amplitude;
        private float frequency;
        private float phaseOffset;

        public CharacterWave(float amplitude = 3f, float frequency = 0.08f, float phaseOffset = 0.5f)
        {
            this.amplitude = amplitude;
            this.frequency = frequency;
            this.phaseOffset = phaseOffset;
        }
 
        public void ApplyCharacterStyle(ref SpriteBatch spriteBatch, ref DynamicSpriteFont font, ref string currentCharacter, ref Vector2 drawPos, ref Color currentColor, ref float rotation, ref float scale, int index, int tick)
        {
            float waveOffset = (float)Math.Sin((tick * frequency) + (index * phaseOffset)) * amplitude;
            drawPos.Y += waveOffset;
        }

        public void DrawCharacterStyle(SpriteBatch spriteBatch, DynamicSpriteFont font, string currentCharacter, Vector2 drawPos, Color currentColor, float rotation, float scale, int index, int tick) { }
    }
}