using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PlayerAttributes
{
    public class UniqueBodyStructureBuff : ModBuff
    {
        public static readonly float passiveTechniqueCostReduction = 0.2f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.UniqueBodyStructure.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.UniqueBodyStructure.Description").WithFormatArgs((int)(passiveTechniqueCostReduction * 100));
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
