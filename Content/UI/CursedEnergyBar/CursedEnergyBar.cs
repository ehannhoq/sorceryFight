using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using Terraria;
using Terraria.Chat.Commands;
using Terraria.UI;

public class CursedEnergyBar : ValueBar
{
    public CursedEnergyBar(Texture2D barTexture, Orientation orientation) : base(barTexture, orientation) {}

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        
        if (IsMouseHovering) 
        {
            var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();
            Main.hoverItemName = $"Cursed Energy: {Math.Round((decimal)player.cursedEnergy, 0)} / {player.maxCursedEnergy}\n" 
                                +$"Regeneration Rate: {player.cursedEnergyRegenPerSecond} CE/s";
        }
    }
}