using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Limitless
{

    public class Infinity : PassiveTechnique
    {
        public override string InternalName => "Infinity";

        public Infinity()
        {
            Technique.cost = 1;
        }

        private const int BOSS_MULTIPLIER = 3;

        private Dictionary<int, Vector2> velocityData = new Dictionary<int, Vector2>();


        public bool waiting = false;

        public override void OnApply(Player player)
        {
            player.SorceryFight().infinity = true;
        }

        public override void OnRemove(Player player)
        {
            player.SorceryFight().infinity = false;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            SorceryFightPlayer sf = player.SorceryFight();
            float infinityDistance = 50f;
            Technique.cost = 1;

            sf.disableRegenFromBuffs = false;

            float accumulativeDamage = 0f;
            int npcInInfinity = 0;
            foreach (Projectile proj in Main.ActiveProjectiles)
            {

                if (proj.hostile)
                {
                    float distance = Vector2.Distance(proj.Center, player.Center);
                    if (distance <= infinityDistance)
                    {
                        accumulativeDamage += proj.damage;
                        npcInInfinity++;

                        proj.velocity *= 0.5f;
                        Vector2 vector = player.Center.DirectionTo(proj.Center);
                        proj.velocity = vector * (3f + player.velocity.Length()) * ((infinityDistance - distance) / 75);
                    }
                }
            }

            foreach (NPC npc in Main.npc)
            {

                if (!npc.friendly && npc.type != NPCID.TargetDummy && npc.active)
                {
                    float distance = Vector2.Distance(npc.Center, player.Center);
                    if (distance <= infinityDistance)
                    {
                        accumulativeDamage += npc.damage;

                        if (!velocityData.ContainsKey(npc.whoAmI))
                        {
                            velocityData[npc.whoAmI] = npc.velocity;
                        }

                        npc.velocity *= 0.5f;
                        Vector2 vector = player.Center.DirectionTo(npc.Center);
                        npc.velocity = vector * (3f + player.velocity.Length()) * ((infinityDistance - distance) / 50);
                    }

                    else if (velocityData.ContainsKey(npc.whoAmI))
                    {
                        npc.velocity = velocityData[npc.whoAmI];
                        velocityData.Remove(npc.whoAmI);
                    }

                }
            }

            if (accumulativeDamage > 0f && !waiting)
            {

                TaskScheduler.Instance.AddContinuousTask(() =>
                {
                    sf.disableRegenFromBuffs = true;
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
                multiplier = BOSS_MULTIPLIER;
            }

            Technique.cost += accumulativeDamage *= multiplier;

            base.Update(player, ref buffIndex);
        }


        public override string GetStats(SorceryFightPlayer sf)
        {
            string baseStats = base.GetStats(sf);
            string additionalStats = SFUtils.GetLocalization(
                "Mods.sorceryFight.PassiveTechniques.Infinity.AdditionalStats")
                .WithFormatArgs(
                    BOSS_MULTIPLIER
                ).Value;
            return baseStats + "\n" + additionalStats;
        }
    }
}
