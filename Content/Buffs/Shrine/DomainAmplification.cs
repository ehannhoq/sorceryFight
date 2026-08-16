using System.Collections.Generic;
using Microsoft.Xna.Framework;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Shrine
{
    public class DomainAmplification : PassiveTechnique
    {
        public override string InternalName => "DomainAmplification";

        public DomainAmplification()
        {
            Technique.cost = 10;
        }

        private const float DAMAGE_REDUCTION = 0.5f;
        private const float BOSS_MULTIPLIER = 1.5f;

        public Dictionary<int, int> auraIndices;

        public override void OnApply(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.domainAmp = true;

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<DomainAmplificationProjectile>(), 0, 0, player.whoAmI);
            }

            sfPlayer.disableCurseTechniques = true;
        }

        public override void OnRemove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.domainAmp = false;

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (auraIndices.ContainsKey(player.whoAmI))
            {
                Main.projectile[auraIndices[player.whoAmI]].Kill();
                auraIndices.Remove(player.whoAmI);
            }

           sfPlayer.disableCurseTechniques = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            float minimumDistance = 25f;
            float accumulativeDamage = 0f;

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.hostile) continue;

                float distance = Vector2.DistanceSquared(proj.Center, player.Center);
                if (distance <= minimumDistance * minimumDistance)
                {
                    accumulativeDamage += proj.damage;
                }
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.type == NPCID.TargetDummy) continue;

                float distance = Vector2.DistanceSquared(npc.Center, player.Center);
                if (distance <= minimumDistance * minimumDistance)
                {
                    accumulativeDamage += npc.damage;
                }
            }

            if (accumulativeDamage > 0f)
            {
                sfPlayer.disableRegenFromBuffs = true;
            }

            float multiplier = 1;
            if (AreThereAnyDamnBosses.BossActive)
            {
                multiplier = BOSS_MULTIPLIER;
            }

            Technique.cost = 10f;
            Technique.cost += accumulativeDamage * multiplier;

            base.Update(player, ref buffIndex);
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string baseStats = base.GetStats(sf);
            string additionalStats = SFUtils.GetLocalization(
                "Mods.sorceryFight.PassiveTechniques.DomainAmplification.AdditionalStats")
                .WithFormatArgs(
                    DAMAGE_REDUCTION,
                    BOSS_MULTIPLIER
                ).Value;
            return baseStats + "\n" + additionalStats;
        }
    }
}
