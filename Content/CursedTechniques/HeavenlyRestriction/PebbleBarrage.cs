using System;
using CalamityMod.Items.Weapons.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Steamworks;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.HeavenlyRestriction
{
    public class PebbleBarrage : CursedTechnique
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.PebbleBarrage.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PebbleBarrage.Description");

        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.PebbleBarrage.LockedDescription");

        public override float Cost => 30f;

        public override Color textColor => Color.White;

        public override bool DisplayNameInGame => false;

        public override int Damage => 50;

        public override int MasteryDamageMultiplier => 50;

        public override float Speed => 30f;

        public override float LifeTime => 300;

        private static Texture2D texture;
        private static Texture2D impactTexture;
        ref float tick => ref Projectile.ai[0];
        ref float scale => ref Projectile.ai[1];
        private Vector2 ownerFreezePos = Vector2.Zero;
        private Vector2 impactPos = Vector2.Zero;
        private int ownerDirection = 0;

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<PebbleBarrage>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override void SetStaticDefaults()
        {
            texture = ModContent.Request<Texture2D>(Texture, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            impactTexture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/HeavenlyRestriction/ImpactRing", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            
            if (Main.myPlayer == Projectile.owner)
            {
                CameraController.CameraShake(20, 15f, 5f);
                player.direction = Main.MouseScreen.X > Main.screenWidth / 2f ? 1 : -1;
            }
            
            ownerFreezePos = player.Center;
            ownerDirection = player.direction;
            Projectile.Center = player.Center + new Vector2(0f, player.height * 1.5f) + new Vector2(35 * player.direction, 0f);
            Projectile.velocity = Vector2.Zero;

            // SoundEngine.PlaySound(SorceryFightSounds.GroundKick, player.Center);
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (tick > LifeTime)
                Projectile.Kill();

            if (tick < 30)
            {
                FreezePlayer(player);

                Vector2 targetPos = player.Center + new Vector2(35 * player.direction, -5f);
                Projectile.Center = Vector2.Lerp(Projectile.Center, targetPos, 0.25f);

                scale = MathF.Sqrt(1f - MathF.Pow((tick / 30) - 1, 2));

                Projectile.rotation += MathHelper.ToRadians(5f) * player.direction;

                player.SorceryFight().disableRegenFromProjectiles = true;
            }
            else if (tick == 30)
            {
                if (Main.myPlayer == player.whoAmI)
                {
                    Projectile.RotateVelocityTowardsCursor();
                    Projectile.velocity *= Speed;
                    Projectile.netUpdate = true;
                }

                Projectile.tileCollide = true;
                impactPos = Projectile.Center;
                // SoundEngine.PlaySound(SorceryFightSounds.PunchImpact, player.Center);
                player.SorceryFight().disableRegenFromProjectiles = false;
            }
            else
                Projectile.rotation = Projectile.velocity.ToRotation();

            if (impactPos != Vector2.Zero)
            {
                impactPos -= Projectile.velocity * 0.1f;
            }

            tick++;
        }


        private void FreezePlayer(Player player)
        {
            if (ownerFreezePos != Vector2.Zero)
                player.Center = ownerFreezePos;

            if (ownerDirection != 0)
                player.direction = ownerDirection;

            player.velocity = Vector2.Zero;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.rotation, src.Size() * 0.5f, scale, SpriteEffects.None);

            if (tick > 30)
            {
                Rectangle impactSrc = new Rectangle(0, 0, impactTexture.Width, impactTexture.Height);

                float impactTick = tick - 30;
                float impactScale = MathF.Sqrt(1 - MathF.Pow((impactTick / 60) - 1, 2)) * 2f;
                float impactOpacity = MathF.Sqrt(1 - MathF.Pow(impactTick / 60, 2));

                impactOpacity = Math.Clamp(impactOpacity, 0f, 1f);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(
                    SpriteSortMode.Immediate,
                    BlendState.NonPremultiplied,
                    SamplerState.LinearClamp,
                    DepthStencilState.None,
                    RasterizerState.CullNone,
                    null,
                    Main.GameViewMatrix.ZoomMatrix
                );

                Main.EntitySpriteDraw(impactTexture, impactPos - Main.screenPosition, impactSrc, Color.White * impactOpacity, Projectile.velocity.ToRotation(), impactSrc.Size() * 0.5f, impactScale, SpriteEffects.None);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin();
            }

            return false;
        }
    }
}