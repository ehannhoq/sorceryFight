using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.UI.Dialog.Actions;
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
        private bool listenForLeftClick = false;
        private Action endOfDialogAction = null;
        private bool clearOptions = false;
        public int dialogIndex;

        private SpecialUIElement background;
        private SpecialUIElement headshotBackground;
        private SpecialUIElement headshot;
        private UIText dialogText;
        private UIImage indicator;

        public DialogUI(Dialog dialog, object initiator)
        {
            this.dialog = dialog;
            this.initiator = initiator;

            background = new SpecialUIElement(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/DialogBox", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
            headshotBackground = new SpecialUIElement(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/HeadshotBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);
            headshot = new SpecialUIElement(ModContent.Request<Texture2D>($"sorceryFight/Content/UI/Dialog/Headshots/{dialog.speaker}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value, transparentPixels: true);
            dialogText = new UIText("", 1f, false);
            indicator = new UIImage(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/DialogNextIndicator", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value);

            TaskScheduler.Instance.AddDelayedTask(() =>
            {
                SetupUI();
            }, 1);
        }

        private void SetupUI()
        {
            float gap = 16f;
            float totalWidth = background.texture.Width + headshotBackground.texture.Width + gap;

            float left = (Main.screenWidth / Main.UIScale / 2) - (totalWidth / 2);
            float top = (Main.screenHeight / Main.UIScale / 2) + (background.texture.Height / 2);

            headshotBackground.Left.Set(left, 0f);
            headshotBackground.Top.Set(top, 0f);

            headshot.Left.Set(left - ((headshot.texture.Width - headshotBackground.texture.Width) / 2f), 0f);
            headshot.Top.Set(top - ((headshot.texture.Height - headshotBackground.texture.Height) / 2f), 0f);

            background.Left.Set(left + headshotBackground.texture.Width + gap, 0f);
            background.Top.Set(top, 0f);

            dialogText.Left.Set(left + headshotBackground.texture.Width + gap + 20f, 0f);
            dialogText.Top.Set(top + 20f, 0f);

            dialogText.Width.Set(background.texture.Width - 40f, 0f);
            dialogText.Height.Set(background.texture.Height - 40f, 0f);

            dialogText.MaxWidth.Set(background.texture.Width - 40f, 0f);
            dialogText.MaxHeight.Set(background.texture.Height - 40f, 0f);

            dialogText.TextOriginX = 0f;
            dialogText.TextOriginY = 0f;
            dialogText.IsWrapped = true;


            indicator.Left.Set(left + headshotBackground.texture.Width + gap + background.texture.Width - indicator.Width.Pixels - 20f, 0f);
            indicator.Top.Set(top + background.texture.Height - indicator.Height.Pixels - 20f, 0f);

            Append(headshotBackground);
            Append(headshot);
            Append(background);
            Append(dialogText);

            dialogIndex = 0;
            DisplayLine(dialog.lines[dialogIndex]);
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
            listenForLeftClick = false;
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
                    var actions = dialog.actions;

                    if (index == dialogCount - 1 && (replies.Count > 0 || actions.Count > 0))
                    {
                        int i = 1;

                        float gap = 16f;
                        float totalWidth = background.texture.Width + headshotBackground.texture.Width + gap;
                        float left = (Main.screenWidth / Main.UIScale / 2) - (totalWidth / 2) + headshotBackground.texture.Width + gap + 20f;

                        foreach (var reply in replies)
                        {
                            DialogReplyText replyText = new(reply.Key, reply.Value);
                            replyText.onClick += () => NextDialog(replyText.dialogKey);

                            float top = (Main.screenHeight / Main.UIScale / 2) + (background.texture.Height / 2) - 10 - (30 * i);

                            replyText.TextOriginX = 0f;
                            replyText.TextOriginY = 0f;

                            replyText.Left.Set(left, 0f);
                            replyText.Top.Set(top, 0f);
                            Append(replyText);

                            i++;
                        }

                        foreach (var action in actions)
                        {
                            if (action.GetType() == typeof(EndOfDialogAction))
                            {
                                if (actions.Count > 1 || replies.Count > 0)
                                    throw new Exception($"'EndOfDialog' actions must be the only action or reply available for a dialog.");
                                
                                listenForLeftClick = true;
                                action.SetInitiator(initiator);
                                endOfDialogAction = action.Invoke;
                                break;
                            }

                            DialogActionText actionText = new(action.GetUIText());
                            actionText.onClick += () =>
                            {
                                EndDialog();
                                action.SetInitiator(initiator);
                                action.Invoke();
                            };

                            float top = (Main.screenHeight / Main.UIScale / 2) + (background.texture.Height / 2) - 10 - (30 * i);

                            actionText.TextOriginX = 0f;
                            actionText.TextOriginY = 0f;

                            actionText.Left.Set(left, 0f);
                            actionText.Top.Set(top, 0f);
                            Append(actionText);

                            i++;
                        }
                    }
                    else
                        listenForLeftClick = true;
                },
                line.Length * 1 + 1);
        }


        private void EndDialog()
        {
            endOfDialogAction?.Invoke();
            dialog = null;
            ModContent.GetInstance<SorceryFightUISystem>().ResetUI();
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (listenForLeftClick && !Elements.Contains(indicator))
                Append(indicator);
            else if (!listenForLeftClick && Elements.Contains(indicator))
                Elements.Remove(indicator);

            if (clearOptions && Elements.Any(e => e is DialogReplyText || e is DialogActionText))
            {
                clearOptions = false;
                Elements.RemoveAll(e => e is DialogReplyText || e is DialogActionText);
            }

            if (listenForLeftClick && Main.mouseLeft && !Main.mouseLeftRelease)
            {
                NextLine();
            }
        }
    }
}
