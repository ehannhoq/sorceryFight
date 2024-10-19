using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using Terraria;
using Terraria.Chat.Commands;
using Terraria.UI;

public class MasteryBar : ValueBar
{
    public MasteryBar(Texture2D barTexture) : base(barTexture) {}

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