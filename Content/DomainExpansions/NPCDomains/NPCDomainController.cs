using System.Collections.Generic;
using System.Linq;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
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
                return 1; // SCal will soon be able to expand more than one domain. ** AFTER BRAIN DAMAGE REWORK **

            return 1;
        }
    }


    /// <summary>
    /// Controls NPC domains. Runs on ALL clients and the server.
    /// </summary>
    public class NPCDomainController : GlobalNPC
    {
        public static Vector2 npcCastingPosition;
        public static bool castingDomain = false;
        public static int domainCounter = 0;
        public static bool playerCastedDomain = false;

        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.GetDomain() != null && lateInstantiation;

        public override void SetDefaults(NPC entity)
        {
            castingDomain = false;
            domainCounter = 0;
        }

        public override bool PreAI(NPC npc)
        {
            base.PreAI(npc);

            if (npcCastingPosition != Vector2.Zero)
            {
                npc.Center = npcCastingPosition;
            }


            // Main.NewText($"{npc.FullName}: Used Domain {domainCounter} times, " + (domainCooldown ? "on cooldown" : "not on cooldown") + $" Can expand domain? {npc.GetDomain().ExpandCondition(npc)}");

            if ((npc.active && !DomainExpansionController.ActiveDomains.Any(domain => domain is NPCDomainExpansion && domain.owner == npc.whoAmI) && domainCounter < npc.GetBrainRefreshCount()
            && npc.GetDomain().ExpandCondition(npc)) || (playerCastedDomain && !DomainExpansionController.ActiveDomains.Any(domain => domain is NPCDomainExpansion && domain.owner == npc.whoAmI && domainCounter < npc.GetBrainRefreshCount())))
            {
                playerCastedDomain = false;

                if (npcCastingPosition == Vector2.Zero)
                    npcCastingPosition = npc.Center;

                if (!castingDomain)
                {
                    castingDomain = true;
                    CameraController.SetCameraPosition(npcCastingPosition, 260);

                    int index;
                    index = CombatText.NewText(npc.getRect(), Color.White, "Domain Expansion:");
                    Main.combatText[index].lifeTime = 90;

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        index = CombatText.NewText(npc.getRect(), Color.White, npc.GetDomain().DisplayName);
                        Main.combatText[index].lifeTime = 90;
                    }, 100);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        DomainExpansionController.ExpandDomain(npc.whoAmI, npc.GetDomain());
                        playerCastedDomain = false;

                        domainCounter++;
                    }, 200);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        castingDomain = false;
                        npcCastingPosition = Vector2.Zero;
                    }, 260);
                }
            }

            if (castingDomain) return false;

            return true;
        }

        public override void Unload()
        {
            castingDomain = false;
            domainCounter = 0;
            npcCastingPosition = Vector2.Zero;
        }
    }
}
