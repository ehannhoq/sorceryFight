using sorceryFight.Rarities;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System.Collections.Generic;

namespace sorceryFight.Content.Items.Accessories
{
    public class CelestialAmulet : ModItem
    {
        public static readonly int ChanceDenominator = 10;

        private const float cursedTechniqueDamageIncrease = 10f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CelestialAmulet.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CelestialAmulet.Tooltip").WithFormatArgs(20, cursedTechniqueDamageIncrease);

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
            sfPlayer.celestialAmulet = true;

            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + (cursedTechniqueDamageIncrease / 100f);
        }
    }
}
