using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content
{
    public class RCTGranter : ModSystem
    {
        private int moonLordIndex = -1;
        private bool moonLordAlive = false;
        public override void PreUpdatePlayers()
        {
            if (!CheckMoonLord()) return;

            foreach (Player player in Main.player)
            {
                if (!player.active || player == null) continue;

                SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
                if (sfPlayer.unlockedRCT) continue;

                if (player.statLife < 10)
                {
                    player.statLife = player.statLifeMax2;
                    sfPlayer.unlockedRCT = true;
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