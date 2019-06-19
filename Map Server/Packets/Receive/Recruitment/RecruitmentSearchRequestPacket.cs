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

namespace Meteor.Map.packets.receive.recruitment
{
    class RecruitmentSearchRequestPacket
    {
        public bool invalidPacket = false;

        public uint purposeId;
        public uint locationId;

        public uint discipleId;
        public uint classjobId;

        public byte unknown1;
        public byte unknown2;
        
        public string text;

        public RecruitmentSearchRequestPacket(byte[] data)
        {
            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryReader binReader = new BinaryReader(mem))
                {
                    try{
                        purposeId = binReader.ReadUInt32();
                        locationId = binReader.ReadUInt32();                       
                        discipleId = binReader.ReadUInt32();
                        classjobId = binReader.ReadUInt32();

                        unknown1 = binReader.ReadByte();
                        unknown2 = binReader.ReadByte();
                        
                        text = Encoding.ASCII.GetString(binReader.ReadBytes(0x20));
                    }
                    catch (Exception){
                        invalidPacket = true;
                    }
                }
            }
        }
    }
}
