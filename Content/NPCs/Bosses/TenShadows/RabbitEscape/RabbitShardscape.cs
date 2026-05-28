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
using Terraria.DataStructures;
using System;
using Microsoft.Build.Evaluation;
using System.Collections.Generic;

namespace sorceryFight.Content.NPCs.Bosses.TenShadows.RabbitEscape
{
    public class RabbitShardscape : ModProjectile
    {

        public int FRAME_COUNT = 1;
        public static readonly int TICKS_PER_FRAME = 1;
        //public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.AmplificationBlue.DisplayName");
        public Texture2D texture;
        public Texture2D texture2;
        public bool animating;
        public float animScale;
        public bool inverted;
        public override void OnSpawn(IEntitySource source)
        {
            if (Main.dedServ) return;
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitShardscape", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                texture2 = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitShardscape_2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            inverted = Main.rand.NextBool();
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 182;
            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.timeLeft = 90;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            if(Projectile.ai[0] == 0)
                behindProjectiles.Add(index);
        }



        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            if (texture == null && !Main.dedServ)
            {
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitShardscape", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                texture2 = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitShardscape_2", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            }
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Projectile.ai[1]++;
            float progress = (int)Projectile.ai[1] / 40f;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            float scaleY = 1f - (float)Math.Pow(1f - progress, 3);
            
            Vector2 origin = new Vector2(texture.Width / 2f, texture.Height);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Bottom - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, new Vector2(1f, scaleY), inverted ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            spriteBatch.Draw(texture2, Projectile.Bottom - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, new Vector2(1f, scaleY), inverted ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin();
            return false;
        }
    }
}