using System;
using CalamityMod.Items.Accessories;
using sorceryFight.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Accessories
{
    [AutoloadEquip([EquipType.HandsOn, EquipType.HandsOff])]
    public class ClawsOfCalamity : ModItem
    {
        public static float cursedTechniqueDamageIncrease = 0.12f;
        public static float blackFlashDamage = 0.30f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.ClawsOfCalamity.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.ClawsOfCalamity.Tooltip").WithFormatArgs((int)(cursedTechniqueDamageIncrease * 100), (int)(blackFlashDamage * 100));

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.LightRed;
            Item.defense = 4;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.MechanicalGlove);
            recipe.AddIngredient(ModContent.ItemType<LunarCursedFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Weapons.Magic.MadAlchemistsCocktailGlove>());
            recipe.Register();
        }
    }
}
