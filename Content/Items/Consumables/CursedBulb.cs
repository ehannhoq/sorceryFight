// using System;
// using CsvHelper;
// using Terraria;
// using Terraria.ModLoader;

// namespace sorceryFight.Content.Items.Consumables
// {
//     public class CursedBulb : ModItem
//     {
//         public override void SetDefaults()
//         {
//             Item.consumable = true;
//             Item.maxStack = 1;
//         }

//         public override void OnConsumeItem(Player player)
//         {
//             SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
//             sf.cursedBulb = !sf.cursedBulb;
//             sf.cursedEnergyRegenPerSecond = sf.calculateCERegenRate();
//         }
//     }
// }