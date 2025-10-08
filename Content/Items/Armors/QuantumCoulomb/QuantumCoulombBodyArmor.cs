using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.QuantumCoulomb
{
    [AutoloadEquip(EquipType.Body)]
    public class QuantumCoulombBodyArmor : ModItem
    {
        public static float allDamage = 0.05f;
        public static float rctOutput = 1.5f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.QuantumCoulombBodyArmor.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.QuantumCoulombBodyArmor.Tooltip").WithFormatArgs((int)(allDamage * 100), (int)(rctOutput * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 60;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            player.GetDamage(DamageClass.Generic) *= 1 + allDamage;
            sfPlayer.additionalRCTHealPerSecond += (int)(sfPlayer.rctBaseHealPerSecond * rctOutput);
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type == ModContent.ItemType<QuantumCoulombBottle>() && body.type == Type && legs.type == ModContent.ItemType<QuantumCoulombChausses>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawOutlines = true;
        }
    }
}