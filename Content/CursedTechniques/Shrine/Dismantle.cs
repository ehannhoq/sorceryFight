using System;
using Microsoft.Build.Evaluation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class Dismantle : CursedTechnique
    {
        public static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.Dismantle.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Dismantle.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Dismantle.LockedDescription");
        public override float Cost => 50f;
        public override Color textColor => new Color(184, 130, 101);
        public override bool DisplayNameInGame => false;
        public override int BaseDamage => 30;
        public override int MaxDamage => 900;
        public override float MaxMastery => 10f;
        public override float Speed => 0f;
        public override float LifeTime => 2f;

        public ref float randomSprite => ref Projectile.ai[1];
        public ref float randomRotation => ref Projectile.ai[2];

        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<Dismantle>();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return NPC.downedBoss3;
        }

        public override void UseTechnique(SorceryFightPlayer sf)
        {
            Player player = sf.Player;
            sf.cursedEnergy -= Cost;

            if (Main.myPlayer == player.whoAmI)
            {
                if (DisplayNameInGame)
                {
                    int index1 = CombatText.NewText(player.getRect(), textColor, DisplayName.Value);
                    Main.combatText[index1].lifeTime = 180;
                }

                Vector2 mousePos = Main.MouseWorld;
                var entitySource = player.GetSource_FromThis();
                int index = Projectile.NewProjectile(entitySource, mousePos, Vector2.Zero, GetProjectileType(), CalculateTrueDamage(sf), 0f, player.whoAmI);
                Main.projectile[index].ai[1] = Main.rand.Next(0, 3);
                Main.projectile[index].ai[2] = Main.rand.NextFloat(0, 6);

            }
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 63;
            Projectile.height = 176;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            Projectile.ai[0] ++;

            if (Projectile.ai[0] >= LifeTime)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;

            if (texture == null)
                texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/Dismantle").Value;

            int frameHeight = texture.Height / 3;
            int frameY = (int)randomSprite * frameHeight;

            Vector2 origin = new Vector2(texture.Width / 2, frameHeight / 2);
            Rectangle srcRectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, srcRectangle, Color.White, (int)randomRotation, origin, 1f, SpriteEffects.None, 0f);

            return false;
        }

    }
}