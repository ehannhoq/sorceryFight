using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Materials
{
    public class MergedCursedFragment : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.MergedCursedFragment.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.MergedCursedFragment.Tooltip");

        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 999;
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.buyPrice(silver: 50);
            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 2);
            recipe.AddIngredient(ModContent.ItemType<FragmentOfCthulhusCurse>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}