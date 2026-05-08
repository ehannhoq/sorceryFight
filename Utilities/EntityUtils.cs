using System;
using Microsoft.Xna.Framework;
using Terraria;

namespace sorceryFight
{
    public static partial class SFUtils
    {
        /// <summary>
        /// Check if Entity is null or Inactive (!active)
        /// </summary>
        /// <param name="entity">Entity to check</param>
        /// <returns>true if entity is null or inactive, otherwise false</returns>
        public static bool IsNullOrInactive(this Entity entity)
        {
            if (entity is null) return true;
            if (!entity.active) return true;

            return false;
        }

    }
}
