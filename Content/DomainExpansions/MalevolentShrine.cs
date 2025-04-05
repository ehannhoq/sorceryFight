using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using sorceryFight.SFPlayer;
using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using sorceryFight.Content.CursedTechniques.Shrine;
using sorceryFight.Content.Buffs.Vessel;

namespace sorceryFight.Content.DomainExpansions
{
    public class MalevolentShrine : DomainExpansion
    {
        public override string InternalName => "MalevolentShrine";
        public override int CostPerSecond { get; set; } = 150;
        public override bool Unlocked(SorceryFightPlayer sf)
        {
            return base.Unlocked(sf) || sf.Player.HasBuff(ModContent.BuffType<KingOfCursesBuff>());
        }
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;
        public override void SetDefaults()
        {
            Scale = 1f;
            base.SetDefaults();
            
            if (Main.dedServ) return;
            DomainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/MalevolentShrine", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        {
            Owners[NPC.whoAmI] = Main.player[(int)NPC.ai[1]];            
            SorceryFightPlayer sfPlayer = Owners[NPC.whoAmI].GetModPlayer<SorceryFightPlayer>();
            if (!NPC.active && BossRushEvent.BossRushActive)
            {
                NPC.active = true;
            }

            sfPlayer.disableRegenFromDE = true;
            float sqrDistanceFromDE = Vector2.DistanceSquared(NPC.Center, Owners[NPC.whoAmI].Center);
            float totalCPS = CostPerSecond > (sqrDistanceFromDE / 15000f) ? CostPerSecond : (sqrDistanceFromDE / 15000f);
            sfPlayer.cursedEnergy -= SFUtils.RateSecondsToTicks(totalCPS);

            if (sfPlayer.Player.dead || sfPlayer.cursedEnergy < 2)
            {
                Remove(sfPlayer);
                return;
            }


            float minDistanceFromPlayer = 700f;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.type != NPCID.TargetDummy && npc.type != ModContent.NPCType<SuperDummyNPC>() && !npc.IsDomain())
                {
                    float sqrDistance = Vector2.DistanceSquared(npc.Center, Owners[NPC.whoAmI].Center);
                    if (sqrDistance < minDistanceFromPlayer.Squared())
                    {
                        NPCDomainEffect(npc);
                    }
                }
            }
        }

        public override void NPCDomainEffect(NPC npc)
        {
            if (Main.myPlayer == Owners[NPC.whoAmI].whoAmI)
            {
                var entitySource = Owners[NPC.whoAmI].GetSource_FromThis();
                Vector2 pos = npc.Center;
                int type = ModContent.ProjectileType<InstantDismantle>();

                int index = Projectile.NewProjectile(entitySource, pos, Vector2.Zero, type, 1, 0f, Owners[NPC.whoAmI].whoAmI);
                Main.projectile[index].ai[0] = 1f;
                Main.projectile[index].ai[1] = Main.rand.Next(0, 3);
                Main.projectile[index].ai[2] = Main.rand.NextFloat(0, 6);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            int frameHeight = DomainTexture.Height / FRAME_COUNT;
            int frameY = NPC.frame.Y * frameHeight;

            Vector2 origin = new Vector2(DomainTexture.Width / 2, frameHeight / 2);
            Rectangle sourceRectangle = new Rectangle(0, frameY, DomainTexture.Width, frameHeight);

            spriteBatch.Draw(DomainTexture, NPC.Center - Main.screenPosition, sourceRectangle, Color.White, NPC.rotation, origin, Scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}