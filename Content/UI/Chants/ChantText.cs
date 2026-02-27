using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.UI.Chat;

namespace sorceryFight.Content.UI.Chants
{
    public struct ChantTextStyle
    {
        public Color textColor;
        public Color text2Color;
        public float borderWidth;
        public Color borderColor;
        public Color border2Color;
        public float glowRadius;
        public Color glowColor;
        public ChantTextStyle(Color textColor, Color text2Color)
        {
            this.textColor = textColor;
            this.text2Color = text2Color;
            borderWidth = 2.0f;
            borderColor = Color.Black;
            border2Color = Color.Black;
            glowRadius = 0.0f;
            glowColor = Color.Black;
        }

        public ChantTextStyle(Color textColor, Color text2Color, float borderWidth, Color borderColor, Color border2Color)
        {
            this.textColor = textColor;
            this.text2Color = text2Color;
            this.borderWidth = borderWidth;
            this.borderColor = borderColor;
            this.border2Color = border2Color;
            glowRadius = 0.0f;
            glowColor = Color.Black;
        }

        public ChantTextStyle(Color textColor, Color text2Color, float borderWidth, Color borderColor, Color border2Color, float glowRadius, Color glowColor)
        {
            this.textColor = textColor;
            this.text2Color = text2Color;
            this.borderWidth = borderWidth;
            this.borderColor = borderColor;
            this.border2Color = border2Color;
            this.glowRadius = glowRadius;
            this.glowColor = glowColor;
        }
    }

    public class ChantText : UIText
    {
        private const int ticksPerChar = 2;
        private const float fadeDuration = 0.3f;
        private const float moveDistance = 10f;
        private string fullText;
        private int charactersDisplayed = 0;
        private int tick;

        private float[] charTimers;
        private Vector2[] charOffsets;

        ChantTextStyle style;

        public ChantText(string text, ChantTextStyle style) : base("", 1, false)
        {
            fullText = text;
            this.style = style;
            charTimers = new float[text.Length];
            charOffsets = new Vector2[text.Length];
        }

        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < charactersDisplayed; i++)
            {
                charTimers[i] = Math.Min(charTimers[i] + dt / fadeDuration, 1f);
            }

            if (charactersDisplayed < fullText.Length)
            {
                if (tick++ >= ticksPerChar)
                {
                    tick = 0;
                    charactersDisplayed++;
                    SoundEngine.PlaySound(SoundID.MenuTick with { PitchVariance = 0.25f, MaxInstances = 0 });

                    charTimers[charactersDisplayed - 1] = 0f;
                    charOffsets[charactersDisplayed - 1] = new Vector2(0, moveDistance);

                    SetText(fullText[..charactersDisplayed]);
                    Recalculate();
                }
            }

            base.Update(gameTime);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var dimensions = GetInnerDimensions();
            Vector2 basePos = dimensions.Position();

            var font = Terraria.GameContent.FontAssets.MouseText.Value;
            float time = Main.GlobalTimeWrappedHourly;
            float lerp = (float)Math.Sin(time * 2f) * 0.5f + 0.5f;

            for (int i = 0; i < charactersDisplayed; i++)
            {
                char c = fullText[i];
                string s = c.ToString();

                float alpha = charTimers[i];
                Vector2 offset = Vector2.Lerp(charOffsets[i], Vector2.Zero, charTimers[i]);

                Color animatedTextColor = Color.Lerp(style.textColor, style.text2Color, lerp) * alpha;
                Color animatedBorderColor = Color.Lerp(style.borderColor, style.border2Color, 1f - lerp) * alpha;

                Vector2 charPos = basePos + offset;

                if (style.glowRadius > 0)
                {
                    int passes = 12;
                    float radius = style.glowRadius;
                    for (int j = 0; j < passes; j++)
                    {
                        float angle = MathHelper.TwoPi * j / passes;
                        Vector2 glowOffset = angle.ToRotationVector2() * radius;
                        if (style.borderWidth > 0) glowOffset *= style.borderWidth;

                        ChatManager.DrawColorCodedString(
                            spriteBatch,
                            font,
                            s,
                            charPos + glowOffset,
                            style.glowColor * 0.25f * alpha,
                            0f,
                            Vector2.Zero,
                            new Vector2(2f)
                        );
                    }
                }

                if (style.borderWidth > 0)
                {
                    int passes = 12;
                    for (int j = 0; j < passes; j++)
                    {
                        float angle = MathHelper.TwoPi * j / passes;
                        Vector2 borderOffset = angle.ToRotationVector2() * style.borderWidth;
                        ChatManager.DrawColorCodedString(
                            spriteBatch,
                            font,
                            s,
                            charPos + borderOffset,
                            animatedBorderColor,
                            0f,
                            Vector2.Zero,
                            new Vector2(2f)
                        );
                    }
                }

                ChatManager.DrawColorCodedString(
                    spriteBatch,
                    font,
                    s,
                    charPos,
                    animatedTextColor,
                    0f,
                    Vector2.Zero,
                    new Vector2(2f)
                );

                basePos.X += font.MeasureString(s).X * 2f;
            }
        }
    }
}