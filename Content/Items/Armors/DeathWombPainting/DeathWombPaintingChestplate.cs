using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.DeathWombPainting
{
    [AutoloadEquip(EquipType.Body)]
    public class DeathWombPaintingChestplate : ModItem
    {
        public static int maxCeIncrease = 75;
        public static float ctDamageIncrease = 0.1f;

        public static int ceRegenFromSetBonus = 5;
        public static int hpRegenFromSetBonus = 10;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.DeathWombPaintingChestplate.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.DeathWombPaintingChestplate.Tooltip").WithFormatArgs(maxCeIncrease, (int)(ctDamageIncrease * 100f));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Green;
            Item.defense = 14;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamageIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.maxCursedEnergyFromOtherSources += maxCeIncrease;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Type && legs.type == ModContent.ItemType<DeathWombPaintingLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SFUtils.GetLocalization("Mods.sorceryFight.Armors.JujutsuHighSetBonus").WithFormatArgs(
                ceRegenFromSetBonus,
                hpRegenFromSetBonus / 2
            ).Value;

            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegenFromSetBonus;
            player.lifeRegen += hpRegenFromSetBonus;
        }

        public override void AddRecipes()
        {
            Recipe recipeAdamantite = Recipe.Create(Item.type);
            recipeAdamantite.AddIngredient(ItemID.AdamantiteBar, 17);
            recipeAdamantite.AddIngredient(ItemID.LifeCrystal, 1);
            recipeAdamantite.AddIngredient(ItemID.SoulofLight, 3);
            recipeAdamantite.AddIngredient(ItemID.SoulofNight, 3);
            recipeAdamantite.AddTile(TileID.MythrilAnvil);
            recipeAdamantite.Register();

            Recipe recipeTitanium = Recipe.Create(Item.type);
            recipeTitanium.AddIngredient(ItemID.TitaniumBar, 17);
            recipeTitanium.AddIngredient(ItemID.LifeCrystal, 1);
            recipeTitanium.AddIngredient(ItemID.SoulofLight, 3);
            recipeTitanium.AddIngredient(ItemID.SoulofNight, 3);
            recipeTitanium.AddTile(TileID.MythrilAnvil);
            recipeTitanium.Register();
        }
    }
}