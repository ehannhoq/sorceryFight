using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Armors.SpecialGradeUniform
{
    [AutoloadEquip(EquipType.Legs)]
    public class SpecialGradePants : ModItem
    {
        public static float cursedTechniqueDamageIncrease = 0.07f;
        public static float cursedTechniqueCostDecrease = 1.05f;
        public static int maxCursedEnergyIncrease = 125; // total 250 increase
        
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradePants.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradePants.Tooltip").WithFormatArgs((int)(cursedTechniqueDamageIncrease * 100), (int)((1 - cursedTechniqueCostDecrease) * 100), maxCursedEnergyIncrease);
        
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 26;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + cursedTechniqueDamageIncrease;
            sfPlayer.maxCursedEnergyFromOtherSources += maxCursedEnergyIncrease;
            sfPlayer.ctCostReduction += 1 - cursedTechniqueCostDecrease;

        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Silk, 16);
            recipe.AddIngredient(ItemID.BeetleHusk, 12);
            recipe.AddIngredient(ItemID.SoulOfLight, 10);
            recipe.AddIngredient(ModContent.ItemType<InfusedCursedFragment>(), 6);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}