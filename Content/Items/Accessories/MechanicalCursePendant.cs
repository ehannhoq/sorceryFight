using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;
using Terraria.ID;
using sorceryFight.Rarities;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Items.Materials;

namespace sorceryFight.Content.Items.Accessories
{
    public class MechanicalCursePendant : ModItem
    {
        public static int maxCursedEnergyIncrease = 500;
        public static int cursedEnergyRegenIncrease = 4;

        public override LocalizedText DisplayName =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.MechanicalCursePendant.DisplayName");

        public override LocalizedText Tooltip =>
            SFUtils.GetLocalization("Mods.sorceryFight.Accessories.MechanicalCursePendant.Tooltip")
            .WithFormatArgs(maxCursedEnergyIncrease, cursedEnergyRegenIncrease);

        public override void SetDefaults()
        {
            Item.accessory = true;
            Item.maxStack = 1;
            Item.width = 38;
            Item.height = 38;
            Item.rare = ModContent.RarityType<SorceryFightAccessory>();
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            sfPlayer.maxCursedEnergyFromOtherSources += maxCursedEnergyIncrease;
            sfPlayer.cursedEnergyRegenFromOtherSources += cursedEnergyRegenIncrease;

            // Enable curse flames on techniques
            sfPlayer.inflictCurseFlames = true;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Type);
            recipe.AddIngredient(ModContent.ItemType<HallowedBar>(), 10);
            recipe.AddIngredient(ModContent.ItemType<MergedCurseFragment>(), 3);
            recipe.AddIngredient(ModContent.ItemType<InfusedCurseFragment>(), 3);
            recipe.AddIngredient(ModContent.ItemType<WulfrumCursePendant>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }
    }
}
            sfPlayer.techniqueCastTimeMultiplier *= 1f - castTimeReduction;

            // CE cost increase
            sfPlayer.techniqueCECOSTMultiplier *= 1f + ceCostIncrease;
        }
    }
}