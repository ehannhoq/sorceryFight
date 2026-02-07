using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.StructureHelper
{
    public class StructureHandler : ModSystem
    {
        internal static string StructurePath => $"{Main.SavePath}/ModSources/sorceryFight/Content/Structures/";
        internal static string selectedStructure = "";
        private static Dictionary<string, Action<string[]>> subCommands = new()
        {
            { "set", SetStructure },
            { "list", ListStructures },
            { "del", DeleteStructure }
        };

        public static void ProcessCommand(string[] args)
        {
            if (args.Length == 0)
            {
                Main.NewText("Usage: /structure [set, list, del]");
                return;
            }

            string command = args[0];
            string[] tokens = args.Skip(1).ToArray();

            if (subCommands.TryGetValue(command, out Action<string[]> action))
            {
                action(tokens);
            }
        }


        private static void SetStructure(string[] args)
        {
            if (args.Length == 0)
            {
                Main.NewText("Usage: /structure set [structure name]");
                return;
            }

            if (args.Length > 1)
            {
                Main.NewText("Structure names does not support spaces!");
                return;
            }

            selectedStructure = args[0];

            Main.NewText($"Current structure set to {selectedStructure}");
        }

        private static void ListStructures(string[] args)
        {
            string[] files = Directory.GetFiles(StructurePath);

            if (files.Length == 0)
            {
                Main.NewText("No structures found!");
                return;
            }

            foreach (string file in files)
            {
                if (file.EndsWith(".tile"))
                    Main.NewText(Path.GetFileNameWithoutExtension(file));
            }
        }

        private static void DeleteStructure(string[] args)
        {
            if (args.Length == 0)
            {
                Main.NewText("Usage: /structure del [structure name]");
                return;
            }

            if (args.Length > 1)
            {
                Main.NewText("Structure names does not support spaces!");
                return;
            }

            string structureName = args[0];

            if (!Directory.Exists(StructurePath + structureName))
            {
                Main.NewText($"{structureName} does not exist!");
                return;
            }

            File.Delete(StructurePath + structureName);
            Main.NewText($"{structureName} deleted!");
        }

        public static void GenerateStructure(StructureTemplate template, Point worldPos)
        {
            if (template == null)
            {
                ModContent.GetInstance<SorceryFight>().Logger.Error("Structure template is null!");
                return;
            }


            for (int x = 0; x < template.Width; x++)
            {
                for (int y = 0; y < template.Height; y++)
                {
                    bool hasTile = template.tiles[x, y].HasTile;
                    ushort tileType = template.tiles[x, y].TileType;
                    string tileClass = template.tiles[x, y].TileClass;
                    short frameX = template.tiles[x, y].FrameX;
                    short frameY = template.tiles[x, y].FrameY;
                    bool isActuated = template.tiles[x, y].IsActuated;
                    bool isHalfBlock = template.tiles[x, y].IsHalfBlock;
                    SlopeType slope = template.tiles[x, y].Slope;
                    byte tileColor = template.tiles[x, y].TileColor;
                    bool tileEcho = template.tiles[x, y].TileEcho;

                    ushort wallType = template.tiles[x, y].WallType;
                    byte wallColor = template.tiles[x, y].WallColor;

                    bool isTileFullBright = template.tiles[x, y].isTileFullBright;
                    bool isWallFullBright = template.tiles[x, y].isWallFullBright;

                    if (hasTile && tileClass == "sorceryFight.StructureHelper.IgnoreTile") continue;

                    Tile tile = Framing.GetTileSafely(worldPos.X + x, worldPos.Y + y);
                    tile.ClearEverything();

                    if (hasTile)
                    {
                        tile.HasTile = true;

                        if (tileClass != "-1")
                        {
                            var moddedTileType = SFUtils.FindTypeAcrossMods(tileClass);

                            var method = typeof(ModContent).GetMethod("TileType", Type.EmptyTypes);
                            var generic = method.MakeGenericMethod(moddedTileType);
                            int moddedTileID = (int)generic.Invoke(null, null);



                            tile.TileType = (ushort)moddedTileID;
                        }
                        else
                            tile.TileType = tileType;

                        tile.TileFrameX = frameX;
                        tile.TileFrameY = frameY;
                        tile.IsActuated = isActuated;
                        tile.IsHalfBlock = isHalfBlock;
                        tile.Slope = slope;
                        tile.TileColor = tileColor;
                        tile.IsTileInvisible = tileEcho;
                        tile.IsTileFullbright = isTileFullBright;
                    }

                    tile.WallType = wallType;
                    tile.WallColor = wallColor;
                    tile.IsWallFullbright = isWallFullBright;
                }
            }

            for (int x = 0; x < template.Width; x++)
            {
                for (int y = 0; y < template.Height; y++)
                {
                    WorldGen.SquareTileFrame(worldPos.X + x, worldPos.Y + y);
                    WorldGen.SquareWallFrame(worldPos.X + x, worldPos.Y + y);
                }
            }
        }


        /// <summary>
        /// Gets a structure template from its structure name.
        /// </summary>
        /// <param name="structureName"></param>
        /// <param name="template"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        public static StructureTemplate GetStructure(string structureName)
        {
            string assetPath = $"sorceryFight/Content/Structures/{structureName}.tile";
            if (!ModContent.FileExists(assetPath))
            {
                throw new FileNotFoundException($"Structure {structureName} not found! Please contact the mod developers.");
            }

            using MemoryStream stream = new MemoryStream(ModContent.GetFileBytes(assetPath));
            using BinaryReader userReader = new BinaryReader(stream);
            return ReadTemplate(userReader);
        }

        private static StructureTemplate ReadTemplate(BinaryReader reader)
        {
            int width = reader.ReadInt32();
            int height = reader.ReadInt32();

            StructureTemplate template = new StructureTemplate(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    bool hasTile = reader.ReadBoolean();
                    ushort tileType = reader.ReadUInt16();
                    string tileClass = reader.ReadString();
                    short frameX = reader.ReadInt16();
                    short frameY = reader.ReadInt16();
                    bool isActuated = reader.ReadBoolean();
                    bool isHalfBlock = reader.ReadBoolean();
                    SlopeType slope = (SlopeType)reader.ReadByte();
                    byte tileColor = reader.ReadByte();
                    bool tileEcho = reader.ReadBoolean();

                    ushort wallType = reader.ReadUInt16();
                    byte wallColor = reader.ReadByte();

                    bool isTileFullBright = reader.ReadBoolean();
                    bool isWallFullBright = reader.ReadBoolean();

                    template.tiles[x, y].HasTile = hasTile;
                    template.tiles[x, y].TileType = tileType;
                    template.tiles[x, y].TileClass = tileClass;
                    template.tiles[x, y].FrameX = frameX;
                    template.tiles[x, y].FrameY = frameY;
                    template.tiles[x, y].IsActuated = isActuated;
                    template.tiles[x, y].IsHalfBlock = isHalfBlock;
                    template.tiles[x, y].Slope = slope;
                    template.tiles[x, y].TileColor = tileColor;
                    template.tiles[x, y].TileEcho = tileEcho;
                    template.tiles[x, y].WallType = wallType;
                    template.tiles[x, y].WallColor = wallColor;
                    template.tiles[x, y].isTileFullBright = isTileFullBright;
                    template.tiles[x, y].isWallFullBright = isWallFullBright;
                }
            }

            return template;
        }
    }
}