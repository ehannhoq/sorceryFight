using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Novelty
{
    public class Lollipop : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.NoveltyItems.Lollipop.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.NoveltyItems.Lollipop.Description");

        public override void SetDefaults()
        {
            Item.width = 48;
            Item.height = 48;
            Item.consumable = true;
            Item.maxStack = 999;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.EatFood;
        }

        public override bool CanUseItem(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            return sfPlayer.cursedEnergy > 0;
        }

        public override bool? UseItem(Player player)
        {
            if (Main.myPlayer == player.whoAmI)
            {
                player.GetModPlayer<SorceryFightPlayer>().cursedEnergy -= 100;
            }

            return true;
        }
    }
}