using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class CursedSpear : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 29;
            Item.maxStack = 1;
            Item.useTime = 10;
            Item.useAnimation = 10;
            Item.shootSpeed = 10;
            Item.damage = 56;
            Item.crit = 11;
            Item.knockBack = 8;
            Item.autoReuse = true;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.shoot = ModContent.ProjectileType<CursedSpearProjectile>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item7;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            return true;
        }
    }

    public class CursedSpearProjectile : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/Items/Weapons/Melee/CursedSpear";

        ref float SwingPhase => ref Projectile.ai[0];
        ref float FinalRotation => ref Projectile.ai[1];
        ref float Tick => ref Projectile.ai[2];
        ref float PhaseOneTarget => ref Projectile.localAI[1];
        ref float PhaseTwoTarget => ref Projectile.localAI[2];

        public override void SetDefaults()
        {
            Projectile.width = 140;
            Projectile.height = 20;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.direction = (Math.Cos(Projectile.velocity.ToRotation()) > 0).ToDirectionInt();
            player.ChangeDir(Projectile.direction);

            if (Main.myPlayer == Projectile.owner)
            {
                Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
                Projectile.rotation = direction.ToRotation();
                Projectile.netUpdate = true;
            }

            PhaseOneTarget = Projectile.rotation + (MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.direction;
            PhaseTwoTarget = Projectile.rotation - (MathHelper.PiOver2 + MathHelper.PiOver4) * Projectile.direction;
            Projectile.rotation = PhaseTwoTarget;

            Tick = 1f;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.MountedCenter + (Vector2.UnitX * 20).RotatedBy(Projectile.rotation);

            if (SwingPhase == 0.0)
            {
                Projectile.rotation = float.Lerp(Projectile.rotation, PhaseOneTarget, 0.20f);
                if (Math.Abs(Projectile.rotation - PhaseOneTarget) < 0.01)
                {
                    SwingPhase = 1.0f;
                }
            }

            if (SwingPhase == 1.0)
            {
                Projectile.rotation = float.Lerp(Projectile.rotation, PhaseTwoTarget, 0.20f);
                if (Math.Abs(Projectile.rotation - PhaseTwoTarget) < 0.01)
                {
                    SwingPhase = 2.0f;

                    if (Main.myPlayer == Projectile.owner)
                    {
                        Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
                        FinalRotation = direction.ToRotation();
                        Projectile.netUpdate = true;
                        return;
                    }
                }
            }

            if (SwingPhase == 2.0)
            {
                Tick *= 0.5f;

                Vector2 target = player.MountedCenter + (Vector2.UnitX * 100).RotatedBy(FinalRotation);
                Projectile.rotation = FinalRotation;
                Projectile.Center = Vector2.Lerp(Projectile.Center, target, 1 - Tick);

                if ((Projectile.Center - target).Length() < 0.01f)
                {
                    Projectile.Kill();
                }
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 center = Projectile.Center;
            Vector2 offset = (Vector2.UnitX * (Projectile.width / 2f)).RotatedBy(Projectile.rotation);

            Vector2 start = center - offset;
            Vector2 end = center + offset;

            float _ = 0f;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.height, ref _);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);
            SpriteEffects flip = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.rotation + MathHelper.PiOver4 * Projectile.direction, src.Size() * 0.5f, 1f, flip);
            return false;
        }

    }
}
