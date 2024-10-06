using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using sorceryFight.Rarities;
using System.Collections.Generic;
using System.Diagnostics;

namespace sorceryFight.Content.Items.Weapons.Melee
{ 
	public class CursedKatana : ModItem
	{
		public override void SetDefaults()
		{
			Item.DamageType = DamageClass.Melee;
			Item.useStyle = ItemUseStyleID.Swing;
			
			Item.damage = 25;
			Item.knockBack = 4;
			Item.value = Item.buyPrice(silver: 1);
			Item.rare = ModContent.RarityType<SorceryFightBlue>();
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			
			Item.width = 40;
			Item.height = 40;

			Item.useTime = 20;
			Item.useAnimation = 20;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
    }
}
