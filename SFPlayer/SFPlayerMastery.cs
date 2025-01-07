using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public static int totalNumberOfBosses;
        public List<int> bossesDefeated;

        public void AddDefeatedBoss(NPC boss)
        {
            if (!bossesDefeated.Contains(boss.type))
                bossesDefeated.Add(boss.type);
        }

        public bool HasDefeatedBoss(NPC boss)
        {
            return bossesDefeated.Contains(boss.type);
        }
    }
}
