using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.UI;
using Terraria.UI.Chat;

public class UIItemTooltipPanel : UIElement
{
    private readonly Item item;
    private TooltipUtils.Line[] lines;
    private int lineCount;

    private readonly float textScale;
    private readonly float padding;
    private readonly float lineSpacing;

    public UIItemTooltipPanel(Item item, float textScale = 0.85f, float padding = 8f, float lineSpacing = 4f)
    {
        this.item = item.Clone();
        this.textScale = textScale;
        this.padding = padding;
        this.lineSpacing = lineSpacing;
        OverflowHidden = false;

        Rebuild();
    }

    public void Rebuild()
    {
        lines = TooltipUtils.BuildTooltipLines(item, out lineCount);

        var font = FontAssets.MouseText.Value;

        float maxW = 0f;
        float totalH = 0f;

        for (int i = 0; i < lineCount; i++)
        {
            if (string.IsNullOrEmpty(lines[i].Text))
                continue;

            Vector2 size = font.MeasureString(lines[i].Text) * textScale;
            if (size.X > maxW) maxW = size.X;
            totalH += size.Y + lineSpacing;
        }

        Width.Set(maxW + padding * 2f, 0f);
        Height.Set(totalH + padding * 2f, 0f);
        Recalculate();
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        var dims = GetDimensions();
        var font = FontAssets.MouseText.Value;

        Vector2 pos = new Vector2(dims.X + padding, dims.Y + padding);

        for (int i = 0; i < lineCount; i++)
        {
            string text = lines[i].Text;
            if (string.IsNullOrEmpty(text))
                continue;

            ChatManager.DrawColorCodedStringWithShadow(
                spriteBatch,
                font,
                text,
                pos,
                lines[i].Color,
                0f,
                Vector2.Zero,
                Vector2.One * textScale,
                spread: 1f
            );

            pos.Y += font.LineSpacing * textScale + lineSpacing;
        }
    }
}