using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace sorceryFight.SFPlayer
{
    public partial class SorceryFightPlayer : ModPlayer
    {
        public static Action OnNewBossDefeated;
        public HashSet<int> bossesDefeated;
        public int numberBossesDefeated => bossesDefeated.Count;

        private byte mechanicalBossesDefeatedFlags;
        public bool defeatedMechBossOne => (mechanicalBossesDefeatedFlags & 0b0001) == 1;
        public bool defeatedMechBossTwo => (mechanicalBossesDefeatedFlags & 0b0010) == 1;
        public bool defeatedMechBossThree => (mechanicalBossesDefeatedFlags & 0b0100) == 1;

        public void AddDefeatedBoss(int bossType)
        {
            if (!bossesDefeated.Contains(bossType))
                OnNewBossDefeated?.Invoke();

            if (IsMechanicalBoss(bossType) && (!defeatedMechBossOne || !defeatedMechBossTwo || !defeatedMechBossThree))
                SetMechanicalBossFlags(bossType);

            bossesDefeated.Add(bossType);
            SorceryFightUI.UpdateTechniqueUI?.Invoke();

            if (Main.netMode == NetmodeID.Server)
            {
                SendBossDefeatedToClients(bossType);
            }
        }


        private bool IsMechanicalBoss(int bossType)
        {
            switch (bossType)
            {
                case NPCID.Retinazer:
                case NPCID.Spazmatism:
                case NPCID.SkeletronPrime:
                case NPCID.TheDestroyer:
                    return true;
            }
            return false;
        }


        private void SetMechanicalBossFlags(int bossType)
        {
            if (!defeatedMechBossOne)
            {
                if (bossType == NPCID.Retinazer)
                    if (Main.npc.Any(npc => npc.boss && npc.type == NPCID.Spazmatism))
                        return;
                if (bossType == NPCID.Spazmatism)
                    if (Main.npc.Any(npc => npc.boss && npc.type == NPCID.Retinazer))
                        return;

                mechanicalBossesDefeatedFlags = 0b0001;
            }

            if (!defeatedMechBossTwo)
            {
                if (bossType == NPCID.Retinazer)
                    if (Main.npc.Any(npc => npc.boss && npc.type == NPCID.Spazmatism))
                        return;
                if (bossType == NPCID.Spazmatism)
                    if (Main.npc.Any(npc => npc.boss && npc.type == NPCID.Retinazer))
                        return;

                mechanicalBossesDefeatedFlags = 0b0011;
            }

            if (!defeatedMechBossThree)
            {
                if (bossType == NPCID.Retinazer)
                    if (Main.npc.Any(npc => npc.boss && npc.type == NPCID.Spazmatism))
                        return;
                if (bossType == NPCID.Spazmatism)
                    if (Main.npc.Any(npc => npc.boss && npc.type == NPCID.Retinazer))
                        return;

                mechanicalBossesDefeatedFlags = 0b0111;
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
            return 100 * numberBossesDefeated;
        }

        public float MasteryCECost()
        {
            return numberBossesDefeated;
        }
    }
}
