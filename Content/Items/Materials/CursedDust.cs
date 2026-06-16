using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Materials
{
    public class CursedDust : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedDust.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedDust.Tooltip");

        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 999;
            Item.width = 24;
            Item.height = 24;
            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.PixieDust, 1);
            recipe.AddIngredient(ItemID.SoulofNight, 1);
            recipe.AddTile(TileID.MythrilAnvil); // also accepts Orichalcum
            recipe.Register();
        }
    }
}