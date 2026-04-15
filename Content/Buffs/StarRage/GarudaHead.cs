using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.Buffs.StarRage
{
    public class GarudaHead : ModProjectile, ILocalizedModType
    {

        Dictionary<int, Projectile> segments = new Dictionary<int, Projectile>();
        public override void SetStaticDefaults()
        {

            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.timeLeft = 10;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.localNPCHitCooldown = 30;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 0;
            Projectile.aiStyle = -1;
            Projectile.DamageType = CursedTechniqueDamageClass.Instance;
            Projectile.netImportant = true;
            Projectile.minionSlots = 0;
            Projectile.minion = true;

            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 100;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();
            //player.AddBuff(ModContent.BuffType<KingofConstellationsBuff>(), 1);
            if (Projectile.type == ModContent.ProjectileType<GarudaHead>())
            {
                if (player.dead || sfPlayer.innateTechnique.Name != "StarRage")
                {
                    sfPlayer.summonGaruda = false;
                }
                if (sfPlayer.summonGaruda)
                {
                    Projectile.timeLeft = 2;
                }
            }
            Projectile.ai[0]++;
            // Hover to the left because evil is not right or something
            // What does this mean? ??
            Vector2 idealPos = new Vector2(player.Center.X - 200, player.Center.Y - 50);
            float distanceFromOwner = Projectile.Distance(idealPos);

            if (distanceFromOwner > 3000)
            {
                Projectile.Center = player.Center;
                Projectile.netUpdate = true;
            }

            NPC target = null;

            if (player.HasMinionAttackTargetNPC && player.MinionAttackTargetNPC >= 0 && player.MinionAttackTargetNPC < Main.maxNPCs)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                float extraDistance = (npc.width / 2) + (npc.height / 2);
                if (Vector2.Distance(Projectile.Center, npc.Center) < (2500 + extraDistance))
                    target = npc;
            }
            target ??= ClosestNPCAt(Projectile.Center, 2500, true);

            if (target != null && Projectile.position.Distance(player.position) <= 2500)
            {   
                SorceryFightPlayer sf = player.SorceryFight();
                if (sf.garudaCurrentTarget != null && sf.garudaCurrentTarget.CanBeChasedBy(null, false))
                {
                    //add function to lock down the target
                    RestrainTarget(sf.garudaCurrentTarget);
                    //Main.NewText("Locking on to target: " + sf.garudaCurrentTarget.FullName);
                }
                else
                {
                    AttackTarget(target);
                }
            }
            else
            {
                float hoverAcceleration = 0.2f;
                if (distanceFromOwner < 200f)
                    hoverAcceleration = 0.12f;
                if (distanceFromOwner < 140f)
                    hoverAcceleration = 0.06f;

                if (distanceFromOwner > 100f)
                {
                    if (Math.Abs(player.Center.X - Projectile.Center.X) > 20f)
                        Projectile.velocity.X += hoverAcceleration * Math.Sign(idealPos.X - Projectile.Center.X);
                    if (Math.Abs(player.Center.Y - Projectile.Center.Y) > 10f)
                        Projectile.velocity.Y += hoverAcceleration * Math.Sign(idealPos.Y - Projectile.Center.Y);
                }
                else if (Projectile.velocity.Length() > 1f)
                    Projectile.velocity *= 0.96f;

                if (Math.Abs(Projectile.velocity.Y) < 1f)
                    Projectile.velocity.Y -= 0.1f;

                if (Projectile.velocity.Length() > 25)
                    Projectile.velocity = Vector2.Normalize(Projectile.velocity) * 25;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            segments.Clear();
            foreach (var projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<GarudaBody>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<GarudaBody>().segmentIndex))
                {
                    SorceryFightMod.Log.Info($"Adding body segment with index: {projectile.ModProjectile<GarudaBody>().segmentIndex}, total so far: {segments.Count}");
                    segments.Add(projectile.ModProjectile<GarudaBody>().segmentIndex, projectile);
                }
                if (projectile.type == ModContent.ProjectileType<GarudaTail>() && projectile.owner == Projectile.owner && projectile.active && !segments.ContainsKey(projectile.ModProjectile<GarudaTail>().segmentIndex))
                {
                    SorceryFightMod.Log.Info($"Adding tail segment with index: {projectile.ModProjectile<GarudaTail>().segmentIndex}");
                    segments.Add(projectile.ModProjectile<GarudaTail>().segmentIndex, projectile);
                }
            }
            for (var i = 1; i <= segments.Count; i++)
            {
                if (i < segments.Count)
                {
                    SorceryFightMod.Log.Info("This is what segment is moving:" + i);
                    if (segments.ContainsKey(i))
                        segments[i].ModProjectile<GarudaBody>().SegmentMove();
                }
                else
                {
                    if (segments.ContainsKey(i))
                        segments[i].ModProjectile<GarudaTail>().SegmentMove();
                }
            }
        }

        public static NPC ClosestNPCAt(Vector2 origin, float maxDistanceToCheck, bool ignoreTiles = true, bool bossPriority = false)
        {
            NPC closestTarget = null;
            float distance = maxDistanceToCheck;
            if (bossPriority)
            {
                bool bossFound = false;
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    // If we've found a valid boss target, ignore ALL targets which aren't bosses.
                    if (bossFound && !(Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye))
                        continue;

                    if (Main.npc[index].CanBeChasedBy(null, false))
                    {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance && canHit)
                        {
                            if (Main.npc[index].boss || Main.npc[index].type == NPCID.WallofFleshEye)
                                bossFound = true;

                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            else
            {
                for (int index = 0; index < Main.npc.Length; index++)
                {
                    if (Main.npc[index].CanBeChasedBy(null, false))
                    {
                        float extraDistance = (Main.npc[index].width / 2) + (Main.npc[index].height / 2);

                        bool canHit = true;
                        if (extraDistance < distance && !ignoreTiles)
                            canHit = Collision.CanHit(origin, 1, 1, Main.npc[index].Center, 1, 1);

                        if (Vector2.Distance(origin, Main.npc[index].Center) < distance && canHit)
                        {
                            distance = Vector2.Distance(origin, Main.npc[index].Center);
                            closestTarget = Main.npc[index];
                        }
                    }
                }
            }
            return closestTarget;
        }

        internal void AttackTarget(NPC target)
        {

            float idealFlyAcceleration = 0.18f;

            Vector2 destination = target.Center;
            float distanceFromDestination = Projectile.Distance(destination);

            // Get a swerve effect if somewhat far from the target.
            if (Projectile.Distance(destination) > 400f)
            {
                Projectile.ai[2] = 0;
                destination += (Projectile.ai[0] % 30f / 30f * MathHelper.TwoPi).ToRotationVector2() * 145f;
                distanceFromDestination = Projectile.Distance(destination);
                idealFlyAcceleration *= 2.5f;
            }

            // Charge if the target is far away.
            if (distanceFromDestination > 2500f)
                idealFlyAcceleration = MathHelper.Min(6f, Projectile.ai[1] + 1f);

            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], idealFlyAcceleration, 0.3f);

            float directionToTargetOrthogonality = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), (destination - Projectile.Center).SafeNormalize(Vector2.Zero));

            // Fly towards the target if it's far.
            if (distanceFromDestination > 320f)
            {
                float speed = Projectile.velocity.Length();
                if (speed < 23f)
                    speed += 0.08f;

                if (speed > 32f)
                    speed -= 0.08f;

                // Go faster if the line of sight is aiming closely at the target.
                if (directionToTargetOrthogonality < 0.85f && directionToTargetOrthogonality > 0.5f)
                    speed += 16f;

                // And go slower otherwise so that the dragon can angle towards the target more accurately.
                if (directionToTargetOrthogonality < 0.5f && directionToTargetOrthogonality > -0.7f)
                    speed -= 16f;

                speed = MathHelper.Clamp(speed, 16f, 34f);

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(destination), Projectile.ai[1]).ToRotationVector2() * speed;

                // failsafe to prevent orbiting
                Projectile.ai[2]++;
                if (Projectile.ai[2] >= 90)
                {
                    Projectile.velocity = Projectile.DirectionTo(destination) * 30;
                    Projectile.ai[2] = 0;
                }
            }

            if (distanceFromDestination > 320f)
            {
                float speed = Projectile.velocity.Length();
                if (speed < 23f)
                    speed += 0.08f;

                if (speed > 32f)
                    speed -= 0.08f;

                // Go faster if the line of sight is aiming closely at the target.
                if (directionToTargetOrthogonality < 0.85f && directionToTargetOrthogonality > 0.5f)
                    speed += 16f;

                // And go slower otherwise so that the dragon can angle towards the target more accurately.
                if (directionToTargetOrthogonality < 0.5f && directionToTargetOrthogonality > -0.7f)
                    speed -= 16f;

                speed = MathHelper.Clamp(speed, 16f, 34f);

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(destination), Projectile.ai[1]).ToRotationVector2() * speed;

                // failsafe to prevent orbiting
                Projectile.ai[2]++;
                if (Projectile.ai[2] >= 90)
                {
                    Projectile.velocity = Projectile.DirectionTo(destination) * 30;
                    Projectile.ai[2] = 0;
                }

            }

        }

        internal void RestrainTarget(NPC target)
        {

            float idealFlyAcceleration = 0.18f;

            Vector2 destination = target.Center;
            float distanceFromDestination = Projectile.Distance(destination);

            if (distanceFromDestination > 1000f)
                //add a check for extreme scaling of the height and width here
            {
                AttackTarget(target);
                return;
            }

            Main.NewText("Distance From Target: " + distanceFromDestination);

            Main.NewText(target.height);
            Main.NewText(target.width);

            //These swerve is what makes Garuda turn around, we want it to turn faster the smaller the create it's constricitng is, so lower distance 
            //Also lower the distance on 

            //ideally we want our base lower value here to be 100f

            if (Projectile.Distance(destination) > 100f)
            {
                Projectile.ai[2] = 0;
                destination += (Projectile.ai[0] % 30f / 30f * MathHelper.TwoPi).ToRotationVector2() * 145f;
                distanceFromDestination = Projectile.Distance(destination);
                idealFlyAcceleration *= 2.5f;
            }

            Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], idealFlyAcceleration, 0.3f);

            float directionToTargetOrthogonality = Vector2.Dot(Projectile.velocity.SafeNormalize(Vector2.Zero), (destination - Projectile.Center).SafeNormalize(Vector2.Zero));

            // Fly towards the target if it's far.
            // Our base value here should be about 300f
            if (distanceFromDestination > 150f)
            {
                float speed = Projectile.velocity.Length();
                if (speed < 23f)
                    speed += 0.08f;

                if (speed > 32f)
                    speed -= 0.08f;

                // Go faster if the line of sight is aiming closely at the target.
                if (directionToTargetOrthogonality < 0.85f && directionToTargetOrthogonality > 0.5f)
                    speed += 16f;

                // And go slower otherwise so that the dragon can angle towards the target more accurately.
                if (directionToTargetOrthogonality < 0.5f && directionToTargetOrthogonality > -0.7f)
                    speed -= 16f;

                speed = MathHelper.Clamp(speed, 16f, 34f);

                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(Projectile.AngleTo(destination), Projectile.ai[1]).ToRotationVector2() * speed;

                // failsafe to prevent orbiting
                //lower this because we want orbit
                Projectile.ai[2]++;
                Main.NewText("Orbit timer: " + Projectile.ai[2]);
                if (Projectile.ai[2] >= 90)
                {
                    Projectile.velocity = Projectile.DirectionTo(destination) * 30;
                    Projectile.ai[2] = 0;
                }
            }
        }


        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Terraria.GameContent.TextureAssets.Projectile[Type].Value;
            Texture2D texBody = ModContent.Request<Texture2D>("sorceryFight/Content/Buffs/StarRage/GarudaBody").Value;
            Texture2D texBody2 = ModContent.Request<Texture2D>("sorceryFight/Content/Buffs/StarRage/GarudaBody2").Value;
            Texture2D texTail = ModContent.Request<Texture2D>("sorceryFight/Content/Buffs/StarRage/GarudaTail").Value;
            Texture2D texTail2 = ModContent.Request<Texture2D>("sorceryFight/Content/Buffs/StarRage/GarudaTail2").Value;
            for (var i = segments.Count; i > 0; i--)
            {
                if (segments.ContainsKey(i))
                {
                    SpriteEffects fx = Math.Abs(segments[i].rotation) > MathHelper.PiOver2 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    if (i < segments.Count - 1)
                    {
                        Main.EntitySpriteDraw((i % 2 != 0 && i < segments.Count) ? texBody2 : texBody, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor), segments[i].rotation + MathHelper.Pi / 2f, texBody.Size() / 2f, segments[i].scale, fx, 0);
                    }
                    else if (i < segments.Count)
                    {
                        Main.EntitySpriteDraw(texTail, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor), segments[i].rotation + MathHelper.Pi / 2f, texBody.Size() / 2f, segments[i].scale, fx, 0);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(texTail2, segments[i].Center - Main.screenPosition, null, segments[i].GetAlpha(lightColor), segments[i].rotation + MathHelper.Pi / 2f, texTail.Size() / 2f, segments[i].scale, fx, 0);

                    }
                }
            }
            Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation + MathHelper.Pi / 2f, tex.Size() / 2f, Projectile.scale, Projectile.velocity.X > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
            return false;

        }

        public override bool MinionContactDamage() => true;

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) => target.AddBuff(BuffID.ShadowFlame, 300);
    }
}
