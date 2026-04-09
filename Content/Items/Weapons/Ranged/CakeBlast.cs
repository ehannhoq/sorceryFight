using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.Content.Projectiles.Ranged;

namespace sorceryFight.Content.Items.Weapons.Ranged
{
    public class CakeBlast : ModItem
    {

        private static Texture2D texture;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.BlackRope.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.BlackRope.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 400;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 10;
            Item.shoot = ModContent.ProjectileType<CakeBlastProjectile>();
            Item.shootSpeed = 0f;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            recipe.AddIngredient(ItemID.Katana);
            recipe.Register();
        }

    }
}