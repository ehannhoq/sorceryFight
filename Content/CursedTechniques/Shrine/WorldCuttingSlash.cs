using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.Providence;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
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
        public override float Cost => 150f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => true;
        public override int Damage => 1;
        public override int MasteryDamageMultiplier => 1;
        public override float Speed => 0f;
        public override float LifeTime => 2f;
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

        public override void SetStaticDefaults()
        {
            incantations = new List<string>()
            {
                "Dragon Scales.",
                "Repulsion.",
                "Paired Falling Stars."
            };
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/WorldCuttingSlash").Value;
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
        }

        public override void AI()
        {
            ai0++;
            Player player = Main.player[Projectile.owner];
            float animTime = incantations.Count * 60f;

            if (ai0 < animTime)
            {
                if (!animating)
                {
                    animating = true;
                    Projectile.damage = 0;
                    Projectile.Hitbox = new Rectangle(0, 0, 0, 0);
                    player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = true;
                    // SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleSnap, Projectile.Center);
                }

                Projectile.Center = player.Center;

                if (ai0 % 90 == 1)
                {
                    int index = CombatText.NewText(player.getRect(), textColor, incantations[(int)ai1]);
                    Main.combatText[index].lifeTime = 60;
                    ai1++;
                }
            }

            if (animating)
            {
                animating = false;
                player.GetModPlayer<SorceryFightPlayer>().disableRegenFromProjectiles = false;
                Projectile.damage = Damage;
                Projectile.Hitbox = hitbox;
                ai2 = 1f;
                // SoundEngine.PlaySound(SorceryFightSounds.HollowPurpleSnap, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.velocity = Projectile.Center.DirectionTo(Main.MouseWorld) * Speed;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2f, texture.Height / 2f), ai2, SpriteEffects.None, 0f);
            return false;
        }
    }
}
