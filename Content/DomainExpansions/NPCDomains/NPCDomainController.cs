using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions.NPCDomains
{
    internal static class NPCDomainUtils
    {
        internal static NPCDomainExpansion GetDomain(this NPC npc)
        {
            return npc.type switch
            {
                NPCID.CultistBoss => new PhantasmicLabyrinth(),
                _ => null
            };
        }
    }
    public class NPCDomainController : GlobalNPC
    {
        public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => entity.GetDomain() != null && lateInstantiation;

        public override bool PreAI(NPC npc)
        {
            base.PreAI(npc);

            if (DomainExpansionController.ActiveDomains.Any(domain => domain is NPCDomainExpansion && domain.owner == npc.whoAmI)) return true;

            if (npc.life <= npc.lifeMax * 0.25)
            {
                DomainExpansionController.ExpandDomain(npc.whoAmI, npc.GetDomain());
            }


            return true;
        }
    }
}
