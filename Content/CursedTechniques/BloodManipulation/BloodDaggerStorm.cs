using System.Buffers.Text;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class BloodDaggerStorm : CursedTechniqueContinuous
    {
        public override string InternalName => "BloodDaggerStorm";

        private float spawnTimer = 0;

        private float BloodCost => Technique.cost;

        public BloodDaggerStorm()
        {
            Technique.baseDamage = 25;
            Technique.damagePerBoss = 10;
            Technique.cost = 40;
            Technique.speed = 16f;
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string localizationCategoryKey = "Mods.sorceryFight.Misc.CursedTechniques";

            string damage = SFUtils.GetLocalization(localizationCategoryKey + ".Damage")
                .WithFormatArgs(CalculateTrueDamage(sf)).Value;

            string ceCost = SFUtils.GetLocalization(localizationCategoryKey + ".ContinuousCost")
                .WithFormatArgs((int)base.CalculateTrueCost(sf)).Value;

            string bloodCost = SFUtils.GetLocalization(localizationCategoryKey + ".ContinuousBloodCost")
                .WithFormatArgs((int)Technique.cost / 2).Value;

            string stats = damage + "\n" + ceCost + "\n" + bloodCost;

            return stats;
        }

        public override bool CanUse(SorceryFightPlayer sf)
        {
            return sf.bloodEnergy > BloodCost;
        }

        public override void DrainCost(SorceryFightPlayer sfPlayer)
        {
            base.DrainCost(sfPlayer);
            sfPlayer.bloodEnergy -= CalculateTrueCost(sfPlayer);
            if (sfPlayer.bloodEnergy <= 1)
                Destroy(sfPlayer);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (keyHeld)
            {
                spawnTimer++;

                SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();

                if (spawnTimer >= 10f)
                {
                    spawnTimer = 0f;
                    Player player = Main.player[Projectile.owner];

                    Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * speed;


                    Projectile.NewProjectile(
                    player.GetSource_FromThis(),
                    player.Center,
                    velocity,
                    ModContent.ProjectileType<BloodDaggerStormProjectile>(),
                    (int)CalculateTrueDamage(sf),
                    0f,
                    player.whoAmI,
                    ai1: -1f
                    );
                }
            }
        }
    }
}