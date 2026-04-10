using System;
using System.Collections.Generic;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public class TechniqueIcon : SpecialUIElement
    {
        internal Texture2D lockedTexture;
        bool unlocked;
        string hoverText;
        internal List<SpecialUIElement> parents;

        public TechniqueIcon(Texture2D texture, bool unlocked, string hoverText) : base(texture)
        {
            this.unlocked = unlocked;
            this.hoverText = hoverText;
            parents = new List<SpecialUIElement>();
            lockedTexture = ModContent.Request<Texture2D>($"sorceryFight/Content/UI/CursedTechniqueMenu/LockedIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public void DrawLines()
        {
            CursedTechniqueMenu ctMenu = (CursedTechniqueMenu)Parent.Parent;
            Color finalColor = unlocked ? Color.White : Color.Gray;
            foreach (SpecialUIElement parent in parents)
            {
                Vector2 thisCenter = new Vector2(ctMenu.Left.Pixels + Left.Pixels + (texture.Width / 2), ctMenu.Top.Pixels + Top.Pixels + (texture.Height / 2));
                Vector2 parentCenter = new Vector2(ctMenu.Left.Pixels + parent.Left.Pixels + (parent.texture.Width / 2), ctMenu.Top.Pixels + parent.Top.Pixels + (parent.texture.Height / 2));

                Main.spriteBatch.DrawLineUI(thisCenter, parentCenter, finalColor, 3f);
            }
        }

        public void DrawIcon()
        {

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);

            if (SorceryFightUI.MouseHovering(this, texture))
                Main.hoverItemName = hoverText;

            CalculatedStyle dim = GetDimensions();
            Texture2D finalTexture = unlocked ? texture : lockedTexture;
            Color finalColor = unlocked ? Color.White : Color.Gray;

            Main.spriteBatch.Draw(finalTexture, new Vector2(dim.X, dim.Y), finalColor);
        }
    }
}
