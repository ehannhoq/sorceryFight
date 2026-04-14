using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.Jetstream
{
    [AutoloadEquip(EquipType.Legs)]
    public class JetstreamLeggings : ModItem
    {
        public static float meleeDmg = 0.16f;
        public static float movementSpeed = 0.20f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JetstreamLeggings.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JetstreamLeggings.Tooltip").WithFormatArgs((int)(meleeDmg * 100), (int)(movementSpeed * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 56;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) *= 1 + meleeDmg;
            player.moveSpeed += movementSpeed;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            // recipe.AddIngredient(ModContent.ItemType<BloodflareCuisses>(), 1);
            // recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            // recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 7);
            // recipe.AddIngredient(ModContent.ItemType<MiracleMatter>(), 1);
            // recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.Register();
        }
    }
}