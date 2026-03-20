using CalamityMod.Particles;
using CalamityMod.Sounds;
using Humanizer;
using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Projectiles.Melee;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
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

        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 50;

        private bool keyHeld = false;

        public float BloodCostPerSecond => 4f;

        public override float Speed => 0f;
        public override float LifeTime => 300f;

        private float spawnTimer = 0;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }


        public static Texture2D texture;

        public bool animating;
        public float animScale;


        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<BloodDaggerStorm>();
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void AI()
        {
            keyHeld = SFKeybinds.UseTechnique.Current;
            Mod.Logger.Info("AI running");

            //ModContent.GetInstance<SorceryFight>().Logger.Info("This is what the bum is doing:" + spawnTimer);
            if (Main.myPlayer == Projectile.owner){
                if (keyHeld)
                    {
                        spawnTimer++;

                        if (spawnTimer >= 10f)
                        {
                            spawnTimer = 0f;

                            SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();
                            sf.bloodEnergyUsagePerSecond += BloodCostPerSecond;
                            Player player = Main.player[Projectile.owner];
                            // auraIndices[player.whoAmI] = Projectile.NewProjectile(entitySource, playerPos, Vector2.Zero, ModContent.ProjectileType<AmplifiedAuraProjectile>(), 0, 0, player.whoAmI);

                            Vector2 velocity = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.Zero) * Speed;

                            Main.NewText("spawning projectile, Velocity: " + velocity);

                            Projectile.NewProjectile(
                            player.GetSource_FromThis(),
                            player.Center,
                            velocity,
                            ModContent.ProjectileType<BloodDaggerStormProjectile>(),
                            Damage,
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