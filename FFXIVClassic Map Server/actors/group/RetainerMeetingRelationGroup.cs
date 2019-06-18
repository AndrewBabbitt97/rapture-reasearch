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
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.send.groups;
using System.Collections.Generic;

namespace FFXIVClassic_Map_Server.actors.group
{
    class RetainerMeetingRelationGroup : Group
    {
        Player player;
        Retainer retainer;

        public RetainerMeetingRelationGroup(ulong groupIndex, Player player, Retainer retainer)
            : base(groupIndex)
        {
            this.player = player;
            this.retainer = retainer;
        }

        public override int GetMemberCount()
        {
            return 2;
        }

        public override List<GroupMember> BuildMemberList(uint id)
        {
            List<GroupMember> groupMembers = new List<GroupMember>();

            groupMembers.Add(new GroupMember(player.actorId, -1, 0x83, false, true, player.customDisplayName));
            groupMembers.Add(new GroupMember(retainer.actorId, -1, 0x83, false, true, retainer.customDisplayName));
            
            return groupMembers;
        }

        public override uint GetTypeId()
        {
            return 50003;
        }

        public override void SendInitWorkValues(Session session)
        {
            SynchGroupWorkValuesPacket groupWork = new SynchGroupWorkValuesPacket(groupIndex);
            groupWork.setTarget("/_init");

            SubPacket test = groupWork.buildPacket(session.id);
            test.DebugPrintSubPacket();
            session.QueuePacket(test);
        }

    }
}
