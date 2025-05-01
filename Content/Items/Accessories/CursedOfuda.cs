using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Accessories
{
    public class CursedOfuda : ModItem
    {
        public static float cursedTechniqueCostDecrease = 0.95f;
        public static float cursedTechniqueCastTimeDecrease = 0.84f;
    
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CursedOfuda.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CursedOfuda.Tooltip").WithFormatArgs((int)((1 - cursedTechniqueCostDecrease) * 100f), (int)((1 - cursedTechniqueCastTimeDecrease) * 100));

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
            sfPlayer.cursedOfuda = true;
        }
    }
}
