using System;
using CsvHelper;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Consumables
{
    public class CursedEffulgentFeather : ModItem
    {
        public override void SetStaticDefaults()
        {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 8));
        }
        public override void SetDefaults()
        {
            Item.consumable = true;
            Item.maxStack = 1;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.rare = ItemRarityID.Master;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                SoundEngine.PlaySound(SoundID.Item4);
                SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
                sf.cursedEffulgentFeather = !sf.cursedEffulgentFeather; // temp
                sf.cursedEnergyRegenPerSecond = sf.calculateCERegenRate();
            }
            return true;
        }
    }
}