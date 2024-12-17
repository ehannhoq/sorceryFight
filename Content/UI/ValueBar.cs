using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using Terraria;
using Terraria.Chat.Commands;
using Terraria.UI;

public enum Orientation
{
    Horizontal,
    Vertical
}
public class ValueBar : UIElement
{
    public Texture2D barTexture;
    public float fillPercentage;
    public Orientation orientation;

    public ValueBar(Texture2D barTexture, Orientation orientation)
    {
        this.barTexture = barTexture;
        this.orientation = orientation;
        Width.Set(barTexture.Width, 0f);
        Height.Set(barTexture.Height, 0f);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        CalculatedStyle dimensions = GetDimensions();

        if (fillPercentage > 1f)
            fillPercentage = 1;

        Rectangle bar;

        switch (orientation)
        {
            case Orientation.Horizontal:
                int croppedWidth = (int)(barTexture.Width * fillPercentage);
                bar = new Rectangle(0, 0,  croppedWidth, barTexture.Height);
                spriteBatch.Draw(barTexture, new Vector2(dimensions.X, dimensions.Y), bar, Color.White);
                break;
            case Orientation.Vertical:
                int croppedHeight = (int)(barTexture.Height * fillPercentage);
                
                bar = new Rectangle(0, 0, barTexture.Width, croppedHeight);
                float rotation = orientation == Orientation.Vertical? MathF.PI : 0f;
                Vector2 origin = new Vector2(barTexture.Width, barTexture.Height);
                spriteBatch.Draw(barTexture, new Vector2(dimensions.X, dimensions.Y), bar, Color.White, rotation, origin, 1, SpriteEffects.None, 0f);
                break;
            default:
                bar = new Rectangle(0, 0, barTexture.Width, barTexture.Height);
                break;

        } 

        

        
    }
}