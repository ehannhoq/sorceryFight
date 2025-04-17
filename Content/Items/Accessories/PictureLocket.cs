using sorceryFight.Rarities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System.Collections.Generic;
using Terraria.ID;

namespace sorceryFight.Content.Items.Accessories
{
    public class PictureLocket : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.PictureLocket.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.PictureLocket.Tooltip");

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
            sfPlayer.pictureLocket = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Chain, 10);
            recipe.AddIngredient(ModContent.ItemType<CalamityMod.Items.Placeables.Furniture.DevPaintings.ThankYouPainting>());
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
