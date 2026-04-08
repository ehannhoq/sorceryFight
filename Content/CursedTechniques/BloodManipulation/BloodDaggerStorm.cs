using Microsoft.Build.Tasks;
using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class BloodDaggerStorm : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.BloodDaggerStorm.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.BloodDaggerStorm.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.BloodDaggerStorm.LockedDescription");
        public override float Cost => 40f;

        public override float BloodCost => 20f;

        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;

        public override int Damage => 100;
        public override int MasteryDamageMultiplier => 50;

        private bool keyHeld = false;

        public override float BloodCostPerSecond => 50f;

        public override float Speed => 0f;
        public override float LifeTime => 300f;

        private float spawnTimer = 0;


        public override bool Unlocked(SorceryFightPlayer sf)
        {
            if (sf.innateTechnique.Name == "Vessel")
            {
                return sf.sukunasFingerConsumed >= 11;
            }   
            else
            {
                return sf.HasDefeatedBoss(NPCID.HallowBoss);
            }
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<BloodDaggerStorm>();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void AI()
        {
            keyHeld = SFKeybinds.UseTechnique.Current;
            //Mod.Logger.Info("AI running");

            if (Main.myPlayer == Projectile.owner){
                if (keyHeld)
                    {
                        spawnTimer++;

                        SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();
                        sf.bloodEnergyUsagePerSecond += BloodCostPerSecond;

                    if (spawnTimer >= 10f)
                        {
                            spawnTimer = 0f;
                            Player player = Main.player[Projectile.owner];
                            // auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<AmplifiedAuraProjectile>(), 0, 0, player.whoAmI);

                            Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * Speed;

                            //Main.NewText("spawning projectile, Velocity: " + velocity);

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

                    } else {
                        Projectile.Kill();
                    }
            }
        }
    }
}