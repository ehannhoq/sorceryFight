using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using Terraria;
using Terraria.Chat.Commands;
using Terraria.UI;

public class CursedEnergyBar : UIElement
{
    public Texture2D barTexture;
    public float fillPercentage;

    public CursedEnergyBar(Texture2D barTexture)
    {
        this.barTexture = barTexture;
        Width.Set(barTexture.Width, 0f);
        Height.Set(barTexture.Height, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        CalculatedStyle dimensions = GetDimensions();

        int croppedWidth = (int)(barTexture.Width * fillPercentage);
        Rectangle bar = new Rectangle(0, 0,  croppedWidth, barTexture.Height);

        spriteBatch.Draw(barTexture, new Vector2(dimensions.X, dimensions.Y), bar, Color.White);
        
        if (IsMouseHovering) 
        {
            var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            Main.hoverItemName = $"Cursed Energy: {Math.Round((decimal)player.cursedEnergy, 2)} / {player.maxCursedEnergy}\n" 
                                +$"Regeneration Rate: {player.cursedEnergyRegenRate * 60} CE/s";
        }
    }
}