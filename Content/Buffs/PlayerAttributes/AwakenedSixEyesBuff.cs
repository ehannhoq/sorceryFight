using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.PlayerAttributes
{
    public class AwakenedSixEyesBuff : ModBuff
    {
        public static readonly float cursedTechniqueCostReduciton = 0.18f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.AwakenedSixEyes.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.PlayerAttributes.AwakenedSixEyes.Description").WithFormatArgs((int)(cursedTechniqueCostReduciton * 100));
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
