using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.TreasureBags;
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
    public class SFGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            CursedModifiers(item, itemLoot);
            SukunasFingers(item, itemLoot);

            // Vanilla Items.
            switch (item.type)
            {
                case ItemID.MoonLordBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestialAmulet>(), CelestialAmulet.ChanceDenominator, 1, 1));
                    break;
                case ItemID.WallOfFleshBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SukunasSkull>(), 10, 1, 1));

                    List<IItemDropRule> rules = itemLoot.Get(true);

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
                                ); // on my soul fuck calamity mod

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

                    itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, emblems));

                    break;
            }
        }

        private void CursedModifiers(Item item, ItemLoot itemLoot)
        {

            Dictionary<int, int> itemLootMap = new()
            {
                // Max CE Modifiers
                { ItemID.SkeletronBossBag, ModContent.ItemType<CursedSkull>() },
                { ItemID.SkeletronPrimeBossBag, ModContent.ItemType<CursedMechanicalSoul>() },
                { ItemID.MoonLordBossBag, ModContent.ItemType<CursedPhantasmalEye>() },
                { ModContent.ItemType<ProvidenceBag>(), ModContent.ItemType<CursedProfanedShards>() },

                // CE Regen Modifiers
                { ItemID.EyeOfCthulhuBossBag, ModContent.ItemType<CursedEye>() },
                { ItemID.WallOfFleshBossBag, ModContent.ItemType<CursedFlesh>() },
                { ItemID.PlanteraBossBag, ModContent.ItemType<CursedBulb>() },
                { ItemID.GolemBossBag, ModContent.ItemType<CursedRock>() },
                { ModContent.ItemType<DragonfollyBag>(), ModContent.ItemType<CursedEffulgentFeather>() },
                { ModContent.ItemType<SignusBag>(), ModContent.ItemType<CursedRuneOfKos>() },

            };

            if (itemLootMap.TryGetValue(item.type, out var loot))
            {
                itemLoot.Add(ItemDropRule.Common(loot, 1, 1, 1));
            }
        }

        private void SukunasFingers(Item item, ItemLoot itemLoot)
        {
            Dictionary<int, int> itemLootMap = new()
            {
                { ItemID.EyeOfCthulhuBossBag, ModContent.ItemType<SukunasFingerI>() },

                { ModContent.ItemType<HiveMindBag>(), ModContent.ItemType<SukunasFingerII>() },
                { ModContent.ItemType<PerforatorBag>(), ModContent.ItemType<SukunasFingerII>() },

                { ItemID.SkeletronBossBag, ModContent.ItemType<SukunasFingerIII>() },

                { ItemID.WallOfFleshBossBag, ModContent.ItemType<SukunasFingerIV>() },

                { ItemID.SkeletronPrimeBossBag, ModContent.ItemType<SukunasFingerV>() },

                { ModContent.ItemType<CalamitasCloneBag>(), ModContent.ItemType<SukunasFingerVI>() },

                { ItemID.PlanteraBossBag, ModContent.ItemType<SukunasFingerVII>() },

                { ModContent.ItemType<LeviathanBag>(), ModContent.ItemType<SukunasFingerVIII>() },

                { ItemID.GolemBossBag, ModContent.ItemType<SukunasFingerIX>() },

                { ModContent.ItemType<RavagerBag>(), ModContent.ItemType<SukunasFingerX>() },

                { ItemID.CultistBossBag, ModContent.ItemType<SukunasFingerXI>() },

                { ModContent.ItemType<AstrumDeusBag>(), ModContent.ItemType<SukunasFingerXII>() },

                { ItemID.MoonLordBossBag, ModContent.ItemType<SukunasFingerXIII>() },

                { ModContent.ItemType<DragonfollyBag>(), ModContent.ItemType<SukunasFingerXIV>() },

                { ModContent.ItemType<ProvidenceBag>(), ModContent.ItemType<SukunasFingerXV>() },

                { ModContent.ItemType<CeaselessVoidBag>(), ModContent.ItemType<SukunasFingerXVI>() },

                { ModContent.ItemType<StormWeaverBag>(), ModContent.ItemType<SukunasFingerXVII>() },

                { ModContent.ItemType<SignusBag>(), ModContent.ItemType<SukunasFingerXVIII>() },

                { ModContent.ItemType<PolterghastBag>(), ModContent.ItemType<SukunasFingerXIX>() },

                { ModContent.ItemType<DevourerofGodsBag>(), ModContent.ItemType<SukunasFingerXX>() }
            };

            if (itemLootMap.TryGetValue(item.type, out var loot))
            {
                itemLoot.Add(ItemDropRule.Common(loot, 1, 1, 1));
            }
        }
    }
}
