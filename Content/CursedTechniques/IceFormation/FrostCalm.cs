using Microsoft.Xna.Framework;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.IceFormation
    {
        public class FrostCalm : CursedTechnique
        {
            public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.FrostCalm.DisplayName");
            public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.FrostCalm.Description");
            public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.FrostCalm.LockedDescription");
            public override float Cost => 5f;
            public override float CursedCostPerSecond => 8f;

            public override Color textColor => new Color(100, 180, 255);
            public override bool DisplayNameInGame => true;

            public override int Damage => 25;
            public override int MasteryDamageMultiplier => 40;

            public override float Speed => 10f;
            public override float LifeTime => 9000f;
            public override bool Unlocked(SorceryFightPlayer sf)
            {
                return sf.HasDefeatedBoss(NPCID.KingSlime);
            }

            public bool keyHeld;
            private int shotCooldown = 0;
            private int chargeTimer = 0;
            private static readonly int CHARGE_TIME = 40;
            private static readonly int SHOT_DELAY = 4;

            public override int GetProjectileType()
            {
                return ModContent.ProjectileType<FrostCalm>();
            }

            public override void SetDefaults()
            {
                base.SetDefaults();
                Projectile.width = 16;
                Projectile.height = 16;
                Projectile.tileCollide = false;
                Projectile.penetrate = -1;
                Projectile.friendly = false;
                Projectile.timeLeft = (int)LifeTime;
            }

            public override int UseTechnique(SorceryFightPlayer sf)
            {
                Player player = sf.Player;

                if (player.whoAmI == Main.myPlayer)
                {
                    var entitySource = player.GetSource_FromThis();

                    sf.cursedEnergy -= CalculateTrueCost(sf);

                    if (DisplayNameInGame)
                    {
                        int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                        Main.combatText[index1].lifeTime = 120;
                    }

                    return Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, GetProjectileType(), 0, 0, player.whoAmI);
                }
                return -1;
            }

            public override void AI()
            {
                Player player = Main.player[Projectile.owner];
                SorceryFightPlayer sf = player.SorceryFight();

                Projectile.Center = player.MountedCenter;

                if (Main.myPlayer == Projectile.owner)
                {
                    keyHeld = SFKeybinds.UseTechnique.Current;
                }

                if (!keyHeld || sf.cursedEnergy <= 0)
                {
                    Projectile.Kill();
                    return;
                }

                ActiveDrain(sf);

                chargeTimer++;

                // Charge-up phase with dust buildup
                if (chargeTimer < CHARGE_TIME)
                {
                    Vector2 mousePos = Main.MouseWorld;
                    Vector2 dir = (mousePos - player.MountedCenter).SafeNormalize(Vector2.Zero);

                    for (int i = 0; i < 2; i++)
                    {
                        float rotMulti = Main.rand.NextFloat(0.3f, 1f);
                        Dust dust = Dust.NewDustPerfect(player.MountedCenter + dir * 30f, DustID.IceTorch);
                        dust.scale = Main.rand.NextFloat(1.2f, 1.8f) * (chargeTimer * 0.025f) - rotMulti * 0.1f;
                        dust.noGravity = true;
                        dust.velocity = new Vector2(0, -2).RotatedByRandom(rotMulti * 0.3f) * (Main.rand.NextFloat(1f, 3.2f) - rotMulti) * (chargeTimer * 0.025f);
                        dust.alpha = Main.rand.Next(90, 150);
                        dust.color = textColor;
                    }
                    return;
                }

                // Firing phase
                if (shotCooldown > 0)
                {
                    shotCooldown--;
                    return;
                }

                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 mousePos = Main.MouseWorld;
                    Vector2 dir = (mousePos - player.MountedCenter).SafeNormalize(Vector2.Zero);

                    SoundEngine.PlaySound(SoundID.Item34, Projectile.Center);

                    for (int i = 0; i < 2; i++)
                    {
                        Projectile.NewProjectile(
                            player.GetSource_FromThis(),
                            player.MountedCenter + dir * 30f,
                            (dir * Speed).RotatedByRandom(0.12f),
                            ModContent.ProjectileType<FrostCalmProjectile>(),
                            (int)CalculateTrueDamage(sf),
                            0,
                            player.whoAmI
                        );
                    }

                    shotCooldown = SHOT_DELAY;
                }
            }

            public override bool PreDraw(ref Color lightColor)
            {
                return false;
            }

        }
    }