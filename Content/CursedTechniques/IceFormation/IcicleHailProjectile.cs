using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Particles;
using System.Linq;
using System;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.IceFormation
{
    public class IcicleHailProjectile : ModProjectile
    {
        private Texture2D texture;
        private const int FRAME_COUNT = 4;
        private const int TICKS_PER_FRAME = 5;


        public static readonly (int width, int height, int frames, string texture)[] Variants = new[]
        {
            (30, 60, 1, "sorceryFight/Content/CursedTechniques/IceFormation/IcicleHailKnifeProjectile"),
            (50, 100, 1, "sorceryFight/Content/CursedTechniques/IceFormation/IcicleHailKnifeProjectile"),
            (80, 160, 4, "sorceryFight/Content/CursedTechniques/IceFormation/IcicleHailProjectile"),
        };

        int variant => (int)Projectile.ai[0];
        ref float scale => ref Projectile.ai[1];

        public override void SetDefaults()
        {
            Projectile.width = 30; //defaults
            Projectile.height = 60;
            Projectile.penetrate = -1;
            Projectile.knockBack = 0;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.timeLeft = 300;
            //Projectile.ai[1] = Main.rand.NextFloat(0.5f, 2f);
        }


        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (scale == 0f)
            {
                //wasn't working in set defaults for whatever reason
                scale = Main.rand.NextFloat(0.5f, 2f);
                Projectile.width = Variants[variant].width;
                Projectile.height = Variants[variant].height;
            }



            int frameCount = Variants[variant].frames;
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame++ >= frameCount - 1)
                    Projectile.frame = 0;
            }


        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>(Variants[variant].texture).Value;

            int frameCount = Variants[variant].frames;
            int frameHeight = texture.Height / frameCount;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}