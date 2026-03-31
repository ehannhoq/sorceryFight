using System;
using System.Collections.Generic;
using System.Composition.Hosting.Core;
using System.Security.Cryptography.X509Certificates;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.Providence;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.Content.Items.Accessories;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class WorldCuttingSlash : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;
        static List<string> incantations;
        static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.WorldCuttingSlash.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.WorldCuttingSlash.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.WorldCuttingSlash.LockedDescription");
        public override float Cost => 1225f;
        public override Color textColor => new Color(245, 214, 208);
        public override bool DisplayNameInGame => false;
        public override int Damage => 20000;
        public override int MasteryDamageMultiplier => 525;
        public override float Speed => 60f;
        public override float LifeTime => 300f;
        ref float castTime => ref Projectile.ai[0];
        ref float totalCastTime => ref Projectile.localAI[0];
        ref float multiplier => ref Projectile.localAI[1];
        ref float slashed => ref Projectile.localAI[2];
        private const float INCANTATION_TIME = 90.0f;
        private const float BUFFER_TIME = 30.0f;
        private const float SHADER_TIME = 120.0f;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<WorldCuttingSlash>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>()) || sf.Player.HasBuff(ModContent.BuffType<KingOfCursesBuff>());
        }

        public override void SetStaticDefaults()
        {
            incantations = new List<string>()
            {
                "Dragon Scales.",
                "Repulsion.",
                "Paired Falling Stars."
            };

            if (Main.dedServ) return;
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/WorldCuttingSlash", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.Hitbox = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, 1, 1);
            Projectile.scale = 0.0f;

            Player player = Main.player[Projectile.owner];
            SorceryFightPlayer sfPlayer = player.SorceryFight();

            sfPlayer.disableRegenFromProjectiles = true;

            multiplier = sfPlayer.cursedOfuda ? CursedOfuda.cursedTechniqueCastTimeDecrease : 1.0f;
            totalCastTime = (incantations.Count * INCANTATION_TIME + SHADER_TIME + BUFFER_TIME) * multiplier;
        }

        public override void AI()
        {
            castTime++;
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.Center;

            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.velocity = (Main.MouseWorld - Projectile.Center).SafeNormalize(Vector2.UnitX);
            }

            float incantationTime = INCANTATION_TIME * incantations.Count * multiplier;
            if (castTime == 1)
            {
                player.SorceryFight().sfUI.InitializeChant(incantations, (int)(INCANTATION_TIME * multiplier), (int)BUFFER_TIME, new UI.Chants.ChantTextStyle(
                    textColor: Color.Black,
                    text2Color: new Color(41, 11, 9, 255),
                    borderWidth: 2.0f,
                    borderColor: Color.White,
                    border2Color: new Color(230, 206, 179, 255),
                    glowRadius: 3.0f,
                    glowColor: new Color(245, 209, 196, 255)
                ));
            }

            if (castTime <= incantationTime + BUFFER_TIME)
            {
                return;
            }

            if (castTime < totalCastTime)
            {
                if (slashed == 0.0f)
                {
                    Projectile.netUpdate = true;
                    CameraController.CameraShake(15, 10, 10);
                    ImpactFrameController.ImpactFrame(Color.White, 2);
                    player.AddBuff(ModContent.BuffType<BurntTechnique>(), SFUtils.BuffSecondsToTicks(5.0f));
                    player.SorceryFight().disableRegenFromProjectiles = false;
                    slashed = 1f;
                    SoundEngine.PlaySound(SorceryFightSounds.WorldCuttingSlash, Projectile.Center);
                }

                float percent = (castTime - totalCastTime) / (totalCastTime - incantationTime) + 1;
                float progress = MathF.Pow((percent * 2) - 1, 3);
                progress = 1 - Math.Clamp(progress, 0.0f, 1.0f);

                if (!Filters.Scene["SF:WorldCuttingSlash"].Active)
                    Filters.Scene.Activate("SF:WorldCuttingSlash").GetShader().UseTargetPosition(Projectile.Center).UseDirection(Projectile.velocity).UseOpacity(1.0f);

                Filters.Scene["SF:WorldCuttingSlash"].GetShader().UseProgress(progress);

                return;
            }


            Filters.Scene["SF:WorldCuttingSlash"].GetShader().UseOpacity(0.0f);
            Filters.Scene.Deactivate("SF:WorldCuttingSlash");
            Projectile.Kill();
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.Defense *= 0.0f;
            modifiers.DefenseEffectiveness *= 0.0f;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (slashed == 1f)
            {
                float useless = 0.0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * 2000.0f, 10.0f, ref useless))
                    return true;
            }

            return false;
        }
    }
}
