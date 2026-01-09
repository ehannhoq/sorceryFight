using System;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class OnBossDeath : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            if (!npc.boss) return;
            foreach (Player player in Main.ActivePlayers)
            {
                if (!npc.playerInteraction[player.whoAmI]) continue;

                SorceryFightPlayer sfPlayer = player.SorceryFight();
                sfPlayer.AddDefeatedBoss(npc.type);
            }
        }
    }
}
