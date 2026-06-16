using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Materials
{
    public class CursedHammer : ModItem
    {
        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedHammer.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Materials.CursedHammer.Tooltip");

        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 99;
            Item.width = 32;
            Item.height = 32;
            Item.value = Item.buyPrice(silver: 75);
            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
        }
    }
}