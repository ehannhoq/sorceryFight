using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Content.Projectiles.Ranged;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using System.Linq;
using Microsoft.Xna.Framework;
using sorceryFight.Utilities;

namespace sorceryFight.Content.Items.Weapons.Ranged
{
    public class CakeBlast : ModItem
    {

        private static Texture2D texture;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Ranged.CakeBlast.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Ranged.CakeBlast.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.damage = 400;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.mana = 10;
            Item.shoot = ModContent.ProjectileType<CakeBlastProjectile>();
            Item.shootSpeed = 0f;
            Item.noMelee = true;
            Item.channel = true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine nameLine = tooltips.FirstOrDefault(t => t.Name == "ItemName");
            if (nameLine != null)
                nameLine.OverrideColor = new Color(100, 35, 78);
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Sunflower, 1);
            recipe.AddIngredient(ItemID.PurificationPowder, 1);
            recipe.AddIngredient(ItemID.GoldButterfly, 2);
            recipe.AddIngredient(ItemID.MilkCarton, 3);
            recipe.AddIngredient(ItemID.FriedEgg, 5);
            recipe.AddIngredient(ItemID.LifeCrystal, 8);
            recipe.AddIngredient(ItemID.FallenStar, 13);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 21);
            recipe.AddIngredient(ItemID.SoulofMight, 34);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

    }
}