using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace sorceryFight.Content.Items.Materials
{
    public class SukunasSkull : ModItem
    {

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Materials.SukunasSkull.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Materials.SukunasSkull.Tooltip");

        public override void SetDefaults()
        {
            Item.material = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
            Item.width = 50;
            Item.height = 50;
        }
    }
}
