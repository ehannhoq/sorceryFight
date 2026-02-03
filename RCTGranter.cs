using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content
{
    public class RCTGranter : ModSystem
    {
        private int moonLordIndex = -1;
        public override void PreUpdatePlayers()
        {
            if (!CheckMoonLord()) return;

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

        public bool CheckMoonLord()
        {
            int moonLordType = NPCID.MoonLordHead;
            if (moonLordIndex >= 0 && Main.npc[moonLordIndex].active && Main.npc[moonLordIndex].type == moonLordType)
                return true;


            moonLordIndex = -1;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (n.type == moonLordType)
                {
                    moonLordIndex = n.whoAmI;
                    break;
                }
            }
            
            return moonLordIndex != -1;
        }
    }
}