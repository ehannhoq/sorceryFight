
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class ReversalRed : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 7; 
        public static readonly int TICKS_PER_FRAME = 5;

        public override string Name { get; set; } = "Cursed Technique Reversal: Red";
        public override string Description
        { 
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.ReversalRed.Description").Value;
            }
        }
        public override float Cost { get; set; } = -1f;
        public override float CostPercentage { get; set; } = 50f;
        public override float MasteryNeeded { get; set; } = 0f;
        public override Color textColor { get; set; } = new Color(224, 74, 74);

        public override int Damage { get; set ; } = 300;
        public override float Speed { get; set; } = 25f;
        public override float LifeTime { get; set; } = 300f;

        public static Texture2D texture;
     
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<ReversalRed>();
        } 

        public override float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.tileCollide = true;
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
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/ReversalRed").Value;


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
                Projectile.NewProjectile(spawnSource, position, velocity, ModContent.ProjectileType<ReversalRed>(), Damage, 0f, player.whoAmI);
            return true;
        }
    }
}