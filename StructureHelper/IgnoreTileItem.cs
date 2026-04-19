using System;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.StructureHelper
{
    public class IgnoreTileItem : ModItem
    {

        public override string Texture => "sorceryFight/StructureHelper/IgnoreTile";


        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.maxStack = 9999;
            Item.createTile = ModContent.TileType<IgnoreTile>();
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.rare = ItemRarityID.White;
        }
    }
}
