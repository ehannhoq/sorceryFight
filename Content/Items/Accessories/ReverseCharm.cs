using sorceryFight.Rarities;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Accessories
{
    public class ReverseCharm : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.ReverseCharm.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.ReverseCharm.Tooltip");

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
            Item.width = 30;
            Item.height = 30;
        }
    }
}