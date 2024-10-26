using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.Audio;

namespace sorceryFight.Content.Items.Consumables
{
    public class CursedProfanedShards : ModItem
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
        }

        public override bool? UseItem(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4);
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            sf.cursedProfaneShards = !sf.cursedProfaneShards; // temp
            sf.maxCursedEnergy = sf.calculateMaxCE();
            return true;
        }
    }
}