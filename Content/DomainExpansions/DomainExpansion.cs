using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight.Content.DomainExpansions
{
    public abstract class DomainExpansion : ModNPC
    {
        public override LocalizedText DisplayName { get; }
        public abstract string Description { get; }
        public abstract string LockedDescription { get; }
        public abstract void NPCDomainEffect(NPC npc);
    }
}
