using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using CalamityMod.Items.Materials;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Materials
{
    public class CursedDischarge : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedDischarge.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedDischarge.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 50;
            Item.height = 50;
            Item.maxStack = 999;
            Item.material = true;

            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);

            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 1);

            // Ancient Manipulator (actual internal name)
            recipe.AddTile(TileID.LunarCraftingStation);

            recipe.Register();
        }
    }
}
