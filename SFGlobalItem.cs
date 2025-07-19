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
    }
}
