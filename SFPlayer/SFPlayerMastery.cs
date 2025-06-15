using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public static Action OnNewBossDefeated;
        public static int totalNumberOfBosses;
        public List<int> bossesDefeated;
        public void AddDefeatedBoss(int bossType)
        {
            if (bossesDefeated.Contains(bossType)) return;

            bossesDefeated.Add(bossType);
            SorceryFightUI.UpdateTechniqueUI?.Invoke();
            OnNewBossDefeated?.Invoke();

            if (Main.netMode == NetmodeID.Server)
            {
                SendBossDefeatedToClients(bossType);
            }
        }


        public void SendBossDefeatedToClients(int bossType)
        {
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)MessageType.DefeatedBoss);
            packet.Write(Player.whoAmI);
            packet.Write(bossType);
            packet.Send();
        }

        public bool HasDefeatedBoss(int id)
        {
            return bossesDefeated.Contains(id);
        }

        public float MasteryDamage()
        {
            return 100 * bossesDefeated.Count;
        }

        public float MasteryCECost()
        {
            return bossesDefeated.Count;
        }
    }
}
