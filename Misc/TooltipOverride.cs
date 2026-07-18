using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI.Chat;

namespace sorceryFight.Misc
{
    public static class TooltipOverride
    {
        private static void DrawNineSliceShaderPanel(SpriteBatch sb, Texture2D tex, Rectangle box, int corner, Effect shader)
        {
            int texMid = tex.Width - corner * 2;
            int boxMidW = box.Width - corner * 2;
            int boxMidH = box.Height - corner * 2;

            var srcTL = new Rectangle(0, 0, corner, corner);
            var srcTM = new Rectangle(corner, 0, texMid, corner);
            var srcTR = new Rectangle(tex.Width - corner, 0, corner, corner);
            var srcML = new Rectangle(0, corner, corner, texMid);
            var srcMM = new Rectangle(corner, corner, texMid, texMid);
            var srcMR = new Rectangle(tex.Width - corner, corner, corner, texMid);
            var srcBL = new Rectangle(0, tex.Height - corner, corner, corner);
            var srcBM = new Rectangle(corner, tex.Height - corner, texMid, corner);
            var srcBR = new Rectangle(tex.Width - corner, tex.Height - corner, corner, corner);

            int left = box.X, top = box.Y;
            int right = box.Right - corner, bottom = box.Bottom - corner;

            var dstTL = new Rectangle(left, top, corner, corner);
            var dstTM = new Rectangle(left + corner, top, boxMidW, corner);
            var dstTR = new Rectangle(right, top, corner, corner);
            var dstML = new Rectangle(left, top + corner, corner, boxMidH);
            var dstMM = new Rectangle(left + corner, top + corner, boxMidW, boxMidH);
            var dstMR = new Rectangle(right, top + corner, corner, boxMidH);
            var dstBL = new Rectangle(left, bottom, corner, corner);
            var dstBM = new Rectangle(left + corner, bottom, boxMidW, corner);
            var dstBR = new Rectangle(right, bottom, corner, corner);

            (Rectangle src, Rectangle dst)[] pieces = {
                (srcTL, dstTL), (srcTM, dstTM), (srcTR, dstTR),
                (srcML, dstML), (srcMM, dstMM), (srcMR, dstMR),
                (srcBL, dstBL), (srcBM, dstBM), (srcBR, dstBR)
            };

            shader.Parameters["uSize"]?.SetValue(new Vector2(box.Width, box.Height));
            shader.Parameters["uTime"]?.SetValue(Main.GlobalTimeWrappedHourly);

            foreach (var (src, dst) in pieces)
            {
                shader.Parameters["uOffset"]?.SetValue(new Vector2(dst.X - box.X, dst.Y - box.Y));
                shader.Parameters["uPieceSize"]?.SetValue(new Vector2(dst.Width, dst.Height));
                sb.Draw(tex, dst, src, Color.White);
            }
        }

        public static void ShaderOverride(ReadOnlyCollection<TooltipLine> lines, ref int x, ref int y, string shaderPath)
        {
            var texts = lines.Select(l => l.Text);

            int widthForBox = (int)texts.Max(t => ChatManager.GetStringSize(FontAssets.MouseText.Value, t, Vector2.One).X) + 30;
            int heightForBox = (int)texts.Sum(t => FontAssets.MouseText.Value.MeasureString(t).Y) + 14;

            Rectangle boxRect = new Rectangle(x - 15, y - 10, widthForBox, heightForBox);

            Effect shader = ModContent.Request<Effect>(shaderPath, AssetRequestMode.ImmediateLoad).Value;
            Texture2D panelTexture = ModContent.Request<Texture2D>("sorceryFight/Misc/PanelMask9").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, shader, Main.UIScaleMatrix);

            DrawNineSliceShaderPanel(Main.spriteBatch, panelTexture, boxRect, 3, shader);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.UIScaleMatrix);
        }
    }
}
