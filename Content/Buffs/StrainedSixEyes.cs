using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class StrainedSixEyes : ModBuff
    {
        public static float reducedMaxCE = 0.20f;
        public static float ceDrain = 50;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.StrainedSixEyes.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.StrainedSixEyes.Description").WithFormatArgs((int)(reducedMaxCE * 100), ceDrain);
        
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            float decreasedMaxCE = sfPlayer.maxCursedEnergy + sfPlayer.maxCursedEnergyFromOtherSources;
            decreasedMaxCE *= reducedMaxCE;
            sfPlayer.maxCursedEnergyFromOtherSources -= decreasedMaxCE;

            sfPlayer.cursedEnergy -= SFUtils.RateSecondsToTicks(ceDrain);
        }

        public override bool RightClick(int buffIndex)
        {
            return SorceryFight.IsDevMode();
        }
    }
}