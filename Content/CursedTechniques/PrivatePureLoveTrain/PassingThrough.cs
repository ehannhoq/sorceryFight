using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.Content.VFX;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using sorceryFight.Utilities.EaseFunctions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.PrivatePureLoveTrain
{
    public class PassingThrough : CursedTechnique
    {
        public static Texture2D texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/PrivatePureLoveTrain/PassingThrough", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public override string InternalName => "PassingThrough";

        public PassingThrough()
        {
            Technique.baseDamage = 60;
            Technique.damagePerBoss = 17;
            Technique.cost = 50;
            Technique.speed = 30f;
            Technique.lifetime = 90;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                var entitySource = player.GetSource_FromThis();

                Vector2 mousePos = Main.MouseWorld;
                Vector2 posOffset = Vector2.UnitX * 750f;
                posOffset = posOffset.RotatedByRandom(2 * MathF.PI);
                Vector2 pos = mousePos + posOffset;

                Vector2 dir = pos.DirectionTo(mousePos);

                SoundEngine.PlaySound(SorceryFightSounds.CommonWoosh, pos);

                sf.disableRegenFromProjectiles = true;

                return Projectile.NewProjectile(entitySource, pos, dir, GetProjectileType(), (int)CalculateTrueDamage(sf), 4f, player.whoAmI);
            }
            return -1;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();

            Projectile.width = 300;
            Projectile.height = 175;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.rotation = Projectile.velocity.ToRotation();

        }


        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitX) * (speed * EaseFunctions.EaseInExponential(power: 2, x: (Technique.lifetime - Projectile.timeLeft) / 30f) + 0.1f);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, sourceRectangle.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            VFXManager.AddVFX(new ImpactCircleVFX(
                center: target.Center,
                lifetime: 30,
                scale: 1.5f
            ));

            for (int i = 0; i < 3; i++)
            {
                StarParticle particle = new StarParticle(
                    position: target.Center + Main.rand.NextVector2Circular(target.width * 0.75f, target.height * 0.75f),
                    velocity: Vector2.Zero,
                    color: Color.White,
                    changeOpacity: true,
                    lifetime: 15,
                    scale: 1f
                );
                ParticleController.SpawnParticle(particle);
            }
        }

        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            player.SorceryFight().disableRegenFromProjectiles = false;
        }
    }

}
