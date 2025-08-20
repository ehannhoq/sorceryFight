using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.EnshroudedOne
{
    [AutoloadEquip(EquipType.Head)]
    public class EnshroudedHair : ModItem
    {
        public static float limitlessDamage = 0.05f;
        public static float allDamage = 0.05f;
        public static int ceRegen = 50;
        public static float rctOutput = 0.5f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedHair.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedHair.Tooltip").WithFormatArgs((int)(limitlessDamage * 100), (int)(allDamage * 100), ceRegen, (int)(rctOutput * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 45;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            if (sfPlayer.innateTechnique.Name == "Limitless")
                player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + limitlessDamage;

            player.GetDamage(DamageClass.Generic) *= 1 + allDamage;
            
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegen;
        }
    }
}