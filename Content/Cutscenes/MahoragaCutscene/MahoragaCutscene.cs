using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.NPCs.Mahoraga;
using sorceryFight.Content.NPCs.TownNPCs;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using sorceryFight.Content.UI.Chants;
using sorceryFight.Packets;
using sorceryFight.Utilities.EaseFunctions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Cutscenes.MahoragaCutscene
{
    public sealed class MahoragaCutscene : Cutscene
    {
        private static Texture2D mahoragaCacoonTexture = ModContent.Request<Texture2D>("sorceryFight/Content/Cutscenes/MahoragaCutscene/MahoragaCacoon", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        private static Texture2D dogTexture = ModContent.Request<Texture2D>("sorceryFight/Content/Cutscenes/MahoragaCutscene/BlackWolfHowl", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        private static Texture2D frogTexture = ModContent.Request<Texture2D>("sorceryFight/Content/Cutscenes/MahoragaCutscene/Frog", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        private const int FADE_TO_BLACK_TIME = 120;
        private const int DOGS_AND_FROGS_TIME = 60;
        private const int MAHORAGA_CACOON_PHASE_ONE_TIME = 290;
        private const int CHANT_TIME_AFTER_MAHORAGA = 180;
        private const int MAHORAGA_CACOON_PHASE_TWO_TIME = MAHORAGA_CACOON_PHASE_ONE_TIME + CHANT_TIME_AFTER_MAHORAGA + 660;
        private static readonly int CACOON_FRAME_HEIGHT = mahoragaCacoonTexture.Height / 2;

        private static Chant MAHORAGA_CHANT_ONE => new Chant(
            "Eight-Handled Sword Divergent Sila Divine General",
            timeBetweenWords: 30,
            timeBetweenCharacters: 5,
            delayAfterChant: 60,
            colors: [
                Color.Wheat,
                new Color(242, 227, 184),
                new Color(207, 185, 145),
            ],
            onEnd: () => {
                ChantManager.InitiateChant(MAHORAGA_CHANT_TWO);
            },
            perCharacterEvent: (currentIndex, remaining) => {
                SoundEngine.PlaySound(SoundID.MenuTick with { PitchVariance = 0.25f, MaxInstances = 0 });
            },
            perCharacterAnimationTime: 30,
            characterAnimationOpacityFadeIn: true
        ); // takes 400 ticks to complete words, 460 to finish chant

        private static Chant MAHORAGA_CHANT_TWO => new Chant(
            "Mahoraga",
            timeBetweenCharacters: 10,
            delayAfterChant: 180,
            scale: 1.5f,
            chantStyles: [
                new CharacterWave(),
                new CharacterShake()
            ],
            perCharacterEvent: (currentIndex, remaining) => {
                SoundEngine.PlaySound(SoundID.MenuTick with { PitchVariance = 0.25f, MaxInstances = 0 });
            }
        ); // takes 80 ticks to complete word, 200 to finish chant

        public override int CutsceneLength => MAHORAGA_CACOON_PHASE_TWO_TIME + 180;

        private NPC megumi;
        private Vector2 mahoragaPos;
        private int mahoragaCacoonFrame;
        private int dogsAndFrogsCount = 0;
        private int[] dogsAndFrogsTimes = [30, 30, 30, 30, 30];
        private int[] dogsAndFrogsCounter = [0, 0, 0, 0, 0];

        public override void OnStart()
        {
            megumi = Main.npc[Main.npc.FindIndex(npc => npc.type == ModContent.NPCType<MegumiFushiguro>())];
            (megumi.ModNPC as MegumiFushiguro).summoningMahoraga = true;

            CutsceneManager.FreezeNPC(megumi, true);
            CutsceneManager.DisablePlayerInput(true);
            Main.hideUI = true;
            CameraController.SetCameraPosition(megumi.Center);

            mahoragaCacoonFrame = 0;
            float heightOffset = (CACOON_FRAME_HEIGHT - megumi.height) / 2f;
            Vector2 offset = new Vector2(300f * -megumi.direction, -heightOffset);
            mahoragaPos = megumi.Center + offset;
        }

        public override void Update()
        {
            if (Timer == MAHORAGA_CACOON_PHASE_TWO_TIME)
            {
                mahoragaCacoonFrame = 1;
                SoundEngine.PlaySound(SorceryFightSounds.MahoragaMouthOpen);
            }

            if (Timer == MAHORAGA_CACOON_PHASE_ONE_TIME)
            {
                SoundEngine.PlaySound(SorceryFightSounds.MahoragaAdaptation);
            }
            
            if (Timer == MAHORAGA_CACOON_PHASE_ONE_TIME + CHANT_TIME_AFTER_MAHORAGA)
                ChantManager.InitiateChant(MAHORAGA_CHANT_ONE);

            if (Timer >= MAHORAGA_CACOON_PHASE_ONE_TIME)
            {
                if (Timer % 5 == 0)
                {
                    Vector2 pos = mahoragaPos - new Vector2(0f, CACOON_FRAME_HEIGHT / 2f) + new Vector2(20f, 0f).RotatedByRandom(MathF.PI);
                    Vector2 velocity = new Vector2(Main.rand.NextFloat(10f, 40f), -5f).RotatedByRandom(MathHelper.PiOver4) * new Vector2(-megumi.direction, 1f);

                    LinearParticle particle = new LinearParticle(pos, velocity, new Color(255, 255, 255, 180), false, 0.9f, 0.5f, Main.rand.Next(10, 50));
                    ParticleController.SpawnParticle(particle);
                }
            }

            if (Timer == DOGS_AND_FROGS_TIME)
                SoundEngine.PlaySound(SorceryFightSounds.DivineDogsHowl);

            if (Timer >= DOGS_AND_FROGS_TIME && Timer <= MAHORAGA_CACOON_PHASE_ONE_TIME)
            {
                int timeSince = Timer - DOGS_AND_FROGS_TIME;
                dogsAndFrogsCount = Math.Min(timeSince / 15 + 1, 5);

                for (int i = 0; i < dogsAndFrogsCount; i++)
                {
                    dogsAndFrogsCounter[i]++;
                }
            }
        }

        public override void DrawBehindNPCs(SpriteBatch spriteBatch)
        {
            Rectangle screen = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, screen, new Color(0, 0, 0, EaseFunctions.EaseInOut(Timer / (float)FADE_TO_BLACK_TIME)));
            

            if (Timer <= MAHORAGA_CACOON_PHASE_ONE_TIME)
            {
                for (int i = 0; i < dogsAndFrogsCount; i++)
                {
                    bool isDog = i % 2 == 0;
                    Texture2D texture = isDog ? dogTexture : frogTexture;

                    float progress = dogsAndFrogsCounter[i] / (float) dogsAndFrogsTimes[1];
                    progress = EaseFunctions.EaseOut(progress);

                    float heightOffset = (texture.Height - megumi.height) / 2f;
                    Vector2 offset = new Vector2(90f * (i + 1) * megumi.direction, heightOffset);

                    Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
                    SpriteEffects flip = megumi.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                    spriteBatch.Draw(texture, megumi.Center - offset - Main.screenPosition, src, new Color(255, 255 ,255, progress), 0f, src.Size() * 0.5f, 1f, flip, 0f);
                }
            }

            if (Timer > MAHORAGA_CACOON_PHASE_ONE_TIME)
            {
                int frameY = CACOON_FRAME_HEIGHT * mahoragaCacoonFrame;
                Rectangle cacoonSrc = new Rectangle(0, frameY, mahoragaCacoonTexture.Width, CACOON_FRAME_HEIGHT);
                SpriteEffects flip = megumi.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                spriteBatch.Draw(mahoragaCacoonTexture, mahoragaPos - Main.screenPosition, cacoonSrc, Color.White, 0f, cacoonSrc.Size() * 0.5f, 1f, flip, 0f);
            }
        }

        public override void OnEnd()
        {
            CameraController.ResetCameraPosition();
            CutsceneManager.DisablePlayerInput(false);
            CutsceneManager.FreezeNPC(megumi, false);
            (megumi.ModNPC as MegumiFushiguro).summoningMahoraga = false;
            Main.hideUI = false;

            if (Main.netMode == NetmodeID.MultiplayerClient && Main.myPlayer == 0) // let owner of the world own the packet
            {
                SpawnNPCOnPlayerPacket.Send(Main.LocalPlayer, (int)mahoragaPos.X, (int)mahoragaPos.Y, ModContent.NPCType<MahoragaBoss>());
            }
            else
            {
                NPC.NewNPC(new EntitySource_WorldEvent(), (int)mahoragaPos.X, (int)mahoragaPos.Y, ModContent.NPCType<MahoragaBoss>());
            }
        }
    }
}
