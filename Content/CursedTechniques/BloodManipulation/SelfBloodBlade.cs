using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.SFPlayer;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.BloodManipulation
{
    public class SelfBloodBlade : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 16;
        public static readonly int TICKS_PER_FRAME = 2;
        public static Texture2D texture;
        
        public override string InternalName => "SelfBloodBlade";

        private float spriteRotation => MathF.Atan2(targetOffsetY, targetOffsetX) + Projectile.frame * MathF.PI / 8f * Main.player[Projectile.owner].direction + MathF.PI;
        private ref float targetOffsetX => ref Projectile.ai[1];
        private ref float targetOffsetY => ref Projectile.ai[2];

        private float BloodCost => Technique.cost / 1f;

        public SelfBloodBlade()
        {
            Technique.baseDamage = 10;
            Technique.damagePerBoss = 7;
            Technique.speed = 20f;
            Technique.cost = 15;
            Technique.lifetime = FRAME_COUNT * TICKS_PER_FRAME;
        }

        public override bool CanUse(SorceryFightPlayer sf)
        {
            return sf.bloodEnergy > BloodCost;
        }

        public override void ApplyCosts(SorceryFightPlayer sfPlayer)
        {
            base.ApplyCosts(sfPlayer);
            sfPlayer.bloodEnergy -= BloodCost;
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            string localizationCategoryKey = "Mods.sorceryFight.Misc.CursedTechniques";
            
            string bloodCost = SFUtils.GetLocalization(localizationCategoryKey + ".BloodCost")
                    .WithFormatArgs((int)BloodCost).Value;

            return base.GetStats(sf) + "\n" + bloodCost;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SorceryFightSounds.CleaveSwing with { Volume = 3f }, Projectile.Center);

            if (Main.myPlayer == Projectile.owner)
            {
                targetOffsetX = Main.MouseWorld.X - Main.LocalPlayer.Center.X;
                targetOffsetY = Main.MouseWorld.Y - Main.LocalPlayer.Center.Y;

                Vector2 direction = new Vector2(targetOffsetX, targetOffsetY);

                Projectile.direction = (Math.Cos(direction.ToRotation()) > 0).ToDirectionInt();
                Main.LocalPlayer.direction = Projectile.direction;

                Projectile.netUpdate = true;
            }
        }


        public override void AI()
        {
            Projectile.HandleProjectileAnimation(FRAME_COUNT, TICKS_PER_FRAME);

            Player player = Main.player[Projectile.owner];
            Vector2 target = player.Center + new Vector2(targetOffsetX, targetOffsetY);
            Vector2 rotationCenter = (player.Center + target) / 2f;

            float progress = 1 - Projectile.timeLeft / (float)Technique.lifetime; 

            float distanceMultiplier = 0.25f * MathF.Cos(4 * MathF.PI * progress) + 0.75f;
            float distance = (player.Center - rotationCenter).Length() * distanceMultiplier;

            Projectile.Center = rotationCenter + Vector2.UnitX.RotatedBy(spriteRotation) * distance;
        }
        
        public override bool PreDraw(ref Color lightColor)
        {
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
