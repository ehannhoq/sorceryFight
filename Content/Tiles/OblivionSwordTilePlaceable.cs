using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Tiles
{
    public class OSTDebug : ModItem
    {
        public override string Texture => "sorceryFight/Content/Tiles/ObliviousSwordTile/0";

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override bool? UseItem(Player player)
        {
            Point pos = Main.MouseWorld.ToTileCoordinates();
            int tileType = ModContent.TileType<ObliviousSwordTile>();

            bool placed = WorldGen.PlaceTile(pos.X, pos.Y, tileType, false, true);
            if (placed && Main.tile[pos.X, pos.Y].HasTile && Main.tile[pos.X, pos.Y].TileType == tileType)
            {
                WorldGen.SquareTileFrame(pos.X, pos.Y);
            }

            return true;
        }
    }
}