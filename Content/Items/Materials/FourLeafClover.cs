using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Materials
{
    public class FourLeafClover : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.FourLeafClover.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.FourLeafClover.Tooltip");

        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 99;
            Item.width = 26;
            Item.height = 26;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
        }
    }
}