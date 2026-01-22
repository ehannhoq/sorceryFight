using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace sorceryFight.Content.UI.Quests.QuestToast
{
    public class QuestToast : UIElement
    {
        public enum QuestToastType
        {
            NewQuest,
            CompletedQuest
        }

        private SorceryFightUI parentState;

        private UIImage background;

        private float time;
        private const float holdTime = 180;
        private const float transitionTime = 45;

        public QuestToast(string questTitle, QuestToastType type, SorceryFightUI parentState)
        {
            this.parentState = parentState;

            time = 0;
            Left.Set(Main.screenWidth / Main.UIScale, 0f);

            background = new UIImage(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Quests/QuestToast/Background"));
            Append(background);

            UIText toastText = new UIText(SFUtils.GetLocalizationValue($"Mods.sorceryFight.Quests.QuestToast.{type}"));
            toastText.Left.Set(20f, 0f);
            toastText.Top.Set(20f, 0f);
            toastText.TextColor = Color.Yellow;

            UIText questText = new UIText(questTitle);
            questText.Left.Set(40f, 0f);
            questText.Top.Set(20f, 0f);

            background.Append(toastText);
            background.Append(questText);

        }

        public override void Update(GameTime gameTime)
        {
            if (time < transitionTime)
            {
                float startPos = Main.screenWidth / Main.UIScale;
                float progress = (transitionTime - time) / transitionTime;
                float easeProg = 1 - MathF.Sqrt(1 - MathF.Pow(progress, 2));
                Left.Set(startPos - (easeProg * background.Width.Pixels), 0f);
            }

            if (time > holdTime)
            {
                float startPos = (Main.screenWidth / Main.UIScale) - background.Width.Pixels;
                float progress = (holdTime + transitionTime - time) / (holdTime + transitionTime);
                float easeProg = 1 - MathF.Sqrt(1 - MathF.Pow(progress, 2));
                Left.Set(startPos + (easeProg * background.Width.Pixels), 0f);
            }

            if (time > holdTime + transitionTime)
            {
                parentState.RemoveElement(this);
            }

            time++;
        }
    }
}