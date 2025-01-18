using System;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SFGlobalBoss : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (!npc.boss) return;
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GeneticReroll>(), 10, 1, 1));
        }
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
