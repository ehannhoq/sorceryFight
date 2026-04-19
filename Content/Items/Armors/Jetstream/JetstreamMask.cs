using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.Jetstream
{
    [AutoloadEquip(EquipType.Head)]
    public class JetstreamMask : ModItem
    {
        public static float meleeDmg = 0.13f;
        public static int critChance = 25;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JetstreamMask.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.JetstreamMask.Tooltip").WithFormatArgs((int)(meleeDmg * 100), critChance);

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawFullHair[Item.headSlot] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 48;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) *= 1 + meleeDmg;
            player.GetCritChance(DamageClass.Melee) += critChance;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            // recipe.AddIngredient(ModContent.ItemType<BloodflareHeadMelee>(), 1);
            // recipe.AddIngredient(ModContent.ItemType<ShadowspecBar>(), 5);
            // recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 5);
            // recipe.AddIngredient(ModContent.ItemType<MiracleMatter>(), 1);
            // recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.Register();
        }
    }
}