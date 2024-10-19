using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public class CursedTechniqueMenu : UIElement
    {
        SorceryFightPlayer player;
        MasteryBar masteryBar;
        public CursedTechniqueMenu(SorceryFightPlayer player)
        {
            this.player = player;

            List<CursedTechnique> techniques = player.innateTechnique.CursedTechniques;

            Vector2 screenCenter = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);
            
            float iconOffset = 15f;
            float rotation = 2 * (float)Math.PI / techniques.Count;
            float magnatude = 60f;

            for (int i = 0; i < techniques.Count; i++)
            {
                Texture2D texture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/{player.innateTechnique.Name}/{i}").Value;
                AbilityIcon icon = new AbilityIcon(texture, i);

                icon.Left.Set(screenCenter.X + magnatude * (float)Math.Cos(i * rotation) - iconOffset, 0f);
                icon.Top.Set(screenCenter.Y + magnatude * (float)Math.Sin(i * rotation) - iconOffset, 0f);

                Append(icon);
            }
            

            Texture2D masteryBorderTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/Mastery_Bar_Border").Value;
            Texture2D masteryBarTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/Mastery_Bar").Value;

            UIImage masteryBorder = new UIImage(masteryBorderTexture);
            masteryBar = new MasteryBar(masteryBarTexture);

            float x = screenCenter.X - masteryBorderTexture.Width / 2;
            float y = screenCenter.Y + 140f;

            masteryBorder.Left.Set(x, 0f);
            masteryBorder.Top.Set(y, 0f);

            masteryBar.Left.Set(x + ((masteryBorderTexture.Width - masteryBarTexture.Width) / 2), 0f);
            masteryBar.Top.Set(y, 0f);

            Append(masteryBorder);
            Append(masteryBar);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            masteryBar.fillPercentage = player.mastery / 100f;
        }
    }
}
