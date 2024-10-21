using sorceryFight.Content.PassiveTechniques.Limitless;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.PassiveTechniques
{
    public class PassiveTechnique : ModBuff
    {
        public virtual new string Name { get; set; } = "";
        public virtual string Stats 
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s";
            }
        }
        public virtual bool isActive { get; set; } = false;
        public virtual float CostPerSecond { get; set; } = 0f;

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public virtual void Apply(Player player) {}
        public virtual void Remove(Player player) {}

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sf = player.GetModPlayer<SorceryFightPlayer>();
            sf.cursedEnergy -= SorceryFight.TicksToSeconds(CostPerSecond);
        }
    }
}
