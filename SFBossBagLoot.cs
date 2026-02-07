using System.Collections.Generic;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SFBossBagLoot : GlobalItem
    {
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => entity.consumable;

        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            LeadingConditionRule firstTimeRule = new LeadingConditionRule(new SFConditions.FirstTimeDefeatingBoss());
            itemLoot.Add(firstTimeRule);

            CursedModifiers(ref item, ref firstTimeRule);
            SukunasFingers(ref item, ref firstTimeRule);

            MoonlordBag(ref item, ref itemLoot);
            WallOfFleshBag(ref item, ref itemLoot);
        }

        private void CursedModifiers(ref Item item, ref LeadingConditionRule firstTimeRule)
        {
            Dictionary<int, int> itemLootMap = new()
            {
                // Max CE Modifiers
                { ItemID.SkeletronBossBag, ModContent.ItemType<CursedSkull>() },
                { ItemID.SkeletronPrimeBossBag, ModContent.ItemType<CursedMechanicalSoul>() },
                { ItemID.MoonLordBossBag, ModContent.ItemType<CursedPhantasmalEye>() },
                // { ModContent.ItemType<ProvidenceBag>(), ModContent.ItemType<CursedProfanedShards>() },

                // CE Regen Modifiers
                { ItemID.EyeOfCthulhuBossBag, ModContent.ItemType<CursedEye>() },
                { ItemID.WallOfFleshBossBag, ModContent.ItemType<CursedFlesh>() },
                { ItemID.PlanteraBossBag, ModContent.ItemType<CursedBulb>() },
                { ItemID.GolemBossBag, ModContent.ItemType<CursedRock>() },
                // { ModContent.ItemType<DragonfollyBag>(), ModContent.ItemType<CursedEffulgentFeather>() },
                // { ModContent.ItemType<SignusBag>(), ModContent.ItemType<CursedRuneOfKos>() },
            };

            if (itemLootMap.TryGetValue(item.type, out var loot))
            {
                firstTimeRule.OnSuccess(ItemDropRule.Common(loot, 1, 1, 1));
            }
        }

        private void SukunasFingers(ref Item item, ref LeadingConditionRule firstTimeRule)
        {
            Dictionary<int, int> itemLootMap = new()
            {
                { ItemID.EyeOfCthulhuBossBag, ModContent.ItemType<SukunasFingerI>() },

                // { ModContent.ItemType<HiveMindBag>(), ModContent.ItemType<SukunasFingerII>() },
                // { ModContent.ItemType<PerforatorBag>(), ModContent.ItemType<SukunasFingerII>() },

                { ItemID.SkeletronBossBag, ModContent.ItemType<SukunasFingerIII>() },

                { ItemID.WallOfFleshBossBag, ModContent.ItemType<SukunasFingerIV>() },

                { ItemID.SkeletronPrimeBossBag, ModContent.ItemType<SukunasFingerV>() },

                // { ModContent.ItemType<CalamitasCloneBag>(), ModContent.ItemType<SukunasFingerVI>() },

                { ItemID.PlanteraBossBag, ModContent.ItemType<SukunasFingerVII>() },

                // { ModContent.ItemType<LeviathanBag>(), ModContent.ItemType<SukunasFingerVIII>() },

                { ItemID.GolemBossBag, ModContent.ItemType<SukunasFingerIX>() },

                // { ModContent.ItemType<RavagerBag>(), ModContent.ItemType<SukunasFingerX>() },

                { ItemID.CultistBossBag, ModContent.ItemType<SukunasFingerXI>() },

                // { ModContent.ItemType<AstrumDeusBag>(), ModContent.ItemType<SukunasFingerXII>() },

                { ItemID.MoonLordBossBag, ModContent.ItemType<SukunasFingerXIII>() },

                // { ModContent.ItemType<DragonfollyBag>(), ModContent.ItemType<SukunasFingerXIV>() },

                // { ModContent.ItemType<ProvidenceBag>(), ModContent.ItemType<SukunasFingerXV>() },

                // { ModContent.ItemType<CeaselessVoidBag>(), ModContent.ItemType<SukunasFingerXVI>() },

                // { ModContent.ItemType<StormWeaverBag>(), ModContent.ItemType<SukunasFingerXVII>() },

                // { ModContent.ItemType<SignusBag>(), ModContent.ItemType<SukunasFingerXVIII>() },

                // { ModContent.ItemType<PolterghastBag>(), ModContent.ItemType<SukunasFingerXIX>() },

                // { ModContent.ItemType<DevourerofGodsBag>(), ModContent.ItemType<SukunasFingerXX>() }
            };

            if (itemLootMap.TryGetValue(item.type, out var loot))
            {
                firstTimeRule.OnSuccess(ItemDropRule.Common(loot, 1, 1, 1));
            }
        }

        private void MoonlordBag(ref Item item, ref ItemLoot itemLoot)
        {
            if (item.type != ItemID.MoonLordBossBag) return;

            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestialAmulet>(), CelestialAmulet.ChanceDenominator, 1, 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<InvertedSpear>(), 5, 1, 1));
            itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<LunarCursedFragment>(), 1, 14, 17));
        }

        private void WallOfFleshBag(ref Item item, ref ItemLoot itemLoot)
        {
            if (item.type != ItemID.WallOfFleshBossBag) return;

            // itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SukunasSkull>(), 10, 1, 1));

            // List<IItemDropRule> rules = itemLoot.Get(true);

            // rules.RemoveAll(rule =>
            // {
            //     if (rule is AllOptionsAtOnceWithPityDropRule itemRule)
            //     {
            //         foreach (var weightedItemStack in itemRule.stacks.ToArray())
            //         {
            //             int itemID = SFUtils.GetInternalFieldFromCalamity<int>(
            //                 "CalamityMod.WeightedItemStack",
            //                 "itemID",
            //                 weightedItemStack
            //             ); // on my soul fuck calamity mod

            //             if (itemID == ItemID.WarriorEmblem)
            //             {
            //                 return true;
            //             }
            //         }
            //     }
            //     return false;
            // });

            int[] emblems = new int[]
            {
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                        // ModContent.ItemType<RogueEmblem>(),
                        ModContent.ItemType<JujutsuEmblem>()
            };

            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, emblems));
        }

    }
}