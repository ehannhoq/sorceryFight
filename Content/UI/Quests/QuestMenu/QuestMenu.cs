using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using sorceryFight.Content.Quests;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.Quests.QuestMenu
{
    public class QuestMenu : UIElement
    {
        private List<Quest> quests;

        private UIImage background;
        private UIList questListContent;
        private UIScrollbar questListScrollbar;

        public QuestMenu()
        {
            quests = Main.LocalPlayer.SorceryFight().currentQuests;

            Width.Set(Main.screenWidth, 0f);
            Height.Set(Main.screenHeight, 0f);

            var backgroundTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/Quests/QuestMenu/Background", AssetRequestMode.ImmediateLoad);
            var questContainerTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/Quests/QuestMenu/QuestContainer", AssetRequestMode.ImmediateLoad);

            background = new UIImage(backgroundTexture);
            background.Width.Set(backgroundTexture.Value.Width, 0f);
            background.Height.Set(backgroundTexture.Value.Height, 0f);
            background.VAlign = 0.5f;
            background.HAlign = 0.5f;
            Append(background);

            background.Recalculate();

            questListContent = new UIList();
            questListContent.Width.Set(background.Width.Pixels - 20f, 0f);
            questListContent.Height.Set(background.Height.Pixels - 32f, 0f);
            questListContent.Left.Set(16f, 0f);
            questListContent.Top.Set(16f, 0f);
            questListContent.ListPadding = 8f;
            background.Append(questListContent);

            Recalculate();

            if (quests.Count > 9)
            {
                questListScrollbar = new UIScrollbar();
                questListScrollbar.SetView(100f, 1000f);
                questListScrollbar.Height.Set(-1000f, 0f);
                questListScrollbar.Left.Set(-1000f, 0f);
                questListScrollbar.Top.Set(0f, 0f);
                questListScrollbar.Width.Set(0f, 0f);
                background.Append(questListScrollbar);
                questListContent.SetScrollbar(questListScrollbar);
            }

            InitializeQuestContainers(questContainerTexture);

            Texture2D closeButtonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/CursedTechniqueMenu/CursedTechniqueMenuBGCloseButton", AssetRequestMode.ImmediateLoad).Value;
            SFButton closeButton = new SFButton(closeButtonTexture, "");
            closeButton.ClickAction += () =>
            {
                var sfUI = (SorceryFightUI)Parent;
                sfUI.RemoveElement(this);
            };
            closeButton.Top.Set(-(closeButtonTexture.Height + 8), 0f);
            background.Append(closeButton);
        }

        private void InitializeQuestContainers(Asset<Texture2D> backgroundTexture)
        {
            for (int i = 0; i < quests.Count; i++)
            {
                Quest quest = quests[i];

                UIImage questBackground = new(backgroundTexture);

                UIText questName = new(quest.DisplayName);
                questName.TextColor = Color.Yellow;
                questName.VAlign = 0.20f;
                questName.Left.Set(15f, 0f);

                UIElement textContainer = new();
                textContainer.Width.Set(1f, 1f);
                textContainer.VAlign = 0.5f;

                UIText questDesc = new(quest.Description, 0.75f);
                questDesc.Width.Set(-30f, 1f);
                questDesc.Left.Set(15f, 0f);
                questDesc.IsWrapped = true;


                questBackground.Append(questName);
                questBackground.Append(textContainer);

                if (quest.completed)
                {
                    UIText completedText = new UIText("COMPLETED", 0.75f);
                    completedText.TextColor = Color.Green;

                    completedText.VAlign = 0.5f;
                    completedText.HAlign = 0.5f;

                    questBackground.Append(completedText);
                }
                else
                    textContainer.Append(questDesc);

                questBackground.Width.Set(0f, 1f);
                questBackground.Height.Set(backgroundTexture.Value.Height, 0f);
                questListContent.Add(questBackground);
            }
            questListContent.Recalculate();
        }
    }
}