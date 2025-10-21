using System.Collections.Generic;
using System.Linq;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.NPCDomains
{
    public static class NPCDomainUtils
    {
        public static NPCDomainExpansion GetDomain(this NPC npc)
        {
            if (npc.type == ModContent.NPCType<SupremeCalamitas>())
                return new AshenedPillarsOfCalamity();

            return npc.type switch
            {
                NPCID.CultistBoss => new PhantasmicLabyrinth(),
                _ => null
            };
        }

        public static int GetBrainRefreshCount(this NPC npc)
        {
            if (npc.type == ModContent.NPCType<SupremeCalamitas>())
                return 2; // SCal will soon be able to expand more than one domain. ** AFTER BRAIN DAMAGE REWORK **

            return 1;
        }
    }


    /// <summary>
    /// Controls NPC domains. Runs on ALL clients and the server.
    /// </summary>
    public class NPCDomainController : GlobalNPC
    {
        public Vector2 npcCastingPosition;
        public bool castingDomain = false;
        public int domainCounter = 0;
        public static bool playerCastedDomain = false;
        public int domainTimer = 0;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.GetDomain() != null && lateInstantiation;

        public override bool InstancePerEntity => true;

        public override void SetDefaults(NPC entity)
        {
            NPCDomainController domainController = entity.GetGlobalNPC<NPCDomainController>();

            domainController.castingDomain = false;
            domainController.domainCounter = 0;
            domainController.npcCastingPosition = Vector2.Zero;
            domainController.domainTimer = 0;
            playerCastedDomain = false;
        }

        public override bool CheckDead(NPC npc)
        {
            NPCDomainController domainController = npc.GetGlobalNPC<NPCDomainController>();

            domainController.castingDomain = false;
            domainController.domainCounter = 0;
            domainController.npcCastingPosition = Vector2.Zero;
            domainController.domainTimer = 0;
            playerCastedDomain = false;

            return base.CheckDead(npc);
        }

        public override bool PreAI(NPC npc)
        {
            base.PreAI(npc);
            NPCDomainController domainController = npc.GetGlobalNPC<NPCDomainController>();

            if (Main.dedServ) return base.PreAI(npc);

            if (domainController.npcCastingPosition != Vector2.Zero)
            {
                npc.Center = domainController.npcCastingPosition;
            }

            // Main.NewText($"{npc.FullName}: Used Domain {domainCounter} times, " + (domainCooldown ? "on cooldown" : "not on cooldown") + $" Can expand domain? {npc.GetDomain().ExpandCondition(npc)}");

            bool canExpand = !DomainExpansionController.ActiveDomains.Any(domain => domain is NPCDomainExpansion && domain.owner == npc.whoAmI) && domainController.domainCounter < npc.GetBrainRefreshCount();

            bool conditionalExpanding = npc.GetDomain().ExpandCondition(npc) && canExpand;
            bool playerExpanding = playerCastedDomain && canExpand;

            if (npc.active && (conditionalExpanding || playerExpanding))
            {
                playerCastedDomain = false;

                if (domainController.npcCastingPosition == Vector2.Zero)
                    domainController.npcCastingPosition = npc.Center;

                domainController.castingDomain = true;
            }


            if (domainController.castingDomain)
            {
                domainController.domainTimer++;

                if (domainController.domainTimer == 1)
                {
                    CameraController.SetCameraPosition(domainController.npcCastingPosition, 260);
                    npc.immortal = true;
                }

                if (domainController.domainTimer == 200)
                {
                    DomainExpansionController.ExpandDomain(npc.whoAmI, npc.GetDomain());
                    playerCastedDomain = false;

                    domainController.domainCounter++;
                }

                if (domainController.domainTimer == 260)
                {
                    domainController.castingDomain = false;
                    domainController.npcCastingPosition = Vector2.Zero;
                    domainController.domainTimer = 0;
                    npc.immortal = false;
                }

                return false;
            }


            return true;
        }

        public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            base.PostDraw(npc, spriteBatch, screenPos, drawColor);
            NPCDomainController domainController = npc.GetGlobalNPC<NPCDomainController>();

            string npcName;
            if (npc.type == ModContent.NPCType<SupremeCalamitas>())
            {
                npcName = "SupremeCalamitas";
            }
            else
            {
                npcName = "LunaticCultist";
            }

            if (domainController.castingDomain)
            {
                if (domainController.domainTimer < 200)
                {

                    Texture2D frame = ModContent.Request<Texture2D>($"sorceryFight/Content/DomainExpansions/IntroCutscenes/{npcName}/{domainController.domainTimer}", ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                    Rectangle src = new Rectangle(0, 0, frame.Width, frame.Height);

                    spriteBatch.Draw(frame, npc.Center - Main.screenPosition, src, Color.White, 0f, src.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
                }

            }
        }

        public override void Unload()
        {
            castingDomain = false;
            domainCounter = 0;
            npcCastingPosition = Vector2.Zero;
        }
    }
}
