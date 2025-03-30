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
using sorceryFight.Content.UI.BlackFlash;

public class SorceryFightUI : UIState
{
    public static Action UpdateTechniqueUI;
    public CursedEnergyBar ceBar;
    public CursedTechniqueMenu ctMenu;
    public PassiveTechniqueSelector ptMenu;
    public FlowStateBar flowStateBar;
    private List<UIElement> elementsToRemove;
    bool initialized;

    public override void OnInitialize()
    {
        LoadCEBar();
        elementsToRemove = new List<UIElement>();
        initialized = false;
        flowStateBar = null;
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

        if (player.sfUI == null)
        {
            player.sfUI = this;
        }

        if (Elements.Contains(flowStateBar))
        {
            flowStateBar.Left.Set(ceBar.Left.Pixels - 10, 0f);
            flowStateBar.Top.Set(ceBar.Top.Pixels + 45, 0f);
        }
    }

    public void InitiateBlackFlashUI(Vector2 npcPos, bool showFlowState)
    {
        if (ModContent.GetInstance<ClientConfig>().BlackFlashScreenEffects)
        {
            Vector2 screenPos = npcPos - Main.screenPosition;
            BFImpactElement bfIE = new BFImpactElement(screenPos);
            Append(bfIE);
        }

        if (showFlowState && !Elements.Contains(flowStateBar))
        {
            flowStateBar = new FlowStateBar();
            Append(flowStateBar);
        }
    }

    public void ClearBlackFlashUI()
    {
        if (Elements.Contains(flowStateBar))
            Elements.Remove(flowStateBar);
    }

    public void LoadCEBar()
    {
        if (Main.dedServ) return;

        string borderBarPath = "sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarBorder";
        string fillBarPath = "sorceryFight/Content/UI/CursedEnergyBar/CursedEnergyBarFill";

        var borderTexture = ModContent.Request<Texture2D>(borderBarPath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        var barTexture = ModContent.Request<Texture2D>(fillBarPath, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

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