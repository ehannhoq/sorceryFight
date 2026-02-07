using System;
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
