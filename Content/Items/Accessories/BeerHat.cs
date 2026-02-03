using sorceryFight.Rarities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System.Collections.Generic;
using System.Linq;
using sorceryFight.Content.Items.Consumables;
using Terraria.ID;

namespace sorceryFight.Content.Items.Accessories
{
    public class BeerHat : ModItem
    {

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.BeerHat.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.BeerHat.Tooltip");

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
            Item.width = 54;
            Item.height = 50;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.SorceryFight().heavenlyRestriction;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.beerHat = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Bottle, 2);
            recipe.AddIngredient(ItemID.Silk, 5);
            recipe.AddIngredient(ItemID.SoulofMight, 1);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
