using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.Content.SFPlayer;

namespace sorceryFight.Content.Buffs.Shrine
{
    public class WheelOfAdaptation : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.WheelOfAdaptation.DisplayName");
        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s"
                    + $"Each adapted damage adds an additional {CostPerSecond} CE/s.";
            }
        }
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.WheelOfAdaptation.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.WheelOfAdaptation.LockedDescription");

        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 10f;

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<WheelOfAdaptation>(), 2);
        }

        public override void Remove(Player player)
        {
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return CalamityMod.DownedBossSystem.downedProvidence;
        }
    }
}
