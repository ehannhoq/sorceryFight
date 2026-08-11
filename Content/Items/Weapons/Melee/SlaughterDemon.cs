using sorceryFight.Content.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class SlaughterDemon : ModItem
    { 
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.SlaughterDemon.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.SlaughterDemon.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 29;
            Item.maxStack = 1;
            Item.useTime = 19;
            Item.damage = 22;
            Item.crit = 8;
            Item.knockBack = 3;
            Item.useAnimation = 19;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient(ItemID.SilverBar, 7);
            recipe.AddIngredient(ItemID.Silk, 3);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
