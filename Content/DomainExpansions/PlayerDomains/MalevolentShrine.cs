using System;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Content.Projectiles.MalevolentShrine;
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

        public override int Tier => 1;

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

        public override void Update()
        {
            if (Main.myPlayer == Main.player[owner].whoAmI)
            {
                var entitySource = Main.player[owner].GetSource_FromThis();
                Vector2 randomOffset = new Vector2(Main.rand.NextFloat(-SureHitRange, SureHitRange), Main.rand.NextFloat(-SureHitRange, SureHitRange));

                int type = ModContent.ProjectileType<CleaveMS>();

                Projectile.NewProjectile(entitySource, Main.player[owner].Center + randomOffset, Vector2.Zero, type, 1, 0f, owner, Main.rand.NextFloat(0, 6));
            }
            base.Update();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
        }
    }
}
