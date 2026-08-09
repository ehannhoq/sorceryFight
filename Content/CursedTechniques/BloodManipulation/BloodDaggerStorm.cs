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

        public BloodDaggerStorm()
        {
            Technique.baseDamage = 20;
            Technique.damagePerBoss = 4;
            Technique.cost = 20;
            Technique.speed = 12f;
        }

        public override void DrainCost(SorceryFightPlayer sfPlayer)
        {
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