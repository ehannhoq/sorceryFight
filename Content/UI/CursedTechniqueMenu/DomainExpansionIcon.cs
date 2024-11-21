using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public class DomainExpansionIcon : UIElement
    {
        public Texture2D texture;
        public Texture2D lockedTexture;
        public bool unlocked;

        public DomainExpansionIcon(Texture2D texture)
        {
            this.texture = texture;
            lockedTexture = ModContent.Request<Texture2D>("sorceryfight/Content/UI/CursedTechniqueMenu/LockedIcon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            unlocked = false;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
            CalculatedStyle dimensions = GetDimensions();

            SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            unlocked = sfPlayer.UnlockedDomain;

            if (SorceryFightUI.MouseHovering(this, texture))
            {
                Main.hoverItemName = unlocked ? sfPlayer.innateTechnique.DomainExpansion.Name + "\n" 
                                                + sfPlayer.innateTechnique.DomainExpansion.Description 
                
                                                : sfPlayer.innateTechnique.DomainExpansion.LockedDescription;
            }

            Texture2D textureToDraw = unlocked ? texture : lockedTexture;

            spriteBatch.Draw(textureToDraw, new Vector2(dimensions.X, dimensions.Y), Color.White);
        }
    }
}