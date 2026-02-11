using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod;
using CalamityMod.Items.Materials;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Projectiles.Melee;
using sorceryFight.Content.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class ChakiraResonant : ModItem
    {
        private const int FRAMES = 10;
        private const int TICKS_PER_FRAME = 6;
        private static Texture2D texture;

        private const int baseDamage = 50;
        private const float damageMultiplier = 3.67f;
        private const float critMultiplier = 1;
        private int addedDamage = 0;
        private float addedCrit = 0;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.ChakiraResonant.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.ChakiraResonant.Tooltip");

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(TICKS_PER_FRAME, FRAMES));
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/Items/Weapons/Melee/ChakiraResonant", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Item.width = 250;
            Item.height = 250;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.damage = 50;
            Item.knockBack = 5;
            Item.noUseGraphic = true;
            Item.channel = true;
            Item.autoReuse = false;
            Item.rare = ModContent.RarityType<SorceryFightLegendary>();
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<ChakiraResonantHoldout>();
            Item.shootSpeed = 24f;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.noMelee = true;
            Item.ArmorPenetration = 1000;
        }

        public override void ModifyWeaponCrit(Player player, ref float crit) => crit = Item.crit + addedCrit;

        public override void UpdateInventory(Player player)
        {
            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();
            addedDamage = (int)Math.Ceiling(MathF.Pow(sfPlayer.numberBossesDefeated, damageMultiplier) / 750f);
            Item.damage = baseDamage + addedDamage;
            addedCrit = critMultiplier * sfPlayer.numberBossesDefeated;
        }

        public override void HoldItem(Player player)
        {
            if (player.whoAmI != Main.myPlayer)
                return;

            SorceryFightPlayer sfPlayer = player.SorceryFight(); 

            if ((!sfPlayer.uniqueBodyStructure && sfPlayer.disableCurseTechniques) || player.HasBuff(ModContent.BuffType<BurntTechnique>()) || sfPlayer.heavenlyRestriction)
                return;

            if (PlayerInput.Triggers.Current.MouseRight && CanUseItem(player) && !Main.mapFullscreen && !Main.blockMouse)
            {
                if (Main.projectile.Any(proj => proj.active && proj.type == Item.shoot && proj.owner == player.whoAmI))
                    return;

                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, Item.shoot, Item.damage, 0f, player.whoAmI, 1f);
            }
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.FindAndReplace("[BOSSES]", Main.LocalPlayer.SorceryFight().numberBossesDefeated.ToString());
            tooltips.FindAndReplace("[DAMAGE]", addedDamage.ToString());
            tooltips.FindAndReplace("[CRIT]", addedCrit.ToString());
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
                return false;

            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, 0f);
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            return true;
        }


        public override bool PreDrawTooltipLine(DrawableTooltipLine line, ref int yOffset)
        {
            if (line.Index != 0) return true;

            float timer = Main.GlobalTimeWrappedHourly % 1f;
            float scale = 1f + timer * 0.3f;
            float alpha = 1f - timer;

            Vector2 offset = new Vector2(scale - 1f, scale - 1f) * 10f;

            Main.spriteBatch.DrawString(
                line.Font,
                line.Text,
                new Vector2(line.X - (8 * offset.X), line.Y - offset.Y),
                line.Color * alpha,
                line.Rotation,
                line.Origin,
                scale,
                SpriteEffects.None,
                0
            );

            return true;
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frameRect, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            int frameHeight = texture.Height / FRAMES;
            int frameY = (int)(Main.GameUpdateCount / TICKS_PER_FRAME % FRAMES) * frameHeight;
            Rectangle srcRect = new Rectangle(0, frameY, texture.Width, frameHeight);

            spriteBatch.Draw(texture, position, srcRect, Color.White, 0f, origin, scale * 1.5f, SpriteEffects.None, 0f);

            return false;
        }

        public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
        {
            spriteBatch.End();
            spriteBatch.Begin(
                SpriteSortMode.Immediate,
                BlendState.NonPremultiplied,
                SamplerState.LinearClamp,
                DepthStencilState.None,
                RasterizerState.CullNone,
                null,
                Main.GameViewMatrix.ZoomMatrix
            );

            int frameHeight = texture.Height / FRAMES;
            int frameY = (int)(Main.GameUpdateCount / TICKS_PER_FRAME % FRAMES) * frameHeight;
            Rectangle srcRect = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = new Vector2(texture.Width / 2f, frameHeight / 2f);

            spriteBatch.Draw(texture, Item.Center - Main.screenPosition, srcRect, lightColor, rotation, origin, scale, SpriteEffects.None, 0f);


            spriteBatch.End();
            spriteBatch.Begin();
            return false;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<ObliviousSword>());
            recipe.AddIngredient(ModContent.ItemType<UnholyEssence>(), 60);
            recipe.AddIngredient(ModContent.ItemType<GalacticaSingularity>(), 4);
            recipe.AddIngredient(ModContent.ItemType<Necroplasm>(), 5);
            recipe.AddIngredient(ModContent.ItemType<DivineGeode>(), 30);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
