using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace sorceryFight.Utilities
{
    //This is mainly intended for IceManipulation Domain / Sword Rain
    //But it is a reusable way to create a randomized pool of projectiles that scale with boss unlocks

    public class ProjectilePool
    {
        private readonly List<ProjectileEntry> entries = new();

        public void Add(ProjectileEntry entry) => entries.Add(entry);

        public ProjectileEntry Pick(SorceryFightPlayer sf)
        {
            List<ProjectileEntry> available = entries.Where(e => e.Unlocked(sf)).ToList();
            
            //ProjectilePool entries should never be empty, always include a base case pls
            if (entries.Count == 0)
                throw new InvalidOperationException("ProjectilePool has no entries registered.");

            //No unlocks yet
            if (available.Count == 0)
                return null;

            int totalWeight = available.Sum(e => e.Weight);
            int roll = Main.rand.Next(totalWeight);
            int cumulative = 0;

            foreach (ProjectileEntry entry in available)
            {
                cumulative += entry.Weight;
                if (roll < cumulative)
                    return entry;
            }

            return available.Last();
        }

        public class ProjectileEntry
        {
            public Func<int> GetProjectileType;
            public Func<SorceryFightPlayer, bool> Unlocked;
            public int Weight;
            public float DamageMultiplier;

            //type, is unlocked, spawn chance, dmg multiplier
            public ProjectileEntry(Func<int> getType, Func<SorceryFightPlayer, bool> unlocked, int weight = 1, float damageMultiplier = 1f)
            {
                GetProjectileType = getType;
                Unlocked = unlocked;
                Weight = weight;
                DamageMultiplier = damageMultiplier;
            }
        }
    }
}