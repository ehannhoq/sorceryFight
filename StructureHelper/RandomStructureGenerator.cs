using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Biomes;
using Terraria.GameContent.Generation;
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
                    structure.SetDefaults();
                    loadedStructures.Add(structure);
                }
            }
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
                        Vector2.Distance(new Vector2(x, y), new Vector2(s.LastGeneratedX, s.LastGeneratedY)) > structure.MinDistance
                    ))
                        continue;

                    structure.GenerationCount++;
                    StructureHandler.GenerateStructure(structure, candidate);

                    structure.LastGeneratedX = x;
                    structure.LastGeneratedY = y;

                    Mod.Logger.Info($"Generated a structure at {x}, {y} | World Pos: {candidate.ToWorldCoordinates().X}, {candidate.ToWorldCoordinates().Y}");
                }
            }
        }
    }
}