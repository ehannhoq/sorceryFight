using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class CursedProfanedShards : ModItem
    {
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
        }

        public override bool? UseItem(Player player)
        {
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            sf.cursedProfaneShards = !sf.cursedProfaneShards;
            sf.maxCursedEnergy = sf.calculateMaxCE();
            return true;
        }
    }
}