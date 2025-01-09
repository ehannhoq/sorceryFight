using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

public class CursedEnergyBar : UIElement
{
    public UIImage border;
    public ValueBar ceBar;
    bool isDragging;
    Vector2 offset;
    Texture2D borderTexture;

    public CursedEnergyBar(Texture2D borderTexture, Texture2D barTexture)
    {
        this.borderTexture = borderTexture;

        Width.Set(borderTexture.Width, 0f);
        Height.Set(borderTexture.Height, 0f);

        border = new UIImage(borderTexture);
        Append(border);

        ceBar = new ValueBar(barTexture, Orientation.Vertical);
        ceBar.Left.Set((borderTexture.Width - barTexture.Width) / 2f, 0f);
        ceBar.Top.Set(0, 0f);
        Append(ceBar);

        Left.Set(1300, 0f);
        Top.Set(20, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (IsMouseHovering)
        {
            var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            Main.hoverItemName = $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.CursedEnergyBar.CE")} {Math.Round((decimal)player.cursedEnergy, 0)} / {player.maxCursedEnergy}\n"
                                + $"{SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.CursedEnergyBar.RegenRate")} {player.cursedEnergyRegenPerSecond} CE/s\n"
                                + SFUtils.GetLocalizationValue("Mods.sorceryFight.Misc.CursedEnergyBar.ToolTip");
        }
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (Main.playerInventory && SorceryFightUI.MouseHovering(this, ceBar.barTexture) && Main.mouseLeft && !isDragging)
        {
            isDragging = true;
            offset = new Vector2(Main.mouseX, Main.mouseY) - new Vector2(Left.Pixels, Top.Pixels);
        }

        if (isDragging)
        {
            float clampedLeft = Math.Clamp(Main.mouseX - offset.X, 0f, Main.screenWidth - borderTexture.Width);
            float clampedTop = Math.Clamp(Main.mouseY - offset.Y, 0f, Main.screenHeight - borderTexture.Height);

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