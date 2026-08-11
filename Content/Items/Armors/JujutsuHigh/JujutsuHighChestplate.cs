using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.Content.Items.Materials;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.JujutsuHigh
{
    [AutoloadEquip(EquipType.Body)]
    public class JujutsuHighChestplate : ModItem
    {
        public static int maxCeIncrease = 20;
        public static float ctDamageIncrease = 0.08f;

        public static int ceRegenFromSetBonus = 10;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighChestplate.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighChestplate.Tooltip").WithFormatArgs(maxCeIncrease, (int)(ctDamageIncrease * 100f));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.White;
            Item.defense = 7;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamageIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.maxCursedEnergyFromOtherSources += maxCeIncrease;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Type && legs.type == ModContent.ItemType<JujutsuHighLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighSetBonus").WithFormatArgs(
                ceRegenFromSetBonus
            ).Value;

            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegenFromSetBonus;
        }

        public override void AddRecipes()
        {
            Recipe recipe = Recipe.Create(Item.type);
            recipe.AddIngredient(ItemID.Silk, 23);
            recipe.AddIngredient(ModContent.ItemType<CursedFragment>(), 5);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}