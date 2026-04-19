using System;
using sorceryFight.SFPlayer;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.UI.Dialog.Conditions
{
    public class BossDefeatedCondition : ICondition
    {
        private string boss;

        public BossDefeatedCondition(string boss)
        {
            this.boss = boss;
        }
        public bool Evaluate(SorceryFightPlayer sfPlayer)
        {
            int npcType = NPCID.Search.GetId(boss);

            if (npcType == -1)
            {
                if (ModContent.TryFind(boss, out ModNPC modNpc))
                {
                    npcType = modNpc.Type;
                }
                else
                {
                    throw new Exception($"Boss '{boss}' not found.");
                }
            }

            if (ModContent.TryFind<ModNPC>(boss, out ModNPC modNPC))
            {

            }
            return sfPlayer.HasDefeatedBoss(npcType);
        }
    }
}