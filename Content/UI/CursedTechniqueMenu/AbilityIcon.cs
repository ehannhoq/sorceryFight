using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.PassiveTechniques;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.CursedTechniqueMenu
{
    public enum AbilityIconType
    {
        PassiveTechnique,
        CursedTechnique
    }
    public class AbilityIcon : UIElement
    {

        public Texture2D texture;
        public int abilityID;
        public AbilityIconType type;
        public bool clicked;

        public AbilityIcon(Texture2D texture, int abilityID, AbilityIconType type)
        {
            this.texture = texture;
            this.abilityID = abilityID;
            this.type = type;
            clicked = false;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);

            CalculatedStyle dimensions = GetDimensions();

            Color color = new Color(255, 255, 255);

            if (clicked)
            {
                color = new Color(120, 120, 120);
            }

            spriteBatch.Draw(texture, new Vector2(dimensions.X, dimensions.Y), color);
            SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();

            if (type == AbilityIconType.CursedTechnique)
            {
                if (sfPlayer.selectedTechnique == sfPlayer.innateTechnique.CursedTechniques[abilityID])
                {
                    clicked = true;
                }
                else
                {
                    clicked = false;
                }
            }
        
            if (SorceryFightUI.MouseHovering(this, texture))
            {   
                switch (type)
                {
                    case AbilityIconType.PassiveTechnique:
                        Main.hoverItemName = $"{sfPlayer.innateTechnique.PassiveTechniques[abilityID].Name}\n"
                                            + $"{sfPlayer.innateTechnique.PassiveTechniques[abilityID].Stats}\n"
                                            + $"{sfPlayer.innateTechnique.PassiveTechniques[abilityID].Description.Value}";
                        break;

                    case AbilityIconType.CursedTechnique:
                        Main.hoverItemName = $"{sfPlayer.innateTechnique.CursedTechniques[abilityID].Name}\n"
                                            + $"{sfPlayer.innateTechnique.CursedTechniques[abilityID].Stats}\n"
                                            + $"{sfPlayer.innateTechnique.CursedTechniques[abilityID].Description}";
                        break;
                }

            }

            if (Main.mouseLeft && Main.mouseLeftRelease && SorceryFightUI.MouseHovering(this, texture))
            {
                Main.mouseLeftRelease = false;
                SoundEngine.PlaySound(SoundID.MenuTick);
                clicked = !clicked;
                sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();

                if (sfPlayer.Player.HasBuff<BurntTechnique>())
                {
                    clicked = false;
                    int index1 = CombatText.NewText(Main.LocalPlayer.getRect(), Color.DarkRed, "You can't use this technique right now!");
                    Main.combatText[index1].lifeTime = 180;
                    return;
                }

                int index = 0;

                switch (type)
                {
                    case AbilityIconType.PassiveTechnique:
                        sfPlayer.innateTechnique.PassiveTechniques[abilityID].isActive = !sfPlayer.innateTechnique.PassiveTechniques[abilityID].isActive;
                        string text = "";
                        if (sfPlayer.innateTechnique.PassiveTechniques[abilityID].isActive)
                            text = $"Activated {sfPlayer.innateTechnique.PassiveTechniques[abilityID].Name}";
                        else
                            text = $"Deactivated {sfPlayer.innateTechnique.PassiveTechniques[abilityID].Name}";

              
                        index = CombatText.NewText(Main.LocalPlayer.getRect(), Color.LightYellow, text);
                        break;

                    case AbilityIconType.CursedTechnique:
                        sfPlayer.selectedTechnique = sfPlayer.innateTechnique.CursedTechniques[abilityID];
                        index = CombatText.NewText(Main.LocalPlayer.getRect(), Color.LightYellow, $"Selected {sfPlayer.selectedTechnique.Name}");
                        break;
                }
                
				Main.combatText[index].lifeTime = 180;
            }
        }
    }
}
