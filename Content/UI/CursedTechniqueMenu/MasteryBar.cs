using Microsoft.Xna.Framework.Graphics;
using Terraria;
using sorceryFight.SFPlayer;

public class MasteryBar : ValueBar
{
    public MasteryBar(Texture2D barTexture, Orientation orientation) : base(barTexture, orientation) {}

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (SorceryFightUI.MouseHovering(this, barTexture)) 
        {
            var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            Main.hoverItemName = $"Mastery: {player.mastery}%";
        }
    }
}