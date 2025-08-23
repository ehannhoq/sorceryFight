using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class SuspiciouslyWellPerservedEye : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.SuspiciouslyWellPerservedEye.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Consumables.SuspiciouslyWellPerservedEye.Tooltip");

        public override void SetDefaults()
        {
            Item.height = 80;
            Item.width = 80;
            Item.consumable = true;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Pink;
        }

        public override bool CanUseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
                return sfPlayer.sixEyes && !sfPlayer.hollowEyes;
            }
            return false;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                player.GetModPlayer<SorceryFightPlayer>().hollowEyes = true;
                SoundEngine.PlaySound(SoundID.Item4);
            }

            return true;
        }
    }
}