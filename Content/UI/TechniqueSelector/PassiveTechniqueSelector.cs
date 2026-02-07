using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
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
                base.DrawSelf(spriteBatch);

                CalculatedStyle dim = GetDimensions();

                Color finalColor = Color.White;

                if (sfPlayer.innateTechnique.PassiveTechniques[id].isActive)
                    finalColor = Color.Gray;


                spriteBatch.Draw(texture, new Vector2(dim.X, dim.Y), finalColor);
            }


            public override void OnClick()
            {
                sfPlayer.innateTechnique.PassiveTechniques[id].isActive = !sfPlayer.innateTechnique.PassiveTechniques[id].isActive;
                SoundEngine.PlaySound(SoundID.Mech with { Volume = 1f });
            }
        }
        SorceryFightPlayer sfPlayer;
        List<TechniqueSelectorButton> icons;
        int unlockedTechniques;
        bool isDragging;
        bool hasRightClicked;
        Vector2 offset;
        public PassiveTechniqueSelector()
        {
            if (Main.dedServ) return;

            icons = new List<TechniqueSelectorButton>();
            sfPlayer = Main.LocalPlayer.SorceryFight();
            isDragging = false;

            ReloadUI();
            SetPosition();
            SorceryFightUI.UpdateTechniqueUI += ReloadUI;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (Main.playerInventory && HoveringOverUI() && Main.mouseLeft && !isDragging)
            {
                isDragging = true;
                offset = new Vector2(Main.mouseX, Main.mouseY) - new Vector2(Left.Pixels, Top.Pixels);
            }

            if (isDragging)
            {
                float clampedLeft = Math.Clamp(Main.mouseX - offset.X, 10, Main.screenWidth - 60 - 10);
                float clampedTop = Math.Clamp(Main.mouseY - offset.Y, 10, Main.screenHeight - 60 * unlockedTechniques - 10);

                Left.Set(clampedLeft, 0f);
                Top.Set(clampedTop, 0f);

                Recalculate();

                if (!Main.mouseLeft)
                {
                    isDragging = false;
                    Recalculate();
                }
            }

            if (Left.Pixels >= Main.screenWidth || Top.Pixels >= Main.screenHeight)
            {
                SetPosition();
            }

            if (Main.playerInventory && HoveringOverUI() && Main.mouseRight && !isDragging)
            {
                Rectangle mouseRect = new Rectangle((int)Main.MouseWorld.X - 8, (int)Main.MouseWorld.Y - 8, 16, 16);
                if (!hasRightClicked)
                {
                    if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    {
                        sfPlayer.PTSelectorPos = Vector2.Zero;
                        SetPosition();
                        CombatText.NewText(mouseRect, Color.White, "UI Position Reset!");
                        Main.mouseRightRelease = true;
                    }
                    else
                    {
                        sfPlayer.PTSelectorPos = new Vector2(this.Left.Pixels, this.Top.Pixels);
                        CombatText.NewText(mouseRect, Color.White, "UI Position Saved!");
                        Main.mouseRightRelease = true;
                    }
                }

            }

            if (Main.mouseRight && HoveringOverUI())
            {
                hasRightClicked = true;
            }
            else if (Main.mouseRightRelease && HoveringOverUI())
            {
                hasRightClicked = false;
            }
        }

        void ReloadUI()
        {
            Elements.Clear();
            icons.Clear();
            unlockedTechniques = 0;

            for (int i = 0; i < sfPlayer.innateTechnique.PassiveTechniques.Count; i++)
            {
                Texture2D ptTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/TechniqueSelector/{sfPlayer.innateTechnique.Name}/p{i}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                string ptHoverText = $"{sfPlayer.innateTechnique.PassiveTechniques[i].DisplayName.Value}\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.CursedEnergyBar.ToolTip")}";
                TechniqueSelectorButton ptIcon = new TechniqueSelectorButton(ptTexture, ptHoverText, i);
                icons.Add(ptIcon);

                if (sfPlayer.innateTechnique.PassiveTechniques[i].Unlocked(sfPlayer))
                {
                    ptIcon.Left.Set(0f, 0f);
                    ptIcon.Top.Set(unlockedTechniques * ptIcon.texture.Height, 0f);
                    unlockedTechniques++;
                    Append(ptIcon);
                }
            }

            Recalculate();
            SetPosition();
        }

        void SetPosition()
        {
            if (sfPlayer.PTSelectorPos == Vector2.Zero)
            {
                Left.Set(110f, 0f);
                Top.Set(Main.screenHeight - (unlockedTechniques * 60) - 50f, 0f);
            }
            else
            {
                Left.Set(sfPlayer.PTSelectorPos.X, 0f);
                Top.Set(sfPlayer.PTSelectorPos.Y, 0f);
            }

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


