using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

public class AreThereAnyDamnBosses : GlobalNPC
{
    public static bool BossActive = false;
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation) => lateInstantiation && entity.boss;
    public override void OnSpawn(NPC npc, IEntitySource source)
    {
        BossActive = true;
    }
    public override void OnKill(NPC npc)
    {
        if (Main.npc.Any(x => x.active && x.boss && x.whoAmI != npc.whoAmI))
            return;
        BossActive = false;
    }
}