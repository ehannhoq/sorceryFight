using CalamityMod.Items.TreasureBags;
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
             if (item.type == ItemID.SkeletronBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedSkull>(), 1, 1, 1));

            if (item.type == ItemID.SkeletronPrimeBossBag || item.type == ItemID.DestroyerBossBag
            || item.type == ItemID.TwinsBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedMechanicalSoul>(), 1, 1, 1));

            if (item.type == ItemID.MoonLordBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedPhantasmalEye>(), 1, 1, 1));

            if (item.type == ModContent.ItemType<ProvidenceBag>())
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedProfanedShards>(), 1, 1, 1));

            
            if (item.type == ItemID.EyeOfCthulhuBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedEye>(), 1, 1, 1));
            
            if (item.type == ItemID.WallOfFleshBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedFlesh>(), 1, 1, 1));

            if (item.type == ItemID.PlanteraBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedBulb>(), 1, 1, 1));

            if (item.type == ItemID.GolemBossBag)
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedRock>(), 1, 1, 1));

            if (item.type == ModContent.ItemType<DragonfollyBag>()) // Why the hell did the devs call the class this
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedEffulgentFeather>(), 1, 1, 1));

            if (item.type == ModContent.ItemType<SignusBag>())
                itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CursedRuneOfKos>(), 1, 1, 1));
        }
    }
}
