using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.CalamityAIs.CalamityBossAIs;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Content.Items.Novelty;
using sorceryFight.Content.Items.Weapons.Melee;
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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedEnergyCoin>(), 4, 1, 1));

            LeadingConditionRule nonExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(nonExpertRule);

            CursedModifiers(ref npc, ref npcLoot, ref nonExpertRule);
            SukunasFingers(ref npc, ref npcLoot, ref nonExpertRule);

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
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<SukunasSkull>(), 10, 1, 1));
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

                { ModContent.NPCType<PerforatorHive>(), soda },
                { ModContent.NPCType<HiveMind>(), soda },
                { ModContent.NPCType<EbonianPaladin>(), soda },
                { ModContent.NPCType<CrimulanPaladin>(), soda }
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

                { ModContent.NPCType<Cryogen>(), tall },
                { ModContent.NPCType<AquaticScourgeHead>(), tall },
                { ModContent.NPCType<BrimstoneElemental>(), tall },
                { ModContent.NPCType<CalamitasClone>(), tall },
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
                { ModContent.NPCType<Anahita>(), tall },
                { ModContent.NPCType<Leviathan>(), tall },
                { ModContent.NPCType<AstrumAureus>(), tall },
                { ModContent.NPCType<PlaguebringerGoliath>(), tall },
                { ModContent.NPCType<RavagerHead>(), tall },
            };
            drops.Add(mugMap);

            // Cursed Energy Two Liter
            int two = ModContent.ItemType<CursedEnergyTwoLiter>();
            Dictionary<int, int> twoMap = new()
            {
                { NPCID.MoonLordCore, two },
                { ModContent.NPCType<ProfanedGuardianHealer>(), two },
                { ModContent.NPCType<ProfanedGuardianDefender>(), two },
                { ModContent.NPCType<ProfanedGuardianCommander>(), two },
                { ModContent.NPCType<Bumblefuck>(), two },
                { ModContent.NPCType<Providence>(), two },
                { ModContent.NPCType<StormWeaverHead>(), two },
                { ModContent.NPCType<CeaselessVoid>(), two },
                { ModContent.NPCType<Signus>(), two },
                { ModContent.NPCType<Polterghast>(), two },
                { ModContent.NPCType<OldDuke>(), two }
            };
            drops.Add(twoMap);

            // Cursed Energy Five Gallon
            int five = ModContent.ItemType<CursedEnergyFiveGallon>();
            Dictionary<int, int> fiveMap = new()
            {
                { ModContent.NPCType<DevourerofGodsHead>(), five },
                { ModContent.NPCType<Yharon>(), five },
                { ModContent.NPCType<Artemis>(), five },
                { ModContent.NPCType<Apollo>(), five },
                { ModContent.NPCType<AresBody>(), five },
                { ModContent.NPCType<ThanatosHead>(), five },
                { ModContent.NPCType<SupremeCalamitas>(), five }
            };
            drops.Add(fiveMap);

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