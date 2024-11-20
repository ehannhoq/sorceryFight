using System;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Signus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.Items.Consumables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SFGlobalNPC : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            AddDrops(npc, npcLoot);
        }

        private void AddDrops(NPC npc, NPCLoot npcLoot)
        {
            if (Main.expertMode || Main.masterMode)
            {
                return;
            }

            if (npc.type == NPCID.Skeleton)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedSkull>(), 1, 1, 1));

            if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.TheDestroyer 
            || npc.type == NPCID.Retinazer)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedMechanicalSoul>(), 1, 1, 1));

            if (npc.type == NPCID.MoonLordCore)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedPhantasmalEye>(), 1, 1, 1));

            if (npc.type == ModContent.NPCType<Providence>())
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedProfanedShards>(), 1, 1, 1));

            
            if (npc.type == NPCID.EyeofCthulhu)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedEye>(), 1, 1, 1));
            
            if (npc.type == NPCID.WallofFlesh)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedFlesh>(), 1, 1, 1));

            if (npc.type == NPCID.Plantera)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedBulb>(), 1, 1, 1));

            if (npc.type == NPCID.Golem)
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedRock>(), 1, 1, 1));

            if (npc.type == ModContent.NPCType<Bumblefuck>()) // Why the hell did the devs call the class this
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedEffulgentFeather>(), 1, 1, 1));

            if (npc.type == ModContent.NPCType<Signus>())
                npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedRuneOfKos>(), 1, 1, 1));
        }
    }
}
