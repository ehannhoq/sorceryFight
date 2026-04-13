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
using sorceryFight.Content.UI;
using sorceryFight.Content.UI.Quests.QuestToast;
using static sorceryFight.Content.UI.Quests.QuestToast.QuestToast;
using sorceryFight.Content.UI.Quests.QuestMenu;
using sorceryFight.Content.UI.Chants;
using sorceryFight.Content.UI.GeneticEditor;

public class SorceryFightUI : UIState
{
    public static Action UpdateTechniqueUI;
    public CursedTechniqueMenu ctMenu;
    public PassiveTechniqueSelector ptMenu;
    public FlowStateBar flowStateBar;
    public QuestToast questToast;
    public QuestMenu questMenu;
    public Chant chantUI;

    private List<UIElement> elementsToRemove;
    bool initialized;

    public override void OnInitialize()
    {

        elementsToRemove = new List<UIElement>();
        initialized = false;
        flowStateBar = null;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        //spriteBatch.End();
        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Main.UIScaleMatrix);

        base.Draw(spriteBatch);

        //spriteBatch.End();
        //spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null, null, Main.UIScaleMatrix);
    }

    public override void Update(GameTime gameTime)
    {
        foreach (UIElement element in elementsToRemove)
        {
            Elements.Remove(element);
        }

        base.Update(gameTime);
        var player = Main.LocalPlayer.SorceryFight();

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
        }

        if (SFKeybinds.OpenTechniqueUI.JustPressed)
        {
            if (!Elements.Contains(ctMenu))
            {
                ctMenu = new CursedTechniqueMenu(player);
                Append(ctMenu);
            }
            else
            {
                Elements.Remove(ctMenu);
            }
        }


        if (SFKeybinds.OpenQuestMenu.JustPressed)
        {
            if (!Elements.Contains(questMenu))
            {
                questMenu = new QuestMenu();
                Append(questMenu);
            }
            else
            {
                Elements.Remove(questMenu);
            }
        }


        if (player.sfUI == null)
        {
            player.sfUI = this;
        }

        if (Elements.Contains(flowStateBar))
        {
            //fix this
            //flowStateBar.Left.Set(ceBar.Left.Pixels - 10, 0f);
            //flowStateBar.Top.Set(ceBar.Top.Pixels + 45, 0f);
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

        for (int i = 0; i < Elements.Count; i++)
        {
            if (Elements[i].GetType() == typeof(BlackFlashWindow))
            {
                RemoveElement(Elements[i]);
                break;
            }
        }
    }

    public void BlackFlashWindow(int lowerBound, int upperBound)
    {
        BlackFlashWindow blackFlashWindow = new BlackFlashWindow(lowerBound, upperBound);
        Append(blackFlashWindow);
    }

    public void GeneticEditorUI()
    {
        GeneticEditorUI geneticEditorUI = new GeneticEditorUI();
        Append(geneticEditorUI);
    }

    public static bool MouseHovering(UIElement ui, Texture2D texture)
    {
        Vector2 mousePos = Main.MouseScreen;
        CalculatedStyle dimensions = ui.GetDimensions();

        return mousePos.X >= dimensions.X && mousePos.X <= dimensions.X + texture.Width &&
                mousePos.Y >= dimensions.Y && mousePos.Y <= dimensions.Y + texture.Height;
    }

    public void QuestToastNotification(string questName, QuestToastType type)
    {
        questToast = new QuestToast(questName, type);
        Append(questToast);
    }

    public void InitializeChant(List<string> chants, int timeBetweenChants, int bufferTime, ChantTextStyle style)
    {
        chantUI = new Chant(chants, timeBetweenChants, bufferTime, style);
        Append(chantUI);
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