using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace sorceryFight.StructureHelper
{
    public class StructureWand : ModItem
    {
        private static Point? firstPoint = null;

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.width = 15;
            Item.height = 15;
            Item.rare = ItemRarityID.Cyan;
        }

        public override bool? UseItem(Player player)
        {            
            if (StructureHandler.selectedStructure == "")
            {
                Main.NewText("No structure selected!");
                return false;
            }

            Point tilePos = Main.MouseWorld.ToTileCoordinates();

            if (firstPoint == null)
            {
                firstPoint = tilePos;
                Main.NewText("First point set to " + tilePos.X + ", " + tilePos.Y);
            }
            else
            {
                Point a = firstPoint.Value;
                Point b = tilePos;

                Point topLeft = new Point(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
                int width = Math.Abs(a.X - b.X) + 1;
                int height = Math.Abs(a.Y - b.Y) + 1;

                StructureTemplate template = new StructureTemplate(width, height);
                template.Capture(topLeft);

                template.SaveToFile(StructureHandler.selectedStructure);

                firstPoint = null;
            }
            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            Point point = Main.MouseWorld.ToTileCoordinates();
            StructureHandler.GenerateStructure(StructureHandler.GetStructure(StructureHandler.selectedStructure), point);
            return false;
        }
    }
}