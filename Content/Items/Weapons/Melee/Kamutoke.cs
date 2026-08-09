using System;
using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Projectiles;
using sorceryFight.Utilities.EaseFunctions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.RGB;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.Items.Weapons.Melee
{
    public class Kamutoke : ModItem
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.Kamutoke.DisplayName");

        public override LocalizedText Tooltip => SFUtils.GetLocalization("Mods.sorceryFight.Weapons.Melee.Kamutoke.Tooltip");

        public override void SetStaticDefaults()
        {
            ItemID.Sets.Spears[Item.type] = true;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 29;
            Item.height = 29;
            Item.maxStack = 1;
            Item.useTime = 10;
            Item.shootSpeed = 10f;
            Item.useAnimation = 10;
            Item.damage = 130;
            Item.crit = 11;
            Item.knockBack = 15;
            Item.autoReuse = true;
            Item.DamageType = CursedTechniqueDamageClass.Instance;
            Item.shoot = ModContent.ProjectileType<KamutokeProjectile>();
            Item.useStyle = ItemUseStyleID.Rapier;
            Item.UseSound = SoundID.Item7;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.useTurn = true;
            Item.channel = true;
        }

        public override bool CanUseItem(Player player)
        {
            if (player.ownedProjectileCounts[Item.shoot] > 0)
                return false;
            return true;
        }

        public override bool AltFunctionUse(Player player) => true;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, player.Center, velocity, type, damage, knockback, player.whoAmI, ai0: player.altFunctionUse == 2 ? 1f : 0f);
            return false;
        }
    }

    public class KamutokeProjectile : ModProjectile
    {
        public override string Texture => "sorceryFight/Content/Items/Weapons/Melee/Kamutoke";

        private bool altUse => Projectile.ai[0] == 1f;
        private ref float spawnedProj => ref Projectile.ai[1];
        private const float TOP_ROTATION = -5f * MathF.PI / 12f;
        private const float BOTTOM_ROTATION = -TOP_ROTATION;

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 15;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.timeLeft = 300;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.ownerHitCheck = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.scale = 2f;
            Projectile.timeLeft = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.direction = (Math.Cos(Projectile.velocity.ToRotation()) > 0).ToDirectionInt();
            player.ChangeDir(Projectile.direction);

            if (!altUse)
            {
                if (Main.myPlayer == Projectile.owner)
                {   
                    Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX); 
                    Projectile.rotation = direction.ToRotation();
                    Projectile.netUpdate = true;    
                }
            }
            else
            {
                Projectile.rotation = TOP_ROTATION * Projectile.direction;
                Projectile.Center = player.Center + (Vector2.UnitX * 10f).RotatedBy(Projectile.rotation);
                Projectile.timeLeft = 60;
            }

            spawnedProj = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!altUse)
            {
                Vector2 offset = Vector2.UnitX * EaseFunctions.EaseInOutZero(1 - (Projectile.timeLeft / 9f)) * 30f;
                offset = offset.RotatedBy(Projectile.rotation);
                Projectile.Center = player.Center + offset;
                return;
            }


            float progress = EaseFunctions.EaseInExponential(7f, 1 - ((Projectile.timeLeft - 2) / 59f));
            float angle = TOP_ROTATION.AngleLerp(BOTTOM_ROTATION, progress);
            Projectile.rotation = Projectile.direction == 1 ? angle : MathHelper.Pi - angle;
            Projectile.Center = player.Center + (Vector2.UnitX * 50f).RotatedBy(Projectile.rotation);

            if (Main.myPlayer == Projectile.owner && MathF.Round(progress, 1) >= 0.8 && spawnedProj == -1)
            {
                Vector2? closestNPCPos = null;
                float closetDistance = 100f;
                foreach (NPC npc in Main.ActiveNPCs)
                {
                    float distance = npc.Center.Distance(Main.MouseWorld);
                    if (distance > KamutokeLightning.MINIMUM_NPC_DISTANCE) continue;

                    if (distance < closetDistance)
                    {
                        closestNPCPos = npc.Center + new Vector2(0f, npc.height / 2f);
                        closetDistance = distance;
                    }
                }
                Vector2 lightningPos = closestNPCPos ?? Main.MouseWorld;

                spawnedProj = Projectile.NewProjectile(player.GetSource_FromThis(), lightningPos, Vector2.Zero, ModContent.ProjectileType<KamutokeLightning>(), 250, 4, Projectile.owner, ai0: KamutokeLightning.STARTING_RECURSIVE_LIGHTING_COUNT);
            }

            float armRotation = MathHelper.Lerp(-MathHelper.PiOver2, MathHelper.PiOver2, progress);
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Quarter, (armRotation - MathHelper.PiOver2) * player.direction);
            player.itemRotation = armRotation;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!altUse)
            {
                Vector2 center = Projectile.Center;
                Vector2 offset = (Vector2.UnitX * (Projectile.width / 2f)).RotatedBy(Projectile.rotation);

                Vector2 start = center - offset;
                Vector2 end = center + offset;

                float _ = 0f;
                return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, Projectile.height, ref _);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Rectangle src = new Rectangle(0, 0, texture.Width, texture.Height);

            SpriteEffects flip = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, src, Color.White, Projectile.rotation + MathHelper.PiOver4 * Projectile.direction, src.Size() * 0.5f, Projectile.scale, flip);
            return false;
        }
    }
}
