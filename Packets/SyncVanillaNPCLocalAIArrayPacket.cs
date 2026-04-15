using sorceryFight;
using System.IO;
using Terraria;

namespace sorceryFight.Packets
{
    internal sealed class SyncVanillaNPCLocalAIArrayPacket : SorceryFightPacket
    {
        public static SyncVanillaNPCLocalAIArrayPacket Instance { get; private set; }

        public static void Send(NPC npc, int toClient = -1, int ignoreClient = -1)
        {
            if (npc is null)
                return;

            var packet = Instance.CreateBasePacket();
            packet.WriteWhoAmI(npc);
            packet.Write(npc.localAI[0]);
            packet.Write(npc.localAI[1]);
            packet.Write(npc.localAI[2]);
            packet.Write(npc.localAI[3]);
            packet.Send(toClient, ignoreClient);
        }

        public override void HandlePacket(BinaryReader packet, int sender)
        {
            var npc = packet.ReadNPC();
            var ai0 = packet.ReadSingle();
            var ai1 = packet.ReadSingle();
            var ai2 = packet.ReadSingle();
            var ai3 = packet.ReadSingle();

            if (npc is null)
                return;

            npc.localAI[0] = ai0;
            npc.localAI[1] = ai1;
            npc.localAI[2] = ai2;
            npc.localAI[3] = ai3;
        }
    }
}