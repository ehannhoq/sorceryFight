using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.NPCDomains
{
    public class PhantasmicLabyrinth : NPCDomainExpansion
    {
        public override string InternalName => "PhantasmicLabyrinth";

        public override SoundStyle CastSound => SorceryFightSounds.UnlimitedVoid;

        public override float SureHitRange => 1150f;

        public override bool ClosedDomain => true;

        float symbolRotation = 0f;
        public Texture2D symbolTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/NPCDomains/PhantasmicLabyrinthSymbol", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawInnerDomain(() =>
            {
                Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

                spriteBatch.Draw(whiteTexture, screenRectangle, Color.Black);
            });

            Rectangle domainTextureSrc = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
            spriteBatch.Draw(DomainTexture, center - Main.screenPosition, domainTextureSrc, Color.White, 0f, domainTextureSrc.Size() * 0.5f, 1f, SpriteEffects.None, 0f);

            Rectangle symbolSrc = new Rectangle(0, 0, symbolTexture.Width, symbolTexture.Height);
            spriteBatch.Draw(symbolTexture, center - Main.screenPosition, symbolSrc, Color.White, symbolRotation, symbolSrc.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
        }

        public override void SureHitEffect(Player player)
        {
            player.statLife = (int)(player.statLifeMax2 * 0.10f);
        }

        public override void Update()
        {
            base.Update();
            
            if ((symbolRotation += 0.01f) > MathF.PI * 2f)
                symbolRotation = 0;
        }

        public override void CloseDomain()
        {
            symbolRotation = 0f;
        }
    }
}
