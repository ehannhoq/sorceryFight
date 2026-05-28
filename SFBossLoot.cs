using System;
using System.Collections.Generic;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Consumables.DeathPainting;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Content.Items.Novelty;
using sorceryFight.Content.Items.Weapons.Melee;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SFBossLoot : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.boss;

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<GeneticReroll>(), 4, 1, 1));
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedEnergyCoin>(), 4, 1, 1));

            if (npc.type == NPCID.CultistBoss)
                npcLoot.Add(ItemDropRule.BossBag(ItemID.CultistBossBag));

            LeadingConditionRule nonExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(nonExpertRule);

            CursedModifiers(ref npc, ref npcLoot, ref nonExpertRule);
            SukunasFingers(ref npc, ref npcLoot, ref nonExpertRule);
            DeathPaintings(ref npc, ref npcLoot, ref nonExpertRule);
            SkeletronPrimeDrops(ref npc, ref npcLoot, ref nonExpertRule);
            MoonLordDrops(ref npc, ref npcLoot, ref nonExpertRule);
            WallOfFleshDrops(ref npc, ref npcLoot, ref nonExpertRule);
            CursedEnergyPotions(ref npc, ref npcLoot, ref nonExpertRule);
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

                // CE Regen Modifiers
                { NPCID.EyeofCthulhu, ModContent.ItemType<CursedEye>() },
                { NPCID.WallofFlesh, ModContent.ItemType<CursedFlesh>() },
                { NPCID.Plantera, ModContent.ItemType<CursedBulb>() },
                { NPCID.Golem, ModContent.ItemType<CursedRock>() },
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
                { NPCID.KingSlime, ModContent.ItemType<SukunasFingerI>() },
                { NPCID.EyeofCthulhu, ModContent.ItemType<SukunasFingerII>() },

                { NPCID.EaterofWorldsHead, ModContent.ItemType<SukunasFingerIII>() },
                { NPCID.BrainofCthulhu, ModContent.ItemType<SukunasFingerIII>() },

                { NPCID.QueenBee, ModContent.ItemType<SukunasFingerIV>() },
                { NPCID.SkeletronHead, ModContent.ItemType<SukunasFingerV>() },
                { NPCID.Deerclops, ModContent.ItemType<SukunasFingerVI>() },
                { NPCID.WallofFlesh, ModContent.ItemType<SukunasFingerVII>() },
                // Sukuna's Finger VIII is dropped by Finger Bearer I.
                { NPCID.QueenSlimeBoss, ModContent.ItemType<SukunasFingerIX>() },
                // Sukuna's Finger X is dropped by the Twins; Handled below.
                { NPCID.TheDestroyer, ModContent.ItemType<SukunasFingerXI>() },
                { NPCID.SkeletronPrime, ModContent.ItemType<SukunasFingerXII>() },
                { NPCID.Plantera, ModContent.ItemType<SukunasFingerXIII>() },
                // Sukuna's Finger XIV is dropped by the Ice Queen; Handled below.
                // Sukuna's Finger XV is dropped by the Pumpking; Handled below.
                // Sukuna's Finger XVI is dropped by Finger Bearer II.
                { NPCID.DukeFishron, ModContent.ItemType<SukunasFingerXVII>() },
                { NPCID.HallowBoss, ModContent.ItemType<SukunasFingerXVIII>() },
                { NPCID.CultistBoss, ModContent.ItemType<SukunasFingerXIX>() },
                { NPCID.MoonLordCore, ModContent.ItemType<SukunasFingerXX>() },
            };

            Dictionary<int, int> expertInclusiveLootMap = new()
            {
                { NPCID.IceQueen, ModContent.ItemType<SukunasFingerXIV>() },
                { NPCID.Pumpking, ModContent.ItemType<SukunasFingerXV>() }
            };

            if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
            {
                firstTimeRule.Add(ItemDropRule.ByCondition(new Conditions.MissingTwin(), ModContent.ItemType<SukunasFingerX>()));
            }

            if (npcLootMap.TryGetValue(npc.type, out int itemID))
                firstTimeRule.OnSuccess(ItemDropRule.Common(itemID));

            if (expertInclusiveLootMap.TryGetValue(npc.type, out int itemID2))
                npcLoot.Add(ItemDropRule.Common(itemID2));
        }

        private void DeathPaintings(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            LeadingConditionRule firstTimeRule = new LeadingConditionRule(new SFConditions.FirstTimeDefeatingBoss());
            nonExpertRule.Add(firstTimeRule);

            Dictionary<int, int> npcLootMap = new()
            {
                { NPCID.KingSlime, ModContent.ItemType<DeathPaintingOne>() },
                { NPCID.EyeofCthulhu, ModContent.ItemType<DeathPaintingTwo>() },
                { NPCID.SkeletronHead, ModContent.ItemType<DeathPaintingThree>() },
                { NPCID.WallofFlesh, ModContent.ItemType<DeathPaintingFour>() },
                { NPCID.TheDestroyer, ModContent.ItemType<DeathPaintingFive>() },
                { NPCID.Plantera, ModContent.ItemType<DeathPaintingSix>() },
                { NPCID.Golem, ModContent.ItemType<DeathPaintingSeven>() },
                { NPCID.MoonLordCore, ModContent.ItemType<DeathPaintingEight>() },

            };

            if (npcLootMap.TryGetValue(npc.type, out int itemID))
            {
                firstTimeRule.OnSuccess(ItemDropRule.Common(itemID));
            }
        }

        private void SkeletronPrimeDrops(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            if (npc.type != NPCID.SkeletronPrime) return;

            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Lollipop>(), 1, 20, 30));
        }

        private void MoonLordDrops(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            if (npc.type != NPCID.MoonLordCore) return;

            nonExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<CelestialAmulet>(), CelestialAmulet.ChanceDenominator));
            nonExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<InvertedSpear>(), 5));
            nonExpertRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<LunarCursedFragment>(), 1, 9, 12));
        }

        private void WallOfFleshDrops(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {
            if (npc.type != NPCID.WallofFlesh) return;

            int[] emblems =
            [
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                        ModContent.ItemType<JujutsuEmblem>()
            ];

            npcLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, emblems));
        }


        private void CursedEnergyPotions(ref NPC npc, ref NPCLoot npcLoot, ref LeadingConditionRule nonExpertRule)
        {

            List<Dictionary<int, int>> drops = new();

            // Cursed Energy Soda
            int soda = ModContent.ItemType<CursedEnergySoda>();
            Dictionary<int, int> sodaMap = new()
            {
                { NPCID.EyeofCthulhu, soda },
                { NPCID.BrainofCthulhu, soda },
                { NPCID.EaterofWorldsHead, soda },
                { NPCID.SkeletronHead, soda },
                { NPCID.QueenBee, soda },
                { NPCID.WallofFlesh, soda },
            };
            drops.Add(sodaMap);
            // Cursed Energy Tall
            int tall = ModContent.ItemType<CursedEnergyTall>();
            Dictionary<int, int> tallMap = new()
            {
                { NPCID.QueenSlimeBoss, tall },
                { NPCID.Retinazer, tall },
                { NPCID.Spazmatism, tall },
                { NPCID.TheDestroyer, tall },
                { NPCID.SkeletronPrime, tall },
                { NPCID.Plantera, tall },
            };
            drops.Add(tallMap);

            // Cursed Energy Mug
            int mug = ModContent.ItemType<CursedEnergyMug>();
            Dictionary<int, int> mugMap = new()
            {
                { NPCID.Golem, mug },
                { NPCID.HallowBoss, mug },
                { NPCID.DukeFishron, mug },
                { NPCID.CultistBoss, mug },
            };
            drops.Add(mugMap);

            // Cursed Energy Two Liter
            int two = ModContent.ItemType<CursedEnergyTwoLiter>();
            Dictionary<int, int> twoMap = new()
            {
                { NPCID.MoonLordCore, two },
            };
            drops.Add(twoMap);

            foreach (Dictionary<int, int> map in drops)
            {
                if (map.TryGetValue(npc.type, out int itemID))
                {
                    npcLoot.Add(ItemDropRule.Common(itemID, 1, 3, 15));
                }
            }
        }
    }
}