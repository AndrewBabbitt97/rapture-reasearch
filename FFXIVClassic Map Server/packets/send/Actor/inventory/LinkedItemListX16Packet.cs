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

using FFXIVClassic_Map_Server.dataobjects;
using System;
using System.Collections.Generic;
using System.IO;

using FFXIVClassic.Common;

namespace  FFXIVClassic_Map_Server.packets.send.actor.inventory
{
    class LinkedItemListX16Packet
    {
        public const ushort OPCODE = 0x14F;
        public const uint PACKET_SIZE = 0x80;

        public static SubPacket BuildPacket(uint playerActorId, InventoryItem[] linkedItemList, List<ushort> slotsToUpdate, ref int listOffset)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    int max;
                    if (slotsToUpdate.Count - listOffset <= 16)
                        max = slotsToUpdate.Count - listOffset;
                    else
                        max = 16;

                    for (int i = 0; i < max; i++)
                    {
                        binWriter.Write((UInt16)slotsToUpdate[i]); //LinkedItemPackageSlot
                        binWriter.Write((UInt16)linkedItemList[slotsToUpdate[i]].slot); //ItemPackage Slot 
                        binWriter.Write((UInt16)linkedItemList[slotsToUpdate[i]].itemPackage); //ItemPackage Code   
                        listOffset++;
                    }

                }
            }

            return new SubPacket(OPCODE, playerActorId, data);
        }

    }
}
