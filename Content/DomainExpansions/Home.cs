using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.Content.CursedTechniques.Vessel;

namespace sorceryFight.Content.DomainExpansions
{
    public class Home : DomainExpansion
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DomainExpansions.Home.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.Home.Description");
        public override int CostPerSecond { get; set; } = 75;
        public override void SetDefaults()
        {
            Scale = 0.0f;
            BackgroundScale = 0.1f;
            GoalScale = 2f;

            base.SetDefaults();

            if (Main.dedServ) return;
            DomainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/Home", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void NPCDomainEffect(NPC npc)
        {
            if (Main.myPlayer == Owners[NPC.whoAmI].whoAmI)
            {
                if (NPC.ai[0] % 2 == 0)
                {
                    var entitySource = Owners[NPC.whoAmI].GetSource_FromThis();
                    Vector2 pos = npc.Center;
                    int type = ModContent.ProjectileType<SoulDismantle>();

                    Projectile.NewProjectile(entitySource, pos, Vector2.Zero, type, 1, 0f, Owners[NPC.whoAmI].whoAmI, default, default, 1);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = DomainTexture.Height;
            int frameY = NPC.frame.Y * DomainTexture.Height;

            Vector2 origin = new Vector2(DomainTexture.Width / 2, frameHeight / 2);
            Vector2 bgOrigin = new Vector2(BackgroundTexture.Width / 2, BackgroundTexture.Height / 2);

            Rectangle sourceRectangle = new Rectangle(0, frameY, DomainTexture.Width, frameHeight);

            spriteBatch.Draw(BackgroundTexture, NPC.Center - Main.screenPosition, default, Color.White, NPC.rotation, bgOrigin, BackgroundScale, SpriteEffects.None, 0f);
            spriteBatch.Draw(DomainTexture, NPC.Center - Main.screenPosition, sourceRectangle, Color.White, NPC.rotation, origin, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}