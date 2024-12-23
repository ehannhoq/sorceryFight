using System.Collections.Generic;
using CalamityMod.Items.TreasureBags;
using CalamityMod.NPCs.Providence;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class SFGlobalItem : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot itemLoot)
        {
            CursedModifiers(item, itemLoot);

            // Vanilla Items.
            switch (item.type)
            {
                case ItemID.MoonLordBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestialAmulet>(), CelestialAmulet.ChanceDenominator, 1, 1));
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
                { ModContent.ItemType<ProvidenceBag>(), ModContent.ItemType<CursedSkull>() },

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
    }
}
