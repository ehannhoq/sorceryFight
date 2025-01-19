using System;
using System.Collections.Generic;
using CalamityMod.NPCs.Providence;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class DivineFlame : CursedTechnique
    {
        public static readonly int FRAME_COUNT = 4;
        public static readonly int TICKS_PER_FRAME = 5;
        static List<Texture2D> textures;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.DivineFlame.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineFlame.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.DivineFlame.LockedDescription");
        public override float Cost => 150f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => true;
        public override int Damage => 1;
        public override int MasteryDamageMultiplier => 1;
        public override float Speed => 30f;
        public override float LifeTime => -1f;

        ref float ai0 => ref Projectile.ai[0];
        ref float ai1 => ref Projectile.ai[1];
        Rectangle hitbox;
        int texturePhase; // 0 -> Fire strands. 1 -> Fire arrow

        bool animating;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<DivineFlame>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<Providence>());
        }

        public override void SetStaticDefaults()
        {
            textures = new List<Texture2D>()
            {
                ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/DivineFlameStrands").Value,
                ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/DivineFlame").Value,
            };
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 0;
            Projectile.height = 0;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            animating = false;
            hitbox = Projectile.Hitbox;
            texturePhase = 0;
        }
        public override void AI()
        {
            ai0++;
            float animTime = 241f;
            Player player = Main.player[Projectile.owner];

            if (Projectile.frameCounter++ >= TICKS_PER_FRAME)
            {
                Projectile.frameCounter = 0;

                if (Projectile.frame++ >= FRAME_COUNT - 1)
                {
                    Projectile.frame = 0;
                }
            }
            
            if (ai0 < animTime)
            {
                if (!animating)
                {
                    animating = true;
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                    Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
                    Projectile.damage = 0;
                    texturePhase = 0;
                }

                Projectile.Center = player.Center + new Vector2(0f, -10f);
                Projectile.timeLeft = 30;

                if (ai0 == 1)
                {
                    int index = CombatText.NewText(player.getRect(), textColor, "Divine Flame");
                    Main.combatText[index].lifeTime = 180;
                }

                if (ai0 == 181)
                {
                    texturePhase = 1;
                    int index = CombatText.NewText(player.getRect(), textColor, "Open.");
                    Main.combatText[index].lifeTime = 180;
                }

                if (ai0 > 181)
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.rotation = Projectile.Center.DirectionTo(Main.MouseWorld).ToRotation() + MathHelper.PiOver2;
                    }
                }
                return;
            }

            if (animating)
            {
                animating = false;
                Projectile.damage = CalculateTrueDamage(player.GetModPlayer<SorceryFightPlayer>());
                Projectile.Hitbox = hitbox;
                Projectile.timeLeft = (int)LifeTime;
                Projectile.Center = player.Center + new Vector2(0f, -10f);
                // SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleSnap, Projectile.Center);
                player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                }
            }

            ai1++;
            if (ai1 >= 60)
            {
                ai1 = 0;
                Projectile.velocity.Y += 0.1f;
            }

            if (Projectile.velocity.Y > 10f)
            {
                Projectile.velocity.Y = 10f;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            int frameHeight = textures[texturePhase].Height / FRAME_COUNT;
            int frameY = Projectile.frame * frameHeight;

            Vector2 origin = new Vector2(textures[texturePhase].Width / 2, frameHeight / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, textures[texturePhase].Width, frameHeight);
            Main.spriteBatch.Draw(textures[texturePhase], Projectile.Center - Main.screenPosition, sourceRectangle, Color.White, Projectile.rotation, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }
    }
}
