using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class Cleave : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 8;
        public static readonly int TICKS_PER_FRAME = 2;
        public static Texture2D texture;

        public override string InternalName => "Cleave";

        private float baseDamagePercent = 0.03f;

        private HashSet<int> bossSegmentTracker = new();

        public Cleave()
        {
            Technique.baseDamage = 30;
            Technique.damagePerBoss = 2;
            Technique.cost = 55;
            Technique.speed = 24f;
            Technique.lifetime = FRAME_COUNT * TICKS_PER_FRAME;
        }

        public override string GetStats(SorceryFightPlayer sf)
        {
            return $"Damage: {MathF.Round(CalculateTrueDamage(sf), 2)} + {Math.Round(baseDamagePercent * 100, 2)}% of target's health\n"
                + $"Cost: {Math.Round(CalculateTrueCost(sf), 2)} CE\n";
        }

        public override float CalculateTrueCost(SorceryFightPlayer sf)
        {
            float masteryMultiplier = 1 - (sf.bossesDefeated.Count / 100f);
            float maxCEPenalty = sf.maxCursedEnergy * 0.45f;
            float finalCost = maxCEPenalty * masteryMultiplier;
            finalCost *= 1 - sf.ctCostReduction;
            return finalCost;
        }

        public override int UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;

            if (player.whoAmI == Main.myPlayer)
            {
                Vector2 playerPos = player.MountedCenter;
                Vector2 mousePos = Main.MouseWorld;
                Vector2 dir = (mousePos - playerPos).SafeNormalize(Vector2.Zero) * speed;
                var entitySource = player.GetSource_FromThis();
                sf.cursedEnergy -= CalculateTrueCost(sf);

                return Projectile.NewProjectile(entitySource, player.Center, dir, GetProjectileType(), CalculateTrueDamage(sf), 0, player.whoAmI);
            }
            return -1;
        }


        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = FRAME_COUNT;

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/Cleave", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }


        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 188;
            Projectile.height = 188;
            Projectile.scale = 0.5f;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            SoundEngine.PlaySound(SorceryFightSounds.CleaveSwing with { Volume = 3f }, Main.player[Projectile.owner].Center);
        }

        public override void AI()
        {
            Projectile.HandleProjectileAnimation(FRAME_COUNT, TICKS_PER_FRAME);

            Player player = Main.player[Projectile.owner];
            Vector2 playerRotatedPoint = player.RotatedRelativePoint(player.MountedCenter, true);
            if (Main.myPlayer == Projectile.owner)
            {
                Projectile.velocity = (Main.MouseWorld - playerRotatedPoint).SafeNormalize(Vector2.UnitX * player.direction);
                Projectile.netUpdate = true;
                Projectile.netUpdate2 = false;
            }

            float velocityAngle = Projectile.velocity.ToRotation();
            Projectile.direction = (Math.Cos(velocityAngle) > 0).ToDirectionInt();
            Projectile.rotation = velocityAngle + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
            Projectile.Center = (playerRotatedPoint + new Vector2(0f, 10f)) + velocityAngle.ToRotationVector2() * 45f;
            player.ChangeDir(Projectile.direction);
        }


        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = texture.Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Rectangle sourceRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 projOrigin = sourceRectangle.Size() * 0.5f;
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0, -32).RotatedBy(Projectile.rotation), sourceRectangle, Color.White, Projectile.rotation, projOrigin, Projectile.scale, spriteEffects, 0f);
            return false;
        }
        

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (bossSegmentTracker.Contains(target.type))
            {
                modifiers.FinalDamage *= 0.01f;
                return;
            }
            else if (target.dontCountMe)
                bossSegmentTracker.Add(target.type);

            modifiers.FinalDamage.Flat += target.life * baseDamagePercent;
        }
    }
}
