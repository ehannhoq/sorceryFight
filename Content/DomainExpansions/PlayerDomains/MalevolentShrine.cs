using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using sorceryFight.Content.Buffs.Vessel;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Content.VFX;
using sorceryFight.SFPlayer;
using sorceryFight.StructureHelper;
using System;
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
        private static Texture2D cleaveTexture = ModContent.Request<Texture2D>("sorceryFight/Content/VFX/CleaveMS", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
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
             VFXManager.AddVFX(new VFXObject(
                cleaveTexture,
                new Vector2(Main.rand.NextFloat(-SureHitRange, SureHitRange), Main.rand.NextFloat(-SureHitRange, SureHitRange))
             ));

            if (Main.ingameOptionsWindow)
                Main.ingameOptionsWindow = false;


            base.Update();
        }

        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return sf.HasDefeatedBoss(NPCID.Golem);
        }
    }
}
