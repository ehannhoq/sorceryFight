using Microsoft.Build.Graph;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Particles;
using sorceryFight.SFPlayer;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.CursedTechniques.StarRage
{
    public class GarudaWhipProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.IsAWhip[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.DefaultToWhip();
            Projectile.WhipSettings.Segments = 20;
            Projectile.WhipSettings.RangeMultiplier = 1f;
            //Projectile.WhipSettings.RangeMultiplier = 0.75f;
        }

        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Poisoned, 300);
            Main.player[Projectile.owner].MinionAttackTargetNPC = target.whoAmI;
        }

        private void DrawLine(List<Vector2> list)
        {
            Texture2D texture = TextureAssets.FishingLine.Value;
            Rectangle frame = texture.Frame();
            Vector2 origin = new Vector2(frame.Width / 2, 2);
            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector2 diff = list[i + 1] - list[i];
                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(list[i].ToTileCoordinates(), Color.Red);
                Vector2 scale = new Vector2(1, (diff.Length() + 2) / frame.Height);
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, SpriteEffects.None, 0);
                pos += diff;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            List<Vector2> list = new List<Vector2>();
            Projectile.FillWhipControlPoints(Projectile, list);
            DrawLine(list);

            SpriteEffects flip = Projectile.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.instance.LoadProjectile(Type);
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Vector2 pos = list[0];

            for (int i = 0; i < list.Count - 1; i++)
            {
                Rectangle frame = new Rectangle(0, 0, 10, 26);
                Vector2 origin = new Vector2(5, 8);
                float scale = 1;

                if (i == list.Count - 2)
                {
                    frame.Y = 74;
                    frame.Height = 18;
                    Projectile.GetWhipSettings(Projectile, out float timeToFlyOut, out int _, out float _);
                    float t = Timer / timeToFlyOut;
                    scale = MathHelper.Lerp(0.5f, 1.5f, Utils.GetLerpValue(0.1f, 0.7f, t, true) * Utils.GetLerpValue(0.9f, 0.7f, t, true));
                }
                else if (i > 10) { frame.Y = 58; frame.Height = 16; }
                else if (i > 5) { frame.Y = 42; frame.Height = 16; }
                else if (i > 0) { frame.Y = 26; frame.Height = 16; }

                Vector2 diff = list[i + 1] - list[i];
                float rotation = diff.ToRotation() - MathHelper.PiOver2;
                Color color = Lighting.GetColor(list[i].ToTileCoordinates());
                Main.EntitySpriteDraw(texture, pos - Main.screenPosition, frame, color, rotation, origin, scale, flip, 0);
                pos += diff;
            }

            return false;
        }
    }
}
