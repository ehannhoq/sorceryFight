using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.Content.UI.Chants;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class WorldCuttingSlash : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;
        static List<string> incantations;

        public override string InternalName => "WorldCuttingSlash";

        ref float castTime => ref Projectile.ai[0];
        ref float multiplier => ref Projectile.localAI[0];
        ref float slashed => ref Projectile.localAI[1];
        ref float finishedChanting => ref Projectile.localAI[2];
        private const int SLASH_TIME = 120;
        private const int BUFFER_TIME = 30;

        public override void SetStaticDefaults()
        {
            incantations = new List<string>()
            {
                "Dragon Scales.",
                "Repulsion.",
                "Paired Falling Stars."
            };
        }


        public WorldCuttingSlash()
        {
            Technique.baseDamage = 4000;
            Technique.damagePerBoss = 200;
            Technique.cost = 1200f;
            Technique.speed = 50;
            Technique.lifetime = 600;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }


        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, 1, 1);
            Projectile.scale = 0.0f;

            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            sfPlayer.disableRegenFromProjectiles = true;

            multiplier = sfPlayer.cursedOfuda ? CursedOfuda.cursedTechniqueCastTimeDecrease : 1.0f;

            ChantManager.InitiateChant(new Chant(
                text: SFUtils.CombineListOfStrings(incantations),
                timeBetweenCharacters: (int)(5 * multiplier),
                timeBetweenWords: (int)(30 * multiplier),
                colors: [
                    Color.Black,
                    new Color(41, 11, 9, 255)
                ],
                delayAfterChant: BUFFER_TIME,
                chantStyles: [
                    new CharacterGlow(new Color(245, 209, 196, 255), glowRadius: 6f),
                    new CharacterStroke(new Color(230, 206, 179, 255), 2),
                ],
                onEnd: () => {
                    int index = Projectile.whoAmI;
                    Main.projectile[index].localAI[2] = 1.0f;
                },
                                perCharacterEvent: (currentIndex, remaining) => {
                    SoundEngine.PlaySound(SoundID.MenuTick with { PitchVariance = 0.25f, MaxInstances = 0 });
                },
                perSentenceEvent: (currentSentenceIndex, remainingSentences) => {
                    if (remainingSentences > 1)
                        SoundEngine.PlaySound(SorceryFightSounds.ChantingChargeUp);
                    else    
                        SoundEngine.PlaySound(SorceryFightSounds.ChantingFinished);
                },
                perCharacterAnimationTime: 15,
                characterStartOffset: new Vector2(20f, 10f),
                characterAnimationOpacityFadeIn: true
            ));
        }


        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);
            }

            if (finishedChanting == 0.0)
                return;

            castTime++;
            if (castTime < SLASH_TIME)
            {
                if (slashed == 0.0f)
                {
                    Projectile.netUpdate = true;
                    CameraController.CameraShake(15, 10, 10);
                    ImpactFrameController.ImpactFrame(Color.White, 2);
                    player.AddBuff(ModContent.BuffType<BurntTechnique>(), SFUtils.BuffSecondsToTicks(5.0f));
                    player.SorceryFight().disableRegenFromProjectiles = false;
                    slashed = 1f;
                    SoundEngine.PlaySound(SorceryFightSounds.WorldCuttingSlash, Projectile.Center);
                }

                float percent = castTime / SLASH_TIME;
                float progress = MathF.Pow((percent * 2) - 1, 3);
                progress = 1 - Math.Clamp(progress, 0.0f, 1.0f);

                if (!Filters.Scene["SF:WorldCuttingSlash"].Active)
                    Filters.Scene.Activate("SF:WorldCuttingSlash").GetShader().UseTargetPosition(Projectile.Center).UseDirection(Projectile.velocity).UseOpacity(1.0f);

                Filters.Scene["SF:WorldCuttingSlash"].GetShader().UseProgress(progress);

                return;
            }


            Filters.Scene["SF:WorldCuttingSlash"].GetShader().UseOpacity(0.0f);
            Filters.Scene.Deactivate("SF:WorldCuttingSlash");
            Projectile.Kill();
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Defense *= 0.0f;
            modifiers.DefenseEffectiveness *= 0.0f;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (slashed == 1f)
            {
                float useless = 0.0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * 2000.0f, 10.0f, ref useless))
                    return true;
            }

            return false;
        }
    }
}
