using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques.Vessel;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.PlayerDomains
{
    public class Home : PlayerDomainExpansion
    {
        public override string InternalName => "Home";

        public override SoundStyle CastSound => SorceryFightSounds.Home;

        public override int Tier => 2;

        public override float SureHitRange => 1150f;

        public override float Cost => 75f;

        public override bool ClosedDomain => true;

        float tick = 0f;
        float bgFade = 0f;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawInnerDomain(() =>
            {
                Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

                spriteBatch.Draw(whiteTexture, screenRectangle, new Color(255, 255, 255, bgFade));
            },
            () => spriteBatch.Draw(BaseTexture, center - Main.screenPosition, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height), Color.White, 0f, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height).Size() * 0.5f, 2f, SpriteEffects.None, 0f)
            );

            if (tick > 180)
            {
                Rectangle src = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
                spriteBatch.Draw(DomainTexture, center - Main.screenPosition, src, Color.White, 0f, src.Size() * 0.5f, 2f, SpriteEffects.None, 0f);
            }
        }

        public override void SureHitEffect(NPC npc)
        {
            if (Main.myPlayer == owner)
            {
                if (tick % 2 == 0)
                {
                    var entitySource = Main.LocalPlayer.GetSource_FromThis();
                    Vector2 pos = npc.Center;
                    int type = ModContent.ProjectileType<SoulDismantle>();

                    Projectile.NewProjectile(entitySource, pos, Vector2.Zero, type, 1, 0f, owner, default, default, 1);
                }
            }
        }


        public override void OnClose()
        {
            tick = 0f;
            bgFade = 0f;
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return true;
        }

        public override void Update()
        {
            base.Update();

            tick++;
            if ((bgFade += 0.02f) > 1)
                bgFade = 1;

        }
    }
}
