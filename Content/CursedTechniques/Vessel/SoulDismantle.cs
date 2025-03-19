using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Vessel
{
    public class SoulDismantle : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 2;
        public static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.SoulDismantle.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.SoulDismantle.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.SoulDismantle.LockedDescription");
        public override float Cost => 30f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => false;
        public override int Damage => 60;
        public override int MasteryDamageMultiplier => 60;
        public override float Speed => 0f;
        public override float LifeTime => 22f;
        List<int> hasHit;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<SoulDismantle>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.SkeletronHead);
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            
            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * Speed;
                var entitySource = player.GetSource_FromThis();
                sf.cursedEnergy -= CalculateTrueCost(sf);

                return Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), 1, 0, player.whoAmI);
            }
            return -1;
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
            hasHit = new List<int>();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (!hasHit.Contains(Projectile.whoAmI))
            {
                SoundEngine.PlaySound(SorceryFightSounds.CleaveSwing with { Volume = 5f }, Projectile.Center);
                modifiers.FinalDamage.Flat = CalculateTrueDamage(Main.player[Projectile.owner].GetModPlayer<SorceryFightPlayer>());
                hasHit.Add(Projectile.whoAmI);
            }
            
            else 
                Projectile.damage = 0;

            base.ModifyHitNPC(target, ref modifiers);
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= LifeTime)
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
            float velocityAngle = Projectile.velocity.ToRotation();
            float offset = 130f * Projectile.scale;

            Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
            Projectile.Center = playerRotatedPoint + velocityAngle.ToRotationVector2() * offset;
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;

            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SorceryFightSounds.CleaveSwing with { Volume = 5f }, player.Center);
                Projectile.ai[1] = Main.rand.NextFloat(0, MathHelper.TwoPi);
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
