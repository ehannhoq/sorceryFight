using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SFGlobalBoss : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (!npc.boss) return;

            foreach (Player player in Main.ActivePlayers)
            {
                if (!npc.playerInteraction[player.whoAmI]) continue;

                SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
                sfPlayer.AddDefeatedBoss(npc);
            }
        }
    }
}
