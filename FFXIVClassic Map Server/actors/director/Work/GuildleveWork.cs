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

namespace FFXIVClassic_Map_Server.actors.director.Work
{

    class GuildleveWork
    {
        public uint startTime = 0;
        public sbyte[] aimNum = new sbyte[4];
        public sbyte[] aimNumNow = new sbyte[4];
        public sbyte[] uiState = new sbyte[4];
        public float[] markerX = new float[3];
        public float[] markerY = new float[3];
        public float[] markerZ = new float[3];
        public sbyte signal;
    }

}
