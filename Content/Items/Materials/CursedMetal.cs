using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Materials
{
    public class CursedMetal : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedMetal.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedMetal.Tooltip");

        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 999;
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.buyPrice(silver: 25);
            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 1);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}