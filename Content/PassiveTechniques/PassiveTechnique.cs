using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.PassiveTechniques
{
    public class PassiveTechnique : ModBuff
    {
        public virtual float CostPerSecond { get; set; } = 0f;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            sf.cursedEnergy -= SorceryFight.SecondsToTicks(CostPerSecond);
        }
    }
}
