using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.Buffs;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using sorceryFight.Content.DomainExpansions;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public class CursedTechniqueMenu : UIElement
    {
        SorceryFightPlayer player;
        MasteryBar masteryBar;
        public CursedTechniqueMenu(SorceryFightPlayer player)
        {
            this.player = player;
            Vector2 screenCenter = new Vector2(Main.screenWidth / 2, Main.screenHeight / 2);

            DisplayCursedTechniques(screenCenter);
            DisplayPassiveTechniques(screenCenter);
            DisplayDomainExpansionIcon(screenCenter);
            DisplayMasteryBar(screenCenter);

            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            masteryBar.fillPercentage = player.mastery / 100f;
        }

        private void DisplayCursedTechniques(Vector2 screenCenter)
        {
            List<CursedTechnique> techniques = player.innateTechnique.CursedTechniques;

            float iconOffset = 15f;
            float rotation = 2 * (float)Math.PI / techniques.Count;
            float magnatude = 60f;

            for (int i = 0; i < techniques.Count; i++)
            {
                Texture2D texture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueIcons/{player.innateTechnique.Name}_{i}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                AbilityIcon icon = new AbilityIcon(texture, i, AbilityIconType.CursedTechnique);

                if (player.selectedTechnique == techniques[i])
                    icon.selected = true;

                icon.Left.Set(screenCenter.X - magnatude * (float)Math.Cos(i * rotation) - iconOffset, 0f);
                icon.Top.Set(screenCenter.Y - magnatude * (float)Math.Sin(i * rotation) - iconOffset, 0f);

                icon.Recalculate();
                Append(icon);
            }
        }

        private void DisplayPassiveTechniques(Vector2 screenCenter)
        {
            List<PassiveTechnique> techniques = player.innateTechnique.PassiveTechniques;
            
            if (techniques.Count == 0)
                return;

            float textureSide = 32;
            float spacing = 40f;
            float totalHeight = (textureSide * techniques.Count) + (spacing * (techniques.Count - 1));
            float yOffset = totalHeight / techniques.Count;

            for (int i = 0; i < techniques.Count; i++)
            {
                Texture2D texture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/PassiveTechniqueIcons/{player.innateTechnique.Name}_{i}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                AbilityIcon icon = new AbilityIcon(texture, i, AbilityIconType.PassiveTechnique);

                if (techniques[i].isActive)
                {
                    icon.selected = true;
                }
                
                icon.Left.Set(screenCenter.X + 140f, 0f);
                icon.Top.Set(screenCenter.Y - yOffset + (i * spacing), 0f);

                Append(icon);
            }
        }

        private void DisplayDomainExpansionIcon(Vector2 screenCenter)
        {
            DomainExpansion de = player.innateTechnique.DomainExpansion;
            Texture2D texture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/DomainExpansionIcons/{de.DisplayName.Value}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            DomainExpansionIcon icon = new DomainExpansionIcon(texture);

            icon.Left.Set(screenCenter.X - 140f, 0f);
            icon.Top.Set(screenCenter.Y - texture.Height / 2, 0f);
            Append(icon);

        }
        private void DisplayMasteryBar(Vector2 screenCenter)
        {
            Texture2D masteryBorderTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/Mastery_Bar_Border", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D masteryBarTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/Mastery_Bar", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            UIImage masteryBorder = new UIImage(masteryBorderTexture);
            masteryBar = new MasteryBar(masteryBarTexture, Orientation.Horizontal);

            float x = screenCenter.X - masteryBorderTexture.Width / 2;
            float y = screenCenter.Y + 140f;

            masteryBorder.Left.Set(x, 0f);
            masteryBorder.Top.Set(y, 0f);

            masteryBar.Left.Set(x + ((masteryBorderTexture.Width - masteryBarTexture.Width) / 2), 0f);
            masteryBar.Top.Set(y, 0f);

            Append(masteryBorder);
            Append(masteryBar);
        }
    }
}
