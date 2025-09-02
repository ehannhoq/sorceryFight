using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PlayerAttributes
{
    public class ChallengersEyeBuff : ModBuff
    {
        public readonly static float cursedTechniqueCostReduction = 0.32f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.ChallengersEye.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.ChallengersEye.Description").WithFormatArgs((int)(cursedTechniqueCostReduction * 100));
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<SorceryFightPlayer>().ctCostReduction += cursedTechniqueCostReduction;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
