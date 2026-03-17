using System;
using System.Globalization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Projectiles.Melee;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace sorceryFight.Content.Projectiles
{
    public class ChakiraResonantBeam : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/CursedTechniques/CursedTechnique";
        public static readonly int FRAMES = 60;
        public static readonly int TICKS_PER_FRAME = 1;

        private Texture2D texture;
        public const float MAX_LENGTH = 2000f;
        public const float STEP_SIZE = 300f;

        private ref float beamLength => ref Projectile.ai[0];
        private ref float beamHeight => ref Projectile.ai[1];
        private ref float tick => ref Projectile.ai[2];

        public override void SetDefaults()
        {
            Projectile.width = 300;
            Projectile.height = 300;
            Projectile.ArmorPenetration = 7;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= FRAMES)
                {
                    Projectile.frame = 0;
                }
            }

            Vector2 direction = Projectile.rotation.ToRotationVector2();
            for (float i = 0f; i < MAX_LENGTH; i += STEP_SIZE)
            {
                Vector2 checkPos = Projectile.Center + direction * i;
                if (!Collision.CanHitLine(Projectile.Center, 1, 1, checkPos, 1, 1))
                {
                    break;
                }
                beamLength = i;
                beamLength = MathHelper.Clamp(beamLength, 0f, MAX_LENGTH);
            }

            if (tick++ >= 60)
                tick = 60;
            float easeOutProg = MathF.Sqrt(1 - MathF.Pow((tick / 60f) - 1, 2));
            easeOutProg = MathHelper.Clamp(easeOutProg, 0.0f, 1.0f);
            beamHeight = easeOutProg;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
                return true;

            float useless = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * beamLength, Projectile.height, ref useless))
                return true;

            return false;
        }
    }
}
