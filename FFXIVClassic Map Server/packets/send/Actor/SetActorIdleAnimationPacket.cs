﻿using System;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.actor
{
    class SetActorIdleAnimationPacket
    {        
        public const ushort OPCODE = 0x144;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint playerActorID, uint targetID, uint idleAnimationId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                   binWriter.Seek(0x6, SeekOrigin.Begin);
                   binWriter.Write((UInt16)(idleAnimationId&0xFFFF));
                }
            }

            return new SubPacket(OPCODE, playerActorID, targetID, data);
        }
    }
}
