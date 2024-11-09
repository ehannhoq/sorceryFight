using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class YourPotential : ModItem
    {
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 1;
            Item.useAnimation = 1;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
                player.GetModPlayer<SorceryFightPlayer>().yourPotentialSwitch = true;

            return true;
        }
    }
}
