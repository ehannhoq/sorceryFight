using CalamityMod;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace sorceryFight.Content.Buffs.Shrine
{
    public class HollowWickerBasketBuff : PassiveTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.HollowWickerBasketBuff.DisplayName");
        public override LocalizedText Description => SFUtils.GetLocalization("Mods.sorceryFight.Buffs.HollowWickerBasketBuff.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.Buffs.HollowWickerBasketBuff.LockedDescription");
        public override string Stats
        {
            get
            {
                return $"CE Consumption: {CostPerSecond} CE/s\n"
                        + $"Damage Reduction: 100%";
            }
        }
        public override bool isActive { get; set; } = false;
        public override float CostPerSecond { get; set; } = 50f;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.Golem);
        }

        public override void Apply(Player player)
        {
            player.AddBuff(ModContent.BuffType<HollowWickerBasketBuff>(), 2);

            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();

            if (player.HasBuff<DomainAmplificationBuff>())
            {
                sfPlayer.innateTechnique.PassiveTechniques[0].isActive = false;
            }

            sfPlayer.hollowWickerBasket = true;
        }

        public override void Remove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.GetModPlayer<SorceryFightPlayer>();
            sfPlayer.hollowWickerBasket = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            CostPerSecond = 50f;

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

            base.Update(player, ref buffIndex);
        }
    }
}