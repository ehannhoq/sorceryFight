using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Utilities.EaseFunctions;
using Terraria;
using Terraria.UI.Chat;

namespace sorceryFight.Content.UI.Chants
{
    public sealed class Chant
    {
        internal string text;
        internal int timeBetweenWords;
        internal List<Color> colors;
        internal int characterIndex;
        internal int timeBetweenCharacters;
        internal float scale;
        internal Action onEnd;
        internal ChantStyle[] chantStyles;
        internal Action<int, int> perCharacterEvent;
        internal Action<int, int> perWordEvent;
        internal Action<int, int> perSentenceEvent;

        internal int totalTime;
        internal Color currentColor;

        internal int tick = 0;
        internal int characterTimer = 0;
        internal bool hasSentences = false;
        internal int currentColorIndex = 0;
        internal int targetColorIndex = 1;
        internal int wordIndex = 0;
        internal int totalWords = 0;
        internal int sentenceIndex = 0;
        internal int totalSentences = 0;
        internal int[] characterAnimationTimes;
        internal int[] currentCharacterAnimationTimes;
        internal Vector2 characterStartOffset;
        internal bool characterAnimationOpacityFadeIn;

        public Chant(string text, int timeBetweenWords = 1,  List<Color> colors = null, int timeBetweenCharacters = 1, float scale = 1f, Action onEnd = null, ChantStyle[] chantStyles = null, Action<int, int> perCharacterEvent = null, Action<int, int> perWordEvent = null, Action<int, int> perSentenceEvent = null, int delayAfterChant = 0, int perCharacterAnimationTime = 0, Vector2? characterStartOffset = null, bool characterAnimationOpacityFadeIn = false)
        {
            this.text = text;
            this.timeBetweenWords = timeBetweenWords;
            this.colors = colors ?? [Color.White];
            this.timeBetweenCharacters = timeBetweenCharacters;
            this.scale = 2f * scale;
            this.chantStyles = chantStyles;
            this.perCharacterEvent = perCharacterEvent;
            this.perWordEvent = perWordEvent;
            this.perSentenceEvent = perSentenceEvent;

            this.hasSentences = text.Contains(". ");

            int typingTime = 0;
            characterAnimationTimes = new int[text.Length];
            currentCharacterAnimationTimes = new int[text.Length];
            this.characterStartOffset = characterStartOffset ?? Vector2.Zero;
            this.characterAnimationOpacityFadeIn = characterAnimationOpacityFadeIn;

            for (int i = 0; i < text.Length - 1; i++)
            {
                char c = text[i];
                if (c == ' ')
                {
                    if (this.hasSentences)
                    {
                        if (i > 0 && text[i - 1] == '.' || text[i - 1] == ',')
                            typingTime += timeBetweenWords;
                    }
                    else
                    {
                        typingTime += timeBetweenWords;
                    }
                }
                else
                    typingTime += timeBetweenCharacters;

                characterAnimationTimes[i] = perCharacterAnimationTime * (i + 1);
            }
            typingTime += perCharacterAnimationTime;

            this.totalTime = typingTime + delayAfterChant;
            this.onEnd = totalTime > 0 ? onEnd : null;
            this.currentColor = this.colors[0];

            this.characterIndex = 0;
            this.totalWords = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
            this.totalSentences = text.Split(new[] { '.', '!', '?', ',' }, StringSplitOptions.RemoveEmptyEntries).Length;

            perCharacterEvent?.Invoke(characterIndex, text.Length - 1 - characterIndex);
            perWordEvent?.Invoke(wordIndex, totalWords - 1 - wordIndex);
            perSentenceEvent?.Invoke(sentenceIndex, totalSentences - 1 - sentenceIndex);
        }


