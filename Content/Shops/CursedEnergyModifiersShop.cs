using CalamityMod;
using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace sorceryFight.Content.Shops
{
    public class CursedEnergyModifiersShop : SorceryFightShop
    {
        public override void Initialize()
        {
            ShopName = "CursedEnergyModifiersShop";

            Condition downedDragonfolly = CalamityConditions.DownedBumblebird;
            Condition downedProvidence = CalamityConditions.DownedProvidence;
            Condition downedCeaselessVoid = CalamityConditions.DownedCeaselessVoid;

            AddItem(ModContent.ItemType<CursedEye>(), Item.buyPrice(gold: 3), Condition.DownedEyeOfCthulhu);
            AddItem(ModContent.ItemType<CursedSkull>(), Item.buyPrice(gold: 4, silver: 50), Condition.DownedSkeletron);
            AddItem(ModContent.ItemType<CursedFlesh>(), Item.buyPrice(gold: 7), Condition.Hardmode);
            AddItem(ModContent.ItemType<CursedMechanicalSoul>(), Item.buyPrice(gold: 10), Condition.DownedSkeletronPrime);
            AddItem(ModContent.ItemType<CursedBulb>(), Item.buyPrice(gold: 15), Condition.DownedPlantera);
            AddItem(ModContent.ItemType<CursedRock>(), Item.buyPrice(gold: 23), Condition.DownedGolem);
            AddItem(ModContent.ItemType<CursedPhantasmalEye>(), Item.buyPrice(gold: 29), Condition.DownedMoonLord);
            AddItem(ModContent.ItemType<CursedEffulgentFeather>(), Item.buyPrice(gold: 35), downedDragonfolly);
            AddItem(ModContent.ItemType<CursedProfanedShards>(), Item.buyPrice(gold: 42), downedProvidence);
            AddItem(ModContent.ItemType<CursedRuneOfKos>(), Item.buyPrice(gold: 50), downedCeaselessVoid);
        }
    }
}