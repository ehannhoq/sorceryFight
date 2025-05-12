using System;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.SFPlayer;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.PlayerDomains
{
    public class MalevolentShrine : PlayerDomainExpansion
    {
        public override string InternalName => "MalevolentShrine";

        public override SoundStyle CastSound => SorceryFightSounds.MalevolentShrine;

        public override float SureHitRange => 1000f;

        public override float Cost => 150f;

        public override bool ClosedDomain => false;

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, DomainTexture.Width, DomainTexture.Height);
            spriteBatch.Draw(DomainTexture, center - Main.screenPosition, sourceRectangle, Color.White, 0f, sourceRectangle.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
        }

        public override void SureHitEffect(NPC npc)
        {
            if (Main.myPlayer == Main.player[owner].whoAmI)
            {
                var entitySource = Main.player[owner].GetSource_FromThis();
                Vector2 pos = npc.Center;
                int type = ModContent.ProjectileType<InstantDismantle>();

                Projectile.NewProjectile(entitySource, pos, Vector2.Zero, type, 1, 0f, owner, 1f, Main.rand.Next(0, 3), Main.rand.NextFloat(0, 6));
            }
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
        }
    }
}