        public void Update()
        {
            if (characterIndex < text.Length - 1)
            {
                int requiredTime = getCharacterTime();
                if (characterTimer >= requiredTime)
                {
                    characterTimer = 0;
                    characterIndex++;

                    for (int i = 0; i <= characterIndex; i++)
                    {
                        currentCharacterAnimationTimes[i]++;
                    }

                    char prevChar = text[characterIndex - 1];   
                    char currentChar = text[characterIndex];

                    perCharacterEvent?.Invoke(characterIndex, text.Length - 1);

                    if (prevChar == ' ' && currentChar != ' ')
                    {
                        wordIndex++;
                        perWordEvent?.Invoke(wordIndex, totalWords - 1 - wordIndex);
                    }

                    if (prevChar == ' ' && currentChar != ' ' && characterIndex >= 2 && (text[characterIndex - 2] == '.' || text[characterIndex - 2] == ','))
                    {
                        sentenceIndex++;
                        perSentenceEvent?.Invoke(sentenceIndex, totalSentences - 1 - sentenceIndex);
                    }

                }

                characterTimer++;
            }

            if (colors.Count > 1)
            {
                Color targetColor = colors[targetColorIndex];
                currentColor = Color.Lerp(currentColor, targetColor, 0.1f);

                if ((currentColor.ToVector3() - targetColor.ToVector3()).Length() < 0.1)
                {
                    targetColorIndex++;
                    targetColorIndex %= colors.Count;
                }
            }
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            var font = Terraria.GameContent.FontAssets.MouseText.Value;
            float width = ChatManager.GetStringSize(font, text, Vector2.One).X * scale;

            var characters = new List<(string currentCharacter, Vector2 drawPos, Color currentColor, float rotation, float scale)>();

            for (int i = 0; i <= characterIndex; i++)
            {
                string currentCharacter = text[i].ToString();
                string precedingText = text.Substring(0, i);

                Vector2 drawPos = new Vector2(
                    (Main.screenWidth / Main.UIScale / 2f) - (width / 2f),
                    Main.screenHeight / Main.UIScale * (5f / 8f)
                ) + new Vector2(ChatManager.GetStringSize(font, precedingText, Vector2.One).X * scale, 0f);


                if (characterAnimationTimes[i] > 0)
                {
                    for (int j = 0; j <= characterIndex; j++)
                    {
                        currentCharacterAnimationTimes[j]++;
                    }

                    float progress = EaseFunctions.EaseInOut(currentCharacterAnimationTimes[i] / (float)characterAnimationTimes[i]);

                    Vector2 progOffset = characterStartOffset;
                    drawPos = Vector2.Lerp(drawPos + progOffset, drawPos, progress);

                    if (characterAnimationOpacityFadeIn)
                        currentColor = new Color(currentColor.R, currentColor.G, currentColor.B, (int)(progress * 255));
                }


                float rotation = 0.0f;

                if (chantStyles != null)
                {
                    foreach (ChantStyle style in chantStyles)
                    {
                        style.ApplyCharacterStyle(ref spriteBatch, ref font, ref currentCharacter, ref drawPos, ref currentColor, ref rotation, ref scale, i, tick);
                    }
                }

                characters.Add((currentCharacter, drawPos, currentColor, rotation, scale));
            }

            if (chantStyles != null)
            {
                for (int i = 0; i < characters.Count; i++)
                {
                    var (currentCharacter, drawPos, currentColor, rotation, scale) = characters[i];
                    foreach (ChantStyle style in chantStyles)
                    {
                        style.DrawCharacterStyle(spriteBatch, font, currentCharacter, drawPos, currentColor, rotation, scale, i, tick);
                    }
                }
            }

            foreach (var (currentCharacter, drawPos, currentColor, rotation, scale) in characters)
            {
                ChatManager.DrawColorCodedString(
                    spriteBatch,
                    font,
                    currentCharacter,
                    drawPos,
                    currentColor,
                    rotation,
                    Vector2.Zero,
                    new Vector2(scale)
                );
            }
        }

        private int getCharacterTime()
        {
            char c = text[characterIndex];
            if (c == ' ')
            {
                if (this.hasSentences)
                {
                    if (characterIndex > 0)
                    {
                        char prevC = text[characterIndex - 1];
                        if (prevC == '.' || prevC == ',')
                            return timeBetweenWords;
                    }
                    return 0;
                }

                return timeBetweenWords;
            }

            return timeBetweenCharacters;
        }
    }
}
