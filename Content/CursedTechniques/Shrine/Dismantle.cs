using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class Dismantle : CursedTechnique
    {
        static Texture2D texture;
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.CursedTechniques.Dismantle.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Dismantle.Description");
        public override string LockedDescription => SFUtils.GetLocalizationValue("Mods.sorceryFight.CursedTechniques.Dismantle.LockedDescription");
        public override float Cost => 150f;
        public override Color textColor => new Color(120, 21, 8);
        public override bool DisplayNameInGame => true;
        public override int Damage => 30;
        public override int MasteryDamageMultiplier => 60;
        public override float Speed => 50f;
        public override float LifeTime => 120f;
        public override int GetProjectileType()
        {
            return ModContent.ProjectileType<Dismantle>();
        }
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.EyeofCthulhu);
        }

        public override void SetStaticDefaults()
        {
            texture = ModContent.Request<Texture2D>("sorceryFight/Content/CursedTechniques/Shrine/Dismantle", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 89;
            Projectile.height = 258;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.ai[0]++;

            if (Projectile.ai[0] >= LifeTime)
            {
                Projectile.Kill();
            }

            float velocityRotation = Projectile.velocity.ToRotation();
            Projectile.direction = (Math.Cos(velocityRotation) > 0).ToDirectionInt();
            Projectile.rotation = velocityRotation + (Projectile.direction == -1).ToInt() * MathHelper.Pi;
        }

        public override bool PreDraw(ref Color lightColor)
        { 
            SpriteEffects spriteEffects = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, spriteEffects, 0f);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            Projectile.Kill();
        }
    }
}
