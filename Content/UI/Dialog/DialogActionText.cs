using System;
using Terraria.GameContent.UI.Elements;

namespace sorceryFight.Content.UI.Dialog
{
    public class DialogActionText : UIText
    {
        public Action onClick;

        public DialogActionText(string text, float textScale = 1, bool large = false) : base(text, textScale, large)
        {
            OnLeftClick += (_, _) => onClick?.Invoke();
        }
    }
}
