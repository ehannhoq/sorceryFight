using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        public Dialog dialog;
        public object initiator;
        private bool showIndicator = false;
        private bool clearOptions = false;
        public int dialogIndex;
        private SpecialUIElement background = new SpecialUIElement(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/DialogBox", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
        private UIText dialogText = new UIText("", 1f, false);
        private SFButton indicator = new SFButton(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/DialogNextIndicator", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, "");
        public DialogUI(Dialog dialog, object initiator)
        {
            this.dialog = dialog;
            this.initiator = initiator;
            SetupUI();
        }

        private void SetupUI()
        {
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
            indicator.ClickAction += NextLine;
        }

        private void NextLine()
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

        private void NextDialog(string dialogKey)
        {
            clearOptions = true;
            dialogIndex = 0;
            dialog = Dialog.Create(dialogKey);

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
                    int index = dialogIndex;
                    int dialogCount = dialog.lines.Count;
                    var replies = dialog.replies;

                    if (index == dialogCount - 1 && replies.Count > 0)
                    {
                        int i = 1;
                        foreach (var reply in replies)
                        {
                            DialogReplyText replyText = new DialogReplyText(reply.Key, reply.Value);
                            replyText.onClick += () => NextDialog(replyText.dialogKey);

                            float left = (Main.screenWidth / Main.UIScale / 2) - (background.texture.Width / 2) + 20;
                            float top = (Main.screenHeight / Main.UIScale / 2) + (background.texture.Height / 2) - 10 - (30 * i);

                            replyText.TextOriginX = 0f;
                            replyText.TextOriginY = 0f;

                            replyText.Left.Set(left, 0f);
                            replyText.Top.Set(top, 0f);
                            Append(replyText);

                            i++;
                        }
                    }
                    else
                    {
                        showIndicator = true;
                    }
                },
                line.Length * 1 + 1);
        }


        private void EndDialog()
        {
            if (dialog.actionName != string.Empty)
            {
                var method = initiator.GetType().GetMethod(dialog.actionName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

                if (method != null)
                {
                    method.Invoke(initiator, null);
                }
                else throw new Exception($"Method {dialog.actionName} not found in {initiator.GetType().Name}");
            }

            dialog = null;
            ModContent.GetInstance<SorceryFightUISystem>().DeactivateDialogUI();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (showIndicator && !Elements.Contains(indicator))
                Append(indicator);
            else if (!showIndicator && Elements.Contains(indicator))
                Elements.Remove(indicator);

            if (clearOptions && Elements.Any(e => e is DialogReplyText))
            {
                clearOptions = false;
                Elements.RemoveAll(e => e is DialogReplyText);
            }
        }
    }
}
