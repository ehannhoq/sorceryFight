using sorceryFight.Content.Items.Materials;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
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
            Item.rare = ItemRarityID.White;
            Item.width = 54;
            Item.height = 50;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.SorceryFight().heavenlyRestriction;
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
            recipe.AddIngredient(ItemID.SoulBottleFright, 15);
            recipe.AddIngredient(ItemID.SoulBottleMight, 15);
            recipe.AddIngredient(ItemID.SoulBottleSight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
