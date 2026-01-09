using System;
using sorceryFight.Content.Items.Armors.Jetstream;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class Bloodshed : ModBuff
    {
        public static float trueMeleeDmg = 0.20f;
        public static float movementSpeed = 0.15f;
        public static int increasedShields = 10;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.Bloodshed.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.Bloodshed.Description").WithFormatArgs((int)(trueMeleeDmg * 100), (int)(movementSpeed * 100), increasedShields);

        public override void Update(Player player, ref int buffIndex)
        {
            CalamityMod.TrueMeleeDamageClass tmInstance = SFUtils.GetInternalFieldFromCalamity<CalamityMod.TrueMeleeDamageClass>("CalamityMod.TrueMeleeDamageClass", "Instance");
            player.GetDamage(tmInstance) *= 1 + trueMeleeDmg;

            player.moveSpeed += movementSpeed;
            player.statDefense += increasedShields;

            if (player.buffTime[buffIndex] <= 1)
            {
                player.AddBuff(ModContent.BuffType<BloodshedCooldown>(), SFUtils.BuffSecondsToTicks(JetstreamChestplate.setBonusBloodshedCooldown));
            }
        }
    }
}
