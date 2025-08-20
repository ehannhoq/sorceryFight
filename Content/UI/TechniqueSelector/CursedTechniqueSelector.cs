using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques;
using sorceryFight.Content.UI.CursedTechniqueMenu;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
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
            public TechniqueSelectorButton(Texture2D texture, string hoverText, int id) : base(texture, hoverText)
            {
                this.id = id;
                Width.Set(texture.Width, 0f);
                Height.Set(texture.Height, 0f);
            }

            public override void OnClick()
            {
                CursedTechniqueSelector p = (CursedTechniqueSelector)Parent;
                p.selectorIndex = p.GetIconIndex(id);
            }
        }
        SorceryFightPlayer sfPlayer;
        List<TechniqueSelectorButton> icons;
        UIImage selectorIcon;
        Texture2D selectorTexture;
        internal int selectorIndex;
        int unlockedTechniques;
        bool isDragging;
        bool hasRightClicked;
        Vector2 offset;
        public CursedTechniqueSelector()
        {
            if (Main.dedServ) return;

            icons = new List<TechniqueSelectorButton>();
            sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            isDragging = false;

            selectorTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/TechniqueSelector/Selector", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            selectorIcon = new UIImage(selectorTexture);

            selectorIndex = 0;

            ReloadUI();
            SetPosition();

            SorceryFightUI.UpdateTechniqueUI += ReloadUI;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (selectorIndex == -1) return;

            if (Elements.Contains(selectorIcon))
            {
                selectorIcon.Left.Set((selectorIndex * 60f) - 6f, 0f);
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

            if (Main.playerInventory && HoveringOverUI() && Main.mouseLeft && !isDragging)
            {
                isDragging = true;
                offset = new Vector2(Main.mouseX, Main.mouseY) - new Vector2(Left.Pixels, Top.Pixels);
            }

            if (Main.playerInventory && HoveringOverUI() && Main.mouseRight && !isDragging)
            {
                Rectangle mouseRect = new Rectangle((int)Main.MouseWorld.X - 8, (int)Main.MouseWorld.Y - 8, 16, 16);
                if (!hasRightClicked)
                {
                    if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift))
                    {
                        sfPlayer.CTSelectorPos = Vector2.Zero;
                        SetPosition();
                        CombatText.NewText(mouseRect, Color.White, "UI Position Reset!");
                        Main.mouseRightRelease = true;
                    }
                    else
                    {
                        sfPlayer.CTSelectorPos = new Vector2(this.Left.Pixels, this.Top.Pixels);
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


            if (isDragging)
            {
                float clampedLeft = Math.Clamp(Main.mouseX - offset.X, 10, Main.screenWidth - (selectorTexture.Width - 12) * unlockedTechniques - 10);
                float clampedTop = Math.Clamp(Main.mouseY - offset.Y, 10, Main.screenHeight - (selectorTexture.Height - 12) - 10);

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
        }

        void ReloadUI()
        {
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
                    ctIcon.Left.Set(unlockedTechniques * ctIcon.texture.Width, 0f);
                    ctIcon.Top.Set(0f, 0f);
                    Append(ctIcon);
                    unlockedTechniques++;
                    icons.Add(ctIcon);
                }
            }

            if (unlockedTechniques > 0)
            {

                if (selectorIndex == -1)
                    selectorIndex = 0;

                if (!Elements.Contains(selectorIcon))
                {
                    Append(selectorIcon);
                }
            }
            else
            {
                selectorIndex = -1;
                if (Elements.Contains(selectorIcon))
                    Elements.Remove(selectorIcon);
            }

            Recalculate();
        }

        void SetPosition()
        {
            if (sfPlayer.CTSelectorPos == Vector2.Zero)
            {
                Left.Set(Main.screenWidth / 2 - (unlockedTechniques * 60 / 2), 0f);
                Top.Set(Main.screenHeight - 110f, 0f);
            }
            else
            {
                Left.Set(sfPlayer.CTSelectorPos.X, 0f);
                Top.Set(sfPlayer.CTSelectorPos.Y, 0f);
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


