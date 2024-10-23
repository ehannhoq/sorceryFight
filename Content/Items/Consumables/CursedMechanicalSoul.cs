// using System;
// using Terraria;
// using Terraria.ModLoader;

// namespace sorceryFight.Content.Items.Consumables
// {
//     public class CursedMechanicalSoul : ModItem
//     {
//         public override void SetDefaults()
//         {
//             Item.consumable = true;
//             Item.maxStack = 1;
//         }

//         public override void OnConsumeItem(Player player)
//         {
//             SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
//             sf.cursedMechanicalSoul = !sf.cursedMechanicalSoul;
//             sf.maxCursedEnergy = sf.calculateCERegenRate();
//         }
//     }
// }