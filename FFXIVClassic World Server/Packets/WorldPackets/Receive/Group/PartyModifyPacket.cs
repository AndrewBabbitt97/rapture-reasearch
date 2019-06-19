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

using System;
using System.IO;
using System.Text;

namespace FFXIVClassic_World_Server.Packets.WorldPackets.Receive.Group
{
    class PartyModifyPacket
    {
        public bool invalidPacket = false;

        public const ushort MODIFY_LEADER = 0;
        public const ushort MODIFY_KICKPLAYER = 1;

        public ushort command;
        public string name;
        public uint actorId;

        public PartyModifyPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try
                    {
                        command = binReader.ReadUInt16();

                        if (command >= 2)
                            actorId = binReader.ReadUInt32();
                        else
                            name = Encoding.ASCII.GetString(binReader.ReadBytes(0x20)).Trim(new[] { '\0' });
                    }
                    catch (Exception)
                    {
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}
