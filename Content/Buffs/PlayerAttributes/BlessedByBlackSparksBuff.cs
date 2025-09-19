using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PlayerAttributes
{
    public class BlessedByBlackSparksBuff : ModBuff
    {
        public static float increasedDamage = 0.5f;
        public static int increasedWindow = 1;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.BlessedByBlackSparksBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.BlessedByBlackSparksBuff.Description").WithFormatArgs(increasedDamage, increasedWindow);
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            sfPlayer.additionalBlackFlashDamageMultiplier += increasedDamage;
            sfPlayer.blackFlashWindowTime += increasedWindow;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}
