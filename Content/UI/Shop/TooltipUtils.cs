using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

public static class TooltipUtils
{
    private const int MaxLines = 40;

    public struct Line
    {
        public string Text;
        public Color Color;

        public Line(string text, Color color)
        {
            Text = text;
            Color = color;
        }
    }

    public static Line[] BuildTooltipLines(Item item, out int lineCount)
    {
        string[] toolTipText = new string[MaxLines];
        string[] toolTipNames = new string[MaxLines];
        bool[] modifier = new bool[MaxLines];
        bool[] badModifier = new bool[MaxLines];

        int yoyoLogo = 0;
        int researchLine = 0;
        int numLines = 0;

        float oldKB = item.knockBack;

        Main.MouseText_DrawItemTooltip_GetLinesInfo(
            item,
            ref yoyoLogo,
            ref researchLine,
            oldKB,
            ref numLines,
            toolTipText,
            modifier,
            badModifier,
            toolTipNames,
            out int prefixLineIndex
        );

        int oneDropLogo = 0;
        Color?[] overrideColors;

        ItemLoader.ModifyTooltips(
            item,
            ref numLines,
            toolTipNames,
            ref toolTipText,
            ref modifier,
            ref badModifier,
            ref oneDropLogo,
            out overrideColors,
            prefixLineIndex
        );

        int finalCount = Math.Min(numLines + 1, MaxLines);
        Line[] result = new Line[finalCount];

        Color nameColor = ItemRarity.GetColor(item.rare);
        result[0] = new Line(item.Name, nameColor);

        for (int i = 0; i < numLines && i + 1 < finalCount; i++)
        {
            string text = toolTipText[i];
            if (string.IsNullOrEmpty(text))
            {
                result[i + 1] = new Line(string.Empty, Color.White);
                continue;
            }

            Color color =
                overrideColors != null && i < overrideColors.Length && overrideColors[i].HasValue
                    ? overrideColors[i].Value
                    : (badModifier[i] ? new Color(255, 80, 80) : Color.White);

            result[i + 1] = new Line(text, color);
        }

        lineCount = finalCount;

        return result;
    }
}