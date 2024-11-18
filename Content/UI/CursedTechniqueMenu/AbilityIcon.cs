using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
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
        public Texture2D lockedTexture;

        public int abilityID;
        public AbilityIconType type;
        public bool unlocked;
        public bool selected;

        public AbilityIcon(Texture2D texture, int abilityID, AbilityIconType type)
        {
            this.texture = texture;
            lockedTexture = ModContent.Request<Texture2D>("sorceryfight/Content/UI/CursedTechniqueMenu/Locked_Icon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            this.abilityID = abilityID;
            this.type = type;
            selected = false;
            unlocked = false;
        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Width.Set(texture.Width, 0f);
            Height.Set(texture.Height, 0f);
            CalculatedStyle dimensions = GetDimensions();

            Color color = new Color(255, 255, 255);

            if (selected)
            {
                color = new Color(120, 120, 120);
            }

            SorceryFightPlayer sfPlayer = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();

            if (sfPlayer.Player.HasBuff<BurntTechnique>())
            {
                selected = false;
            }

            switch (type)
            {
                case AbilityIconType.CursedTechnique:
                    DrawCursedTechnique(sfPlayer);
                    break;
                case AbilityIconType.PassiveTechnique:
                    DrawPassiveTechnique(sfPlayer);
                    break;
            }

            Texture2D finalTexture = unlocked ? texture : lockedTexture;

            spriteBatch.Draw(finalTexture, new Vector2(dimensions.X, dimensions.Y), color);
        }

        private void DrawCursedTechnique(SorceryFightPlayer sfPlayer)
        {
            unlocked = sfPlayer.innateTechnique.CursedTechniques[abilityID].Unlocked;
            selected = sfPlayer.selectedTechnique == sfPlayer.innateTechnique.CursedTechniques[abilityID];

            if (SorceryFightUI.MouseHovering(this, texture))
            {
                Main.hoverItemName = unlocked ? $"{sfPlayer.innateTechnique.CursedTechniques[abilityID].Name}\n"
                                        + $"{sfPlayer.innateTechnique.CursedTechniques[abilityID].Stats}\n"
                                        + $"{sfPlayer.innateTechnique.CursedTechniques[abilityID].Description}" : sfPlayer.innateTechnique.CursedTechniques[abilityID].LockedDescription;

                if (Main.mouseLeft && Main.mouseLeftRelease && unlocked)
                {
                    Main.mouseLeftRelease = false;
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    if (sfPlayer.Player.HasBuff<BurntTechnique>())
                    {
                        int index1 = CombatText.NewText(Main.LocalPlayer.getRect(), Color.DarkRed, "You can't use this technique right now!");
                        Main.combatText[index1].lifeTime = 180;
                        return;
                    }

                    sfPlayer.selectedTechnique = sfPlayer.innateTechnique.CursedTechniques[abilityID];
                    int index = CombatText.NewText(Main.LocalPlayer.getRect(), Color.LightYellow, $"Selected {sfPlayer.selectedTechnique.Name}");
                    Main.combatText[index].lifeTime = 180;
                }
            }
        }

        private void DrawPassiveTechnique(SorceryFightPlayer sfPlayer)
        {
            unlocked = sfPlayer.innateTechnique.PassiveTechniques[abilityID].Unlocked;
            selected = sfPlayer.innateTechnique.PassiveTechniques[abilityID].isActive;

            if (SorceryFightUI.MouseHovering(this, texture))
            {
                Main.hoverItemName = unlocked ? $"{sfPlayer.innateTechnique.PassiveTechniques[abilityID].Name}\n"
                                     + $"{sfPlayer.innateTechnique.PassiveTechniques[abilityID].Stats}\n"
                                     + $"{sfPlayer.innateTechnique.PassiveTechniques[abilityID].Description.Value}" : sfPlayer.innateTechnique.PassiveTechniques[abilityID].LockedDescription;

                if (Main.mouseLeft && Main.mouseLeftRelease && unlocked)
                {
                    Main.mouseLeftRelease = false;
                    SoundEngine.PlaySound(SoundID.MenuTick);

                    if (sfPlayer.Player.HasBuff<BurntTechnique>())
                    {
                        int index1 = CombatText.NewText(Main.LocalPlayer.getRect(), Color.DarkRed, "Your technique is exhausted!");
                        Main.combatText[index1].lifeTime = 180;
                        return;
                    }

                    sfPlayer.innateTechnique.PassiveTechniques[abilityID].isActive = !sfPlayer.innateTechnique.PassiveTechniques[abilityID].isActive;

                    string text = "";
                    if (sfPlayer.innateTechnique.PassiveTechniques[abilityID].isActive)
                        text = $"Activated {sfPlayer.innateTechnique.PassiveTechniques[abilityID].Name}";

                    else
                        text = $"Deactivated {sfPlayer.innateTechnique.PassiveTechniques[abilityID].Name}";

                    int index = CombatText.NewText(Main.LocalPlayer.getRect(), Color.LightYellow, text);
                    Main.combatText[index].lifeTime = 180;
                }
            }
        }
    }
}

