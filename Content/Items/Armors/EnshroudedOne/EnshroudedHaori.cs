using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.EnshroudedOne
{
    [AutoloadEquip(EquipType.Body)]
    public class EnshroudedHaori : ModItem
    {
        public static float limitlessDamage = 0.25f;
        public static float otherCTDamage = 0.13f;
        public static int ceRegen = 50;
        public static float rctOutput = 2.0f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedHaori.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedHaori.Tooltip").WithFormatArgs((int)(limitlessDamage * 100), (int)(otherCTDamage * 100), ceRegen, (int)(rctOutput * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 55;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            if (sfPlayer.innateTechnique.Name == "Limitless")
                player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + limitlessDamage;
            else
                player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + otherCTDamage;

            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegen;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<EnshroudedHair>() && body.type == Type && legs.type == ModContent.ItemType<EnshroudedPants>();
        }
    }
}
