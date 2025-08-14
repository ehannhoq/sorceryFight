using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace sorceryFight.Content.Tiles
{
    public class EnshroudedOneStatue : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileSolid[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileNoFail[Type] = true;
            MinPick = 400;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
            TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 18 };
            TileObjectData.newTile.LavaDeath = false;
            

            TileObjectData.addTile(Type);
        }

        public override bool RightClick(int i, int j)
        {
            return true;
        }
    }

    public class EnshroudedOneStatueItem : ModItem
    {
        public override string Texture => "sorceryFight/Content/Tiles/EnshroudedOneStatue";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 48;
            Item.createTile = ModContent.TileType<EnshroudedOneStatue>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.maxStack = 99;
            Item.consumable = true;
        }
    }
}