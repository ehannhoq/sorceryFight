using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Armors.JujutsuHighUniform
{
    [AutoloadEquip(EquipType.Legs)]
    public class JujutsuHighPants : ModItem
    {
        public static float cursedTechniqueDamageIncrease = 0.02f;
        public static float critChanceIncrease = 3f;
        
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighPants.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighPants.Tooltip").WithFormatArgs((int)(cursedTechniqueDamageIncrease * 100), (int)critChanceIncrease);
       
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + cursedTechniqueDamageIncrease;
            player.GetCritChance(DamageClass.Generic) += critChanceIncrease;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Silk, 12);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 6);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}