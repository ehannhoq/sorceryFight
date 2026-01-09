using System;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Content.Projectiles.VFX;
using sorceryFight.SFPlayer;
using sorceryFight.StructureHelper;
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

        private static StructureTemplate msStructure => StructureHandler.GetStructure("MalevolentShrine");
        private StructureTemplate worldSnippet;
        private Point structureAnchor;

        public override void Draw(SpriteBatch spriteBatch)
        {
        }

        public override void OnExpand()
        {
            worldSnippet = new StructureTemplate(msStructure.Width, msStructure.Height);
            structureAnchor = Main.player[owner].Center.ToTileCoordinates() - new Point(msStructure.Width / 2, msStructure.Height / 2 + 3);
            worldSnippet.Capture(structureAnchor);

            StructureHandler.GenerateStructure(msStructure, structureAnchor);
        }

        public override void OnClose()
        {
            StructureHandler.GenerateStructure(worldSnippet, structureAnchor);

            worldSnippet = null;
            structureAnchor = Point.Zero;
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

            if (Main.ingameOptionsWindow)
                Main.ingameOptionsWindow = false;


            base.Update();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(ModContent.NPCType<DevourerofGodsHead>());
        }


    }
}
