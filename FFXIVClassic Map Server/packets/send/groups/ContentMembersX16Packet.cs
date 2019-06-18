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
using System;
using System.Collections.Generic;
using System.IO;

namespace FFXIVClassic_Map_Server.packets.send.group
{
    class ContentMembersX16Packet
    {
        public const ushort OPCODE = 0x0184;
        public const uint PACKET_SIZE = 0xF0;

        public static SubPacket buildPacket(uint playerActorID, uint locationCode, ulong sequenceId, List<GroupMember> entries, ref int offset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write List Header
                    binWriter.Write((UInt64)locationCode);
                    binWriter.Write((UInt64)sequenceId);
                    //Write Entries
                    int max = 16;
                    if (entries.Count-offset < 16)
                        max = entries.Count - offset;
                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Seek(0x10 + (0xC * i), SeekOrigin.Begin);

                        GroupMember entry = entries[i];
                        binWriter.Write((UInt32)entry.actorId);
                        binWriter.Write((UInt32)1001);
                        binWriter.Write((UInt32)1);

                        offset++;
                    }                    
                }
            }

            return new SubPacket(OPCODE, playerActorID, data);
        }
    }
}
