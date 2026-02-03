using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.PlayerDomains
{
    public class SimpleDomainFloating : PlayerDomainExpansion, ISimpleDomain
    {
        public override string InternalName => "SimpleDomainFloating";

        public override SoundStyle CastSound => SorceryFightSounds.CommonWoosh;

        public override int Tier => 5;

        public override float SureHitRange => 100f;

        public override bool ClosedDomain => true;
        public override float Cost => 25f;


        private int frame = 0;
        private int frameTime = 0;
        private Texture2D expandedTexture => ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/PlayerDomains/SimpleDomainFloating", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        private const int initFrames = 11;
        private Texture2D initTexture => ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/PlayerDomains/SimpleDomainFloatingExpanding", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        private Texture2D currentTexture;


        private const int frames = 3;
        private const int ticksPerFrame = 2;

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (currentTexture == null)
            {
                currentTexture = initTexture;
            }

            int currentFrames = currentTexture == initTexture ? initFrames : frames;

            int frameHeight = currentTexture.Height / currentFrames;
            int frameY = frameHeight * frame;

            Rectangle src = new Rectangle(0, frameY, currentTexture.Width, frameHeight);
            spriteBatch.Draw(currentTexture, center - Main.screenPosition, src, Color.White, 0f, src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
        }

        public override void Update()
        {
            if (frameTime++ >= ticksPerFrame)
            {
                frameTime = 0;

                if (currentTexture == initTexture)
                {
                    this.center = Main.player[owner].Center;
                    if (frame++ >= initFrames - 1)
                    {
                        currentTexture = expandedTexture;
                        frame = 0;
                        frameTime = 0;
                    }
                }
                else
                {
                    if (frame++ >= frames - 1)
                    {
                        frame = 0;
                    }
                }
            }

            SorceryFightPlayer sfPlayer = Main.LocalPlayer.SorceryFight();

            if (Main.myPlayer == owner)
            {
                sfPlayer.disableRegenFromDE = true;
                sfPlayer.cursedEnergy -= SFUtils.RateSecondsToTicks(Cost);

                if (sfPlayer.Player.dead || sfPlayer.cursedEnergy < 10)
                {
                    DomainExpansionController.CloseDomain(id);
                }
            }

            // Since domains are synced across all clients, we're allowed to have each client determine logic for themselves, even if they don't own the domain.
            if (Main.dedServ) return;
            
            sfPlayer = Main.LocalPlayer.SorceryFight();
            sfPlayer.inSimpleDomain = false;

            foreach (Player player in Main.ActivePlayers)
            {
                if (player.whoAmI != Main.LocalPlayer.whoAmI) continue;

                if (Vector2.Distance(player.Center, this.center) < SureHitRange)
                {
                    sfPlayer.inSimpleDomain = true;
                    if (!sfPlayer.disableCurseTechniques) sfPlayer.disableCurseTechniques = true;
                    Main.LocalPlayer.wingTime = Main.LocalPlayer.wingTimeMax;
                }
            }
        }

        public override void OnClose()
        {
            currentTexture = null;
            frame = 0;
            frameTime = 0;
            Main.LocalPlayer.SorceryFight().inSimpleDomain = false;
            Main.LocalPlayer.SorceryFight().disableCurseTechniques = false;
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.CultistBoss);
        }

        public override void SureHitEffect(NPC npc)
        {
        }
    }
}
