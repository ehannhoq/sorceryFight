using System;
using System.Collections.Generic;
using System.IO;

namespace sorceryFight.StructureHelper
{
    public abstract class RandomStructure
    {
        /// <summary>
        /// The structure template that will be used to generate the structure.
        /// </summary>
        public StructureTemplate Template;

        /// <summary>
        /// The minimum depth of the structure.
        /// </summary>
        public int MinDepth = 100;

        /// <summary>
        /// The chance of generating the structure at a given spot.
        /// </summary>
        public float Chance = 0.05f;

        /// <summary>
        /// The maximum number of times the structure can be generated.
        /// </summary>
        public int GenerationLimit = 1;

        /// <summary>
        /// The minimum distance between generated structures.
        /// </summary>
        public int MinDistance = 500;

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
        /// Sets the default values for the structure, such as the structure template, minimum depth, chance, and generation limit.
        /// </summary>
        public abstract void SetDefaults();

        public virtual void AddLoot() { }
    }
}
