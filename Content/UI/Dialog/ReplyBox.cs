using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.UI.Dialog
{
    public class ReplyBox : UIElement
    {
        internal class StretchImage : UIElement
        {
            private Texture2D texture;
            private Vector2 dimensions;

            internal StretchImage(Texture2D texture, Vector2 dimensions)
            {
                this.texture = texture;
                this.dimensions = dimensions;

                Width.Set(dimensions.X, 0f);
                Height.Set(dimensions.Y, 0f);
            }

            protected override void DrawSelf(SpriteBatch spriteBatch)
            {
                Color drawColor = Color.White;

                if (Parent is ReplyBox replyBox)
                    drawColor = replyBox.CurrentColor;


                spriteBatch.Draw(
                    texture,
                    new Rectangle((int)Parent.Left.Pixels + (int)Left.Pixels, (int)Parent.Top.Pixels + (int)Top.Pixels, (int)dimensions.X, (int)dimensions.Y),
                    drawColor
                );
            }
        }

        public float gap;
        public string dialogKey;
        public Action onClick;
        bool isMouseHoveringRelease;
        public Color CurrentColor => IsMouseHovering
            ? Color.Gray
            : Color.White;

        public ReplyBox(string text, float gap = 4f)
        {
            this.gap = gap;

            Vector2 size = FontAssets.MouseText.Value.MeasureString(text);

            UIImage leftEdge = new UIImage(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/ReplyEdge", ReLogic.Content.AssetRequestMode.ImmediateLoad));
            UIImage rightEdge = new UIImage(ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/ReplyEdge", ReLogic.Content.AssetRequestMode.ImmediateLoad));

            leftEdge.Left.Set(0f, 0f);
            leftEdge.Top.Set(0f, 0f);

            StretchImage replyBG = new StretchImage(
                ModContent.Request<Texture2D>("sorceryFight/Content/UI/Dialog/ReplyBackground", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                new Vector2(size.X + 2 * gap + leftEdge.Width.Pixels, leftEdge.Height.Pixels + 8f)
            );

            replyBG.Left.Set(leftEdge.Width.Pixels / 2f, 0f);
            replyBG.Top.Set(-(replyBG.Height.Pixels - leftEdge.Height.Pixels) / 2f + 1f, 0f);

            rightEdge.Left.Set(replyBG.Width.Pixels, 0f);
            rightEdge.Top.Set(0f, 0f);

            Append(replyBG);
            Append(leftEdge);
            Append(rightEdge);


            float textPosX = leftEdge.Width.Pixels + gap;
            float textPosY = (leftEdge.Height.Pixels - size.Y) / 2f;

            UIText uiText = new UIText(text);
            uiText.Left.Set(textPosX, 0f);
            uiText.Top.Set(-textPosY, 0f);
            Append(uiText);

            Width.Set(leftEdge.Width.Pixels + replyBG.Width.Pixels + rightEdge.Width.Pixels, 0f);
            Height.Set(replyBG.Height.Pixels, 0f);
            Recalculate();

            OnLeftClick += (_, _) => onClick?.Invoke();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (IsMouseHovering)
            {
                if (!isMouseHoveringRelease)
                    SoundEngine.PlaySound(SoundID.MenuTick);

                isMouseHoveringRelease = true;
            }
            else
            {
                isMouseHoveringRelease = false;
            }

            foreach (var element in Elements)
            {
                if (element is UIText text)
                {
                    text.TextColor = IsMouseHovering ? Color.Gray : Color.White;
                }
            }
        }
    }
}