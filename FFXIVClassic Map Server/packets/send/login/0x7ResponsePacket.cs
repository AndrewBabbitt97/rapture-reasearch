﻿using FFXIVClassic.Common;
using System;
using System.IO;

using FFXIVClassic.Common;

namespace FFXIVClassic_Map_Server.packets.send.login
{
    class Login0x7ResponsePacket
    {
        public static BasePacket BuildPacket(uint sourceActorId, uint time, uint type)
        {
            byte[] data = new byte[0x18];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((short)0x18);
                        binWriter.Write((short)type);
                        binWriter.Write((uint)0);
                        binWriter.Write((uint)0);
                        binWriter.Write((uint)0xFFFFFD7F);

                        binWriter.Write((uint)sourceActorId);
                        binWriter.Write((uint)time);
                    }
                    catch (Exception)
                    {                        
                    }
                }
            }

            return BasePacket.CreatePacket(data, false, false);
        }
    }
}
