﻿/*
===========================================================================
Copyright (C) 2015-2019 Project Meteor Dev Team

This file is part of Project Meteor Server.

Project Meteor Server is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Project Meteor Server is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with Project Meteor Server. If not, see <https:www.gnu.org/licenses/>.
===========================================================================
*/

using Meteor.Common;
using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group
{
    class ModifyLinkshellPacket
    {
        public const ushort OPCODE = 0x1026;
        public const uint PACKET_SIZE = 0x60;

        public static SubPacket BuildPacket(Session session, ushort changeArg, string name, string newName, ushort crest, uint master)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write(Encoding.ASCII.GetBytes(name), 0, Encoding.ASCII.GetByteCount(name) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(name));
                    binWriter.Write((UInt16)changeArg);
                    switch (changeArg)
                    {
                        case 0:
                            binWriter.Write(Encoding.ASCII.GetBytes(newName), 0, Encoding.ASCII.GetByteCount(newName) >= 0x20 ? 0x20 : Encoding.ASCII.GetByteCount(newName));
                            break;
                        case 1:
                            binWriter.Write((UInt16)crest);
                            break;
                        case 2:
                            binWriter.Write((UInt32)master);
                            break;
                    }
                    
                }
            }
            return new SubPacket(true, OPCODE, session.id, data);
        }      
    }
}
