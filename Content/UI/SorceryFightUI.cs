using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight;
using sorceryFight.Content.UI.CursedTechniqueMenu;
using sorceryFight.Content.UI.InnateTechniqueSelector;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;
using Terraria.UI;
using sorceryFight.Content.UI.TechniqueSelector;
using System;

public class SorceryFightUI : UIState
{
    public static Action UpdateTechniqueUI;
    public CursedEnergyBar ceBar;
    public CursedTechniqueMenu ctMenu;
    public PassiveTechniqueSelector ptMenu;
    private List<UIElement> elementsToRemove;
    bool initialized;

    public override void OnInitialize()
    {
        LoadCEBar();
        elementsToRemove = new List<UIElement>();
        initialized = false;
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
            Elements.Clear();
            player.innateTechnique = null;
            initialized = false;

            InnateTechniqueSelector innateTechniqueSelector = new InnateTechniqueSelector();
            Append(innateTechniqueSelector);
            Recalculate();
            player.yourPotentialSwitch = false;
        }

        if (player.innateTechnique == null) return;

        if (!initialized)
        {
            initialized = true;
            CursedTechniqueSelector ctSelector = new CursedTechniqueSelector();
            Append(ctSelector);

            PassiveTechniqueSelector ptSelector = new PassiveTechniqueSelector();
            Append(ptSelector);
            
            Append(ceBar);
        }

        ceBar.ceBar.fillPercentage = player.cursedEnergy / player.maxCursedEnergy;

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

        ceBar = new CursedEnergyBar(borderTexture, barTexture);
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

    public override void OnActivate()
    {
        Elements.Clear();
    }
}