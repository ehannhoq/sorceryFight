using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using sorceryFight.Content.CursedTechniques;
using System.Linq;
using sorceryFight.Rarities;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using sorceryFight.Content.InnateTechniques;
using System;

namespace sorceryFight.Content.Items
{ 
	public class CursedTechniqueItem : ModItem
	{
		public SorceryFightPlayer player;
		public override void SetDefaults()
		{
			Item.width = -1;
			Item.height = -1;
			Item.maxStack = 1;
			Item.value = Item.sellPrice(0, 0, 0, 1);
			Item.rare = ModContent.RarityType<SorceryFightRed>();

			Item.useStyle = ItemUseStyleID.HoldUp; // If this is anything else it will bug tf out
			Item.noUseGraphic = true;

			Item.useTime = 10;
			Item.useAnimation = 10;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 1);
			recipe.Register();
		}
		
		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (player.selectedTechnique == null)
				return;
				
			for (int i = tooltips.Count - 1; i >= 0; i --)
			{
				tooltips.RemoveAt(i);
			}

			tooltips.Add(new TooltipLine(Mod, "name", player.innateTechnique.Name));
			string keybind = SFKeybinds.OpenTechniqueUI.GetAssignedKeys().FirstOrDefault() ?? "Unbound";
			tooltips.Add(new TooltipLine(Mod, "keybind", $"Press [{keybind}] to open menu."));

			tooltips.Add(new TooltipLine(Mod, "ctName", $"Equipped: {player.selectedTechnique.Name}")
			{
				OverrideColor = new SorceryFightGold().RarityColor
			});

			if (this.player.selectedTechnique.Name != "None Selected.")
			{
				tooltips.Add(new TooltipLine(Mod, "ceDamage", $"Damage: {player.selectedTechnique.Damage}"));
				tooltips.Add(new TooltipLine(Mod, "ceCost", $"Cost: {CalculateCECost(player, player.selectedTechnique)} CE"));
			}

		}

        public override void UpdateInventory(Player player)
        {	
			this.player = player.GetModPlayer<SorceryFightPlayer>();

			if (this.player.selectedTechnique != null)
			{
				Item.shoot = this.player.selectedTechnique.GetProjectileType();

				Item.damage = this.player.selectedTechnique.Damage;
				Item.shootSpeed = this.player.selectedTechnique.Speed;
			}
			else
			{
				this.player.selectedTechnique = new CursedTechnique();
			}

			// Subject to change, once more innate techniques are added.
			if (player.HeldItem.type == ModContent.ItemType<CursedTechniqueItem>() && SFKeybinds.OpenTechniqueUI.JustPressed)
			{
				if (this.player.innateTechnique.Name == "No Innate Technique")
				{
					this.player.innateTechnique = new LimitlessTechnique();
				}
			}
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (this.player.hasUIOpen)
			{
				return false;
			}
			
			float ceDue = CalculateCECost(this.player, this.player.selectedTechnique);

			if (this.player.cursedEnergy >= ceDue)
			{
				this.player.cursedEnergy -= ceDue;
				Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);

				int index = CombatText.NewText(player.getRect(), this.player.selectedTechnique.textColor,this.player.selectedTechnique.Name);
				Main.combatText[index].lifeTime = 180;
			}
			else
			{
				int index = CombatText.NewText(player.getRect(), Color.DarkRed, "Not enough Cursed Energy!");
				Main.combatText[index].lifeTime = 180;
			}

			return false;
        }

		private float CalculateCECost(SorceryFightPlayer sf, CursedTechnique ct)
		{	
			if (ct.Cost == -1)
			{
				return sf.maxCursedEnergy * (ct.CostPercentage / 100);
			}

			if (ct.CostPercentage == -1)
			{
				return ct.Cost;
			}

			return 0;
		}
    }
}

