using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.PlayerAttributes;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs
{
    public abstract class PassiveTechnique : ModBuff
    {
        public abstract string Stats { get;}
        public abstract string LockedDescription { get; }

        public virtual bool isAura { get; } = false;

        public abstract bool isActive { get; set; }
        public abstract float CostPerSecond { get; set; }

        public virtual float BloodRegenPerSecond { get; set; }

        public virtual Color selectorBGColor { get; set; }
        public virtual Color selectorBorderColor { get; set; }

        public abstract bool Unlocked(SorceryFightPlayer sf);

        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.buffNoSave[Type] = true;
        }

        public abstract void Apply(Player player);
        public abstract void Remove(Player player);

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sf = player.SorceryFight();

            float finalCostPerSecond = CostPerSecond;

            float finalBloodRegenPerSecond = BloodRegenPerSecond;

            if (sf.uniqueBodyStructure)
                finalCostPerSecond *= 1 - UniqueBodyStructureBuff.passiveTechniqueCostReduction;

            sf.cursedEnergyUsagePerSecond += finalCostPerSecond;

            sf.bloodEnergyRegenPerSecond += finalBloodRegenPerSecond;
        }

        public virtual bool UseCondition(Player player)
        {
            return true;
        }

    }
}
