using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.StarRage
{
    public class GarudaBody : ModProjectile
    {
        public int segmentIndex = 1;

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 3000;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.aiStyle = -1;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.minion = true;

        }

        public override void OnSpawn(IEntitySource source)
        {
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<GarudaTail>() && projectile.owner == Projectile.owner && projectile.active)
                {
                    segmentIndex = projectile.ModProjectile<GarudaTail>().segmentIndex;
                    projectile.ModProjectile<GarudaTail>().segmentIndex++;
                }
            }
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            if (Projectile.owner == Main.myPlayer)
            {
                if (sfPlayer.summonGaruda)
                    Projectile.timeLeft = 2;
            }
            else
            {
                Projectile.timeLeft = 2;
            }
        }

        internal void SegmentMove()
        {
            Player player = Main.player[Projectile.owner];
            var live = false;
            Projectile nextSegment = new Projectile();
            GarudaHead head = new GarudaHead();
            foreach (Projectile projectile in Main.ActiveProjectiles)
            {
                if (projectile.type == Type && projectile.owner == Projectile.owner)
                {
                    if (projectile.ModProjectile<GarudaBody>().segmentIndex == segmentIndex - 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                }
                if (projectile.type == ModContent.ProjectileType<GarudaHead>() && projectile.owner == Projectile.owner)
                {
                    if (segmentIndex == 1)
                    {
                        live = true;
                        nextSegment = projectile;
                    }
                    head = projectile.ModProjectile<GarudaHead>();
                }
            }
            if (!live) Projectile.Kill();
            Vector2 destinationOffset = nextSegment.Center + nextSegment.velocity - Projectile.Center;
            if (nextSegment.rotation != Projectile.rotation)
            {
                float angle = MathHelper.WrapAngle(nextSegment.rotation - Projectile.rotation);
                //destinationOffset = Projectile.Center.DirectionTo(nextSegment.Center);
                destinationOffset = destinationOffset.RotatedBy(angle * 0.1f);
            }
            Projectile.rotation = destinationOffset.ToRotation();
            if (destinationOffset != Vector2.Zero)
            {
                Projectile.Center = nextSegment.Center + nextSegment.velocity - destinationOffset.SafeNormalize(Vector2.Zero) * 20f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.player[Projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<GarudaHead>()] > 0)
            {
                foreach (Projectile projectile in Main.ActiveProjectiles)
                {
                    if (projectile.type == ModContent.ProjectileType<GarudaHead>() && projectile.owner == Projectile.owner)
                    {
                        projectile.Kill();
                    }
                }
            }
        }

        //public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 300);
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(segmentIndex);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            segmentIndex = reader.ReadInt32();
        }

    }
}
