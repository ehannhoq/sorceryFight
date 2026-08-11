using sorceryFight.Content.Items.Consumables;
using sorceryFight.Content.Items.Weapons.Melee;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Shops
{
    public class JuzoKumiyaShop : SorceryFightShop
    {
        public override void Initialize()
        {
            ShopName = "JuzoKumiyaShop";    

            AddItem(ModContent.ItemType<DragonBone>(), Item.buyPrice(gold: 15), Condition.DownedPlantera);
            AddItem(ModContent.ItemType<BlackRope>(), Item.buyPrice(gold: 17, silver: 10), Condition.DownedPlantera);
        }
    }
}