using sorceryFight.Content.Buffs.Limitless;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public class PassiveTechnique : ModBuff
    {
        public virtual new string Name { get; set; } = "";
        public virtual string Stats 
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n";
            }
        }
        public virtual string LockedDescription
        {
            get; set;
        }

        public virtual bool isActive { get; set; } = false;
        public virtual float CostPerSecond { get; set; } = 0f;
        public virtual bool Unlocked { get; set; } = false;

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
            sf.cursedEnergyUsagePerSecond += CostPerSecond;
        }
    }
}
