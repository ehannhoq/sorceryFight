using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;

namespace sorceryFight.Content.UI.Dialog
{
    public class DialogReplyText : UIText
    {
        public string dialogKey;
        public Action onClick;

        public DialogReplyText(string text, string dialogKey, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
            this.dialogKey = dialogKey;
            OnLeftClick += (_, _) => onClick?.Invoke();
        }
    }
}
