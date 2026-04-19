using sorceryFight.Rarities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Materials
{
    public class LunarCursedFragment : ModItem
    {

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Materials.LunarCursedFragment.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Materials.LunarCursedFragment.Tooltip");


        public override void SetDefaults()
        {
            Item.material = true;
            Item.rare = ModContent.RarityType<SorceryFightMaterial>();
            Item.width = 50;
            Item.height = 50;
            Item.maxStack = 999;
        }
    }
}
