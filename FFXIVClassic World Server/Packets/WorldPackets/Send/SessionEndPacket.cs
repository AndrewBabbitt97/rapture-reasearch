﻿/*
===========================================================================
Copyright (C) 2015-2019 FFXIV Classic Server Dev Team

This file is part of FFXIV Classic Server.

FFXIV Classic Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

FFXIV Classic Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with FFXIV Classic Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using FFXIVClassic.Common;
using FFXIVClassic_World_Server.DataObjects;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Send
{
    class SessionEndPacket
    {
        public const ushort OPCODE = 0x1001;
        public const uint PACKET_SIZE = 0x38;

        public static SubPacket BuildPacket(Session session)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((UInt32)0);
                    }
                    catch (Exception)
                    { }
                }
            }

            return new SubPacket(true, OPCODE, session.sessionId, data);
        }

        public static SubPacket BuildPacket(Session session, uint destinationZoneId, string destinationPrivateArea, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    try
                    {
                        binWriter.Write((UInt32)destinationZoneId);
                        binWriter.Write((UInt16)spawnType);
                        binWriter.Write((Single)spawnX);
                        binWriter.Write((Single)spawnY);
                        binWriter.Write((Single)spawnZ);
                        binWriter.Write((Single)spawnRotation);

                    }
                    catch (Exception)
                    { }
                }
            }

            return new SubPacket(true, OPCODE, session.sessionId, data);
        }
    }
}
