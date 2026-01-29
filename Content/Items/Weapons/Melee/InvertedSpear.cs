using System;
using System.Linq;
using CalamityMod;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Projectiles.Melee;
using sorceryFight.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class InvertedSpear : ModItem
    {
        private static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.InvertedSpear.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.InvertedSpear.Tooltip");

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Items/Weapons/Melee/InvertedSpear", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Item.width = 75;
            Item.height = 90;
            Item.maxStack = 1;
            Item.useTime = 1;
            Item.damage = 700;
            Item.knockBack = 5;
            Item.channel = true;
            Item.shootSpeed = 24f;
            Item.autoReuse = false;
            Item.useAnimation = 1;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<SorceryFightWeapon>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<InvertedSpearSlash>();
            Item.DamageType = DamageClass.Melee;
            Item.noMelee = true;
            Item.ArmorPenetration = 1000;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 1;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameRect, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            spriteBatch.Draw(texture, position, frameRect, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, null, lightColor, rotation, texture.Size() * 0.5f, scale, SpriteEffects.None, 0f);
            return false;
        }

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

        public override bool AltFunctionUse(Player player) => true;

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse == 2)
            {
                if (Item.shoot == ModContent.ProjectileType<InvertedSpearSlash>())
                {
                    Item.shoot = ModContent.ProjectileType<InvertedSpearCharge>();
                }

                else if (Item.shoot == ModContent.ProjectileType<InvertedSpearCharge>())
                {
                    Item.shoot = ModContent.ProjectileType<InvertedSpearSlash>();
                }

                return true;
            }
            return null;
        }
    }
}
