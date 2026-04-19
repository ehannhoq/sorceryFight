using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
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

        private UIImage background;

        private float time = 0;
        private const float holdTime = 180;
        private const float transitionTime = 45;

        public QuestToast(string questTitle, QuestToastType type)
        {
            Left.Set(Main.screenWidth, 0f);
            Top.Set(0f, 0f);

            var backgroundTexture = ModContent.Request<Texture2D>("sorceryFight/Content/UI/Quests/QuestToast/Background", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            background = new UIImage(backgroundTexture);
            background.Width.Set(backgroundTexture.Value.Width, 0f);
            background.Height.Set(backgroundTexture.Value.Height, 0f);
            Append(background);

            UIText toastText = new UIText(SFUtils.GetLocalizationValue($"Mods.sorceryFight.Quests.QuestToast.{type}"), 1.5f);
            toastText.Left.Set(20f, 0f);
            toastText.Top.Set(20f, 0f);
            toastText.TextColor = Color.Yellow;

            UIText questText = new UIText(questTitle);
            questText.Left.Set(20f, 0f);
            questText.Top.Set(60f, 0f);

            background.Append(toastText);
            background.Append(questText);
        }

        public override void Update(GameTime gameTime)
        {
            float offScreenX = Main.screenWidth + (background.Width.Pixels / 2f);
            float onScreenX = offScreenX - background.Width.Pixels * 1.5f;

            if (time < transitionTime)
            {
                float progress = time / transitionTime;
                float ease = MathF.Sqrt(1 - MathF.Pow(progress - 1, 2));
                Left.Set(MathHelper.Lerp(offScreenX, onScreenX, ease), 0f);
            }
            else if (time < holdTime)
            {
                Left.Set(onScreenX, 0f);
            }
            else if (time < holdTime + transitionTime)
            {
                float progress = (time - holdTime) / transitionTime;
                float ease = 1 - MathF.Sqrt(1 - MathF.Pow(progress, 2));
                Left.Set(MathHelper.Lerp(onScreenX, offScreenX, ease), 0f);
            }
            else
            {
                SorceryFightUI sfUI = (SorceryFightUI)Parent;
                sfUI.RemoveElement(this);
                Recalculate();
                return;
            }

            time++;
        }
    }
}