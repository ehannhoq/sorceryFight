using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.CursedTechniques.BloodManipulation;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using static sorceryFight.Utilities.ProjectilePool;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;

namespace sorceryFight.Content.CursedTechniques.IceFormation
{
    public class IcicleHail : CursedTechnique
    {
        public static readonly ProjectilePool Pool = new ProjectilePool();

        //static code for setting up the projectile pool
        public override void SetStaticDefaults()
        {
            Pool.Add(new ProjectileEntry(
                () => ModContent.ProjectileType<IcicleHailProjectile>(),
                sf => true,
                weight: 10,
                damageMultiplier: 1f
            ));

            Pool.Add(new ProjectileEntry(
                () => ProjectileID.Blizzard,
                sf => sf.HasDefeatedBoss(NPCID.SkeletronHead),
                weight: 5,
                damageMultiplier: 1f
            ));

            Pool.Add(new ProjectileEntry(
                () => ProjectileID.FrostWave,
                sf => sf.HasDefeatedBoss(NPCID.MoonLordCore),
                weight: 2,
                damageMultiplier: 1f
            ));
        }


        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.IcicleHail.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.IcicleHail.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.IcicleHail.LockedDescription");

        public override Color textColor => new Color(255, 0, 0);
        public override bool DisplayNameInGame => true;

        public override float Cost => 0f;
        public override int Damage => 100;
        public override int MasteryDamageMultiplier => 50;

        private bool keyHeld = false;

        public override float CursedCostPerSecond => 20f;

        public override float Speed => 0f;
        public override float LifeTime => 300f;

        private float spawnTimer = 0;


        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.HallowBoss);

        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<IcicleHail>();
        }


        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public override void AI()
        {
            keyHeld = SFKeybinds.UseTechnique.Current;
            //Mod.Logger.Info("AI running");

            if (Main.myPlayer == Projectile.owner)
            {
                if (keyHeld)
                {
                    spawnTimer++;

                    SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();
                    ActiveDrain(sf);

                    if (spawnTimer >= 5f)
                    {
                        spawnTimer = 0f;
                        Player player = Main.player[Projectile.owner];


                        //spawn swords from the sky
                        float randomX = Main.rand.NextFloat(-300f, 300f);
                        Vector2 spawnPosition = new Vector2(player.Center.X + randomX, player.Center.Y - 800f);

                        Vector2 targetPosition = Main.MouseWorld + new Vector2(Main.rand.NextFloat(-70f, 70f), 0f);
                        Vector2 velocity = (targetPosition - spawnPosition).SafeNormalize(Vector2.Zero) * 40f;

                        //variants switching between our variants
                        int variant = Main.rand.Next(IcicleHailProjectile.Variants.Length);


                        //code to summon our projectiles

                        Projectile.NewProjectile(
                            player.GetSource_FromThis(),
                            spawnPosition,
                            velocity,
                            ModContent.ProjectileType<IcicleHailProjectile>(),
                            Damage,
                            0f,
                            player.whoAmI,
                            variant
                        );

                        //code to summon extra projectiles from other stuff
                        ProjectileEntry entry = IcicleHail.Pool.Pick(sf);

                        int index = Projectile.NewProjectile(
                            player.GetSource_FromThis(),
                            spawnPosition,
                            velocity,
                            entry.GetProjectileType(),
                            Damage,
                            0f,
                            player.whoAmI
                        );


                        if (index >= 0 && index < Main.maxProjectiles)
                        {
                            //cap entity lifetime and adjust dmg
                            Main.projectile[index].timeLeft = 300;
                            Main.projectile[index].damage = (int)(Main.projectile[index].damage * entry.DamageMultiplier);
                        }

                    }

                }
                else
                {
                    Projectile.Kill();
                }
            }
        }
    }
}