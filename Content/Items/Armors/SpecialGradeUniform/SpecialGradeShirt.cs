using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Items.Materials;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using sorceryFight.Rarities;

namespace sorceryFight.Content.Items.Armors.SpecialGradeUniform
{
    [AutoloadEquip(EquipType.Body)]
    public class SpecialGradeShirt : ModItem
    {  
        public static float cursedTechniqueDamageIncrease = 0.07f;
        public static float rctOutput = 1.5f;
        public static int maxCursedEnergyIncrease = 125; // total 250 increase

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeShirt.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeShirt.Tooltip").WithFormatArgs((int)(cursedTechniqueDamageIncrease * 100), (int)(rctOutput * 100), maxCursedEnergyIncrease);

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 42;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1f + cursedTechniqueDamageIncrease;
            sfPlayer.maxCursedEnergyFromOtherSources += maxCursedEnergyIncrease;
            sfPlayer.additionalRCTHealPerSecond += (int)(1.5f * sfPlayer.rctBaseHealPerSecond);
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ItemID.Silk, 18);
            recipe.AddIngredient(ItemID.BeetleHusk, 14);
            recipe.AddIngredient(ItemID.SoulofLight, 12);
            recipe.AddIngredient(ModContent.ItemType<InfusedCursedFragment>(), 7);
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.Register();
        }
    }
}