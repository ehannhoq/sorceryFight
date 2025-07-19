using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Items.Accessories;
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
    public class SFBossLoot : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.boss;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GeneticReroll>(), 4, 1, 1));

            LeadingConditionRule nonExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(nonExpertRule);

            CursedModifiers(ref npc, ref npcLoot, ref nonExpertRule);
            SukunasFingers(ref npc, ref npcLoot, ref nonExpertRule);
            MoonLordDrops(ref npc, ref npcLoot, ref nonExpertRule);
            WallOfFleshDrops(ref npc, ref npcLoot, ref nonExpertRule);
        }

        // Non-Expert only
        private void CursedModifiers(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            LeadingConditionRule firstTimeRule = new LeadingConditionRule(new SFConditions.FirstTimeDefeatingBoss());
            nonExpertRule.Add(firstTimeRule);

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
                firstTimeRule.OnSuccess(ItemDropRule.Common(itemID));
            }
        }

        // Non-Expert only
        private void SukunasFingers(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            LeadingConditionRule firstTimeRule = new LeadingConditionRule(new SFConditions.FirstTimeDefeatingBoss());
            nonExpertRule.Add(firstTimeRule);

            Dictionary<int, int> npcLootMap = new()
            {
                { NPCID.EyeofCthulhu, ModContent.ItemType<SukunasFingerI>() },

                { ModContent.NPCType<HiveMind>(), ModContent.ItemType<SukunasFingerII>() },
                { ModContent.NPCType<PerforatorHive>(), ModContent.ItemType<SukunasFingerII>() },

                { NPCID.SkeletronHead, ModContent.ItemType<SukunasFingerIII>() },

                { NPCID.WallofFlesh, ModContent.ItemType<SukunasFingerIV>() },

                { NPCID.SkeletronPrime, ModContent.ItemType<SukunasFingerV>() },

                { ModContent.NPCType<CalamitasClone>(), ModContent.ItemType<SukunasFingerVI>() },

                { NPCID.Plantera, ModContent.ItemType<SukunasFingerVII>() },

                { ModContent.NPCType<Anahita>(), ModContent.ItemType<SukunasFingerVIII>() },

                { NPCID.Golem, ModContent.ItemType<SukunasFingerIX>() },

                { ModContent.NPCType<RavagerBody>(), ModContent.ItemType<SukunasFingerX>() },

                { NPCID.CultistBoss, ModContent.ItemType<SukunasFingerXI>() },

                { ModContent.NPCType<AstrumDeusHead>(), ModContent.ItemType<SukunasFingerXII>() },

                { NPCID.MoonLordCore, ModContent.ItemType<SukunasFingerXIII>() },

                { ModContent.NPCType<Bumblefuck>(), ModContent.ItemType<SukunasFingerXIV>() },

                { ModContent.NPCType<Providence>(), ModContent.ItemType<SukunasFingerXV>() },

                { ModContent.NPCType<CeaselessVoid>(), ModContent.ItemType<SukunasFingerXVI>() },

                { ModContent.NPCType<StormWeaverHead>(), ModContent.ItemType<SukunasFingerXVII>() },

                { ModContent.NPCType<Signus>(), ModContent.ItemType<SukunasFingerXVIII>() },

                { ModContent.NPCType<Polterghast>(), ModContent.ItemType<SukunasFingerXIX>() },

                { ModContent.NPCType<DevourerofGodsHead>(), ModContent.ItemType<SukunasFingerXX>() }
            };

            if (npcLootMap.TryGetValue(npc.type, out int itemID))
            {
                firstTimeRule.OnSuccess(ItemDropRule.Common(itemID));
            }
        }

        private void MoonLordDrops(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            if (npc.type != NPCID.MoonLordCore) return;

            nonExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CelestialAmulet>(), CelestialAmulet.ChanceDenominator));
            nonExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<LunarCursedFragment>(), 1, 9, 12));
        }

        private void WallOfFleshDrops(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            if (npc.type != NPCID.WallofFlesh) return;

            List<IItemDropRule> rules = npcLoot.Get(true);

            rules.RemoveAll(rule =>
            {
                if (rule is AllOptionsAtOnceWithPityDropRule itemRule)
                {
                    foreach (var weightedItemStack in itemRule.stacks.ToArray())
                    {
                        int itemID = SFUtils.GetInternalFieldFromCalamity<int>(
                            "CalamityMod.WeightedItemStack",
                            "itemID",
                            weightedItemStack
                        );

                        if (itemID == ItemID.WarriorEmblem)
                        {
                            return true;
                        }
                    }
                }
                return false;
            });

            int[] emblems = new int[]
            {
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                        ModContent.ItemType<RogueEmblem>(),
                        ModContent.ItemType<JujutsuEmblem>()
            };

            npcLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, emblems));
        }
    }
}