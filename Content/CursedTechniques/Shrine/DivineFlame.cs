using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.Content.DomainExpansions;
using sorceryFight.Content.DomainExpansions.PlayerDomains;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.Particles;

using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class DivineFlame : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 9;
        public static readonly int TICKS_PER_FRAME = 2;
        static List<Texture2D> textures;

        public override string InternalName => "DivineFlame";


        ref float castTimer => ref Projectile.ai[0];
        Rectangle hitbox;
        int texturePhase; // 0 -> Fire strands. 1 -> Fire arrow, 2 -> Explosion
        bool casting;

        public DivineFlame()
        {
            Technique.baseDamage = 600;
            Technique.damagePerBoss = 40;
            Technique.cost = 450;
            Technique.speed = 30;
            Technique.lifetime = 400;
        }

        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            textures = new List<Texture2D>()
            {
                ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/DivineFlameStrands", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
                ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/DivineFlame", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value,
            };
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 101;
            Projectile.height = 200;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            casting = false;
            hitbox = Projectile.Hitbox;
            texturePhase = 0;
        }

        
        public override void AI()
        {
            castTimer++;
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            float totalCastTime = sfPlayer.cursedOfuda ? 150f * CursedOfuda.cursedTechniqueCastTimeDecrease : 150f;
            float transitionTime = sfPlayer.cursedOfuda ? 15f * CursedOfuda.cursedTechniqueCastTimeDecrease : 15f;

            Projectile.HandleProjectileAnimation(FRAME_COUNT, TICKS_PER_FRAME);

            if (castTimer < totalCastTime)
            {
                if (!casting)
                {
                    casting = true;
                    player.SorceryFight().disableRegenFromProjectiles = true;
                    Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
                    Projectile.velocity = Vector2.Zero;
                    Projectile.damage = 0;
                    texturePhase = 0;
                }

                if (castTimer == 1)
                    SoundEngine.PlaySound(SorceryFightSounds.DivineFlameChargeUp with { Volume = 2f }, player.Center);

                Projectile.Center = player.Center + Vector2.UnitX.RotatedBy(Projectile.rotation) * 5f;

                if (DomainExpansionController.ActiveDomains.Any(de => de.owner == Projectile.owner && de.GetType() == typeof(MalevolentShrine)))
                {
                    if (!Main.dedServ && Main.myPlayer == Projectile.owner)
                    {
                        if (!Filters.Scene["SF:DivineFlame"].IsActive()) Filters.Scene.Activate("SF:DivineFlame").GetShader().UseOpacity(1f);

                        //this formula transitions the blackhole from small to large
                        Filters.Scene["SF:DivineFlame"].GetShader().UseProgress(castTimer / totalCastTime);
                    }
                }

                // if (castTimer < (int)transitionTime)
                // {
                //     Vector2 pos = Projectile.Center;
                //     Vector2 velocity = new Vector2(Main.rand.NextFloat(-10f, 10f), Main.rand.NextFloat(-10f, 10f));
                //     GlowSparkParticle particle = new GlowSparkParticle(pos, velocity, false, 60, 0.01f, textColor, new Vector2(1, 1));
                //     GeneralParticleHandler.SpawnParticle(particle);
                // }

                if (castTimer == (int)transitionTime)
                {
                    texturePhase = 1;
                    int index = CombatText.NewText(player.getRect(), new Color(242, 140, 44), "Divine Flame");
                    Main.combatText[index].lifeTime = 60;
                    // for (int i = 0; i < 3; i++)
                    // {
                    //     Vector2 pos = Projectile.Center;
                    //     Vector2 velocity = new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-50f, 50f));
                    //     GlowSparkParticle particle = new GlowSparkParticle(pos, velocity, false, 60, 0.1f, textColor, new Vector2(1, 1));
                    //     GeneralParticleHandler.SpawnParticle(particle);
                    // }
                }

                if (castTimer == (int)totalCastTime - 10)
                {
                    int index = CombatText.NewText(player.getRect(), new Color(242, 140, 44), "Open.");
                    Main.combatText[index].lifeTime = 180;
                }

                if (castTimer > (int)transitionTime)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation();
                        Projectile.netUpdate = true;
                    }

                    player.direction = (Math.Cos(Projectile.rotation) > 0).ToDirectionInt();

                    Vector2 pos = Projectile.Center;
                    Vector2 velocity = Vector2.UnitX.RotatedBy(Projectile.rotation + MathHelper.Pi).RotatedByRandom(0.5f) * Main.rand.NextFloat(20f);
                    LinearParticle particle = new LinearParticle(
                        position: pos,
                        velocity: velocity,
                        color: new Color(242, 180, 133)
                    );
                    ParticleController.SpawnParticle(particle);
                }
                return;
            }

            if (casting)
            {
                casting = false;
                Projectile.damage = (int)CalculateTrueDamage(sfPlayer);
                Projectile.width = 227;
                Projectile.height = 49;
                Projectile.Hitbox = hitbox;
                Projectile.timeLeft = lifetime;
                Projectile.Center = player.Center;
                Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.rotation) * speed;

                ImpactFrameController.ImpactFrame(new Color(242, 180, 133), 6);
                CameraController.CameraShake(6, 25, 7);


                SoundEngine.PlaySound(SorceryFightSounds.DivineFlameShoot, Projectile.Center);
                sfPlayer.disableRegenFromProjectiles = false;
                if (Main.myPlayer == Projectile.owner)
                {
                    if (!Main.dedServ)
                        if (Filters.Scene["SF:DivineFlame"].IsActive())
                        {
                            Filters.Scene["SF:DivineFlame"].GetShader().UseOpacity(0f);
                            Filters.Scene["SF:DivineFlame"].Deactivate();
                        }

                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            if (texturePhase == 0)
            {
                int frameHeight = textures[texturePhase].Height / FRAME_COUNT;
                int frameY = Projectile.frame * frameHeight;

                Vector2 origin = new Vector2(textures[texturePhase].Width / 2, frameHeight / 2);
                Rectangle sourceRectangle = new Rectangle(0, frameY, textures[texturePhase].Width, frameHeight);
                Main.spriteBatch.Draw(textures[texturePhase], Main.LocalPlayer.Center - Main.screenPosition + new Vector2(0f, -30f), sourceRectangle, Color.White, Projectile.rotation + (MathHelper.Pi / 6), origin, 1f, SpriteEffects.None, 0f);
            }
            else
            {
                Vector2 origin = new Vector2(textures[texturePhase].Width / 2, textures[texturePhase].Height / 2);
                Main.spriteBatch.Draw(textures[texturePhase], Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            }

            return false;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (DomainExpansionController.ActiveDomains.Any(de => de.owner == Projectile.owner && de.GetType() == typeof(MalevolentShrine)))
            {
                modifiers.FinalDamage.Flat += modifiers.FinalDamage.Flat;
            }
            base.ModifyHitNPC(target, ref modifiers);
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            SoundEngine.PlaySound(SorceryFightSounds.DivineFlameExplosion, Projectile.Center);

            for (int i = 0; i < 20; i++)
            {
                Vector2 vel = new Vector2(Main.rand.NextFloat(-50f, 50f), Main.rand.NextFloat(-50f, 50f));
                LinearParticle particle = new LinearParticle(
                    position: Projectile.Center, 
                    velocity: vel, 
                    color: new Color(232, 157, 100),
                    isUIParticle: false, 
                    drag: 0.99f,
                    scale: 3f, 
                    lifetime: 60);
                ParticleController.SpawnParticle(particle);

                Vector2 vel2 = new Vector2(Main.rand.NextFloat(-25f, 25f), Main.rand.NextFloat(-25f, 25f));
                LinearParticle particle2 = new LinearParticle(
                    position: Projectile.Center, 
                    velocity: vel2, 
                    color: new Color(245, 199, 164),
                    isUIParticle: false, 
                    drag: 0.99f,
                    scale: 1f, 
                    lifetime: 60);
                ParticleController.SpawnParticle(particle2);
            }

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.friendly || npc.type == NPCID.TargetDummy) continue;

                float distance = Vector2.Distance(npc.Center, Projectile.Center);
                if (distance < 750f)
                {
                    npc.AddBuff(BuffID.OnFire, SFUtils.BuffSecondsToTicks(10f));
                    if (npc.whoAmI != target.whoAmI)
                        Main.player[Projectile.owner].ApplyDamageToNPC(npc, baseDamage / 3, 0f, Projectile.direction, false, CursedTechniqueDamageClass.Instance, false);
                }
            }
            Projectile.Kill();
        }
    }
}
