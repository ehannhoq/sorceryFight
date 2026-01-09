using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.CursedEnergyTraits
{
    public class OverflowingCursedEnergy : ModBuff
    {
        public readonly static float maxCursedEnergyIncrease = 0.23f;
        public readonly static int cursedEnergyRegenIncrease = 2;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergyTraits.OverflowingCursedEnergy.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergyTraits.OverflowingCursedEnergy.Description").WithFormatArgs((int)(maxCursedEnergyIncrease * 100), cursedEnergyRegenIncrease);
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            sfPlayer.maxCursedEnergyFromOtherSources += sfPlayer.maxCursedEnergy * maxCursedEnergyIncrease;
            sfPlayer.cursedEnergyRegenFromOtherSources += cursedEnergyRegenIncrease;
        }

        public override bool RightClick(int buffIndex)
        {
            if (SorceryFight.IsDevMode())
            {
                Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>().overflowingEnergy = false;
                return true;
            }
            return false;
        }
    }
}
