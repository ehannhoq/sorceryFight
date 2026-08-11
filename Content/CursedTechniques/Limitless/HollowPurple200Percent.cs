using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using System;
using sorceryFight.Content.Items.Accessories;
using Terraria.DataStructures;
using sorceryFight.Content.Particles;

using sorceryFight.Content.UI.Chants;
using sorceryFight.Utilities.EaseFunctions;
using Terraria.ID;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class HollowPurple200Percent : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;
        public static readonly Vector2 blueOffset = new Vector2(-60f, -20f);
        public static readonly Vector2 redOffset = new Vector2(60f, -20f);

        public override string InternalName => "HollowPurple200Percent";

        public static Texture2D texture;
        public static Texture2D flashTexture;

        private ref float time => ref Projectile.ai[0];
        private ref float blueIndex => ref Projectile.ai[1];
        private ref float redIndex => ref Projectile.ai[2];

        private int totalTime;
        private int[] individualIncantationTime = new int[4];

        private const int TIME_BETWEEN_CHARACTERS = 4;
        private const int TIME_BETWEEN_SENTENCES = 15;

        public HollowPurple200Percent()
        {
            Technique.baseDamage = 3000;
            Technique.damagePerBoss = 250;
            Technique.cost = 1200f;
            Technique.speed = 50;
            Technique.lifetime = 500;
        }


        public override void SetDefaults()
        {
            Projectile.width = 250;
            Projectile.height = 250;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            sfPlayer.disableRegenFromProjectiles = true;
            float multiplier = sfPlayer.cursedOfuda ? CursedOfuda.cursedTechniqueCastTimeDecrease : 1f;

            Projectile.damage = 0;
            Projectile.velocity = Vector2.Zero;

            List<string> incantations = [
                "Nine Ropes.",
                "Polarized Light.",
                "Crow and Declaration.",
                "Between Front and Back.",
            ];

            ChantManager.InitiateChant(new Chant(
                text: SFUtils.CombineListOfStrings(incantations),
                timeBetweenCharacters: (int)(TIME_BETWEEN_CHARACTERS * multiplier),
                timeBetweenWords: (int)(TIME_BETWEEN_SENTENCES * multiplier),
                delayAfterChant: 30,
                colors: [
                    new Color(216, 157, 237, 255),
                    new Color(176, 76, 212, 255)
                ],
                chantStyles: [
                    new CharacterGlow(
                        new Color(203, 165, 232, 255),
                        glowRadius: 6f
                    ),
                    new CharacterStroke(
                        new Color(82, 41, 107, 255),
                        borderWidth: 2f
                    ),
                ],
                scale: 1f,
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

            for (int i = 0; i < incantations.Count; i++)
            {
                string str = incantations[i];

                int charTime = (int)(str[..^1].Length * TIME_BETWEEN_CHARACTERS * multiplier);
                totalTime += charTime;
                individualIncantationTime[i] = charTime;

                if (i > 0)
                    individualIncantationTime[i] += individualIncantationTime[i - 1]  + (int)(TIME_BETWEEN_SENTENCES * multiplier); 

                totalTime += (int)(TIME_BETWEEN_SENTENCES * multiplier);
            }
            totalTime += 15;
        }

        public override void AI()
        {
            time += 1;

            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            int blueCastTime = individualIncantationTime[0];
            int redCastTime = individualIncantationTime[1];
            int collisionStartTime = individualIncantationTime[2];

            Projectile.HandleProjectileAnimation(FRAME_COUNT, TICKS_PER_FRAME);

            Vector2 bluePosition = player.Center + blueOffset;
            Vector2 redPosition = player.Center + redOffset;


            if (time < totalTime)
            {
                Projectile.Center = player.Center + new Vector2(0f, -30f);

                if (time == blueCastTime)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        blueIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), bluePosition, Vector2.Zero, ModContent.ProjectileType<AmplificationBlue>(), 0, 0f, Projectile.owner, default, 1);
                        Projectile.netUpdate = true;
                    }
                }

                if (time < blueCastTime)
                    return;

                Projectile blue = Main.projectile[(int)blueIndex];
                blue.Center = bluePosition;
                blue.timeLeft = 60;

                Vector2 blueParticleOffsetPosition = blue.Center + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                Vector2 blueParticleVelocity = blueParticleOffsetPosition.DirectionTo(player.Center + new Vector2(0f, -20f)) * 2;
                LinearParticle blueParticle = new LinearParticle(blueParticleOffsetPosition, blueParticleVelocity, new Color(108, 158, 240), false, 0.9f, 1, 30);
                ParticleController.SpawnParticle(blueParticle);

                if (time == redCastTime)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        redIndex = Projectile.NewProjectile(Projectile.GetSource_FromThis(), redPosition, Vector2.Zero, ModContent.ProjectileType<MaximumOutputRed>(), 0, 0f, Projectile.owner, default, 1);
                        Projectile.netUpdate = true;
                    }
                }

                if (time < redCastTime)
                    return;
                    
                Projectile red = Main.projectile[(int)redIndex];
                red.Center = redPosition;
                red.timeLeft = 60;

                Vector2 redParticleOffsetPosition = redPosition + new Vector2(Main.rand.NextFloat(-20f, 20f), Main.rand.NextFloat(-20f, 20f));
                Vector2 redParticleVelocity = redParticleOffsetPosition.DirectionTo(player.Center + new Vector2(0f, -20f)) * 2;
                LinearParticle redParticle = new LinearParticle(redParticleOffsetPosition, redParticleVelocity, new Color(224, 74, 74), false, 0.9f, 1, 30);
                ParticleController.SpawnParticle(redParticle);
                                    
                if (time >= collisionStartTime)
                {
                    float timeLeft = totalTime - collisionStartTime;
                    float progress = (time - collisionStartTime) / timeLeft;

                    float lerp = EaseFunctions.EaseInExponential(3f, progress);

                    if (MathF.Round(progress, 1) == 0.5)
                        SoundEngine.PlaySound(SorceryFightSounds.CommonWoosh, Projectile.Center);

                    blue.Center = Vector2.Lerp(blue.Center, player.Center + new Vector2(0.0f, -20.0f), lerp);                
                    red.Center = Vector2.Lerp(red.Center, player.Center + new Vector2(0.0f, -20.0f), lerp);                
                    return;
                }
            }


            if (time == totalTime)
            {
                Projectile.damage = CalculateTrueDamage(sfPlayer);
                Projectile.timeLeft = lifetime;
                Main.projectile[(int)blueIndex].Kill();
                Main.projectile[(int)redIndex].Kill();
                Projectile.Center = player.Center + new Vector2(0f, -40f);
                SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleSnap, Projectile.Center);
                sfPlayer.disableRegenFromProjectiles = false;

                CameraController.CameraShake(30, 75, 10);
                ImpactFrameController.ImpactFrame(new Color(239, 138, 242), 8);

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * speed;
                    sfPlayer.AddDeductableDebuff(ModContent.BuffType<BurntTechnique>(), 5);
                    Projectile.netUpdate = true;
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

            for (int i = 0; i < 40; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7));

                LinearParticle particle = new LinearParticle(target.Center, Projectile.velocity + variation, new Color(239, 138, 242), false, 0.9f, 1, 30);
                ParticleController.SpawnParticle(particle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/HollowPurple200Percent", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;


            if (flashTexture == null && !Main.dedServ)
                flashTexture = ModContent.Request<Texture2D>("sorceryFight/Content/VFX/HollowPurpleFlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, time <= totalTime ? 0f : 2f, SpriteEffects.None, 0f);

            float collisionStartTime = individualIncantationTime[2];
            float totalCastTime = totalTime;

            if (time >= collisionStartTime && time < totalCastTime)
            {
                float timeLeft = totalTime - collisionStartTime;
                float progress = (time - collisionStartTime) / timeLeft;
                progress = EaseFunctions.EaseInExponential(3f, progress);
                Rectangle flashSource = new Rectangle(0, 0, flashTexture.Width, flashTexture.Height);

                spriteBatch.End();
                spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    null,
                    Main.GameViewMatrix.ZoomMatrix
                );

                spriteBatch.Draw(flashTexture, Projectile.Center - Main.screenPosition, flashSource, new Color(255, 255, 255, (int)(255 * progress)), 0.0f, flashSource.Size() * 0.5f, 4.0f, SpriteEffects.None, 0f);


                spriteBatch.End();
                spriteBatch.Begin(
                    SpriteSortMode.Deferred,
                    BlendState.AlphaBlend,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    null,
                    Main.GameViewMatrix.TransformationMatrix
                );
            }

            return false;
        }
    }
}
