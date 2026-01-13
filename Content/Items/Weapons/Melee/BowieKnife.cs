using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class BowieKnife : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;

            Item.damage = 30;
            Item.knockBack = 6f;
            Item.DamageType = DamageClass.Melee;

            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTime = 19;
            Item.useAnimation = 19;
            Item.autoReuse = true;

            Item.crit = 5;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            recipe.AddIngredient(ModContent.ItemType<SulphuricScale>(), 3);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
            recipe.AddIngredient(ItemID.DemoniteBar, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            recipe = Recipe.Create(Type);
            recipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            recipe.AddIngredient(ModContent.ItemType<SulphuricScale>(), 3);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
            recipe.AddIngredient(ItemID.CrimtaneBar, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
