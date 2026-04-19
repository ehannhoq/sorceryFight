
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using sorceryFight.Content.Particles;
using sorceryFight.Content.Particles.UIParticles;
using Terraria.Audio;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class ReversalRed : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4; 
        public static readonly int TICKS_PER_FRAME = 3;

        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.ReversalRed.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.ReversalRed.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.ReversalRed.LockedDescription");
        public override float Cost { get; } = 300f;
        public override Color textColor { get; } = new Color(224, 74, 74);

       
        public override int Damage => 3000;
        public override int MasteryDamageMultiplier => 80;
        public override float Speed { get; } = 32f;
        public override float LifeTime { get; } = 200f;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.unlockedRCT;
        }

        public static Texture2D texture;
        public override bool DisplayNameInGame { get; } = true;

        public bool animating;
        public float animScale;
        public Rectangle hitbox;
        
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<ReversalRed>();
        } 

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            animating = false;
            animScale = 0f;
            hitbox = Projectile.Hitbox;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            int i = base.UseTechnique(sf);

            if (i != -1)
            {
                SoundEngine.PlaySound(SorceryFightSounds.CommonFire, Main.projectile[i].Center);

                Vector2 dir = Main.projectile[i].velocity.SafeNormalize(Vector2.UnitX);
                dir *= 10;

                for (int j = 0; j < 8; j++)
                {
                    Vector2 variation = new Vector2(Main.rand.NextFloat(-7, 2), Main.rand.NextFloat(-7, 2));
                    LinearParticle linearParticle = new LinearParticle(Main.projectile[i].Center, dir + variation, textColor, false, 0.9f, 1f, 30);
                    ParticleController.SpawnParticle(linearParticle);
                }
            }
            
            return i;

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.penetrate = 0;

            for (int i = 0; i < 10; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-7, 7), Main.rand.NextFloat(-7, 7));
                LinearParticle linearParticle = new LinearParticle(Projectile.Center, Projectile.velocity + variation, textColor, false, 0.9f, 1.5f, 30);
                ParticleController.SpawnParticle(linearParticle);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/ReversalRed").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 2f, SpriteEffects.None, 0f);
            return false;
        }
    }
}