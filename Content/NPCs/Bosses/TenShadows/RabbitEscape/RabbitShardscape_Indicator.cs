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
    public class RabbitShardscape_Indicator : ModProjectile
    {

        public static readonly int FRAME_COUNT = 2;
        public static readonly int TICKS_PER_FRAME = 30;
        //public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.AmplificationBlue.DisplayName");
        public Texture2D texture;
        public bool animating;
        public float animScale;
        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
            Main.projFrames[Projectile.type] = FRAME_COUNT;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitShardscape_Indicator", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 60;
            Projectile.height = 182;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 150;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }

        }
        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<RabbitShardscape>(), 150, 0, Projectile.owner, ai0: 0);
        }


        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
        {
            overPlayers.Add(index);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/NPCs/Bosses/TenShadows/RabbitEscape/RabbitShardscape_Indicator", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);
            return false;
        }
    }
}