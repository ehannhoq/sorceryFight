using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using sorceryFight.Content.Buffs.Vessel;

namespace sorceryFight.Content.Buffs.Shrine
{
    public class HollowWickerBasket : PassiveTechnique
    {
        public override string InternalName => "HollowWickerBasket";

        public HollowWickerBasket()
        {
            Technique.cost = 50;
        }

        private const float DAMAGE_NEGATION = 1.0f;
        private const float SPEED_REDUCTION = 0.1f;
        private const int BOSS_REDUCTION = 3;

        public Dictionary<int, int> auraIndices;
        public bool waiting = false;

        public override void OnApply(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            sfPlayer.hollowWickerBasket = true;

            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<HollowWickerBasketProjectile>(), 0, 0, player.whoAmI);

            }

            sfPlayer.disableCurseTechniques = true;
        }

        public override void OnRemove(Player player)
        {
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.hollowWickerBasket = false;


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
            player.moveSpeed -= SPEED_REDUCTION;

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

            if (accumulativeDamage > 0f && !waiting)
            {

                TaskScheduler.Instance.AddContinuousTask(() =>
                {
                    sfPlayer.disableRegenFromBuffs = true;
                },
                300);

                TaskScheduler.Instance.AddDelayedTask(() =>
                {
                    waiting = false;
                },
                301);

                waiting = true;
            }

            int multiplier = 1;
            if (AreThereAnyDamnBosses.BossActive)
            {
                multiplier = BOSS_REDUCTION;
            }

            Technique.cost = 50f;
            Technique.cost += accumulativeDamage * 3f * multiplier;

            base.Update(player, ref buffIndex);
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string baseStats = base.GetStats(sf);
            string additionalStats = SFUtils.GetLocalization(
                "Mods.sorceryFight.PassiveTechniques.HollowWickerBasket.AdditionalStats")
                .WithFormatArgs(
                    (int)(DAMAGE_NEGATION * 100),
                    (int)(SPEED_REDUCTION * 100),
                    BOSS_REDUCTION
                ).Value;
            return baseStats + "\n" + additionalStats;
        }
    }
}
