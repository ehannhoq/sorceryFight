using sorceryFight.SFPlayer;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace sorceryFight
{
    public static partial class SFUtils
    {
        public static NetworkText GetNetworkText(string key)
        {
            return NetworkText.FromKey(key);
        }


        #region Optimized RW
        public static void WritePackedWorldPosition(this BinaryWriter writer, Vector2 worldPositionX16) => WritePackedWorldPosition(writer, (int)worldPositionX16.X, (int)worldPositionX16.Y);
        public static void WritePackedWorldPosition(this BinaryWriter writer, int worldX, int worldY)
        {
            int tileX = Math.DivRem(worldX, 16, out int remX);
            int tileY = Math.DivRem(worldY, 16, out int remY);
            byte remByte = (byte)(remX << 4 | remY);
            writer.Write((ushort)Math.Clamp(tileX, 0, ushort.MaxValue)); // If you actually have world size above 65535 tiles in axis, Good luck on that
            writer.Write((ushort)Math.Clamp(tileY, 0, ushort.MaxValue));
            writer.Write(remByte);
        }

        public static Vector2 ReadPackedWorldPosition(this BinaryReader reader)
        {
            reader.ReadPackedWorldPosition(out var worldX, out var worldY);
            return new Vector2(worldX, worldY);
        }

        public static void ReadPackedWorldPosition(this BinaryReader reader, out int worldX, out int worldY)
        {
            var tileX = (int)reader.ReadUInt16();
            var tileY = (int)reader.ReadUInt16();
            var remByte = reader.ReadByte();
            var remX = remByte >> 4;
            var remY = remByte & 0b1111;
            worldX = (tileX * 16) + remX;
            worldY = (tileY * 16) + remY;
        }

        #endregion

        #region TileEntity RW
        public static void WriteTileEntityID(this BinaryWriter writer, TileEntity tileEntity)
        {
            if (tileEntity is null)
            {
                writer.Write(int.MaxValue);
                return;
            }

            if (!TileEntity.ByID.ContainsKey(tileEntity.ID))
            {
                writer.Write(int.MaxValue);
                return;
            }

            writer.Write(tileEntity.ID);
        }

        public static TileEntityType ReadTileEntity<TileEntityType>(this BinaryReader reader) where TileEntityType : TileEntity
            => ReadTileEntity(reader) as TileEntityType;

        public static TileEntity ReadTileEntity(this BinaryReader reader)
        {
            var id = reader.ReadInt32();
            bool exists = TileEntity.ByID.TryGetValue(id, out TileEntity tileEntity);

            return exists ? tileEntity : null;
        }
        #endregion TileEntity RW

        #region Entity RW
        public static void WriteWhoAmI(this BinaryWriter writer, ModPlayer player) => WriteWhoAmI(writer, player?.Player);
        public static void WriteWhoAmI(this BinaryWriter writer, Player player)
        {
            byte whoAmI = (byte)(player?.whoAmI ?? Main.maxPlayers);
            writer.Write(whoAmI);
        }

        public static void WriteWhoAmI(this BinaryWriter writer, ModNPC npc) => WriteWhoAmI(writer, npc?.NPC);
        public static void WriteWhoAmI(this BinaryWriter writer, NPC npc)
        {
            byte whoAmI = (byte)(npc?.whoAmI ?? Main.maxNPCs);
            writer.Write(whoAmI);
        }

        public static SorceryFightPlayer ReadCalamityPlayer(this BinaryReader reader, bool nullOnInactive = true) => ReadPlayer(reader, nullOnInactive)?.SorceryFight() ?? null;
        public static Player ReadPlayer(this BinaryReader reader, bool nullOnInactive = true)
        {
            int index = reader.ReadByte();

            if (index >= Main.maxPlayers)
                return null;

            var player = Main.player[index];

            if (nullOnInactive && player.IsNullOrInactive())
                return null;

            return player;
        }

        public static NPCType ReadModNPC<NPCType>(this BinaryReader reader, bool nullOnInactive = true) where NPCType : ModNPC => ReadNPC(reader, nullOnInactive)?.ModNPC as NPCType;
        public static ModNPC ReadModNPC(this BinaryReader reader, bool nullOnInactive = true) => ReadNPC(reader, nullOnInactive)?.ModNPC ?? null;
        public static NPC ReadNPC(this BinaryReader reader, bool nullOnInactive = true)
        {
            int index = reader.ReadByte();

            if (index >= Main.maxNPCs)
                return null;

            var npc = Main.npc[index];

            if (nullOnInactive && npc.IsNullOrInactive())
                return null;

            return npc;
        }
        #endregion Entity RW
    }
}

