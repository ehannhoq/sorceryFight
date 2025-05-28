using System.Linq;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.NPCDomains
{
    internal static class NPCDomainUtils
    {
        internal static NPCDomainExpansion GetDomain(this NPC npc)
        {
            if (npc.type == ModContent.NPCType<SupremeCalamitas>())
                return new AshenedPillarsOfCalamity();

            return npc.type switch
            {
                NPCID.CultistBoss => new PhantasmicLabyrinth(),
                _ => null
            };
        }
    }
    /// <summary>
    /// Controls NPC domains. Runs on ALL clients and the server.
    /// </summary>
    public class NPCDomainController : GlobalNPC
    {
        public static bool npcIsCastingDomain;
        public static Vector2 npcCastingPosition;
        public static bool castingDomain;


        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.GetDomain() != null && lateInstantiation;

        public override bool PreAI(NPC npc)
        {
            base.PreAI(npc);

            if (npc.type == NPCID.CultistBoss) return LunaticCultistDomainController(npc);
            if (npc.type == ModContent.NPCType<SupremeCalamitas>()) return SupremeCalamitasDomainController(npc);

            return true;
        }

        private bool LunaticCultistDomainController(NPC npc)
        {
            if (npc.active && npc.life >= npc.lifeMax * 0.01f && npc.life < npc.lifeMax * 0.10f && !DomainExpansionController.ActiveDomains.Any(domain => domain is NPCDomainExpansion && domain.owner == npc.whoAmI))
            {
                npcIsCastingDomain = true;
                if (npcCastingPosition == Vector2.Zero)
                    npcCastingPosition = npc.Center;


                npc.Center = npcCastingPosition;

                if (!castingDomain)
                {
                    castingDomain = true;
                    int index;
                    index = CombatText.NewText(npc.getRect(), Color.White, "Domain Expansion:");
                    Main.combatText[index].lifeTime = 90;

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        index = CombatText.NewText(npc.getRect(), Color.White, "Phantasmic Labyrinth");
                        Main.combatText[index].lifeTime = 90;
                    }, 100);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        DomainExpansionController.ExpandDomain(npc.whoAmI, npc.GetDomain());
                    }, 200);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        castingDomain = false;
                        npcIsCastingDomain = false;
                        npcCastingPosition = Vector2.Zero;
                    }, 400);
                }

                if (castingDomain) return false;

            }

            return true;
        }

        private bool SupremeCalamitasDomainController(NPC npc)
        {
            if (npc.active && npc.life >= npc.lifeMax * 0.80f && npc.life < npc.lifeMax * 0.90f && !DomainExpansionController.ActiveDomains.Any(domain => domain is NPCDomainExpansion && domain.owner == npc.whoAmI))
            {
                npcIsCastingDomain = true;
                if (npcCastingPosition == Vector2.Zero)
                    npcCastingPosition = npc.Center;


                npc.Center = npcCastingPosition;

                if (!castingDomain)
                {
                    castingDomain = true;
                    int index;
                    index = CombatText.NewText(npc.getRect(), Color.White, "Domain Expansion:");
                    Main.combatText[index].lifeTime = 90;

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        index = CombatText.NewText(npc.getRect(), Color.White, "Ashened Pillars of Calamity");
                        Main.combatText[index].lifeTime = 90;
                    }, 100);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        DomainExpansionController.ExpandDomain(npc.whoAmI, npc.GetDomain());
                    }, 200);

                    TaskScheduler.Instance.AddDelayedTask(() =>
                    {
                        castingDomain = false;
                        npcIsCastingDomain = false;
                        npcCastingPosition = Vector2.Zero;
                    }, 400);
                }

                if (castingDomain) return false;

            }

            return true;
        } 
    }
}
