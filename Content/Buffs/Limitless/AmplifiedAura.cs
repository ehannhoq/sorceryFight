using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using sorceryFight.SFPlayer;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.Limitless
{
    public class AmplifiedAura : PassiveTechnique
    {
        public override string InternalName => "AmplifiedAura";

        public AmplifiedAura()
        {
            Technique.cost = 10;
        }

        private const float SPEED_MULTIPLIER = 50f;
        private const float DAMAGE_MULTIPLIER = 10f;

        protected Dictionary<int, int> auraIndices;

        public override void OnApply(Player player)
        {
            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (Main.myPlayer == player.whoAmI && !auraIndices.ContainsKey(player.whoAmI))
            {
                Vector2 playerPos = player.MountedCenter;
                var entitySource = player.GetSource_FromThis();

                auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<AmplifiedAuraProjectile>(), 0, 0, player.whoAmI);
            }

            player.SorceryFight().disableCurseTechniques = true;
        }

        public override void OnRemove(Player player)
        {
            if (auraIndices == null)
                auraIndices = new Dictionary<int, int>();

            if (auraIndices.ContainsKey(player.whoAmI))
            {
                Main.projectile[auraIndices[player.whoAmI]].Kill();
                auraIndices.Remove(player.whoAmI);
            }

            Technique.cost = 10f; // Base

            SorceryFightPlayer sf = player.SorceryFight();
            float newCPS = sf.maxCursedEnergy / 100 + Technique.cost;

            if (newCPS > Technique.cost)
                Technique.cost = newCPS;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);

            Technique.cost = 10f; // Base

            SorceryFightPlayer sf = player.SorceryFight();
            float newCPS = sf.maxCursedEnergy / 100 + Technique.cost;

            if (newCPS > Technique.cost)
                Technique.cost = newCPS;

            player.moveSpeed *= (SPEED_MULTIPLIER / 100) + 1;
            player.GetDamage(DamageClass.Melee) *= (DAMAGE_MULTIPLIER / 100) + 1;
            player.GetDamage(DamageClass.Ranged) *= (DAMAGE_MULTIPLIER / 100) + 1;
            player.GetDamage(DamageClass.Magic) *= (DAMAGE_MULTIPLIER / 100) + 1;
            player.GetDamage(DamageClass.Summon) *= (DAMAGE_MULTIPLIER / 100) + 1;
            player.GetDamage(CursedTechniqueDamageClass.Instance) *= (DAMAGE_MULTIPLIER / 100) + 1;
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string baseStats = base.GetStats(sf);
            string additionalStats = SFUtils.GetLocalization(
                "Mods.sorceryFight.PassiveTechniques.AmplifiedAura.AdditionalStats")
                .WithFormatArgs(
                    SPEED_MULTIPLIER,
                    DAMAGE_MULTIPLIER
                ).Value;
            return baseStats + "\n" + additionalStats;
        }
    }
}
