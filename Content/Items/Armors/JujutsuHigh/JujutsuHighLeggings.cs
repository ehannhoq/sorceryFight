using sorceryFight.Content.Items.Materials;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.JujutsuHigh
{
    [AutoloadEquip(EquipType.Legs)]
    public class JujutsuHighLeggings : ModItem
    {
        public static int maxCeIncrease = 10;
        public static float ctDamageIncrease = 0.08f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighLeggings.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighLeggings.Tooltip").WithFormatArgs(maxCeIncrease, (int)(ctDamageIncrease * 100f));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.White;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamageIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.maxCursedEnergyFromOtherSources += maxCeIncrease;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient(ItemID.Silk, 14);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 3);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}