using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Content.Items.Accessories;

namespace sorceryFight.Content.Items.Accessories
{
    public class WeddingRing : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.WeddingRing.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.WeddingRing.Tooltip");

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.width = 28;
            Item.height = 28;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            // Effect will be added later (intentionally empty for now)
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.GoldRing, 1);
            recipe.AddIngredient(ModContent.ItemType<WulfrumCursePendant>(), 1);
            recipe.AddIngredient(ModContent.ItemType<MergedCursedFragment>(), 2);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}