using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;

namespace sorceryFight.Content.UI.Dialog
{
    public class DialogActionText : UIText
    {
        public Action onClick;

        public DialogActionText(string text, float textScale = 1, bool large = false) : base(text, textScale, large) {}
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Vector2 mousePos = Main.MouseWorld - Main.screenPosition;
            Vector2 dimensions = FontAssets.MouseText.Value.MeasureString(Text);

            if (mousePos.X > Left.Pixels && mousePos.X < Left.Pixels + dimensions.X && mousePos.Y > Top.Pixels && mousePos.Y < Top.Pixels + dimensions.Y)
            {
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    Clicked();
                }
            }

            base.DrawSelf(spriteBatch);
        }

        public virtual void Clicked()
        {
            onClick.Invoke();
        }
    }
}
