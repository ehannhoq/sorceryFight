using CalamityMod.Items.Materials;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Accessories
{
    public class CursedOfuda : ModItem
    {
        public static float cursedTechniqueCostDecrease = 0.95f;
        public static float cursedTechniqueCastTimeDecrease = 0.84f;
    
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CursedOfuda.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CursedOfuda.Tooltip").WithFormatArgs((int)((1 - cursedTechniqueCostDecrease) * 100f), (int)((1 - cursedTechniqueCastTimeDecrease) * 100));

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
            Item.width = 54;
            Item.height = 50;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.ctCostReduction += 1 - cursedTechniqueCostDecrease;
            sfPlayer.cursedOfuda = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<InfusedCursedFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 7);
            recipe.AddIngredient(ModContent.ItemType<CoreofCalamity>(), 12);
            recipe.AddIngredient(ModContent.ItemType<UnholyCore>(), 2);
            recipe.Register();
        }
    }
}
