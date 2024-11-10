using System;
using System.Collections.Generic;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.InnateTechniques;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.InnateTechniqueSelector
{
    public class InnateTechniqueSelector : UIElement
    {
        private List<Vector2> iconPositions;
        private int timeCounter;
        bool animate;
        public InnateTechniqueSelector()
        {
            iconPositions = new List<Vector2>();
            timeCounter = 0;
            animate = false;

            Vector2 screenCenter = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            float magnatude = 60f;
            float rotation = 2 * (float)Math.PI / InnateTechnique.InnateTechniques.Count;
            for (int i = 0; i < InnateTechnique.InnateTechniques.Count; i++)
            {
                iconPositions.Add(new Vector2(
                    screenCenter.X - magnatude * (float)Math.Cos(i * rotation),
                    screenCenter.Y - magnatude * (float)Math.Sin(i * rotation)
                ));
            }

            
            UIText title = new UIText("Choose your Innate Technique.", 1.5f, false);
            title.Left.Set(screenCenter.X - 180f, 0f);
            title.Top.Set(screenCenter.Y - 150, 0f);
            Append(title);

            DrawLimitless();

            Recalculate();
        }

        private void DrawLimitless()
        {
                InnateTechnique t = new LimitlessTechnique();
                Texture2D iconTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{t.Name}_Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                Texture2D backgroundTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/InnateTechniqueSelector/{t.Name}_BG", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                
                SpecialUIElement background = new SpecialUIElement(backgroundTexture, -1f, 0.05f);
                background.Left.Set(iconPositions[0].X, 0f);
                background.Top.Set(iconPositions[0].Y, 0f);
                
                TechnqiueButton button = new TechnqiueButton(iconTexture, t.Name, t);
                button.Left.Set(iconPositions[0].X, 0f);
                button.Top.Set(iconPositions[0].Y, 0f);

                background.Recalculate();
                button.Recalculate();

                Append(background);
                Append(button);
        }
        public void OnClick()
        {
            SorceryFightUI sfUI = (SorceryFightUI)Parent;
            sfUI.LoadCEBar();
            sfUI.ToBeRemoved(this);
        }
    }
}
