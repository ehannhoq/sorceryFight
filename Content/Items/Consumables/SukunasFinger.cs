using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class SukunasFinger : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.SukunasFinger.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.SukunasFinger.Tooltip");
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ModContent.RarityType<SorceryFightRed>();
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
                
                if (!sf.innateTechnique.Name.Equals("Shrine")) return false;

                if (sf.sukunasFingerConsumed++ < 20) return true;
                else return false;
            }
            return false;
        }
    }
}