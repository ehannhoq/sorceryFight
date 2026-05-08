using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria.ID;

namespace sorceryFight.StructureHelper
{
    public enum BiomeTile : int
    {
        None = -1,
        Jungle = TileID.Mud,
        Snow = TileID.SnowBlock,
        Desert = TileID.Sand,
        Corruption = TileID.Ebonstone,
        Crimson = TileID.Crimstone,
        Hallow = TileID.Pearlstone,
        Ocean = TileID.Sand,
        Cavern = TileID.Stone,
        Underworld = TileID.Ash
    }
    
    public abstract class RandomStructure
    {
        /// <summary>
        /// A reference to the current instance of the structure.
        /// </summary>
        public RandomStructure Structure => this;


        /// <summary>
        /// The structure template that will be used to generate the structure.
        /// </summary>
        public StructureTemplate Template;

        /// <summary>
        /// The minimum depth of the structure. Defaults to 100 tiles below the top of the world.ß
        /// </summary>
        public int MinDepth = 100;

        /// <summary>
        /// The chance of generating the structure at a given spot. Defaults to 5% (0.05f)
        /// </summary>
        public float Chance = 0.05f;

        /// <summary>
        /// The maximum number of times the structure can be generated. Defaults to 1.
        /// </summary>
        public int GenerationLimit = 1;

        /// <summary>
        /// A guaranteed position for the structure to be generated at. Structure.GenerationLimit must be set to 1 for this to work. In world coordinates.
        /// </summary>
        public Point GuaranteedPosition = Point.Zero;

        /// <summary>
        /// The minimum distance in tiles between generated structures. Defaults to 500.
        /// </summary>
        public int MinDistance = 500;

        /// <summary>
        /// The biome the structure can spawn in. Defaults to BiomeTile.None, which means it can spawn anywhere.
        /// </summary>
        public BiomeTile SpawnInBiome = BiomeTile.None;

        /// <summary>
        /// If true, the structure can be placed in lava. Defaults to false.
        /// </summary>
        public bool IgnoreLava = false;

        /// <summary>
        /// If true, the structure can be placed in water. Defaults to false.
        /// </summary>
        public bool IgnoreWater = false;

        /// <summary>
        /// A list of tile IDs that the structure cannot be placed on. Defaults to an empty list.
        /// </summary>
        public List<int> BlacklistedTiles = new List<int>();


        /// <summary>
        /// The number of times the structure has been generated.
        /// </summary>
        public int GenerationCount { get; set; } = 0;

        /// <summary>
        /// The last position the structure was generated at.
        /// </summary>
        public int LastGeneratedX { get; set; } = 0;

        /// <summary>
        /// The last position the structure was generated at.
        /// </summary>
        public int LastGeneratedY { get; set; } = 0;

        /// <summary>
        /// Called on mod load. Use this to set default values for the structure.
        /// </summary>OnLoad
        public virtual void OnLoad() { }

        /// <summary>
        /// Called before the structure is generated. Use this to access world data.
        /// </summary>
        public virtual void PreGenerate() { }

    }
}
