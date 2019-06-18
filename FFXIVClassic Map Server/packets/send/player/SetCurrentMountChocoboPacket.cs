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

namespace FFXIVClassic_Map_Server.packets.send.player
{
    class SetCurrentMountChocoboPacket
    {
        public const int CHOCOBO_NORMAL = 0;

        public const int CHOCOBO_LIMSA1 = 0x1;
        public const int CHOCOBO_LIMSA2 = 0x2;
        public const int CHOCOBO_LIMSA3 = 0x3;
        public const int CHOCOBO_LIMSA4 = 0x4;

        public const int CHOCOBO_GRIDANIA1 = 0x1F;
        public const int CHOCOBO_GRIDANIA2 = 0x20;
        public const int CHOCOBO_GRIDANIA3 = 0x21;
        public const int CHOCOBO_GRIDANIA4 = 0x22;

        public const int CHOCOBO_ULDAH1 = 0x3D;
        public const int CHOCOBO_ULDAH2 = 0x3E;
        public const int CHOCOBO_ULDAH3 = 0x3F;
        public const int CHOCOBO_ULDAH4 = 0x40;

        public const ushort OPCODE = 0x0197;
        public const uint PACKET_SIZE = 0x28;

        public static SubPacket BuildPacket(uint sourceActorId, int appearanceId)
        {
            byte[] data = new byte[PACKET_SIZE - 0x20];
            data[5] = (byte)(appearanceId & 0xFF);
            return new SubPacket(OPCODE, sourceActorId, data);
        }
    }
}
