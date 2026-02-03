using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.CursedEnergyTraits
{
    public class ExplosiveCursedEnergy : ModBuff
    {
        public readonly static float cursedTechniqueDamageIncrease = 0.05f;
        public readonly static float cursedTechniqueCostIncrease = 0.05f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergyTraits.ExplosiveCursedEnergy.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergyTraits.ExplosiveCursedEnergy.Description").WithFormatArgs((int)(cursedTechniqueDamageIncrease * 100), (int)(cursedTechniqueCostIncrease * 100));
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 + cursedTechniqueDamageIncrease;
            player.SorceryFight().ctCostReduction -= cursedTechniqueCostIncrease;
        }

        public override bool RightClick(int buffIndex)
        {
            if (SorceryFight.IsDevMode())
            {
                Main.LocalPlayer.SorceryFight().explosiveCursedEnergy = false;
                return true;
            }
            return false;
        }
    }
}
