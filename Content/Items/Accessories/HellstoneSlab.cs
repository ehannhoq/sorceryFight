using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Items.Materials;

namespace sorceryFight.Content.Items.Accessories
{
    public class HellstoneSlab : ModItem
    {
        public static int maxCursedEnergyIncrease = 200;
        public static int cursedEnergyRegenIncrease = 3;
        
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.HellstoneSlab.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.HellstoneSlab.Tooltip").WithFormatArgs(maxCursedEnergyIncrease, cursedEnergyRegenIncrease);

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
            Item.width = 32;
            Item.height = 32;
        }
       
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            
            sfPlayer.maxCursedEnergyFromOtherSources += maxCursedEnergyIncrease;
            sfPlayer.cursedEnergyRegenFromOtherSources += cursedEnergyRegenIncrease;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.HellstoneBar, 10);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 5);
            recipe.AddTile(TileID.Hellforge);
            recipe.Register();
        }
    }
}