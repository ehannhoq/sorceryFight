using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content
{
    public class RCTGranter : ModSystem
    {
        private int planteraIndex = -1;
        public override void PreUpdatePlayers()
        {
            if (!CheckPlantera()) return;

            foreach (Player player in Main.player)
            {
                if (!player.active || player == null) continue;

                SorceryFightPlayer sfPlayer = player.SorceryFight();

                if (sfPlayer.unlockedRCT) continue;

                if (player.statLife < 10)
                {
                    player.dead = false;
                    player.immuneTime = 120;
                    player.respawnTimer = 0;
                    player.statLife = 1;
                    player.creativeGodMode = true;
                    sfPlayer.rctAnimation = true;
                }
            }
        }

        public bool CheckPlantera()
        {
            int planteraType = NPCID.Plantera;
            if (planteraIndex >= 0 && Main.npc[planteraIndex].active && Main.npc[planteraIndex].type == planteraType)
                return true;


            planteraIndex = -1;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == planteraType)
                {
                    planteraIndex = n.whoAmI;
                    break;
                }
            }
            
            return planteraIndex != -1;
        }
    }
}