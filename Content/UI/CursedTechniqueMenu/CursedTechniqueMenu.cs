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
using Terraria.ID;

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
        bool isDragging;
        Vector2 offset;
        SorceryFightPlayer sfPlayer;
        bool isInitialized;


        public CursedTechniqueMenu(SorceryFightPlayer sfPlayer)
        {
            if (Main.dedServ) return;

            isDragging = false;
            this.sfPlayer = sfPlayer;
            isInitialized = false;

            Texture2D treeBGTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/{sfPlayer.innateTechnique.Name}/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            borderTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGBorder", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            closeButtonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGCloseButton", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            moveButtonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGMoveButton", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

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

            AddIcons();

            Recalculate();
        }

        private void AddIcons()
        {
            Texture2D masteryIconTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/BossKillsIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D rctIconTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/RCTIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D simpleDomainIconTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/SimpleDomainIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D domainIconTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/{sfPlayer.innateTechnique.Name}/DomainIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Texture2D lockedTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/SpecialLockedIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            string masteryIconHoverText = $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.MasteryIcon.Info")}" +
                             $"\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.MasteryIcon.BossesDefeated")} {sfPlayer.bossesDefeated.Count}" +
                             $"\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.MasteryIcon.CostReduction")} {sfPlayer.bossesDefeated.Count}%";
            SpecialUIElement masteryIcon = new SpecialUIElement(masteryIconTexture, masteryIconHoverText);
            masteryIcon.Left.Set(borderTexture.Width - masteryIconTexture.Width - 28f, 0f);
            masteryIcon.Top.Set(closeButtonTexture.Height + 34f, 0f);
            Append(masteryIcon);


            List<Vector2> conditionalIconPositions = new List<Vector2>();
            int conditionalIconsCount = 5;
            int conditionalIconSize = 40;
            for (int i = 0; i < conditionalIconsCount; i++)
            {
                Vector2 pos = new Vector2(borderTexture.Width - conditionalIconSize - 28f, closeButtonTexture.Height + 34f + (i + 1) * (conditionalIconSize + 6f));
                conditionalIconPositions.Add(pos);
            }

            int index = 0;

            if (sfPlayer.sukunasFingerConsumed >= 1)
            {
                Texture2D sukunasFingerTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/SukunasFingerIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                string sukunasFingerHoverText = $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.SukunasFingerIcon.Info")}\n{sfPlayer.sukunasFingerConsumed} {SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.SukunasFingerIcon.Consumed")}";
                SpecialUIElement sukunasFingerIcon = new SpecialUIElement(sukunasFingerTexture, sukunasFingerHoverText);
                sukunasFingerIcon.Left.Set(conditionalIconPositions[index].X, 0f);
                sukunasFingerIcon.Top.Set(conditionalIconPositions[index].Y, 0f);
                Append(sukunasFingerIcon);
                index++;
            }

            string rctIconHoverText = sfPlayer.unlockedRCT ? $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.RCTIcon.Info")}" + $"\n{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.RCTIcon.ContinuousRCT.Info")}"
             : $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.RCTIcon.LockedInfo")}";
            Texture2D finalRCTTexture = sfPlayer.unlockedRCT ? ref rctIconTexture : ref lockedTexture;
            SpecialUIElement rctIcon = new SpecialUIElement(finalRCTTexture, rctIconHoverText);
            rctIcon.Left.Set(conditionalIconPositions[index].X, 0f);
            rctIcon.Top.Set(conditionalIconPositions[index].Y, 0f);
            Append(rctIcon);
            index++;

            string simpleDomainIconHoverText = sfPlayer.HasDefeatedBoss(NPCID.CultistBoss) ? $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.SimpleDomainIcon.Info")}"
            : $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.UI.SimpleDomainIcon.LockedInfo")}";
            Texture2D finalSimpleDomainTexture = sfPlayer.innateTechnique.DomainExpansion.Unlocked(sfPlayer) ? ref simpleDomainIconTexture : ref lockedTexture;
            SpecialUIElement simpleDomainIcon = new SpecialUIElement(finalSimpleDomainTexture, simpleDomainIconHoverText);
            simpleDomainIcon.Left.Set(conditionalIconPositions[index].X, 0f);
            simpleDomainIcon.Top.Set(conditionalIconPositions[index].Y, 0f);
            Append(simpleDomainIcon);
            index++;

            string domainIconHoverText = sfPlayer.innateTechnique.DomainExpansion.Unlocked(sfPlayer) ? $"{sfPlayer.innateTechnique.DomainExpansion.DisplayName}\n{sfPlayer.innateTechnique.DomainExpansion.Description}"
             : $"{sfPlayer.innateTechnique.DomainExpansion.LockedDescription}";
            Texture2D finalDomainTexture = sfPlayer.innateTechnique.DomainExpansion.Unlocked(sfPlayer) ? ref domainIconTexture : ref lockedTexture;
            SpecialUIElement domainIcon = new SpecialUIElement(finalDomainTexture, domainIconHoverText);
            domainIcon.Left.Set(conditionalIconPositions[index].X, 0f);
            domainIcon.Top.Set(conditionalIconPositions[index].Y, 0f);
            Append(domainIcon);
            index++;

        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (Main.dedServ) return;

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
