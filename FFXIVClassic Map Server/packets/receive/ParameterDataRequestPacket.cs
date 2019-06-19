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
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FFXIVClassic_Map_Server.packets.receive
{
    class ParameterDataRequestPacket
    {
        public const ushort OPCODE = 0x012F;
        public const uint PACKET_SIZE = 0x48;

        public bool invalidPacket = false;

        public uint actorID;
        public string paramName;
       
        public ParameterDataRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        actorID = binReader.ReadUInt32();
                        List<byte> strList = new List<byte>();
                        byte curByte;
                        while ((curByte = binReader.ReadByte()) != 0 && strList.Count<=0x20)
                        {
                            strList.Add(curByte);
                        }
                        paramName = Encoding.ASCII.GetString(strList.ToArray());
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}
