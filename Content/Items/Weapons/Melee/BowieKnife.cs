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
            Item.maxStack = 1;

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
            // --- Demonite version ---
            Recipe demoniteRecipe = Recipe.Create(Type);
            demoniteRecipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            demoniteRecipe.AddIngredient(ModContent.ItemType<SulphuricScale>(), 3);
            demoniteRecipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
            demoniteRecipe.AddIngredient(ItemID.DemoniteBar, 3);
            demoniteRecipe.AddTile(TileID.Anvils);
            demoniteRecipe.Register();

            // --- Crimtane version ---
            Recipe crimtaneRecipe = Recipe.Create(Type);
            crimtaneRecipe.AddRecipeGroup(RecipeGroupID.IronBar, 10);
            crimtaneRecipe.AddIngredient(ModContent.ItemType<SulphuricScale>(), 3);
            crimtaneRecipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 8);
            crimtaneRecipe.AddIngredient(ItemID.CrimtaneBar, 3);
            crimtaneRecipe.AddTile(TileID.Anvils);
            crimtaneRecipe.Register();
        }
    }
}
