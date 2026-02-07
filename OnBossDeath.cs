using sorceryFight.SFPlayer;
using Terraria;
using Terraria.ModLoader;

namespace sorceryFight
{
    public class OnBossDeath : GlobalNPC
    {
        public override void OnKill(NPC npc)
        {
            foreach (Player player in Main.ActivePlayers)
            {
                if (!npc.playerInteraction[player.whoAmI]) continue;

                SorceryFightPlayer sfPlayer = player.SorceryFight();

                sfPlayer.OnKilledNPC(npc.type);

                if (npc.boss)
                    sfPlayer.AddDefeatedBoss(npc.type);
            }
        }
    }
}
