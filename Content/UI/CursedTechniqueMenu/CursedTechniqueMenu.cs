using System;
using sorceryFight.SFPlayer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Microsoft.Xna.Framework.Graphics;

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

        SorceryFightPlayer player;
        Texture2D borderTexture;
        Texture2D closeButtonTexture;
        SpecialUIElement moveButton;
        Texture2D moveButtonTexture;
        CursedTechniqueTree ctTree;
        bool isDragging;
        Vector2 offset;

        public CursedTechniqueMenu(SorceryFightPlayer player)
        {
            this.player = player;
            isDragging = false;

            Texture2D treeBGTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/{player.innateTechnique.Name}/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            borderTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGBorder", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            closeButtonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGCloseButton", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            moveButtonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGMoveButton", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            float left = Main.screenWidth - borderTexture.Width - 300f;
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
                float clampedLeft = Math.Clamp(Main.mouseX - offset.X, 6f, Main.screenWidth - borderTexture.Width);
                float clampedTop = Math.Clamp(Main.mouseY - offset.Y, 0, Main.screenHeight - borderTexture.Height - closeButtonTexture.Height - 6f);

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
