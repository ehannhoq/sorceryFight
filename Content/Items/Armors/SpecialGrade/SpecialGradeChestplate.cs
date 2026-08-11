using JetBrains.Annotations;
using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.Content.Items.Materials;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.SpecialGrade
{
    [AutoloadEquip(EquipType.Body)]
    public class SpecialGradeChestplate : ModItem
    {
        public static int maxCEIncrease = 500;
        public static int ceRegenIncrease = 100;
        public static float ctDamageIncrease = 0.2f;

        public static float costReductionFromSetBonus = 0.07f;


        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeChestplate.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeChestplate.Tooltip").WithFormatArgs(maxCEIncrease, ceRegenIncrease, (int)(ctDamageIncrease * 100f));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 23;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamageIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.maxCursedEnergyFromOtherSources += maxCEIncrease;
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegenIncrease;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Type && legs.type == ModContent.ItemType<SpecialGradeLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SFUtils.GetLocalization("Mods.sorceryFight.Armors.SpecialGradeSetBonus").WithFormatArgs(
                (int)(costReductionFromSetBonus * 100f)
            ).Value;
            
            player.SorceryFight().ctCostReduction += costReductionFromSetBonus;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient(ItemID.ChlorophyteBar, 15);
            recipe.AddIngredient(ModContent.ItemType<InfusedCursedFragment>(), 13);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}