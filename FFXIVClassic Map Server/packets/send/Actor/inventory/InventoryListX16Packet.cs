﻿using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class InventoryListX16Packet
    {
        public const ushort OPCODE = 0x014A;
        public const uint PACKET_SIZE = 0x720;

        public static SubPacket buildPacket(uint playerActorID, List<Item> items, ref int listOffset)
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
                        binWriter.Write(items[i].toPacketBytes());
                        listOffset++;
                    }
                }
            }

            return new SubPacket(OPCODE, playerActorID, playerActorID, data);
        }
    }
}
