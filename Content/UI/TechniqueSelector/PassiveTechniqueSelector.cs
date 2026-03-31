using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.UI.CursedTechniqueMenu;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.TechniqueSelector
{
    public class PassiveTechniqueSelector : UIElement
    {
        internal class TechniqueSelectorButton : SFButton
        {
            int id;
            SorceryFightPlayer sfPlayer;
            public TechniqueSelectorButton(Texture2D texture, string hoverText, int id) : base(texture, hoverText)
            {
                this.id = id;
                sfPlayer = Main.LocalPlayer.SorceryFight();
                Width.Set(texture.Width, 0f);
                Height.Set(texture.Height, 0f);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                CalculatedStyle dims = GetDimensions();
                Rectangle bgRect = dims.ToRectangle();
                bgRect.Inflate(4, 4);
                Rectangle borderRect = bgRect;
                borderRect.Inflate(2, 2);

                Color borderColor;
                if (sfPlayer.innateTechnique.PassiveTechniques[id].selectorBorderColor != default)
                    borderColor = sfPlayer.innateTechnique.PassiveTechniques[id].selectorBorderColor;
                else
                    borderColor = sfPlayer.innateTechnique.innateBorderColor;

                Color bgColor;
                if (sfPlayer.innateTechnique.PassiveTechniques[id].selectorBGColor != default)
                    bgColor = sfPlayer.innateTechnique.PassiveTechniques[id].selectorBGColor;
                else
                    bgColor = sfPlayer.innateTechnique.innateBGColor;

                //darken the background when the technique is active
                if (sfPlayer.innateTechnique.PassiveTechniques[id].isActive)
                    bgColor = new Color((int)(bgColor.R * 0.8f), (int)(bgColor.G * 0.8f), (int)(bgColor.B * 0.8f), bgColor.A);

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.X, bgRect.Y - 2, bgRect.Width, 2), borderColor);
                // bottom border
                
                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.X, bgRect.Bottom, bgRect.Width, 2), borderColor);
                // left border

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.X - 2, bgRect.Y - 2, 2, bgRect.Height + 4), borderColor);
                // right border

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.Right, bgRect.Y - 2, 2, bgRect.Height + 4), borderColor);
                // top border

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, bgRect, bgColor);


                base.DrawSelf(spriteBatch);
            }


            public override void OnClick()
            {
                sfPlayer.innateTechnique.PassiveTechniques[id].isActive = !sfPlayer.innateTechnique.PassiveTechniques[id].isActive;
                SoundEngine.PlaySound(SoundID.Mech with { Volume = 1f });
            }
        }
        internal const float DefaultPTSelectorPosX = 9.114583f;
        internal const float DefaultPTSelectorPosY = 50f;
        private const float MouseDragEpsilon = 0.05f;
        internal const int ButtonGap = 12;
        private static Vector2? dragOffset = null;
        SorceryFightPlayer sfPlayer;
        List<TechniqueSelectorButton> icons;
        int unlockedTechniques;
        public PassiveTechniqueSelector()
        {
            if (Main.dedServ) return;

            icons = new List<TechniqueSelectorButton>();
            sfPlayer = Main.LocalPlayer.SorceryFight();

            ReloadUI();

            SorceryFightUI.UpdateTechniqueUI += ReloadUI;
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (unlockedTechniques == 0) return;

            // --- Screen ratio position ---
            Vector2 screenRatioPosition = new Vector2(
                ModContent.GetInstance<ClientConfig>().PTSelectorPosX,
                ModContent.GetInstance<ClientConfig>().PTSelectorPosY
            );

            if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
                screenRatioPosition.X = DefaultPTSelectorPosX;
            if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
                screenRatioPosition.Y = DefaultPTSelectorPosY;

            Vector2 screenPos;
            screenPos.X = (int)(screenRatioPosition.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenRatioPosition.Y * 0.01f * Main.screenHeight);

            Left.Set(screenPos.X, 0f);
            Top.Set(screenPos.Y, 0f);
            Recalculate();

            // --- Mouse drag ---
            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            if (HoveringOverUI())
            {
                if (!ModContent.GetInstance<ClientConfig>().PTSelectorPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                Vector2 newScreenRatioPosition = screenRatioPosition;

                if (!ModContent.GetInstance<ClientConfig>().PTSelectorPosLock && ms.LeftButton == ButtonState.Pressed)
                {
                    if (!dragOffset.HasValue)
                        dragOffset = mousePos - screenPos;

                    Vector2 newCorner = mousePos - dragOffset.GetValueOrDefault(Vector2.Zero);
                    newScreenRatioPosition.X = (100f * newCorner.X) / Main.screenWidth;
                    newScreenRatioPosition.Y = (100f * newCorner.Y) / Main.screenHeight;
                }

                Vector2 delta = newScreenRatioPosition - screenRatioPosition;
                if (Math.Abs(delta.X) >= MouseDragEpsilon || Math.Abs(delta.Y) >= MouseDragEpsilon)
                {
                    ModContent.GetInstance<ClientConfig>().PTSelectorPosX = newScreenRatioPosition.X;
                    ModContent.GetInstance<ClientConfig>().PTSelectorPosY = newScreenRatioPosition.Y;
                }

                if (dragOffset.HasValue && ms.LeftButton == ButtonState.Released)
                {
                    dragOffset = null;
                    ModContent.GetInstance<ClientConfig>().SaveChanges();
                }
            }
            else
            {
                if (dragOffset.HasValue && ms.LeftButton == ButtonState.Released)
                {
                    dragOffset = null;
                    ModContent.GetInstance<ClientConfig>().SaveChanges();
                }
            }
        }

        void ReloadUI()
        {
            SorceryFightUI.UpdateTechniqueUI -= ReloadUI;

            Elements.Clear();
            icons.Clear();
            unlockedTechniques = 0;

            for (int i = 0; i < sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                if (sfPlayer.innateTechnique.PassiveTechniques[i].Unlocked(sfPlayer))
                {
                    Texture2D ptTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/TechniqueSelector/{sfPlayer.innateTechnique.Name}/p{i}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    string ptHoverText = $"{sfPlayer.innateTechnique.PassiveTechniques[i].DisplayName.Value}\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.CursedEnergyBar.ToolTip")}";
                    TechniqueSelectorButton ptIcon = new TechniqueSelectorButton(ptTexture, ptHoverText, i);
                    ptIcon.Left.Set(0f, 0f);
                    ptIcon.Top.Set(unlockedTechniques * (ptIcon.texture.Height + ButtonGap), 0f);
                    Append(ptIcon);
                    icons.Add(ptIcon);
                    unlockedTechniques++;
                }
            }

            Width.Set(60f, 0f);
            Height.Set(unlockedTechniques * 60f + (unlockedTechniques - 1) * ButtonGap, 0f);
            Recalculate();

            SorceryFightUI.UpdateTechniqueUI += ReloadUI;
        }

        bool HoveringOverUI()
        {
            foreach (TechniqueSelectorButton icon in icons)
            {
                if (!Elements.Contains(icon)) continue;
                if (SorceryFightUI.MouseHovering(icon, icon.texture)) return true;
            }
            return false;
        }
    }
}


