using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.SFPlayer;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class SelfBloodBlade : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 16;
        public static readonly int TICKS_PER_FRAME = 1;
        public static Texture2D texture;

        public override string InternalName => "SelfBloodBlade";


        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                var entitySource = player.GetSource_FromThis();
                return Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, GetProjectileType(), CalculateTrueDamage(sf), 0, player.whoAmI);
            }
            return -1;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 188;
            Projectile.height = 188;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }


        public override void AI()
        {
            Projectile.ai[0]++;

            if (Main.myPlayer == Projectile.owner && SFKeybinds.UseTechnique.Current)
            {
                Projectile.ai[0]--;

            }
                if (Projectile.ai[0] >= lifetime)
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


            Player player = Main.player[Projectile.owner];
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);

            Vector2 aimDirection = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
            float aimAngle = aimDirection.ToRotation();

            Projectile.velocity = aimDirection;
            Projectile.direction = (Math.Cos(aimAngle) > 0).ToDirectionInt();
            Projectile.rotation = aimAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            Projectile.Center = playerRotatedPoint + aimDirection * 120f;

            player.ChangeDir(Projectile.direction);

            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SorceryFightSounds.CleaveSwing with { Volume = 3f }, player.Center);
            }

        }



        public override bool PreDraw(ref Color lightColor)
        {
            //int frameHeight = texture.Height / FRAME_COUNT;
            //int frameY = Projectile.frame * frameHeight;

            //Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            //Vector2 projOrigin = sourceRectangle.Size() * 0.5f;
            //SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            //Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, -32).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation, projOrigin, animScale, spriteEffects, 0f);
            //return false;

            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null && !Main.dedServ)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/BloodManipulation/SelfBloodBlade", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Player player = Main.player[Projectile.owner];
            SpriteEffects effects = player.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 2f, effects, 0f);

            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            int paintingCount = Main.player[Projectile.owner].SorceryFight().deathPaintings.Count(p => p);
            target.AddBuff(ModContent.BuffType<BloodPoison>(), paintingCount * 60);
        }

    }
}
