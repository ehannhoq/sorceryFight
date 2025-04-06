using System;
using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Rarities;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class CrikySword : ModItem
    {
        private const int FRAMES = 9;
        private const int TICKS_PER_FRAME = 5;
        private static Texture2D texture;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.CrikySword.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.CrikySword.Tooltip");

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(TICKS_PER_FRAME, FRAMES));

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Items/Weapons/Melee/CrikySword", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Item.width = 75;
            Item.height = 90;
            Item.maxStack = 1;
            Item.useTime = 1;
            Item.damage = 1850;
            Item.knockBack = 5;
            Item.useAnimation = 1;
            Item.noUseGraphic = true;
            Item.rare = ModContent.RarityType<SorceryFightRed>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<CrikySwordSlash>();
            Item.DamageType = DamageClass.MeleeNoSpeed;
            Item.noMelee = true;
            Item.ArmorPenetration = 1000;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = 1;

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameRect, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int frameHeight = texture.Height / FRAMES;
            int frameY = (int)(Main.GameUpdateCount / TICKS_PER_FRAME % FRAMES) * frameHeight;
            Rectangle srcRect = new Rectangle(0, frameY, texture.Width, frameHeight);

            spriteBatch.Draw(texture, position, srcRect, Color.White, 0f, origin, scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            int frameHeight = texture.Height / FRAMES;
            int frameY = (int)(Main.GameUpdateCount / TICKS_PER_FRAME % FRAMES) * frameHeight;
            Rectangle srcRect = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, srcRect, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);
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
    }
}
