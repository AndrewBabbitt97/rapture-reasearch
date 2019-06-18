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
using FFXIVClassic_World_Server.DataObjects.Group;
using System;
using System.IO;

namespace FFXIVClassic_World_Server.Packets.Send.Subpackets.Groups
{
    class SetActiveLinkshellPacket
    {
      
        public const ushort OPCODE = 0x018A;
        public const uint PACKET_SIZE = 0x98;

        public static SubPacket BuildPacket(uint sessionId, ulong groupId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    //Write the LS groupId
                    binWriter.Write((UInt64)groupId);
                    
                    //Write the LS group type
                    binWriter.Seek(0x40, SeekOrigin.Begin);
                    binWriter.Write((UInt32)Group.CompanyGroup);

                    //Seems to be a index but can only set one active so /shrug
                    binWriter.Seek(0x60, SeekOrigin.Begin);
                    binWriter.Write((UInt32)(groupId == 0 ? 0 : 1));
                }
            }

            return new SubPacket(OPCODE, sessionId, data);
        }
    }
}
