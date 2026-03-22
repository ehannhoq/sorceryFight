using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.NPCDomains
{
    public class FieldOfHallowedButterflies : NPCDomainExpansion
    {
        public override string InternalName => "FieldOfHallowedButterflies";

        public override SoundStyle CastSound => SorceryFightSounds.PhantasmicLabyrinth;

        public override int Tier => 2;

        public override float SureHitRange => 1150f;

        public override bool ClosedDomain => true;

        // Texture2D symbolTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/NPCDomains/PhantasmicLabyrinthSymbol", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        public override bool ExpandCondition(NPC npc)
        {
            if (npc.life > npc.lifeMax * 0.01f && npc.life <= npc.lifeMax * 0.95f) return true;

            return false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            DrawInnerDomain(() =>
            {
                Texture2D whiteTexture = TextureAssets.MagicPixel.Value;
                Rectangle screenRectangle = new Rectangle(0, 0, Main.screenWidth, Main.screenHeight);

                Rectangle domainTextureSrc = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
                spriteBatch.Draw(DomainTexture, center - Main.screenPosition, domainTextureSrc, Color.White, 0f, domainTextureSrc.Size() * 0.5f, 2, SpriteEffects.None, 0f);
            },

            () => spriteBatch.Draw(BaseTexture, center - Main.screenPosition, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height), Color.White, 0f, new Rectangle(0, 0, BaseTexture.Width, BaseTexture.Height).Size() * 0.5f, 2f, SpriteEffects.None, 0f)
            );
        }

        public override void SureHitEffect(Player player)
        {
            
        }

        public override void Update()
        {
            base.Update();
        }

        public override void OnClose()
        {
        }
    }
}
