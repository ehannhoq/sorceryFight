using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Armors.EnshroudedOne
{
    [AutoloadEquip(EquipType.Legs)]
    public class EnshroudedPants : ModItem
    {
        public static float limitlessDamage = 0.20f;
        public static float otherCTDamage = 0.10f;
        public static float rctOutput = 0.5f;
        public static float movementSpeed = 0.1f;
        public static float flightTime = 0.1f;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedPants.DisplayName");
        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Armors.EnshroudedPants.Tooltip").WithFormatArgs((int)(limitlessDamage * 100), (int)(otherCTDamage * 100), (int)(rctOutput * 100), (int)(movementSpeed * 100), (int)(flightTime * 100));

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.rare = ItemRarityID.Pink;
            Item.defense = 50;
        }

        public override void UpdateEquip(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            if (sfPlayer.innateTechnique.Name == "Limitless")
                player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + limitlessDamage;
            else
                player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + otherCTDamage;

            player.moveSpeed *= 1 + movementSpeed;
            player.wingTimeMax += (int)(player.wingTimeMax * flightTime);
        }
    }
}