using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.WorldBuilding;

namespace sorceryFight.StructureHelper
{
    public class RandomStructureGenerator : ModSystem
    {
        private List<RandomStructure> loadedStructures = new List<RandomStructure>();

        public override void Load()
        {
            foreach (var type in AssemblyManager.GetLoadableTypes(ModContent.GetInstance<SorceryFight>().Code))
            {
                if (type.IsAbstract || !typeof(RandomStructure).IsAssignableFrom(type))
                    continue;

                if (Activator.CreateInstance(type) is RandomStructure structure)
                {
                    structure.OnLoad();
                    structure.Template = StructureHandler.GetStructure(type.Name);
                    loadedStructures.Add(structure);
                }
            }

            Mod.Logger.Info($"Loaded {loadedStructures.Count} generable structures.");
        }

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int index = tasks.FindIndex(pass => pass.Name == "Remove Broken Traps");
            if (index != -1)
            {
                tasks.Insert(index + 1, new PassLegacy("SorceryUtil_Random_Structures", GenerateStructures));
            }
        }

        private void GenerateStructures(GenerationProgress progress, GameConfiguration configuration)
        {
            foreach (RandomStructure structure in loadedStructures)
            {
                structure.PreGenerate();

                if (structure.GenerationLimit == 1 && structure.GuaranteedPosition != Point.Zero)
                {
                    StructureHandler.GenerateStructure(structure.Template, structure.GuaranteedPosition);
                    structure.GenerationCount++;
                    structure.LastGeneratedX = structure.GuaranteedPosition.X;
                    structure.LastGeneratedY = structure.GuaranteedPosition.Y;

                    Mod.Logger.Info($"Generated {structure.GetType().Name} at {structure.GuaranteedPosition.X}, {structure.GuaranteedPosition.Y} | World Pos: {structure.GuaranteedPosition.ToWorldCoordinates().X}, {structure.GuaranteedPosition.ToWorldCoordinates().Y}");
                }

                int tries = 1000;
                while (structure.GenerationCount < structure.GenerationLimit && tries-- > 0)
                {
                    int x = WorldGen.genRand.Next(100, Main.maxTilesX - 100);
                    int y = WorldGen.genRand.Next(structure.MinDepth, Main.maxTilesY - 150);

                    Point candidate = new Point(x, y);

                    if (WorldGen.genRand.NextFloat() > structure.Chance)
                        continue;

                    if (loadedStructures.Any(s =>

                        s != structure &&
                        s.GenerationCount > 0 &&
                        Vector2.Distance(new Vector2(x, y), new Vector2(s.LastGeneratedX, s.LastGeneratedY)) < structure.MinDistance
                    ))
                        continue;


                    if (!CanGenerateHere(candidate, structure))
                        continue;

                    structure.GenerationCount++;
                    StructureHandler.GenerateStructure(structure.Template, candidate);

                    structure.LastGeneratedX = x;
                    structure.LastGeneratedY = y;

                    Mod.Logger.Info($"Generated {structure.GetType().Name} at {x}, {y} | World Pos: {candidate.ToWorldCoordinates().X}, {candidate.ToWorldCoordinates().Y}");
                }
            }
        }

        private bool CanGenerateHere(Point pos, RandomStructure structure)
        {
            int width = structure.Template.Width;
            int height = structure.Template.Height;
            
            for (int x = pos.X; x < pos.X + width; x++)
            {
                for (int y = pos.Y; y < pos.Y + height; y++)
                {
                    if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                        return false;

                    Tile tile = Framing.GetTileSafely(x, y);

                    if (structure.IgnoreLava && tile.LiquidType == LiquidID.Lava)
                        return false;

                    if (structure.IgnoreWater && tile.LiquidType == LiquidID.Water)
                        return false;

                    if (structure.BlacklistedTiles.Count > 0 && structure.BlacklistedTiles.Contains(tile.TileType))
                        return false;

                    if (structure.SpawnInBiome != BiomeTile.None && tile.TileType != (int)structure.SpawnInBiome)
                        return false;
                }
            }

            return true;
        }
    }
}