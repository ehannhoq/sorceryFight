using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace sorceryFight.Content.Items.Accessories
{
    public class BlessingOfTheHonoredOne : ModItem
    {
        public static float limitlessDamageIncrease = 0.13f;
        public static float cursedTechniqueDamageIncrease = 0.05f;

        private const int FRAMES = 6;
        private const int TICKS_PER_FRAME = 8;
        private static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.BlessingOfTheHonoredOne.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.BlessingOfTheHonoredOne.Tooltip").WithFormatArgs((int)(limitlessDamageIncrease * 100), (int)(cursedTechniqueDamageIncrease * 100));

        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(TICKS_PER_FRAME, FRAMES));
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
            Item.width = 54;
            Item.height = 50;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            if (sfPlayer.innateTechnique != null)
            {
                if (sfPlayer.innateTechnique.Name.Equals("Limitless"))
                    player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + limitlessDamageIncrease;
                else
                    player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + cursedTechniqueDamageIncrease;

            }
        }


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

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Materials.CoreofEleum>(), 3);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddIngredient(ItemID.SoulofLight, 3);
            recipe.AddIngredient(ItemID.Ectoplasm, 2);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}
