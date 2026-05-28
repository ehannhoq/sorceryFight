using System;
using System.Collections.Generic;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.TenShadows
{
    public class TotalityBuff : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.TotalityBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.TotalityBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.TotalityBuff.LockedDescription");
        public override bool isActive { get; set; } = false;

        public override float CostPerSecond { get; set; } = 10f;

        public virtual int DefenseAddition { get; set; } = 12;
        public virtual float DamageNegation { get; set; } = 0.10f;

        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + $"Turns Divine Dog Summon into Totality summon\n";
            }
        }


        public Dictionary<int, int> auraIndices;

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<TotalityBuff>(), 2);

        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.TotalityToggle = false;

        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            // return sf.HasDefeatedBoss(ModContent.NPCType<PerforatorHive>()) || sf.HasDefeatedBoss(ModContent.NPCType<HiveMind>());
            return true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            sfPlayer.TotalityToggle = true;

            base.Update(player, ref buffIndex);
        }
    }
}
