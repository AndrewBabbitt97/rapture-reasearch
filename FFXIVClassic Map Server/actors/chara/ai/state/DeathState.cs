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

using System;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class DeathState : State
    {
        DateTime despawnTime;
        public DeathState(Character owner, DateTime tick, uint timeToFadeOut) 
            : base(owner, null)
        {
            owner.Disengage();
            owner.ChangeState(SetActorStatePacket.MAIN_STATE_DEAD);
            owner.statusEffects.RemoveStatusEffectsByFlags((uint)StatusEffectFlags.LoseOnDeath);
            //var deathStatePacket = SetActorStatePacket.BuildPacket(owner.actorId, SetActorStatePacket.MAIN_STATE_DEAD2, owner.currentSubState);
            //owner.zone.BroadcastPacketAroundActor(owner, deathStatePacket);
            canInterrupt = false;
            startTime = tick;
            despawnTime = startTime.AddSeconds(timeToFadeOut);
        }

        public override bool Update(DateTime tick)
        {
            // todo: set a flag on chara for accept raise, play animation and spawn
            if (owner.GetMod((uint)Modifier.Raise) > 0)
            {
                owner.Spawn(tick);
                return true;
            }

            // todo: handle raise etc
            if (tick >= despawnTime)
            {
                // todo: for players, return them to homepoint
                owner.Despawn(tick);
                return true;
            }
            return false;
        }
    }
}
