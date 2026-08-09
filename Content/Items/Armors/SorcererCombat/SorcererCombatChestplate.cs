using sorceryFight.Content.Items.Armors.QuantumCoulomb;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.SorcererCombat
{
    [AutoloadEquip(EquipType.Body)]
    public class SorcererCombatChestplate : ModItem
    {
        public static int maxCeIncrease = 200;
        public static float ctDamageIncrease = 0.12f;

        public static float movementSpeedFromSetBonus = 0.14f;
        public static int defenseFromSetBonus = 9;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SorcererCombatChestplate.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.SorcererCombatChestplate.Tooltip").WithFormatArgs(maxCeIncrease, (int)(ctDamageIncrease * 100f));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Blue;
            Item.defense = 25;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamageIncrease;
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.maxCursedEnergyFromOtherSources += maxCeIncrease;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Type && legs.type == ModContent.ItemType<SorcererCombatLeggings>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = SFUtils.GetLocalization("Mods.sorceryFight.Armors.SorcererCombatSetBonus").WithFormatArgs(
                (int)(movementSpeedFromSetBonus * 100f),
                defenseFromSetBonus
            ).Value;
            
            player.moveSpeed += movementSpeedFromSetBonus;
            player.statDefense += defenseFromSetBonus;
        }
    }
}