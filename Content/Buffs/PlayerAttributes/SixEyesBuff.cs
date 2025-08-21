using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PlayerAttributes
{
    public class SixEyesBuff : ModBuff
    {
        public static readonly float cursedTechniqueCostReduciton = 0.25f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.SixEyes.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.SixEyes.Description").WithFormatArgs((int)(cursedTechniqueCostReduciton * 100));
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
