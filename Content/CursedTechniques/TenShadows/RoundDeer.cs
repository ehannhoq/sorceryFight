using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.TenShadows
{
    public class RoundDeer : CursedTechniqueSummon
    {
        public static Texture2D spawnTexture;
        public static Texture2D texture;
        private const int SPAWN_FRAMES = 15;
        private const int SPAWN_TICKS = 60;
        private bool spawnAnimDone = false;
        private int spawnFrameCounter = 0;
        private int spawnFrame = 0;

        public static readonly int FRAME_COUNT = 1;
        public static readonly int TICKS_PER_FRAME = 5;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.RoundDeer.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.RoundDeer.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.RoundDeer.LockedDescription");
        public override float Cost => 35f;
        public override float CursedCostPerSecond => 2f;
        public override Color textColor => new Color(120, 40, 200);
        public override bool DisplayNameInGame => true;
        public override int Damage => 22;
        public override int MasteryDamageMultiplier => 40;
        public override float Speed => 12f;
        public override float LifeTime => 0f;
        public override float DetectionRange => 800f;

        public override bool SentryTileCollide => true;
        //public override bool FollowsPlayer => false;

        private const float HEAL_RADIUS = 300f;
        private const int HEAL_AMOUNT = 100;
        private const float HEAL_COOLDOWN = 120f;

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<RoundDeer>();
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string stats = base.GetStats(sf);
            stats += $"Detection: {DetectionRange / 16f:F0} tiles\n";
            stats += "Toggle: Press to summon/dismiss\n";
            return stats;
        }

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = FRAME_COUNT;
        }

        public override void SummonSetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 50;
        }
        public override void SummonAI()
        {

            if (!spawnAnimDone)
            {
                spawnFrameCounter++;
                if (spawnFrameCounter >= SPAWN_TICKS / SPAWN_FRAMES)
                {
                    spawnFrameCounter = 0;
                    spawnFrame++;
                    if (spawnFrame >= SPAWN_FRAMES)
                        spawnAnimDone = true;
                }
                return;
            }

            //change owner to our identifiers
            Projectile.spriteDirection = (Owner.Center.X > Projectile.Center.X) ? 1 : -1;

            AnimateFrames(FRAME_COUNT, TICKS_PER_FRAME);

            if (Main.rand.NextBool(4))
            {
                float dustCount = MathHelper.TwoPi * HEAL_RADIUS / 8f;
                for (int i = 0; i < dustCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / dustCount;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Cloud);
                    dust.position = Projectile.Center + angle.ToRotationVector2() * HEAL_RADIUS;
                    dust.scale = 0.7f;
                    dust.noGravity = true;
                    dust.velocity = Vector2.Zero;
                }
            }

            SummonState++;
            if (SummonState >= HEAL_COOLDOWN && Projectile.owner == Main.myPlayer)
            {
                SummonState = 0f;

                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    Player p = Main.player[i];
                    if (!p.active || p.dead)
                        continue;

                    if (Vector2.Distance(Projectile.Center, p.Center) <= HEAL_RADIUS)
                    {
                        p.Heal(HEAL_AMOUNT);
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            SpriteEffects flip = Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            if (!spawnAnimDone)
            {
                if (spawnTexture == null && !Main.dedServ)
                    spawnTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/RoundDeerSpawn", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

                int frameHeight = spawnTexture.Height / SPAWN_FRAMES;
                int frameY = spawnFrame * frameHeight;
                Vector2 origin = new Vector2(spawnTexture.Width / 2, frameHeight / 2);
                Rectangle sourceRectangle = new Rectangle(0, frameY, spawnTexture.Width, frameHeight);

                //Main.NewText($"Spawn frame: {spawnFrame} / {SPAWN_FRAMES - 1}, counter: {spawnFrameCounter}");
                //Main.NewText($"spawnTexture actual size: {spawnTexture.Width}x{spawnTexture.Height}");

                spriteBatch.Draw(spawnTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }
            else
            {
                if (texture == null && !Main.dedServ)
                    texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/TenShadows/RoundDeer").Value;

                int frameHeight = texture.Height / FRAME_COUNT;
                int frameY = Projectile.frame * frameHeight;
                Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
                Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

                spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, Projectile.scale, flip, 0f);
            }

            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustPerfect(
                    Projectile.Center + Main.rand.NextVector2Circular(20f, 20f),
                    DustID.Shadowflame
                );
                dust.velocity = Main.rand.NextVector2Circular(4f, 4f);
                dust.scale = Main.rand.NextFloat(1f, 1.5f);
                dust.noGravity = true;
            }

            base.OnKill(timeLeft);
        }
    }
}
