// using System;
// using Terraria;
// using Terraria.ModLoader;

// namespace sorceryFight.Content.Items.Consumables
// {
//     public class CursedSkull : ModItem
//     {
//         public override void SetDefaults()
//         {
//             Item.consumable = true;
//             Item.maxStack = 1;
//         }

//         public override void OnConsumeItem(Player player)
//         {
//             SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
//             sf.cursedSkull = !sf.cursedSkull;
//             sf.maxCursedEnergy = sf.calculateCERegenRate();
//         }
//     }
// }