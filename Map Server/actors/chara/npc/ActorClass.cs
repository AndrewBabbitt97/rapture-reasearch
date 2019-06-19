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

namespace FFXIVClassic_Map_Server.actors.chara.npc
{
    class ActorClass
    {
        public readonly uint actorClassId;
        public readonly string classPath;
        public readonly uint displayNameId;
        public readonly uint propertyFlags;
        public readonly string eventConditions;

        public readonly ushort pushCommand;
        public readonly ushort pushCommandSub;
        public readonly byte pushCommandPriority;

        public ActorClass(uint id, string classPath, uint nameId, uint propertyFlags, string eventConditions, ushort pushCommand, ushort pushCommandSub, byte pushCommandPriority)
        {
            this.actorClassId = id;
            this.classPath = classPath;
            this.displayNameId = nameId;
            this.propertyFlags = propertyFlags;
            this.eventConditions = eventConditions;

            this.pushCommand = pushCommand;
            this.pushCommandSub = pushCommandSub;
            this.pushCommandPriority = pushCommandPriority;
        }
    }
}
