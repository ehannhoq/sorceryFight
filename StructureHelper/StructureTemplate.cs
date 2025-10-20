using System.IO;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace sorceryFight.StructureHelper
{
    public class StructureTemplate
    {
        internal int Width;
        internal int Height;
        internal TileData[,] tiles;



        internal StructureTemplate(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            tiles = new TileData[width, height];
        }

        internal struct TileData
        {
            public bool HasTile;
            public ushort TileType;
            public string TileClass;
            public short FrameX, FrameY;
            public bool IsActuated;
            public bool IsHalfBlock;
            public SlopeType Slope;
            public byte TileColor;
            public bool TileEcho;

            public ushort WallType;
            public byte WallColor;

            public bool isTileFullBright;
            public bool isWallFullBright;
        }

        public void Capture(Point origin)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Tile tile = Main.tile[origin.X + x, origin.Y + y];

                    tiles[x, y] = new TileData
                    {
                        HasTile = tile.HasTile,
                        TileType = tile.TileType,
                        FrameX = tile.TileFrameX,
                        FrameY = tile.TileFrameY,
                        IsActuated = tile.IsActuated,
                        IsHalfBlock = tile.IsHalfBlock,
                        Slope = tile.Slope,
                        TileColor = tile.TileColor,
                        TileEcho = tile.IsTileInvisible,

                        WallType = tile.WallType,
                        WallColor = tile.WallColor,

                        isTileFullBright = tile.IsTileFullbright,
                        isWallFullBright = tile.IsWallFullbright
                    };
                    
                     if (tiles[x, y].HasTile)
                    {
                        var modTile = ModContent.GetModTile(tiles[x, y].TileType);

                        tiles[x, y].TileClass = modTile != null ? modTile.GetType().ToString() : "-1";
                    }
                }
            }
        }

        internal void SaveToFile(string name)
        {
            Directory.CreateDirectory(StructureHandler.StructurePath);

            using BinaryWriter writer = new BinaryWriter(File.Create(Path.Combine(StructureHandler.StructurePath, name + ".tile")));

            writer.Write(Width);
            writer.Write(Height);

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    writer.Write(tiles[x, y].HasTile);
                    writer.Write(tiles[x, y].TileType);

                    if (tiles[x, y].TileClass != null)
                        writer.Write(tiles[x, y].TileClass);
                    else
                        writer.Write("-1");

                    writer.Write(tiles[x, y].FrameX);
                    writer.Write(tiles[x, y].FrameY);
                    writer.Write(tiles[x, y].IsActuated);
                    writer.Write(tiles[x, y].IsHalfBlock);
                    writer.Write((byte)tiles[x, y].Slope);
                    writer.Write(tiles[x, y].TileColor);
                    writer.Write(tiles[x, y].TileEcho);

                    writer.Write(tiles[x, y].WallType);
                    writer.Write(tiles[x, y].WallColor);

                    writer.Write(tiles[x, y].isTileFullBright);
                    writer.Write(tiles[x, y].isWallFullBright);

                }
            }

            Main.NewText("Structure saved to " + Path.Combine(StructureHandler.StructurePath, name + ".tile"));
        }
    }
}