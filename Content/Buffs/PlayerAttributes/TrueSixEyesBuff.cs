using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PlayerAttributes
{
    public class TrueSixEyesBuff : ModBuff
    {
        public static readonly float cursedTechniqueCostReduciton = 0.27f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.TrueSixEyes.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.TrueSixEyes.Description").WithFormatArgs((int)(cursedTechniqueCostReduciton * 100));
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = false;
            Main.persistentBuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.SorceryFight().ctCostReduction += cursedTechniqueCostReduciton;
            player.AddBuff(BuffID.Dangersense, 2);
            player.AddBuff(BuffID.NightOwl, 2);
            player.AddBuff(BuffID.Spelunker, 2);
            player.AddBuff(BuffID.Hunter, 2);
        }

        public override bool RightClick(int buffIndex)
        {
            if (SorceryFight.IsDevMode())
            {
                Main.LocalPlayer.SorceryFight().sixEyesLevel = 0;
                return true;
            }
            return false;
        }
    }
}
