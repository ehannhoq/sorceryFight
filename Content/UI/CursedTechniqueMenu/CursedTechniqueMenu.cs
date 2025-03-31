using System;
using sorceryFight.SFPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public class CursedTechniqueMenu : UIElement
    {
        internal class CTMenuCloseButton : SFButton
        {
            public CTMenuCloseButton(Texture2D texture, string hoverText) : base(texture, hoverText) { }

            public override void OnClick()
            {
                var ctMenu = (CursedTechniqueMenu)Parent;
                var sfUI = (SorceryFightUI)ctMenu.Parent;
                sfUI.RemoveElement(ctMenu);
            }
        }

        Texture2D borderTexture;
        Texture2D closeButtonTexture;
        SpecialUIElement moveButton;
        Texture2D moveButtonTexture;
        CursedTechniqueTree ctTree;
        bool isDragging;
        Vector2 offset;

        public CursedTechniqueMenu(SorceryFightPlayer sfPlayer)
        {
            if (Main.dedServ) return;

            isDragging = false;

            Texture2D treeBGTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/{sfPlayer.innateTechnique.Name}/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            borderTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGBorder", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            closeButtonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGCloseButton", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            moveButtonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGMoveButton", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D masteryIconTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/BossKillsIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D rctIconTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/RCTIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D domainIconTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/{sfPlayer.innateTechnique.Name}/DomainIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float left = 100f - 6f - closeButtonTexture.Height;
            float top = Main.screenHeight - borderTexture.Height - 100f;
            Left.Set(left, 0f);
            Top.Set(top, 0f);

            UIImage background = new UIImage(borderTexture);
            CTMenuCloseButton closeButton = new CTMenuCloseButton(closeButtonTexture, "");
            moveButton = new SpecialUIElement(moveButtonTexture, "Drag me!");

            background.Left.Set(0f, 0f);
            background.Top.Set(closeButtonTexture.Height + 6f, 0f);
            closeButton.Left.Set(0f, 0f);
            closeButton.Top.Set(0f, 0f);
            moveButton.Left.Set(closeButtonTexture.Width + 12f, 0f);
            moveButton.Top.Set(closeButtonTexture.Height / 2f - moveButtonTexture.Height / 2f, 0f);

            Append(background);
            Append(closeButton);
            Append(moveButton);

            CursedTechniqueTree ctTree = new CursedTechniqueTree(closeButtonTexture, treeBGTexture);
            ctTree.Left.Set(0f, 0f);
            ctTree.Top.Set(0f, 0f);
            Append(ctTree);

            string masteryIconHoverText = $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.MasteryIcon.Info")}" +
                                        $"\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.MasteryIcon.BossesDefeated")} {sfPlayer.bossesDefeated.Count}" +
                                        $"\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.MasteryIcon.CostReduction")} {sfPlayer.bossesDefeated.Count}%";
            SpecialUIElement masteryIcon = new SpecialUIElement(masteryIconTexture, masteryIconHoverText);
            masteryIcon.Left.Set(borderTexture.Width - masteryIconTexture.Width - 28f, 0f);
            masteryIcon.Top.Set(closeButtonTexture.Height + 34f, 0f);
            Append(masteryIcon);

            List<Vector2> conditionalIconPositions = new List<Vector2>();
            int conditionalIconsCount = 3;
            int conditionalIconSize = 40;
            for (int i = 0; i < conditionalIconsCount; i++)
            {
                Vector2 pos = new Vector2(borderTexture.Width - conditionalIconSize - 28f, closeButtonTexture.Height + 34f + (i + 1) * (conditionalIconSize + 6f));
                conditionalIconPositions.Add(pos);
            }

            int conditionalIconPosUsed = 0;

            if (sfPlayer.sukunasFingerConsumed > 0)
            {
                Texture2D sukunasFingerTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/SukunasFingerIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                string sukunasFingerHoverText = $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.SukunasFingerIcon.Info")}\n{sfPlayer.sukunasFingerConsumed} {SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.SukunasFingerIcon.Consumed")}";
                SpecialUIElement sukunasFingerIcon = new SpecialUIElement(sukunasFingerTexture, sukunasFingerHoverText);
                sukunasFingerIcon.Left.Set(conditionalIconPositions[0 + conditionalIconPosUsed].X, 0f);
                sukunasFingerIcon.Top.Set(conditionalIconPositions[0 + conditionalIconPosUsed].Y, 0f);
                Append(sukunasFingerIcon);
                conditionalIconPosUsed++;
            }
            
            if (sfPlayer.unlockedRCT)
            {
                string rctIconHoverText = $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.RCTIcon.Info")}" +
                                        $"\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.RCTIcon.ContinuousRCT.Info")}" +
                                        $"\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.RCTIcon.ContinuousRCT.Keybind")} {SFKeybinds.UseRCT.GetAssignedKeys()[sfPlayer.Player.whoAmI]}";
                SpecialUIElement rctIcon = new SpecialUIElement(rctIconTexture, rctIconHoverText);
                rctIcon.Left.Set(conditionalIconPositions[0 + conditionalIconPosUsed].X, 0f);
                rctIcon.Top.Set(conditionalIconPositions[0 + conditionalIconPosUsed].Y, 0f);
                Append(rctIcon);
                conditionalIconPosUsed++;
            }

            if (sfPlayer.UnlockedDomain)
            {
                string domainIconHoverText = $"{sfPlayer.innateTechnique.DomainExpansion.DisplayName.Value}\n{sfPlayer.innateTechnique.DomainExpansion.Description}";
                SpecialUIElement domainIcon = new SpecialUIElement(domainIconTexture, domainIconHoverText);
                domainIcon.Left.Set(conditionalIconPositions[0 + conditionalIconPosUsed].X, 0f);
                domainIcon.Top.Set(conditionalIconPositions[0 + conditionalIconPosUsed].Y, 0f);
                Append(domainIcon);
            }


            Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool isHovering = SorceryFightUI.MouseHovering(moveButton, moveButtonTexture);

            if (isHovering && Main.mouseLeft && !isDragging)
            {
                isDragging = true;
                offset = new Vector2(Main.mouseX, Main.mouseY) - new Vector2(Left.Pixels, Top.Pixels);
            }

            if (isDragging)
            {
                float clampedLeft = Math.Clamp(Main.mouseX - offset.X, 0f, Main.screenWidth - borderTexture.Width);
                float clampedTop = Math.Clamp(Main.mouseY - offset.Y, 6f, Main.screenHeight - borderTexture.Height - closeButtonTexture.Height - 6f);

                Left.Set(clampedLeft, 0f);
                Top.Set(clampedTop, 0f);

                Recalculate();

                if (!Main.mouseLeft)
                {
                    isDragging = false;
                    Recalculate();
                }
            }
        }
    }
}
