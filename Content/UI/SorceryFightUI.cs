using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using sorceryFight.Content.UI.CursedTechniqueMenu;
using sorceryFight.Content.UI.InnateTechniqueSelector;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

public class SorceryFightUI : UIState
{
    public CursedEnergyBar ceBar;
    public CursedTechniqueMenu ctMenu;
    private List<UIElement> elementsToRemove;

    public override void OnInitialize()
    {
        LoadCEBar();
        elementsToRemove = new List<UIElement>();
    }

    public override void Update(GameTime gameTime)
    {
        foreach (UIElement element in elementsToRemove)
        {
            Elements.Remove(element);
        }

        base.Update(gameTime);
        var player = Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>();

        if (player.yourPotentialSwitch)
        {
            InnateTechniqueSelector innateTechniqueSelector = new InnateTechniqueSelector();
            Append(innateTechniqueSelector);
            Recalculate();
            player.yourPotentialSwitch = false;
        }

        if (player.innateTechnique == null) return;

        ceBar.fillPercentage = player.cursedEnergy / player.maxCursedEnergy;

        if (SFKeybinds.OpenTechniqueUI.JustPressed)
        {
            if (!Elements.Contains(ctMenu))
            {
                ctMenu = new CursedTechniqueMenu(player);
                player.hasUIOpen = true;
                Append(ctMenu);
            }
            else
            {
                Elements.Remove(ctMenu);
                player.hasUIOpen = false; 
            }
        }
    }

    public void LoadCEBar()
    {
        string borderBarPath = "sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarBorder";
        string fillBarPath = "sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarFill";

        ModContent.Request<Texture2D>(borderBarPath).Wait();
        ModContent.Request<Texture2D>(fillBarPath).Wait();

        var borderTexture = ModContent.Request<Texture2D>(borderBarPath).Value;
        var barTexture = ModContent.Request<Texture2D>(fillBarPath).Value;

        UIImage borderBar = new UIImage(borderTexture);
        ceBar = new CursedEnergyBar(barTexture, Orientation.Vertical);

        int x = Main.screenWidth - 300;
        int y = 20;

        borderBar.Left.Set(x, 0f);
        borderBar.Top.Set(y, 0f);
        ceBar.Left.Set(x + ((borderTexture.Width - barTexture.Width) / 2), 0f);
        ceBar.Top.Set(y, 0f);

        Append(borderBar);
        Append(ceBar);
    }

    public static bool MouseHovering(UIElement ui, Texture2D texture)
    {
        Vector2 mousePos = Main.MouseScreen;
        CalculatedStyle dimensions = ui.GetDimensions();
        
        return mousePos.X >= dimensions.X && mousePos.X <= dimensions.X + texture.Width && 
                mousePos.Y >= dimensions.Y && mousePos.Y <= dimensions.Y + texture.Height;
    }

    public void RemoveElement(UIElement element)
    {
        elementsToRemove.Add(element);
    }
}