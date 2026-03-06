using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using sorceryFight.Rarities;
using sorceryFight.SFPlayer;

namespace sorceryFight.Content.Items.Accessories
{
    public class DuplexFace : ModItem
    {
        public static float curseDamageIncrease = 0.05f;
        public static float shrineDamageIncrease = 0.15f;
        public static float ceCostIncrease = 0.03f;
        public static float castTimeReduction = 0.13f;

        public static int maxCursedEnergyIncrease = 500;
        public static int cursedEnergyRegenIncrease = 2;

        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.DuplexFace.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.DuplexFace.Tooltip")
            .WithFormatArgs(
                (int)(curseDamageIncrease * 100),
                (int)(shrineDamageIncrease * 100),
                cursedEnergyRegenIncrease,
                maxCursedEnergyIncrease,
                (int)(ceCostIncrease * 100),
                (int)(castTimeReduction * 100)
            );

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.width = 40;
            Item.height = 40;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            // Base curse damage
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + curseDamageIncrease;

            // Shrine bonus
            if (sfPlayer.innateTechnique != null &&
                sfPlayer.innateTechnique.Name.Contains("Shrine"))
            {
                player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + shrineDamageIncrease;
            }

            // CE stats
            sfPlayer.maxCursedEnergyFromOtherSources += maxCursedEnergyIncrease;
            sfPlayer.cursedEnergyRegenFromOtherSources += cursedEnergyRegenIncrease;

            // Technique modifiers
            sfPlayer.cursedEnergyCostMultiplier *= 1f + ceCostIncrease;
            sfPlayer.techniqueCastTimeMultiplier *= 1f - castTimeReduction;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return player.GetModPlayer<SorceryFightPlayer>().duplexBody;
        }
    }
}