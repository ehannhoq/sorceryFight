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
    public class CursedTechniqueSelector : UIElement
    {
        internal class TechniqueSelectorButton : SFButton
        {
            internal int id;

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

                //fall back on innateBGColor if there is no selector color in the Cursed Technique
                Color borderColor;
                if (sfPlayer.innateTechnique.CursedTechniques[id].selectorBorderColor != default)
                    borderColor = sfPlayer.innateTechnique.CursedTechniques[id].selectorBorderColor;
                else
                    borderColor = sfPlayer.innateTechnique.innateBorderColor;

                Color bgColor;
                if (sfPlayer.innateTechnique.CursedTechniques[id].selectorBGColor != default)
                    bgColor = sfPlayer.innateTechnique.CursedTechniques[id].selectorBGColor;
                else
                    bgColor = sfPlayer.innateTechnique.innateBGColor;

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.X, bgRect.Y - 2, bgRect.Width, 2), borderColor);
                // bottom border

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.X, bgRect.Bottom, bgRect.Width, 2), borderColor);
                // left border

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.X - 2, bgRect.Y - 2, 2, bgRect.Height + 4), borderColor);
                // right border

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(bgRect.Right, bgRect.Y - 2, 2, bgRect.Height + 4), borderColor);
                // top border

                spriteBatch.Draw(TextureAssets.MagicPixel.Value, bgRect, bgColor);

                //rewrite this later
                Color iconColor = sfPlayer.innateTechnique.CursedTechniques[id].UseCondition(sfPlayer)
                    ? Color.White
                    : Color.Gray;

                //spriteBatch.Draw(texture, new Vector2(dims.X, dims.Y), new Color(0,0,0,255));

                base.DrawSelf(spriteBatch);
            }

            public override void OnClick()
            {
                CursedTechniqueSelector p = (CursedTechniqueSelector)Parent;
                p.selectorIndex = p.GetIconIndex(id);
            }
        }

        internal const float DefaultCTSelectorPosX = 46.875f;
        internal const float DefaultCTSelectorPosY = 90.789f;
        private const float MouseDragEpsilon = 0.05f;

        //This button gap is needed because we expand the background behind each button
        internal const int ButtonGap = 12;

        private static Vector2? dragOffset = null;

        SorceryFightPlayer sfPlayer;
        List<TechniqueSelectorButton> icons;
        UIImage selectorIcon;
        Texture2D selectorTexture;
        internal int selectorIndex;
        int unlockedTechniques;
        public CursedTechniqueSelector()
        {
            if (Main.dedServ) return;

            icons = new List<TechniqueSelectorButton>();
            sfPlayer = Main.LocalPlayer.SorceryFight();

            selectorTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/TechniqueSelector/Selector", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            selectorIcon = new UIImage(selectorTexture);

            selectorIndex = 0;

            ReloadUI();

            SorceryFightUI.UpdateTechniqueUI += ReloadUI;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (selectorIndex == -1) return;

            if (Elements.Contains(selectorIcon))
            {
                selectorIcon.Left.Set((selectorIndex * (60f + ButtonGap)) - 6f, 0f);
                selectorIcon.Top.Set(-6f, 0f);
            }

            if (SFKeybinds.CycleSelectedTechniqueUp.JustPressed)
            {
                selectorIndex++;
                if (selectorIndex >= unlockedTechniques)
                {
                    selectorIndex = 0;
                }
                SoundEngine.PlaySound(SoundID.Mech with { Volume = 1f });
            }

            if (SFKeybinds.CycleSelectedTechniqueDown.JustPressed)
            {
                selectorIndex--;
                if (selectorIndex < 0)
                {
                    selectorIndex = unlockedTechniques - 1;
                }
                SoundEngine.PlaySound(SoundID.Mech with { Volume = 1f });
            }

            if (selectorIndex != -1)
                sfPlayer.selectedTechnique = sfPlayer.innateTechnique.CursedTechniques[icons[selectorIndex].id];
            
            #region Screen Position Logic
            Vector2 screenRatioPosition = new Vector2(
                 ModContent.GetInstance<ClientConfig>().CTSelectorPosX,
                 ModContent.GetInstance<ClientConfig>().CTSelectorPosY
             );

            if (screenRatioPosition.X < 0f || screenRatioPosition.X > 100f)
                screenRatioPosition.X = DefaultCTSelectorPosX;
            if (screenRatioPosition.Y < 0f || screenRatioPosition.Y > 100f)
                screenRatioPosition.Y = DefaultCTSelectorPosY;

            Vector2 screenPos;
            screenPos.X = (int)(screenRatioPosition.X * 0.01f * Main.screenWidth);
            screenPos.Y = (int)(screenRatioPosition.Y * 0.01f * Main.screenHeight);

            Left.Set(screenPos.X, 0f);
            Top.Set(screenPos.Y, 0f);
            Recalculate();

            MouseState ms = Mouse.GetState();
            Vector2 mousePos = Main.MouseScreen;

            if (HoveringOverUI())
            {
                if (!ModContent.GetInstance<ClientConfig>().CTSelectorPosLock)
                    Main.LocalPlayer.mouseInterface = true;

                Vector2 newScreenRatioPosition = screenRatioPosition;

                if (!ModContent.GetInstance<ClientConfig>().CTSelectorPosLock && ms.LeftButton == ButtonState.Pressed)
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
                    ModContent.GetInstance<ClientConfig>().CTSelectorPosX = newScreenRatioPosition.X;
                    ModContent.GetInstance<ClientConfig>().CTSelectorPosY = newScreenRatioPosition.Y;
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
            #endregion
        }

        void ReloadUI()
        {
            SorceryFightUI.UpdateTechniqueUI -= ReloadUI;

            Elements.Clear();
            icons.Clear();
            unlockedTechniques = 0;
            selectorIndex = -1;

            for (int i = 0; i < sfPlayer.innateTechnique.CursedTechniques.Count; i++)
            {
                Texture2D ctTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/TechniqueSelector/{sfPlayer.innateTechnique.Name}/c{i}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                string ctHoverText = $"{sfPlayer.innateTechnique.CursedTechniques[i].DisplayName.Value}\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.CursedEnergyBar.ToolTip")}";
                TechniqueSelectorButton ctIcon = new TechniqueSelectorButton(ctTexture, ctHoverText, i);

                if (sfPlayer.innateTechnique.CursedTechniques[i].Unlocked(sfPlayer))
                {
                    ctIcon.Left.Set(unlockedTechniques * (ctIcon.texture.Width + ButtonGap), 0f);
                    ctIcon.Top.Set(0f, 0f);
                    Append(ctIcon);
                    unlockedTechniques++;
                    icons.Add(ctIcon);
                }
            }

            Width.Set(unlockedTechniques * 60f + (unlockedTechniques - 1) * ButtonGap, 0f);
            Height.Set(60f, 0f);
            Recalculate();

            if (unlockedTechniques > 0)
            {
                if (selectorIndex == -1)
                    selectorIndex = 0;

                if (!Elements.Contains(selectorIcon))
                    Append(selectorIcon);
            }
            else
            {
                selectorIndex = -1;
                if (Elements.Contains(selectorIcon))
                    Elements.Remove(selectorIcon);
            }

            Recalculate();
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

        int GetIconIndex(int iconID)
        {
            for (int i = 0; i < icons.Count; i++)
            {
                if (icons[i].id == iconID) return i;
            }
            return -1;
        }
    }
}


