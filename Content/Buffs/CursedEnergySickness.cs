using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class CursedEnergySickness : ModBuff
    {
        public float reductionPercent = 0.0f;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergySickness.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergySickness.Description").WithFormatArgs((int)reductionPercent * 100);
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.whoAmI != Main.myPlayer) return;

            int timeLeft = player.buffTime[buffIndex];
            reductionPercent = timeLeft / 60.0f * 0.05f;
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= 1 - reductionPercent;
        }
        public override bool RightClick(int buffIndex)
        {
            return SorceryFight.IsDevMode();
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;

            int index = player.FindBuffIndex(Type);
            if (index >= 0)
            {
                int timeLeft = player.buffTime[index];
                float reductionPercent = timeLeft / 60.0f * 0.05f;
                tip = SFUtils.GetLocalization("Mods.sorceryFight.Buffs.CursedEnergySickness.Description")
                             .WithFormatArgs((int)(reductionPercent * 100)).Value;
            }
        }
    }
}
