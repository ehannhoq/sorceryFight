using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using Terraria.Graphics.CameraModifiers;
using Terraria.DataStructures;
using System;
using Microsoft.Build.Evaluation;
using System.Collections.Generic;

namespace sorceryFight.Content.NPCs.Bosses.TenShadows.RabbitEscape
{
    public class RabbitMoonlord : ModProjectile
    {

        public static readonly int FRAME_COUNT = 3;
        public static readonly int TICKS_PER_FRAME = 1;
        //public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.AmplificationBlue.DisplayName");
        public Texture2D texture;
        public Texture2D armTexture;
        public Texture2D faceTexture;
        public Vector2 originalPosition;
        public Vector2 newPosition; //ts so unoptimized ;-;
        public float leftRotation;
        public float rightRotation;
        public bool animating;
        public float animScale;
        public bool inverted;
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.dedServ) return;
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitMoonlord", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                armTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitMoonlord_Arm", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                faceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitMoonlord_Face", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            
            originalPosition = Projectile.position;
            Projectile.position.Y += 368f;
            newPosition = Projectile.position;
            SoundEngine.PlaySound(SoundID.Roar);
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 7.5f, 3f, 120, 1000f));
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 210;
            Projectile.height = 442;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 900;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.ai[0]++;
            leftRotation = MathHelper.ToRadians(-15f) * (float)Math.Sin(Projectile.ai[0] * 0.05f);
            rightRotation = MathHelper.ToRadians(15f) * (float)Math.Sin(Projectile.ai[0] * 0.05f);
            if (Projectile.ai[0] < 120f)
            {
                float t = Projectile.ai[0] / 120f; 
                t = 1f - (float)Math.Pow(1f - t, 3);
                Projectile.position.Y = MathHelper.Lerp(newPosition.Y, originalPosition.Y, t);
            }

            if(Projectile.ai[0] == 210f)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center - Vector2.UnitY * 135f, Vector2.Zero, ModContent.ProjectileType<RabbitLaser>(), 0, 0, 255);
            }
        }
        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            behindProjectiles.Add(index);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            if (texture == null && !Main.dedServ)
            {
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitMoonlord", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                armTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitMoonlord_Arm", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                faceTexture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitMoonlord_Face", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height / 2f);
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 arm1Origin = new Vector2(armTexture.Width / 2f + 85f, armTexture.Height / 2f - 35f);
            Vector2 arm2Origin = new Vector2(armTexture.Width / 2f - 85f, armTexture.Height / 2f - 35f);
            Rectangle armSR = new Rectangle(0, 0, armTexture.Width, armTexture.Height);
            int frameHeight = faceTexture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;
            Vector2 faceOrigin = new Vector2(faceTexture.Width / 2f, faceTexture.Height / 2f);
            Rectangle faceSR = new Rectangle(0, frameY, faceTexture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            spriteBatch.Draw(armTexture, Projectile.Center - Main.screenPosition + new Vector2(115f, -37f), armSR, Color.White, rightRotation, arm2Origin, 1f, SpriteEffects.FlipHorizontally, 0f);
            spriteBatch.Draw(armTexture, Projectile.Center - Main.screenPosition + new Vector2(-115f, -37f), armSR, Color.White, leftRotation, arm1Origin, 1f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }
    }
}