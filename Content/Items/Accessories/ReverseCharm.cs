using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Accessories
{
    public class ReverseCharm : ModItem
    {
        public static readonly int additionalHeal = 12;
        public static readonly float additionalEfficiency = 0.10f;
    
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.ReverseCharm.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Accessories.ReverseCharm.Tooltip").WithFormatArgs(additionalHeal, (int)(additionalEfficiency * 100));

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
            Item.width = 54;
            Item.height = 50;
        }

        public override bool CanEquipAccessory(Player player, int slot, bool modded)
        {
            return !player.SorceryFight().heavenlyRestriction;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            base.UpdateAccessory(player, hideVisual);

            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.additionalRCTHealPerSecond += additionalHeal; 
            sfPlayer.rctEfficiency += additionalEfficiency;
        }
    }
}
