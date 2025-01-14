using System;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Shrine
{
    public class DomainAmplificationBuff : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.DomainAmplificationBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.DomainAmplificationBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.DomainAmplificationBuff.LockedDescription");
        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n";
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 10f;

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<DomainAmplificationBuff>(), 2);

            if (player.HasBuff<HollowWickerBasketBuff>())
            {
                player.GetModPlayer<SorceryFightPlayer>().innateTechnique.PassiveTechniques[1].isActive = false;
            }
        }

        public override void Remove(Player player)
        {

        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.WallofFlesh);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
        }
    }
}
