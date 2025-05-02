using System.Collections.Generic;
using CalamityMod.Items.TreasureBags;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Items.Materials;
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
            SukunasFingers(item, itemLoot);

            // Vanilla Items.
            switch (item.type)
            {
                case ItemID.MoonLordBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<CelestialAmulet>(), CelestialAmulet.ChanceDenominator, 1, 1));
                    break;
                case ItemID.WallOfFleshBossBag:
                    itemLoot.Add(ItemDropRule.Common(ModContent.ItemType<SukunasSkull>(), 10, 1, 1));
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
