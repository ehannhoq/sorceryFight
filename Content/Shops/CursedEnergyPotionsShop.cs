using sorceryFight.Content.Items.Consumables;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Shops
{
    public class CursedEnergyPotionsShop : SorceryFightShop
    {
        public override void Initialize()
        {
            ShopName = "CursedEnergyPotionsShop";

            AddItem(ModContent.ItemType<CursedEnergySoda>(), Item.buyPrice(silver: 25), Condition.DownedEyeOfCthulhu);
            AddItem(ModContent.ItemType<CursedEnergyTall>(), Item.buyPrice(gold: 1, silver: 10), Condition.DownedQueenSlime);
            AddItem(ModContent.ItemType<CursedEnergyMug>(), Item.buyPrice(gold: 3, silver: 50), Condition.DownedGolem);
            // AddItem(ModContent.ItemType<CursedEnergyTwoLiter>(), Item.buyPrice(gold: 5, silver: 75, copper: 30), CalamityConditions.DownedGuardians);
            // AddItem(ModContent.ItemType<CursedEnergyFiveGallon>(), Item.buyPrice(gold: 7, silver: 40), CalamityConditions.DownedDevourerOfGods);
        }
    }
}