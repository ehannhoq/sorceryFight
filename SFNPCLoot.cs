using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags;
using CalamityMod.NPCs;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.StormWeaver;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Items.Materials;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.DropHelper;

namespace sorceryFight
{
    public class SFNPCLoot : GlobalNPC
    {
        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            CursedFragments(npc, npcLoot);

        }

        private void CursedFragments(NPC npc, NPCLoot npcLoot)
        {
            if (npc.friendly) return;

            int cursedFragment = ModContent.ItemType<CursedFragment>();

            Dictionary<int, int> npcLootMap = new()
            {
                { NPCID.BloodCrawler, cursedFragment },
                { NPCID.FaceMonster, cursedFragment },
                { NPCID.Crimslime, cursedFragment },
                { NPCID.Crimera, cursedFragment },
                { NPCID.Herpling, cursedFragment },
                { NPCID.BloodJelly, cursedFragment },
                { NPCID.BloodMummy, cursedFragment },
                { NPCID.CrimsonAxe, cursedFragment },
                { NPCID.IchorSticker, cursedFragment },
                { NPCID.FloatyGross, cursedFragment },
                { NPCID.BigMimicCrimson, cursedFragment },
                { NPCID.PigronCrimson, cursedFragment },
                { NPCID.DesertGhoulCrimson, cursedFragment },

                { NPCID.EaterofSouls, cursedFragment },
                { NPCID.CorruptSlime, cursedFragment },
                { NPCID. DevourerHead, cursedFragment },
                { NPCID.Corruptor, cursedFragment },
                { NPCID.DarkMummy, cursedFragment },
                { NPCID.CursedHammer, cursedFragment },
                { NPCID.Clinger, cursedFragment },
                { NPCID.BigMimicCorruption, cursedFragment },
            };

            if (npcLootMap.TryGetValue(npc.type, out int itemID))
            {
                npcLoot.Add(ItemDropRule.Common(itemID, 6, 1, 3));
            }            
        }
    }
}
