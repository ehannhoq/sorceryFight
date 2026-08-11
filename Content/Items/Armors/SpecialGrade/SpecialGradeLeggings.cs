using sorceryFight.Content.Items.Materials;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.SpecialGrade
{
    [AutoloadEquip(EquipType.Legs)]
    public class SpecialGradeLeggings : ModItem
    {
        public static int maxCEIncrease = 100;
        public static int ceRegenIncrease = 25;
        public static float movementSpeedIncrease = 0.15f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeLeggings.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeLeggings.Tooltip").WithFormatArgs(maxCEIncrease, ceRegenIncrease, (int)(movementSpeedIncrease * 100f));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 15;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += movementSpeedIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.maxCursedEnergyFromOtherSources += maxCEIncrease;
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegenIncrease;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 12);
            recipe.AddIngredient(ModContent.ItemType<InfusedCursedFragment>(), 9);
            recipe.AddIngredient(ItemID.Ectoplasm, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}