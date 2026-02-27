using System;
using System.Collections.Generic;
using CalamityMod.Fonts;
using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace sorceryFight.Content.UI.Chants
{
    public class Chant : UIElement
    {
        private List<ChantText> texts;
        float tick = 0;
        float lifetime;
        int timeBetweenChants;

        public Chant(List<string> chants, int timeBetweenChants, int bufferTime, ChantTextStyle style, float scale = 2.0f)
        {
            texts = new List<ChantText>(chants.Count);
            var font = FontAssets.MouseText.Value;
            float totalWidth = 0.0f;
            float gap = 16.0f * scale;
            this.timeBetweenChants = timeBetweenChants;

            foreach (string chant in chants)
            {
                ChantText text = new ChantText(chant, style);

                texts.Add(text);

                Vector2 size = font.MeasureString(chant) * scale;
                totalWidth += size.X + gap;
            }

            Left.Set((Main.screenWidth / Main.UIScale) / 2f - totalWidth / 2f, 0.0f);
            Top.Set((Main.screenHeight / Main.UIScale) / 2f + font.MeasureString("A").Y, 0.0f);

            float cursor = 0.0f;

            for (int i = 0; i < chants.Count; i++)
            {
                texts[i].Left.Set(cursor, 0.0f);
                texts[i].Top.Set(0.0f, 0.0f);

                Vector2 size = font.MeasureString(chants[i]) * scale;
                cursor += size.X + gap;
            }

            lifetime = timeBetweenChants * chants.Count + bufferTime;
        }

        public override void Update(GameTime gameTime)
        {
            if (tick++ >= lifetime)
            {
                SorceryFightUI sfUI = (SorceryFightUI)Parent;
                sfUI.RemoveElement(this);
                return;
            }

            int index = (int)(tick / timeBetweenChants) % texts.Count;
            if (!Elements.Contains(texts[index]))
                Append(texts[index]);


            base.Update(gameTime);
        }
    }
}