using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Items.Materials;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class PotentialSword : ModItem
    { 
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.PotentialSword.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.PotentialSword.Tooltip");

        public override void SetDefaults()
        {
            Item.width = 43;
            Item.height = 88;
            Item.maxStack = 1;
            Item.useTime = 12;
            Item.damage = 18;
            Item.crit = 15;
            Item.knockBack = 4;
            Item.useAnimation = 12;
            Item.autoReuse = true;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.Item1;
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            Recipe recipeCrimtane = Recipe.Create(Item.type);
            recipeCrimtane.AddIngredient(ItemID.CrimtaneBar, 14);
            recipeCrimtane.AddIngredient(ItemID.BlackInk, 5);
            recipeCrimtane.AddIngredient(ModContent.ItemType<CursedFragment>(), 12);
            recipeCrimtane.AddTile(TileID.Anvils);
            recipeCrimtane.Register();

            Recipe recipeDemonite = Recipe.Create(Item.type);
            recipeDemonite.AddIngredient(ItemID.DemoniteBar, 14);
            recipeDemonite.AddIngredient(ItemID.BlackInk, 5);
            recipeDemonite.AddIngredient(ModContent.ItemType<CursedFragment>(), 12);
            recipeDemonite.AddTile(TileID.Anvils);
            recipeDemonite.Register();
        }
    }
}
