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
            {
                bossesDefeated.Add(boss.type);
                SorceryFightUI.UpdateTechniqueUI.Invoke();
            }
        }

        public bool HasDefeatedBoss(int id)
        {
            return bossesDefeated.Contains(id);
        }

        public float MasteryDamage()
        {
            return 100 * bossesDefeated.Count;
        }
        
        public float MasteryCECost()
        {
            return bossesDefeated.Count;
        }
    }
}
