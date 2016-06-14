﻿using FFXIVClassic_Map_Server.dataobjects;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryListX16Packet
    {
        public const ushort OPCODE = 0x014A;
        public const uint PACKET_SIZE = 0x720;

        public static SubPacket BuildPacket(uint playerActorId, List<InventoryItem> items, ref int listOffset)
        {
            return BuildPacket(playerActorId, playerActorId, items, ref listOffset);
        }

        public static SubPacket BuildPacket(uint sourceActorId, uint targetActorId, List<InventoryItem> items, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (items.Count - listOffset <= 16)
                        max = items.Count - listOffset;
                    else
                        max = 16;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write(items[listOffset].ToPacketBytes());
                        listOffset++;
                    }
                }
            }

            return new SubPacket(OPCODE, sourceActorId, targetActorId, data);
        }
    }
}
