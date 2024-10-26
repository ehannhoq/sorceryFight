using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Limitless
{
    public class MaximumOutputBlue : CursedTechnique
    {

        public static readonly int FRAME_COUNT = 8; 
        public static readonly int TICKS_PER_FRAME = 5;

        public override string Name { get; set; } = "Maximum Cursed Energy Output: Blue";
        public override string Description
        { 
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.MaximumOutputBlue.Description").Value;
            }
        }
        public override string LockedDescription
        {
            get
            {
                return Language.GetText("Mods.sorceryFight.CursedTechniques.MaximumOutputBlue.LockedDescription").Value;
            }
        }
        public override float Cost { get; set; } = 150f;
        public override float CostPercentage { get; set; } = -1f;
        public override float MasteryNeeded { get; set; } = 0f;
        public override Color textColor { get; set; } = new Color(108, 158, 240);

        public override int Damage { get; set ; } = 100;
        public override float Speed { get; set; } = 20f;
        public override float LifeTime { get; set; } = 300f;
        public override bool Unlocked
        {
            get
            {
                return NPC.downedMechBossAny;
            }
        }
        public virtual float AttractionRadius { get; set; } = 130f;
        public virtual float AttractionStrength { get; set; } = 18f;
        public NPC lockOnTarget;

        public static Texture2D texture;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;
        }

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<MaximumOutputBlue>();
        } 

        public override float GetProjectileSpeed()
        {
            return Speed;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.tileCollide = true;
            Projectile.penetrate = 10;
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

            // TODO:
            // For some reason the second if statement (distance <= AttractionRadius) is never
            // satisified.
                
            // foreach (Projectile proj in Main.projectile)
            // {
            //     if (!proj.hostile && proj != Main.projectile[Projectile.whoAmI])
            //     {
            //         float distance = Vector2.Distance(proj.Center, Projectile.Center);
                    
            //         if (distance <= AttractionRadius)
            //         {
            //             Vector2 direction = proj.Center.DirectionTo(Projectile.Center);
            //             Vector2 newVelocity = Vector2.Lerp(proj.velocity, direction * AttractionStrength, 0.1f);

            //             proj.velocity = newVelocity;
            //         }
            //     }
            // }

            foreach (NPC npc in Main.npc)
            {
                if (!npc.friendly && npc.type != NPCID.TargetDummy)
                {
                    float distance = Vector2.Distance(npc.Center, Projectile.Center);
                    if (distance <= AttractionRadius)
                    {
                        Vector2 direction = npc.Center.DirectionTo(Projectile.Center);
                        Vector2 newVelocity = Vector2.Lerp(npc.velocity, direction * AttractionStrength, 0.1f);

                        npc.velocity = newVelocity;
                    }
                }
            }

            Vector2 projDirection = Projectile.Center.DirectionTo(Main.MouseWorld);
            Vector2 projVelocity = Vector2.Lerp(Projectile.velocity, projDirection * 30f, 0.1f);
            Projectile.velocity = projVelocity;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Limitless/MaximumOutputBlue").Value;


            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 0.5f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            lockOnTarget = target; 
        }

        public override bool Shoot(Terraria.DataStructures.IEntitySource spawnSource, Vector2 position, Vector2 velocity, Player player)
        {
            if (base.Shoot(spawnSource, position, velocity, player))
                Projectile.NewProjectile(spawnSource, position, velocity, ModContent.ProjectileType<MaximumOutputBlue>(), Damage, 0f, player.whoAmI);
            return true;
        }
    }
}