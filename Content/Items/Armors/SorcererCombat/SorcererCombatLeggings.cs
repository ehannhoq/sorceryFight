using sorceryFight.Content.Items.Materials;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.SorcererCombat
{
    [AutoloadEquip(EquipType.Legs)]
    public class SorcererCombatLeggings : ModItem
    {
        public static int ceRegenIncrease = 50;
        public static float ctDamageIncrease = 0.12f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SorcererCombatLeggings.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SorcererCombatLeggings.Tooltip").WithFormatArgs(ceRegenIncrease, (int)(ctDamageIncrease * 100f));

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamageIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegenIncrease;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 12;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient(ItemID.HallowedBar, 11);
            recipe.AddIngredient(ModContent.ItemType<InfusedCursedFragment>(), 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}