using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Vessel
{
    public class SoulDismantle : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 2;
        public static Texture2D texture;

        public override string InternalName => "SoulDismantle";
        
        ref float spawnedFromDE => ref Projectile.ai[2];

        public SoulDismantle()
        {
            Technique.baseDamage = 4;
            Technique.damagePerBoss = 4;
            Technique.cost = 20;
            Technique.lifetime = FRAME_COUNT * TICKS_PER_FRAME;
        }


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Vessel/SoulDismantle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 170;
            Projectile.height = 140;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.DefenseEffectiveness *= 0;
            modifiers.FinalDamage.Flat = CalculateTrueDamage(Main.player[Projectile.owner].SorceryFight());
            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void OnSpawn(IEntitySource source)
        {                
            Player player = Main.player[Projectile.owner];
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            float velocityAngle = Projectile.velocity.ToRotation();
            float offset = 130f * Projectile.scale;

            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
                Projectile.netUpdate = true;
            }
            
            Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
        }


        public override void AI()
        {
            Projectile.ai[0]++;

            Projectile.HandleProjectileAnimation(FRAME_COUNT, TICKS_PER_FRAME);

            if (spawnedFromDE == 0)
            {
                Player player = Main.player[Projectile.owner];
                Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
                float velocityAngle = Projectile.velocity.ToRotation();
                float offset = 130f * Projectile.scale;

                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
                    Projectile.netUpdate = true;
                }

                Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
                Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            }

            if (Projectile.ai[0] == 1)
            {
                Projectile.ai[1] = Main.rand.NextFloat(0, MathHelper.TwoPi);
                SoundEngine.PlaySound(SorceryFightSounds.CleaveSwing with { Volume = 5f }, Projectile.Center);
                SoundEngine.PlaySound(SorceryFightSounds.SoulDismantle, Projectile.Center);
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;

            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, -32).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation + Projectile.ai[1], projOrigin, 2f, SpriteEffects.None, 0f);
            return false;
        }
    }
}
