
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class HollowPurple : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;

        public override string Name { get; set; } = "Hollow Technique: Purple";
        public override string Description
        { 
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.HollowPurple.Description").Value;
            }
        }
        public override string LockedDescription
        {
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.HollowPurple.LockedDescription").Value;
            }
        }
        public override float Cost { get; set; } = 500f;
        public override float CostPercentage { get; set; } = -1f;
        public override float MasteryNeeded { get; set; } = 0f;
        public override Color textColor { get; set; } = new Color(235, 117, 233);

        public override int Damage { get; set ; } = 5000;
        public override float Speed { get; set; } = 35f;
        public override float LifeTime { get; set; } = 500f;
        public override bool Unlocked
        {
            get
            {
                return NPC.downedMoonlord;
            }
        }

        public static Texture2D texture;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<HollowPurple>();
        } 

        public override float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 50;
            Projectile.height = 50;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Projectile.ai[0] += 1;

            if (Projectile.ai[0] > LifeTime)
            {
                Projectile.Kill();
            }

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/HollowPurple").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 0.5f, SpriteEffects.None, 0f);

            return false;
        }

        public override bool Shoot(Terraria.DataStructures.IEntitySource spawnSource, Vector2 position, Vector2 velocity, Player player)
        {
            if (base.Shoot(spawnSource, position, velocity, player))
                Projectile.NewProjectile(spawnSource, position, velocity, ModContent.ProjectileType<HollowPurple>(), Damage, 0f, player.whoAmI);
            return true;
        }
    }
}