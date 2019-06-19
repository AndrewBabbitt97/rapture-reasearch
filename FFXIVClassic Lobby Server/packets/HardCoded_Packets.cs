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

namespace FFXIVClassic_Lobby_Server.packets
{
    class HardCoded_Packets
    {
        public static byte[] g_secureConnectionAcknowledgment =
        {
	        0x00, 0x00, 0x00, 0x00, 0xA0, 0x02, 0x01, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x90, 0x02, 0x0A, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x0C, 0x69, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x00, 0xD0, 0xED, 0x45, 0x02, 0x00, 0x00, 0x00, 0x00, 
	        0xC0, 0xED, 0xDF, 0xFF, 0xAF, 0xF7, 0xF7, 0xAF, 0x10, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 
	        0x42, 0x82, 0x63, 0x52, 0x01, 0x00, 0x00, 0x00, 0x10, 0xEF, 0xDF, 0xFF, 0x53, 0x61, 0x6D, 0x70, 
	        0x6C, 0x65, 0x20, 0x53, 0x61, 0x6D, 0x70, 0x6C, 0x65, 0x20, 0x52, 0x75, 0x6E, 0x52, 0x75, 0x6E, 
	        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x02, 0x00, 0xF7, 0xAF, 0xAF, 0xF7, 0x00, 0x00, 0xB8, 0x6C, 0x4D, 0x02, 0x00, 0x00, 0x00, 0x00, 
	        0x10, 0x6C, 0x4D, 0x02, 0x00, 0x00, 0x00, 0x00, 0x40, 0x2C, 0xAC, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x63, 0x72, 0x65, 0x61, 0x74, 0x65, 0x43, 0x61, 0x6C, 0x6C, 0x62, 0x61, 0x63, 0x6B, 0x4F, 0x62, 
	        0x6A, 0x65, 0x63, 0x74, 0x2E, 0x2E, 0x2E, 0x5B, 0x36, 0x36, 0x2E, 0x31, 0x33, 0x30, 0x2E, 0x39, 
	        0x39, 0x2E, 0x38, 0x32, 0x3A, 0x36, 0x33, 0x34, 0x30, 0x37, 0x5D, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x70, 0xEE, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0x6C, 0x4E, 0x38, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x32, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0xAF, 0xF7, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x38, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0xC0, 0xEE, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0xFE, 0x4E, 0x38, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x0B, 0x00, 0x00, 0x01, 0x01, 0x00, 0x00, 0x00, 0x20, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 
	        0x00, 0x01, 0xCC, 0xCC, 0x0C, 0x69, 0x00, 0xE0, 0xD0, 0x58, 0x33, 0x02, 0x00, 0x00, 0x00, 0x00, 
	        0x10, 0x00, 0x00, 0x00, 0x30, 0x00, 0x00, 0x00, 0x80, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 
	        0xC0, 0xEE, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0xD0, 0xED, 0x45, 0x02, 0x00, 0x00, 0x00, 0x00, 
	        0xF0, 0xEE, 0xDF, 0xFF, 0xAF, 0xF7, 0xF7, 0xAF, 0x20, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 
	        0x0C, 0x69, 0x00, 0xE0, 0x00, 0x00, 0x00, 0x00, 0x10, 0x6C, 0x4D, 0x02, 0x00, 0x00, 0x00, 0x00, 
	        0x45, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x33, 0x34, 0x30, 0x37, 0x00, 0x00, 0x00, 0x00, 
	        0x90, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0x18, 0xBE, 0x34, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0xD8, 0x32, 0xAC, 0x01, 0x00, 0x00, 0x00, 0x00, 0xD0, 0x32, 0xAC, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x02, 0x00, 0xF7, 0xAF, 0x42, 0x82, 0x63, 0x52, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 
	        0x00, 0x00, 0x36, 0x36, 0x2E, 0x31, 0x33, 0x30, 0x2E, 0x39, 0x39, 0x2E, 0x38, 0x32, 0x00, 0x00, 
	        0x00, 0x00, 0x36, 0x36, 0x2E, 0x31, 0x33, 0x30, 0x2E, 0x39, 0x39, 0x2E, 0x38, 0x32, 0x00, 0xFF, 
	        0x90, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0x24, 0xCF, 0x76, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x10, 0x6C, 0x4D, 0x02, 0x00, 0x00, 0x00, 0x00, 0x70, 0x7A, 0xB7, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x10, 0x6C, 0x4D, 0x02, 0x00, 0x00, 0x00, 0x00, 
	        0x90, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0xD1, 0xF3, 0x37, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x10, 0x6C, 0x4D, 0x02, 0x00, 0x00, 0x00, 0x00, 0xA0, 0x32, 0xAC, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0xC0, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0xE8, 0x3E, 0x77, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x70, 0x99, 0xAA, 0x01, 0x0C, 0x69, 0x00, 0xE0, 0xA0, 0x32, 0xAC, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x58, 0x59, 0x33, 0x02, 0x00, 0x00, 0x00, 0x00, 0x10, 0x6C, 0x4D, 0x02, 0x00, 0x00, 0x00, 0x00, 
	        0xE0, 0xEF, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0x05, 0x3F, 0x77, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x0C, 0x69, 0x00, 0xE0, 0x0C, 0x69, 0x00, 0xE0, 0xA0, 0x32, 0xAC, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0x00, 0xF0, 0xDF, 0xFF, 0x7F, 0xFD, 0xFF, 0xFF, 0x23, 0x3F, 0x77, 0x01, 0x00, 0x00, 0x00, 0x00, 
	        0xC0, 0x5A, 0x33, 0x02, 0x0C, 0x69, 0x00, 0xE0, 0xA0, 0x32, 0xAC, 0x01, 0x00, 0x00, 0x00, 0x00, 
        };

    }
}
