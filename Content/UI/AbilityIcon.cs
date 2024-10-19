using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.UI;

namespace sorceryFight.Content.UI
{
    public class AbilityIcon : UIElement
    {

        public Texture2D texture;
        public int abilityID;

        public AbilityIcon(Texture2D texture, int abilityID)
        {
            this.texture = texture;
            this.abilityID = abilityID;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);

            CalculatedStyle dimensions = GetDimensions();

            spriteBatch.Draw(texture, new Vector2(dimensions.X, dimensions.Y), Color.White);
        
            if (isHovering(dimensions))
            {   
                SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
                Main.hoverItemName = $"{sfPlayer.innateTechnique.CursedTechniques[abilityID].Name}";
            }

            if (Main.mouseLeft && Main.mouseLeftRelease && isHovering(dimensions))
            {
                Main.mouseLeftRelease = false;
                SoundEngine.PlaySound(SoundID.MenuTick);
                SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
                sfPlayer.selectedTechnique = sfPlayer.innateTechnique.CursedTechniques[abilityID];
                int index = CombatText.NewText(Main.LocalPlayer.getRect(), Color.LightYellow, $"Selected {sfPlayer.selectedTechnique.Name}");
				Main.combatText[index].lifeTime = 180;
            }
        }

        private bool isHovering(CalculatedStyle dimensions)
        {
            Vector2 mousePos = Main.MouseScreen;
            
            return mousePos.X >= dimensions.X && mousePos.X <= dimensions.X + texture.Width && 
                    mousePos.Y >= dimensions.Y && mousePos.Y <= dimensions.Y + texture.Height;
        }
    }
}
