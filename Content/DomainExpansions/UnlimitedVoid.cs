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

namespace sorceryFight.Content.DomainExpansions
{
    public class UnlimitedVoid : DomainExpansion
    {
        public override LocalizedText DisplayName => SFUtils.GetLocalization("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.DisplayName");
        public override string Description => SFUtils.GetLocalizationValue("Mods.sorceryFight.DomainExpansions.UnlimitedVoid.Description");
        public override int CostPerSecond { get; set; } =  100;
        public static int FRAME_COUNT = 1;
        public static int TICKS_PER_FRAME = 1;
        public static Dictionary<int, float[]> frozenValues;
        public override void SetDefaults()
        {
            frozenValues = new Dictionary<int, float[]>();

            Scale = 0.1f;
            BackgroundScale = 0.1f;
            GoalScale = 2f;

            base.SetDefaults();
            
            if (Main.dedServ) return;
            DomainTexture = ModContent.Request<Texture2D>("sorceryFight/Content/DomainExpansions/UnlimitedVoid", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
        }

        public override void AI()
        {
            base.AI();

            if (NPC.ai[0] < 200)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 offsetPos = NPC.Center + new Vector2(Main.rand.NextFloat(-2000f, 2000f), Main.rand.NextFloat(-2000f, 2000f));
                    Vector2 velocity = NPC.Center.DirectionTo(offsetPos) * 40f;

                    LineParticle particle = new LineParticle(NPC.Center, velocity, false, 180, 1, Color.LightSteelBlue);
                    GeneralParticleHandler.SpawnParticle(particle);
                }
            }
        
            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (!proj.friendly)
                {
                    float distance = Vector2.DistanceSquared(proj.Center, NPC.Center);
                    if (distance < SureHitDistance.Squared())
                    {
                        proj.Kill();
                    }
                }
            }
        }

        public override void NPCDomainEffect(NPC npc)
        {
            int npcID = npc.whoAmI;
            
            if (!frozenValues.ContainsKey(npcID))
            {
                frozenValues[npcID] = new float[6];
                float[] data = [npc.ai[0], npc.ai[1], npc.ai[2], npc.ai[3], npc.position.X, npc.position.Y];
                Array.Copy(data, frozenValues[npcID], 6);
            }

            npc.position = new Vector2(frozenValues[npcID][4], frozenValues[npcID][5]);

            if (!AffectedByFrozenAI(npc))
            {
                return;
            }
            npc.ai[0] = frozenValues[npcID][0];
            npc.ai[1] = frozenValues[npcID][1];
            npc.ai[2] = frozenValues[npcID][2];
            npc.ai[3] = frozenValues[npcID][3];

        }

        public override void Remove(SorceryFightPlayer sfPlayer)
        {
            frozenValues.Clear();
            base.Remove(sfPlayer);
        }



        private bool AffectedByFrozenAI(NPC npc)
        {
            if (npc.type == NPCID.MoonLordHand)
                return false;
            if (npc.type == NPCID.MoonLordHead)
                return false;

            return true;
        }
    }
}