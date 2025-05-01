using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;

namespace sorceryFight.Content.Items.Accessories
{
    public class CursedBlindfold : ModItem
    {
        public static float limitlessDamageIncrease = 0.06f;
        public static float cursedTechniqueDamageIncrease = 0.03f;
        public static int maxCursedEnergyIncrease = 200;
        public static int cursedEnergyRegenIncrease = 2;
        
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CursedBlindfold.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.CursedBlindfold.Tooltip").WithFormatArgs((int)(limitlessDamageIncrease * 100), (int)(cursedTechniqueDamageIncrease * 100), maxCursedEnergyIncrease, cursedEnergyRegenIncrease);

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
            if (sfPlayer.innateTechnique != null)
            {
                if (sfPlayer.innateTechnique.Name.Equals("Limitless"))
                    player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + limitlessDamageIncrease;
                else
                    player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + cursedTechniqueDamageIncrease;
            }
            sfPlayer.maxCursedEnergyFromOtherSources += maxCursedEnergyIncrease;
            sfPlayer.cursedEnergyRegenFromOtherSources += cursedEnergyRegenIncrease;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return player.GetModPlayer<SorceryFightPlayer>().sixEyes;
        }
    }
}
