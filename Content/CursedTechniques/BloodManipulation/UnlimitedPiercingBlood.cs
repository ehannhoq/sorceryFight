using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using CalamityMod.NPCs.OldDuke;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class UnlimitedPiercingBlood : CursedTechnique
    {
        private const int CONVERGENCE_FRAMES = 5;
        private const int BEAM_FRAMES = 5;
        private const int COLLISION_FRAMES = 5;
        private const int TICKS_PER_FRAME = 5;
        private int convergenceFrame = 0;
        private int beamFrame = 0;
        private int collisionFrame = 0;
        private int frameTime = 0;
        public static Texture2D texture;
        public static Texture2D convergenceTexture;
        public static Texture2D collisionTexture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.UnlimitedPiercingBlood.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.UnlimitedPiercingBlood.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.UnlimitedPiercingBlood.LockedDescription");
        public override float Cost => 750f;
        public override Color textColor => new Color(132, 4, 4);
        public override bool DisplayNameInGame => true;
        public override int Damage => 400;
        public override int MasteryDamageMultiplier => 24;
        public override float Speed => 0f;

        //Lifetime is made useless but must be implmented 
        public override float LifeTime => 240f;

        //this number gets doubled in SorceryFightPlayer.ApplyBloodCost, so the actual cost is 8 CE/s
        public override float BloodCostPerSecond => 125f;

        private bool keyHeld = false;
        private const float MAX_LENGTH = 1600f;
        private const float STEP_SIZE = 4f;
        private const float BASE_BEAM_HEIGHT = 0.5f;

        private int daggerSpawnTimer = 0;

        ref float justSpawned => ref Projectile.ai[0];
        ref float beamHeight => ref Projectile.ai[1];

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<UnlimitedPiercingBlood>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            if (sf.innateTechnique.Name == "Vessel")
            {
                return sf.sukunasFingerConsumed >= 19;
            }
            else
            {
                return sf.HasDefeatedBoss(ModContent.NPCType<OldDuke>());
            }
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/UnlimitedPiercingBlood", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            convergenceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/UnlimitedConvergence", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            collisionTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/UnlimitedPiercingBloodCollision", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            beamHeight = 0.0f;
            //Projectile.timeLeft = (int)LifeTime;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            int index = base.UseTechnique(sf);
            Main.projectile[index].rotation = (Main.MouseWorld - sf.Player.Center).ToRotation();
            return index;
        }

        public override void AI()
        {
            if (Main.myPlayer == Projectile.owner)
            {
                keyHeld = SFKeybinds.UseTechnique.Current;

                Player player = Main.player[Projectile.owner];
                Projectile.Center = player.Center;

                float targetRotation = (Main.MouseWorld - player.Center).ToRotation();
                Projectile.rotation = SFUtils.LerpAngle(Projectile.rotation, targetRotation, 0.2f);
                Projectile.direction = Projectile.rotation.ToRotationVector2().X > 0 ? 1 : -1;
                player.ChangeDir(Projectile.direction);
            }


            if (frameTime++ > TICKS_PER_FRAME)
            {
                frameTime = 0;
                if (convergenceFrame++ >= CONVERGENCE_FRAMES - 1)
                {
                    convergenceFrame = 0;
                }
                if (collisionFrame++ >= COLLISION_FRAMES - 1)
                {
                    collisionFrame = 0;
                }
                if (beamFrame++ >= BEAM_FRAMES - 1)
                {
                    beamFrame = 0;
                }
            }

            //if (convergenceFrame != CONVERGENCE_FRAMES - 1) return;

            if (justSpawned == 0f)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (i == Projectile.whoAmI)
                        continue;

                    Projectile proj = Main.projectile[i];

                    if (proj.type == ModContent.ProjectileType<PiercingBlood>() && proj.owner == Projectile.owner)
                    {
                        proj.Kill();
                    }
                }
                justSpawned = 1f;
                Main.player[Projectile.owner].SorceryFight().disableRegenFromProjectiles = true;
                SoundEngine.PlaySound(SorceryFightSounds.PiercingBlood, Projectile.Center);
            }

            if (beamHeight < 2.0f && keyHeld)
                beamHeight += 0.2f;

            //eventually create code to make sure only one instance of this projecitle can exist at a time, same for normal piercing blood
            if (keyHeld)
            {
                SorceryFightPlayer sf = Main.player[Projectile.owner].SorceryFight();
                sf.bloodEnergyUsagePerSecond += BloodCostPerSecond;
            }

            if (!keyHeld)
            {
                beamHeight -= 0.2f;
                Main.player[Projectile.owner].SorceryFight().disableRegenFromProjectiles = false;
                if (beamHeight <= 0f)
                    Projectile.Kill();
            }

            if (Main.myPlayer == Projectile.owner)
            {
                float beamLength = 0f;
                Vector2 direction = Projectile.rotation.ToRotationVector2();
                for (float i = 0f; i < MAX_LENGTH; i += STEP_SIZE)
                {
                    Vector2 checkPos = Projectile.Center + direction * i;
                    if (!Collision.CanHitLine(Projectile.Center, 1, 1, checkPos, 1, 1))
                    {
                        break;
                    }
                    beamLength = i;
                }
                Projectile.localAI[0] = beamLength;

                if (keyHeld && beamLength > 20f)
                {
                    if (daggerSpawnTimer++ < 10)
                    {
                        return;
                    }
                    else
                    {
                        foreach (NPC npc in Main.ActiveNPCs)
                        {
                            if (!npc.active || !npc.HasBuff(ModContent.BuffType<BloodPoison>()))
                                continue;

                            //spawn at a random point along the beam but not more than 1000f
                            float beamSpawnArea = Math.Min(Main.rand.NextFloat(0f, beamLength), 1000f);
                            Vector2 spawnPos = Projectile.Center + direction * beamSpawnArea;
                            Vector2 velocity = (npc.Center - spawnPos).SafeNormalize(Vector2.Zero) * 10f;

                            if (npc.Distance(spawnPos) > 2000f)
                                continue;



                            //Main.NewText("CREATING PROJECTILE" + beamSpawnArea);

                            Projectile.NewProjectile(
                                Projectile.GetSource_FromThis(),
                                spawnPos,
                                velocity,
                                ModContent.ProjectileType<UnlimitedPiercingBloodProjectile>(),
                                500,
                                0f,
                                Projectile.owner,
                                ai1: (float)npc.whoAmI
                            );
                        }
                        daggerSpawnTimer = 0;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            float beamLength = Projectile.localAI[0] - 50f;
            beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);


            Vector2 beamStart = Projectile.Center + Projectile.rotation.ToRotationVector2() * 2 * (convergenceTexture.Width / 2) - Main.screenPosition;
            //Vector2 beamOrigin = new Vector2(0, texture.Height / 2);
            Vector2 beamScale = new Vector2((beamLength - convergenceTexture.Width / 2) / texture.Width, BASE_BEAM_HEIGHT * beamHeight);


            int beamFrameHeight = convergenceTexture.Height / CONVERGENCE_FRAMES;
            int beamFrameY = convergenceFrame * beamFrameHeight;


            Vector2 beamOrigin = new Vector2(0, beamFrameHeight / 2);
            Rectangle beamSourceRectangle = new Rectangle(0, beamFrameY, convergenceTexture.Width, beamFrameHeight);


            Main.EntitySpriteDraw(texture, beamStart, beamSourceRectangle, Color.White, Projectile.rotation, beamOrigin, beamScale, SpriteEffects.None, 0f);


            int convFrameHeight = convergenceTexture.Height / CONVERGENCE_FRAMES;
            int convFrameY = convergenceFrame * convFrameHeight;

            Vector2 convergenceOrigin = new Vector2(convergenceTexture.Width / 2, convFrameHeight / 2);
            Rectangle convergenceSourceRectangle = new Rectangle(0, convFrameY, convergenceTexture.Width, convFrameHeight);

            Main.EntitySpriteDraw(convergenceTexture, beamStart, convergenceSourceRectangle, Color.White, Projectile.rotation, convergenceOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);

            int collisionFrameHeight = collisionTexture.Height / COLLISION_FRAMES;
            int collisionFrameY = collisionFrame * collisionFrameHeight;

            if (beamLength > 20f)
            {
                Vector2 beamEnd = beamStart + Projectile.rotation.ToRotationVector2() * (beamLength - 25);

                Vector2 collisionOrigin = new Vector2(collisionTexture.Width / 2, collisionFrameHeight / 2);
                Rectangle collisionSourceRectangle = new Rectangle(0, collisionFrameY, collisionTexture.Width, collisionFrameHeight);

                Main.EntitySpriteDraw(collisionTexture, beamEnd, collisionSourceRectangle, Color.White, Projectile.rotation, collisionOrigin, new Vector2(1f, beamScale.Y), SpriteEffects.None, 0f);
            }


            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * Projectile.localAI[0], beamHeight * Projectile.scale, ref useless))
                return true;

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            int paintingCount = Main.player[Projectile.owner].SorceryFight().deathPaintings.Count(p => p);
            target.AddBuff(ModContent.BuffType<BloodPoison>(), paintingCount * 60);

        }

    }
}
