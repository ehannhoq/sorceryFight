using System;
using Microsoft.Xna.Framework;
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
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + $"Damage Reduction: 50%";
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 10f;

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<DomainAmplificationBuff>(), 2);
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            if (player.HasBuff<HollowWickerBasketBuff>())
            {
                sfPlayer.innateTechnique.PassiveTechniques[1].isActive = false;
            }

            sfPlayer.domainAmp = true;
        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            sfPlayer.domainAmp = false;
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.WallofFlesh);
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CostPerSecond = 10f;

            float minimumDistance = 25f;

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile) continue;

                float distance = Vector2.DistanceSquared(proj.Center, player.Center);
                if (distance <= minimumDistance * minimumDistance)
                {
                    CostPerSecond += proj.damage;
                }
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.type == NPCID.TargetDummy || npc.IsDomain()) continue;

                float distance = Vector2.DistanceSquared(npc.Center, player.Center);
                if (distance <= minimumDistance * minimumDistance)
                {
                    CostPerSecond += npc.damage;
                }
            }

            CostPerSecond *= 0.5f;

            base.Update(player, ref buffIndex);
        }
    }
}
