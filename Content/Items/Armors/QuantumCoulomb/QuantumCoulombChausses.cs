using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.QuantumCoulomb
{
    [AutoloadEquip(EquipType.Legs)]
    public class QuantumCoulombChausses : ModItem
    {
        public static float ctDamage = 0.07f;
        public static float allDamage = 0.05f;
        public static float rctEff = 0.25f;
        public static float movementSpeed = 0.15f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.QuantumCoulombChausses.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.QuantumCoulombChausses.Tooltip").WithFormatArgs((int)(ctDamage * 100), (int)(allDamage * 100), (int)(rctEff * 100), (int)(movementSpeed * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 55;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + ctDamage;
            player.GetDamage(DamageClass.Generic) *= 1 + allDamage;
            player.moveSpeed *= 1 + movementSpeed;
            sfPlayer.rctEfficiency += rctEff;
        }
    }
}