using System.Collections.Generic;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Consumables.DeathPainting;
using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Content.Items.Weapons.Melee;
using sorceryFight.Utilities;
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
            DeathPaintings(ref item, ref firstTimeRule);

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

                // CE Regen Modifiers
                { ItemID.EyeOfCthulhuBossBag, ModContent.ItemType<CursedEye>() },
                { ItemID.WallOfFleshBossBag, ModContent.ItemType<CursedFlesh>() },
                { ItemID.PlanteraBossBag, ModContent.ItemType<CursedBulb>() },
                { ItemID.GolemBossBag, ModContent.ItemType<CursedRock>() },
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
                { ItemID.KingSlimeBossBag, ModContent.ItemType<SukunasFingerI>() },
                { ItemID.EyeOfCthulhuBossBag, ModContent.ItemType<SukunasFingerII>() },

                { ItemID.EaterOfWorldsBossBag, ModContent.ItemType<SukunasFingerIII>() },
                { ItemID.BrainOfCthulhuBossBag, ModContent.ItemType<SukunasFingerIII>() },

                { ItemID.QueenBeeBossBag, ModContent.ItemType<SukunasFingerIV>() },
                { ItemID.SkeletronBossBag, ModContent.ItemType<SukunasFingerV>() },
                { ItemID.DeerclopsBossBag, ModContent.ItemType<SukunasFingerVI>() },
                { ItemID.WallOfFleshBossBag, ModContent.ItemType<SukunasFingerVII>() },
                // Sukuna's Finger VIII is dropped by the Finger Bearer I.
                { ItemID.QueenSlimeBossBag, ModContent.ItemType<SukunasFingerIX>() },
                { ItemID.TwinsBossBag, ModContent.ItemType<SukunasFingerX>() },
                { ItemID.DestroyerBossBag, ModContent.ItemType<SukunasFingerXI>() },
                { ItemID.SkeletronPrimeBossBag, ModContent.ItemType<SukunasFingerXII>() },
                { ItemID.PlanteraBossBag, ModContent.ItemType<SukunasFingerXIII>() },
                // Sukuna's Finger XIV is dropped by the Ice Queen.
                // Sukuna's Finger XV is dropped by the Pumpking.
                // Sukuna's Finger XVI is dropped by Finger Bearer II.
                { ItemID.FishronBossBag, ModContent.ItemType<SukunasFingerXVII>() },
                { ItemID.FairyQueenBossBag, ModContent.ItemType<SukunasFingerXVIII>() },
                { ItemID.CultistBossBag, ModContent.ItemType<SukunasFingerXIX>() },
                { ItemID.MoonLordBossBag, ModContent.ItemType<SukunasFingerXX>() },
            };

            if (itemLootMap.TryGetValue(item.type, out var loot))
            {
                firstTimeRule.OnSuccess(ItemDropRule.Common(loot, 1, 1, 1));
            }

            if (item.type == ItemID.MoonLordBossBag)
            {
                firstTimeRule.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SukunasSkull>(), 1, 1, 1));
            }
        }

        private void DeathPaintings(ref Item item, ref LeadingConditionRule firstTimeRule)
        {
            Dictionary<int, int> itemLootMap = new()
             {
                { ItemID.KingSlimeBossBag, ModContent.ItemType<DeathPaintingOne>() },
                { ItemID.EyeOfCthulhuBossBag, ModContent.ItemType<DeathPaintingTwo>() },
                { ItemID.SkeletronBossBag, ModContent.ItemType<DeathPaintingThree>() },
                { ItemID.WallOfFleshBossBag, ModContent.ItemType<DeathPaintingFour>() },
                { ItemID.TheDestroyer, ModContent.ItemType<DeathPaintingFive>() },
                { ItemID.PlanteraBossBag, ModContent.ItemType<DeathPaintingSix>() },
                { ItemID.GolemBossBag, ModContent.ItemType<DeathPaintingSeven>() },
                { ItemID.MoonLordBossBag, ModContent.ItemType<DeathPaintingEight>() },
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

            int[] emblems =
            [
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                        ModContent.ItemType<JujutsuEmblem>()
            ];

            itemLoot.Add(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, emblems));
        }

    }
}