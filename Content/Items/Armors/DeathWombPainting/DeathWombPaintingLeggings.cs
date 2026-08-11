using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.DeathWombPainting
{
    [AutoloadEquip(EquipType.Legs)]
    public class DeathWombPaintingLeggings : ModItem
    {
        public static int ceRegenIncrease = 15;
        public static float ctDamageIncrease = 0.08f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.DeathWombPaintingLeggings.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.DeathWombPaintingLeggings.Tooltip").WithFormatArgs(ceRegenIncrease, (int)(ctDamageIncrease * 100f));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.defense = 9;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamageIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegenIncrease;
        }

        public override void AddRecipes()
        {
            Recipe recipeAdamantite = Recipe.Create(Item.type);
            recipeAdamantite.AddIngredient(ItemID.AdamantiteBar, 14);
            recipeAdamantite.AddIngredient(ItemID.LifeCrystal, 1);
            recipeAdamantite.AddIngredient(ItemID.SoulofLight, 2);
            recipeAdamantite.AddIngredient(ItemID.SoulofNight, 2);
            recipeAdamantite.AddTile(TileID.MythrilAnvil);
            recipeAdamantite.Register();

            Recipe recipeTitanium = Recipe.Create(Item.type);
            recipeTitanium.AddIngredient(ItemID.TitaniumBar, 14);
            recipeTitanium.AddIngredient(ItemID.LifeCrystal, 1);
            recipeTitanium.AddIngredient(ItemID.SoulofLight, 2);
            recipeTitanium.AddIngredient(ItemID.SoulofNight, 2);
            recipeTitanium.AddTile(TileID.MythrilAnvil);
            recipeTitanium.Register();
        }
    }
}