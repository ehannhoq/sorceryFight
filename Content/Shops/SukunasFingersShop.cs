using sorceryFight.Content.Items.Consumables.SukunasFinger;
using sorceryFight.Content.Items.Consumables.DeathPainting;
using sorceryFight.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Shops
{
    public class SukunasFingersShop : SorceryFightShop
    {
        public override void Initialize()
        {
            ShopName = "SukunasFingersShop";

            AddItem(ModContent.ItemType<SukunasFingerI>(), Item.buyPrice(silver: 50), Condition.DownedKingSlime);
            AddItem(ModContent.ItemType<SukunasFingerII>(), Item.buyPrice(gold: 1), Condition.DownedEyeOfCthulhu);
            AddItem(ModContent.ItemType<SukunasFingerIII>(), Item.buyPrice(gold: 1, silver: 50), Condition.DownedEowOrBoc);
            AddItem(ModContent.ItemType<SukunasFingerIV>(), Item.buyPrice(gold: 2), Condition.DownedQueenBee);
            AddItem(ModContent.ItemType<SukunasFingerV>(), Item.buyPrice(gold: 3), Condition.DownedSkeletron);
            AddItem(ModContent.ItemType<SukunasFingerVI>(), Item.buyPrice(gold: 4), Condition.DownedDeerclops);
            AddItem(ModContent.ItemType<SukunasFingerVII>(), Item.buyPrice(gold: 5), Condition.Hardmode);
            AddItem(ModContent.ItemType<SukunasFingerVIII>(), Item.buyPrice(gold: 7), SFConditions.DownedFingerBearerI);
            AddItem(ModContent.ItemType<SukunasFingerIX>(), Item.buyPrice(gold: 9), Condition.DownedQueenSlime);
            AddItem(ModContent.ItemType<SukunasFingerX>(), Item.buyPrice(gold: 12), Condition.DownedTwins);
            AddItem(ModContent.ItemType<SukunasFingerXI>(), Item.buyPrice(gold: 16), Condition.DownedDestroyer);
            AddItem(ModContent.ItemType<SukunasFingerXII>(), Item.buyPrice(gold: 21), Condition.DownedSkeletronPrime);
            AddItem(ModContent.ItemType<SukunasFingerXIII>(), Item.buyPrice(gold: 28), Condition.DownedPlantera);
            AddItem(ModContent.ItemType<SukunasFingerXIV>(), Item.buyPrice(gold: 35), Condition.DownedIceQueen);
            AddItem(ModContent.ItemType<SukunasFingerXV>(), Item.buyPrice(gold: 42), Condition.DownedPumpking);
            AddItem(ModContent.ItemType<SukunasFingerXVI>(), Item.buyPrice(gold: 47), SFConditions.DownedFingerBearerII);
            AddItem(ModContent.ItemType<SukunasFingerXVII>(), Item.buyPrice(gold: 53), Condition.DownedDukeFishron);
            AddItem(ModContent.ItemType<SukunasFingerXVIII>(), Item.buyPrice(gold: 59), Condition.DownedEmpressOfLight);
            AddItem(ModContent.ItemType<SukunasFingerXIX>(), Item.buyPrice(gold: 68), Condition.DownedCultist);
            AddItem(ModContent.ItemType<SukunasFingerXX>(), Item.buyPrice(gold: 75), Condition.DownedMoonLord);
        }
    }
}