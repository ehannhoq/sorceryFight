using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Vessel
{
    public class LineDevestation : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 1;
        public static readonly int TICKS_PER_FRAME = 1;
        public static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.LineDevestation.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.LineDevestation.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.LineDevestation.LockedDescription");
        public override float Cost => 200f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => false;
        public override int Damage => 60;
        public override int MasteryDamageMultiplier => 40;
        public override float Speed => 12f;
        public override float LifeTime => 180f;

        public bool animating;
        public float animScale;

        private List<Vector2> dotPositions = new List<Vector2>();
        private bool initialized = false;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<LineDevestation>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.sukunasFingerConsumed >= 15;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 65;
            Projectile.height = 65;
            Projectile.tileCollide = true;
            animating = false;
            Projectile.penetrate = -1;
            animScale = 1.25f;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0] += 1;
            float beginAnimTime = 30f;

            if (Projectile.ai[0] > LifeTime + beginAnimTime)
                Projectile.Kill();

            if (Projectile.ai[0] < beginAnimTime)
            {
                if (!animating)
                {
                    animating = true;
                    SoundEngine.PlaySound(SorceryFightSounds.AmplificationBlueChargeUp, Projectile.Center);
                }
                return;
            }

            if (animating)
            {
                Projectile.tileCollide = true;
                animating = false;
            }

            // Run once after the anim delay
            if (!initialized)
            {
                initialized = true;

                Projectile.localAI[0] = Projectile.Center.X;
                Projectile.localAI[1] = Projectile.Center.Y;

                Vector2 spawnPos = Projectile.Center;
                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);

                float maxLength = 3000f;
                float dotSpacing = 40f;

                float forwardLength = GetRayLength(spawnPos, direction, maxLength);
                float backwardLength = GetRayLength(spawnPos, -direction, maxLength);

                int forwardCount = (int)(forwardLength / dotSpacing);
                int backwardCount = (int)(backwardLength / dotSpacing);

                for (int i = -backwardCount; i <= forwardCount; i++)
                    dotPositions.Add(spawnPos + direction * (dotSpacing * i));
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Vessel/LineDevestation").Value;

            Texture2D dot = TextureAssets.MagicPixel.Value;
            Rectangle dotSourceRect = new Rectangle(0, 0, 1, 1);

            //this restart code is here because it doesn't work without it for whatever reason, artifacts to the bottom of the screen
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            foreach (Vector2 dotPos in dotPositions)
            {
                Vector2 screenPos = dotPos - Main.screenPosition;
                if (screenPos.X < -50 || screenPos.X > Main.screenWidth + 50 ||
                    screenPos.Y < -50 || screenPos.Y > Main.screenHeight + 50)
                    continue;

                int dotWidth = 16;  // line length
                int dotHeight = 4;  // line thickness

                Rectangle destination = new Rectangle(
                    (int)screenPos.X - dotWidth / 2,
                    (int)screenPos.Y - dotHeight / 2,
                    dotWidth,
                    dotHeight
                );


                spriteBatch.Draw(dot, screenPos, dotSourceRect, Color.White * 0.85f, Projectile.rotation, new Vector2(dotWidth / 2f, dotHeight / 2f), new Vector2(dotWidth, dotHeight), SpriteEffects.None, 0f);
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, origin, animScale, SpriteEffects.None, 0f);

            return false;
        }

        private static float GetRayLength(Vector2 start, Vector2 direction, float maxLength, float step = 8f)
        {
            for (float dist = 0; dist < maxLength; dist += step)
            {
                Vector2 checkPos = start + direction * dist;
                int tileX = (int)(checkPos.X / 16f);
                int tileY = (int)(checkPos.Y / 16f);

                if (!WorldGen.InWorld(tileX, tileY)) return dist;

                Tile tile = Main.tile[tileX, tileY];
                if (tile != null && tile.HasTile && Main.tileSolid[tile.TileType])
                    return dist;
            }
            return maxLength;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);

        }
    }
}
