using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.Dialog
{
    public class DialogUI : UIState
    {
        public Dialog dialog = null;
        private bool showIndicator = false;
        public int dialogIndex;
        private SpecialUIElement background = new SpecialUIElement(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/DialogBox", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
        private UIText dialogText = new UIText("", 1f, false);
        private SFButton indicator = new SFButton(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/DialogNextIndicator", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "");

        public DialogUI(Dialog dialog)
        {
            Top.Set(0, 0f);
            Left.Set(0, 0f);

            this.dialog = dialog;

            float left = (Main.screenWidth / Main.UIScale / 2) - (background.texture.Width / 2);
            float top = (Main.screenHeight / Main.UIScale / 2) + (background.texture.Height / 2);

            background.Left.Set(left, 0f);
            background.Top.Set(top, 0f);


            dialogText.Left.Set(left + 20f, 0f);
            dialogText.Top.Set(top + 20f, 0f);

            dialogText.Width.Set(background.texture.Width - 40f, 0f);
            dialogText.Height.Set(background.texture.Height - 40f, 0f);

            dialogText.MaxWidth.Set(background.texture.Width - 40f, 0f);
            dialogText.MaxHeight.Set(background.texture.Height - 40f, 0f);

            dialogText.TextOriginX = 0f;
            dialogText.TextOriginY = 0f;
            dialogText.IsWrapped = true;


            indicator.Left.Set(left + background.texture.Width - indicator.texture.Width - 20f, 0f);
            indicator.Top.Set(top + background.texture.Height - indicator.texture.Height - 20f, 0f);

            Append(background);
            Append(dialogText);

            dialogIndex = 0;
            DisplayLine(dialog.lines[dialogIndex]);
            indicator.ClickAction += NextDialog;
        }


        private void NextDialog()
        {
            SoundEngine.PlaySound(SoundID.MenuTick, Main.LocalPlayer.Center);

            dialogIndex++;

            if (dialogIndex >= dialog.lines.Count)
            {
                EndDialog();
                return;
            }

            DisplayLine(dialog.lines[dialogIndex]);
        }

        private void DisplayLine(string line)
        {
            showIndicator = false;
            dialogText.SetText("");

            for (int i = 0; i < line.Length; i++)
            {
                int index = i;
                TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        dialogText.SetText($"{dialogText.Text + line[index]}");
                    },
                    index * 1 + 1);
            }

            TaskScheduler.Instance.AddDelayedTask(() =>
                {
                    showIndicator = true;
                },
                line.Length * 1 + 1);
        }


        private void EndDialog()
        {
            dialog = null;
            ModContent.GetInstance<SorceryFightUISystem>().DeactivateDialog();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (showIndicator && !Elements.Contains(indicator))
                Append(indicator);
            else if (!showIndicator && Elements.Contains(indicator))
                Elements.Remove(indicator);
        }
    }
}
