using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Projectiles.Melee;
using sorceryFight.Rarities;
using sorceryFight.Utilities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    //based off a copy of CrikySword
    public class BlackRope : ModItem
    {
        //private const int FRAMES = 10;
        //private const int TICKS_PER_FRAME = 5;
        private static Texture2D texture;
        
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.BlackRope.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.BlackRope.Tooltip");

        public override void SetStaticDefaults()
        {
            //Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(TICKS_PER_FRAME, FRAMES));

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Items/Weapons/Melee/BlackRope", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Item.width = 75;
            Item.height = 90; 
            Item.maxStack = 1;
            Item.useTime = 1;
            
            //set low to attempt to test pvp cte debuff
            Item.damage = 1000;
            Item.shootSpeed = 4f;
            Item.autoReuse = false;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<SorceryFightWeapon>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<BlackRopeWhip>();

            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.noMelee = true;
            Item.ArmorPenetration = 1000;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 1;

        
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            return true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.LunarBar, 5);
            recipe.AddIngredient(ItemID.Katana);
            recipe.Register();
        }
    }
}
