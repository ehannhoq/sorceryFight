using System;
using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.SummonItems;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Signus;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Limitless;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SFGlobalNPCLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            CursedModifiers(npc, npcLoot);

            // Vanilla NPC's.
            switch (npc.type)
            {
                case NPCID.MoonLordCore:
                    if (!Main.expertMode)
                        npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestialAmulet>(), CelestialAmulet.ChanceDenominator, 1, 1));
                    break;
            }
        }

        private void CursedModifiers(NPC npc, NPCLoot npcLoot)
        {
            if (Main.expertMode)
            {
                return;
            }

            Dictionary<int, int> npcLootMap = new()
            {
                // Max CE Modifiers
                { NPCID.SkeletronHead, ModContent.ItemType<CursedSkull>() },
                { NPCID.SkeletronPrime, ModContent.ItemType<CursedMechanicalSoul>() },
                { NPCID.MoonLordCore, ModContent.ItemType<CursedPhantasmalEye>() },
                { ModContent.NPCType<Providence>(), ModContent.ItemType<CursedProfanedShards>() },

                // CE Regen Modifiers
                { NPCID.EyeofCthulhu, ModContent.ItemType<CursedEye>() },
                { NPCID.WallofFlesh, ModContent.ItemType<CursedFlesh>() },
                { NPCID.Plantera, ModContent.ItemType<CursedBulb>() },
                { NPCID.Golem, ModContent.ItemType<CursedRock>() },
                { ModContent.NPCType<Bumblefuck>(), ModContent.ItemType<CursedEffulgentFeather>() },
                { ModContent.NPCType<Signus>(), ModContent.ItemType<CursedRuneOfKos>() },
    
            };

            if (npcLootMap.TryGetValue(npc.type, out int itemID))
            {
                npcLoot.Add(ItemDropRule.Common(itemID, 1, 1, 1));
            }
        }
    }
}
