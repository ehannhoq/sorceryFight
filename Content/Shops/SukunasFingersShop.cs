using sorceryFight.Content.Items.Consumables.SukunasFinger;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Shops
{
    public class SukunasFingersShop : SorceryFightShop
    {
        public override void Initialize()
        {
            ShopName = "SukunasFingersShop";

            // Condition downedHiveMindOrPerforator = CalamityConditions.DownedHiveMindOrPerforator;
            // Condition downedCalamitasClone = CalamityConditions.DownedCalamitasClone;
            // Condition downedLeviathan = CalamityConditions.DownedLeviathan;
            // Condition downedRavager = CalamityConditions.DownedRavager;
            // Condition downedAstrumDeus = CalamityConditions.DownedAstrumDeus;
            // Condition downedDragonfolly = CalamityConditions.DownedBumblebird;
            // Condition downedProvidence = CalamityConditions.DownedProvidence;
            // Condition downedCeaselessVoid = CalamityConditions.DownedCeaselessVoid;
            // Condition downedStormWeaver = CalamityConditions.DownedStormWeaver;
            // Condition downedSignus = CalamityConditions.DownedStormWeaver;
            // Condition downedPolterghast = CalamityConditions.DownedPolterghast;
            // Condition downedDoG = CalamityConditions.DownedStormWeaver;
            
            AddItem(ModContent.ItemType<SukunasFingerI>(), Item.buyPrice(silver: 50), Condition.DownedEyeOfCthulhu);
            // AddItem(ModContent.ItemType<SukunasFingerII>(), Item.buyPrice(gold: 1), downedHiveMindOrPerforator);
            AddItem(ModContent.ItemType<SukunasFingerIII>(), Item.buyPrice(gold: 1, silver: 50), Condition.DownedSkeletron);
            AddItem(ModContent.ItemType<SukunasFingerIV>(), Item.buyPrice(gold: 2), Condition.Hardmode);
            AddItem(ModContent.ItemType<SukunasFingerV>(), Item.buyPrice(gold: 3), Condition.DownedSkeletronPrime);
            // AddItem(ModContent.ItemType<SukunasFingerVI>(), Item.buyPrice(gold: 4), downedCalamitasClone);
            AddItem(ModContent.ItemType<SukunasFingerVII>(), Item.buyPrice(gold: 5), Condition.DownedPlantera);
            // AddItem(ModContent.ItemType<SukunasFingerVIII>(), Item.buyPrice(gold: 7), downedLeviathan);
            AddItem(ModContent.ItemType<SukunasFingerIX>(), Item.buyPrice(gold: 9), Condition.DownedGolem);
            // AddItem(ModContent.ItemType<SukunasFingerX>(), Item.buyPrice(gold: 12), downedRavager);
            AddItem(ModContent.ItemType<SukunasFingerXI>(), Item.buyPrice(gold: 16), Condition.DownedCultist);
            // AddItem(ModContent.ItemType<SukunasFingerXII>(), Item.buyPrice(gold: 21), downedAstrumDeus);
            AddItem(ModContent.ItemType<SukunasFingerXIII>(), Item.buyPrice(gold: 28), Condition.DownedMoonLord);
            // AddItem(ModContent.ItemType<SukunasFingerXIV>(), Item.buyPrice(gold: 35), downedDragonfolly);
            // AddItem(ModContent.ItemType<SukunasFingerXV>(), Item.buyPrice(gold: 42), downedProvidence);
            // AddItem(ModContent.ItemType<SukunasFingerXVI>(), Item.buyPrice(gold: 47), downedCeaselessVoid);
            // AddItem(ModContent.ItemType<SukunasFingerXVII>(), Item.buyPrice(gold: 53), downedStormWeaver);
            // AddItem(ModContent.ItemType<SukunasFingerXVIII>(), Item.buyPrice(gold: 59), downedSignus);
            // AddItem(ModContent.ItemType<SukunasFingerXIX>(), Item.buyPrice(gold: 68), downedPolterghast);
            // AddItem(ModContent.ItemType<SukunasFingerXX>(), Item.buyPrice(gold: 75), downedDoG);
        }
    }
}