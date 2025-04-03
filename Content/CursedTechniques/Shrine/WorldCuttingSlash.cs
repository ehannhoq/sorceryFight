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
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
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
        public override int Damage => 50000;
        public override int MasteryDamageMultiplier => 550;
        public override float Speed => 60f;
        public override float LifeTime => 300f;
        ref float ai0 => ref Projectile.ai[0];
        ref float ai1 => ref Projectile.ai[1];
        ref float ai2 => ref Projectile.ai[2];
        Rectangle hitbox;
        bool animating;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<WorldCuttingSlash>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
        }

        public override float CalculateTrueDamage(SorceryFightPlayer sf)
        {
            return base.CalculateTrueDamage(sf) * (1 + (0.01f * sf.sukunasFingerConsumed));
        }

        public override float CalculateTrueCost(SorceryFightPlayer sf)
        {
            return base.CalculateTrueCost(sf) * (0.99f * sf.sukunasFingerConsumed);
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
            Projectile.width = 172;
            Projectile.height = 498;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            animating = false;
            hitbox = Projectile.Hitbox;
        }

        public override void AI()
        {
            ai0++;
            Player player = Main.player[Projectile.owner];
            float animTime = incantations.Count * 90f;

            if (ai0 < animTime)
            {
                if (!animating)
                {
                    animating = true;
                    Projectile.damage = 0;
                    Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                }

                Projectile.Center = player.Center;
                Projectile.timeLeft = 30;

                if (ai0 % 90 == 1)
                {
                    int index = CombatText.NewText(player.getRect(), textColor, incantations[(int)ai1]);
                    SoundEngine.PlaySound(SorceryFightSounds.CommonHeartBeat, player.Center);
                    Main.combatText[index].lifeTime = 60;
                    ai1++;

                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 velocity = new Vector2(Main.rand.NextFloat(-15, 15), Main.rand.NextFloat(-15, 15));
                        LineParticle particle = new LineParticle(Projectile.Center, velocity, false, 30, 1, textColor);
                        GeneralParticleHandler.SpawnParticle(particle);
                    }
                }
                
                if (ai0 == animTime - 40)
                {
                    SoundEngine.PlaySound(SorceryFightSounds.CommonWoosh, Projectile.Center);
                }

                return;
            }

            if (animating)
            {
                animating = false;
                Projectile.damage = (int)CalculateTrueDamage(player.GetModPlayer<SorceryFightPlayer>());
                Projectile.Hitbox = hitbox;
                player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                Projectile.timeLeft = (int)LifeTime;
                ai2 = 1f;
                Projectile.Center = player.Center;
                int index = CombatText.NewText(player.getRect(), textColor, "Dismantle");
                Main.combatText[index].lifeTime = 180;
                SoundEngine.PlaySound(SorceryFightSounds.WorldCuttingSlash, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                    player.AddBuff(ModContent.BuffType<BurntTechnique>(), SFUtils.BuffSecondsToTicks(5));

                }
                float velocityRotation = Projectile.velocity.ToRotation();
                Projectile.direction = (Math.Cos(velocityRotation) > 0).ToDirectionInt();
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 10; i++)
            {
                Vector2 variation = new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5));

                LineParticle particle = new LineParticle(target.Center, Projectile.velocity + variation, false, 30, 1, textColor);
                GeneralParticleHandler.SpawnParticle(particle);
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), ai2, SpriteEffects.None, 0f);
            return false;
        }
    }
}
