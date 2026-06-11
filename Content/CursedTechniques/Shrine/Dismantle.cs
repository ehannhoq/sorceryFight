using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.SFPlayer;
using sorceryFight.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.Shrine
{
    public class Dismantle : CursedTechnique
    {
        static Texture2D texture;

        public override string InternalName => "Dismantle";


        public override void SetStaticDefaults()
        {
            if (Main.dedServ) return;
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

            if (Projectile.ai[0] >= lifetime)
            {
                Projectile.Kill();
            }

            if (Projectile.ai[0] == 1)
            {
                SoundEngine.PlaySound(SorceryFightSounds.DismantleSlice, Projectile.Center);
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
