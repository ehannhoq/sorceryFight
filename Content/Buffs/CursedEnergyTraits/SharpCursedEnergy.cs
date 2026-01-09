using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.CursedEnergyTraits
{
    public class SharpCursedEnergy : ModBuff
    {
        public readonly static float cursedTechniqueDamageIncrease = 0.03f;
        public readonly static float inflictBleedingChance = 0.25f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergyTraits.SharpCursedEnergy.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergyTraits.SharpCursedEnergy.Description").WithFormatArgs((int)(cursedTechniqueDamageIncrease * 100), (int)(inflictBleedingChance * 100));
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + cursedTechniqueDamageIncrease;
        }

        public override bool RightClick(int buffIndex)
        {
            if (SorceryFight.IsDevMode())
            {
                Main.LocalPlayer.GetModPlayer<SorceryFightPlayer>().sharpCursedEnergy = false;
                return true;
            }
            return false;
        }
    }
}
