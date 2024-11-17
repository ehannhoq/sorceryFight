using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using Terraria;
using Terraria.Chat.Commands;
using Terraria.UI;

public class ValueBar : UIElement
{
    public Texture2D barTexture;
    public float fillPercentage;

    public ValueBar(Texture2D barTexture)
    {
        this.barTexture = barTexture;
        Width.Set(barTexture.Width, 0f);
        Height.Set(barTexture.Height, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        CalculatedStyle dimensions = GetDimensions();

        if (fillPercentage > 1f)
            fillPercentage = 1;

        int croppedWidth = (int)(barTexture.Width * fillPercentage);

        Rectangle bar = new Rectangle(0, 0,  croppedWidth, barTexture.Height);

        spriteBatch.Draw(barTexture, new Vector2(dimensions.X, dimensions.Y), bar, Color.White);
    }
}