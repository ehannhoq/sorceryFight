using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.QuantumCoulomb
{
    [AutoloadEquip(EquipType.Head)]
    public class QuantumCoulombBottle : ModItem
    {
        public static float ctDamage = 0.12f;
        public static float allDamage = 0.05f;
        public static int ceRegen = 50;
        public static float rctEff = 0.15f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.QuantumCoulombBottle.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.QuantumCoulombBottle.Tooltip").WithFormatArgs((int)(ctDamage * 100), (int)(allDamage * 100), ceRegen, (int)(rctEff * 100));

        public override void SetStaticDefaults()
        {
            ArmorIDs.Head.Sets.DrawHead[Item.headSlot] = false;
        }
        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 45;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamage;
            player.GetDamage(DamageClass.Generic) *= 1 + allDamage;
            sfPlayer.cursedEnergyRegenFromOtherSources += ceRegen;
            sfPlayer.rctEfficiency += rctEff;
        }
    }
}