using System;
using System.Collections.Generic;
using CalamityMod.Particles;
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

namespace sorceryFight.Content.DomainExpansions
{
    public class MalevolentShrine : DomainExpansion
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DomainExpansions.MalevolentShrine.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.MalevolentShrine.Description");
        public override int CostPerSecond { get; set; } = 100;
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;
        public override void SetDefaults()
        {
            DomainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/MalevolentShrine", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
            Scale = 1f;
            base.SetDefaults();
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
            sfPlayer.cursedEnergy -= SorceryFight.RateSecondsToTicks(CostPerSecond + (sqrDistanceFromDE / 13000f));

            if (sfPlayer.Player.dead || sfPlayer.cursedEnergy < 2)
            {
                Remove(sfPlayer);
            }


            float minDistanceFromPlayer = 700f;
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && !npc.friendly && npc.type != NPCID.TargetDummy && npc.type != ModContent.NPCType<SuperDummyNPC>() && !npc.IsDomain())
                {
                    float sqrDistance = Vector2.DistanceSquared(npc.Center, Owners[NPC.whoAmI].Center);
                    if (sqrDistance < minDistanceFromPlayer * minDistanceFromPlayer)
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