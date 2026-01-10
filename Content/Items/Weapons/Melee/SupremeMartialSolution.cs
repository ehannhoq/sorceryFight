using CalamityMod;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Items.Materials;
using sorceryFight.Content.Projectiles.Abilities;
using sorceryFight.Content.Projectiles.Melee;
using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class SupremeMartialSolution : ModItem
    {
        private const int RMB_COOLDOWN_TICKS = 60 * 20; // 20 seconds

        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.SupremeMartialSolution.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.SupremeMartialSolution.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.maxStack = 1;

            // Basic jab attack
            Item.damage = 100;
            Item.knockBack = 3f;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;

            Item.noUseGraphic = true;
            Item.noMelee = true;

            Item.shoot = ModContent.ProjectileType<SupremeMartialJab>();
            Item.shootSpeed = 18f;

            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.rare = ModContent.RarityType<SorceryFightWeapon>();
            Item.value = Item.sellPrice(platinum: 1);
        }

        public override void ModifyWeaponCrit(Player player, ref float crit)
        {
            crit = 3f;
        }

        // Enables right-click
        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            // Left-click: jab
            if (player.altFunctionUse != 2)
            {
                Item.useTime = 10;
                Item.useAnimation = 10;
                Item.shoot = ModContent.ProjectileType<SupremeMartialJab>();
                return player.ownedProjectileCounts[Item.shoot] <= 0;
            }

            // Right-click: lightning ability
            return player.GetModPlayer<SFPlayer>()
                         .CanUseAbility(RMB_COOLDOWN_TICKS, 1350);
        }

        public override bool Shoot(
            Player player,
            EntitySource_ItemUse_WithAmmo source,
            Vector2 position,
            Vector2 velocity,
            int type,
            int damage,
            float knockback)
        {
            // Right-click lightning strike
            if (player.altFunctionUse == 2)
            {
                Vector2 targetPos = Main.MouseWorld;

                Projectile.NewProjectile(
                    source,
                    targetPos + new Vector2(0f, -600f),
                    Vector2.UnitY * 30f,
                    ModContent.ProjectileType<CursedLightningStrike>(),
                    2300,
                    0f,
                    player.whoAmI
                );

                player.GetModPlayer<SFPlayer>()
                      .ConsumeAbility(RMB_COOLDOWN_TICKS, 1350);

                return false;
            }

            // Left-click jab projectile
            Projectile.NewProjectile(
                source,
                player.Center,
                velocity,
                type,
                damage,
                knockback,
                player.whoAmI
            );

            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);

            recipe.AddIngredient(ModContent.ItemType<ForsakenSaber>());
            recipe.AddIngredient(ModContent.ItemType<CoreOfCalamity>(), 3);
            recipe.AddIngredient(ModContent.ItemType<CursedDischarge>(), 5);
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddIngredient(ItemID.BowieKnife);

            // Ancient Manipulator (real internal name)
            recipe.AddTile(TileID.LunarCraftingStation);

            recipe.Register();
        }
    }
}
