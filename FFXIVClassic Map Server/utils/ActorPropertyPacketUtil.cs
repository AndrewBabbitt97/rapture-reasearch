﻿using FFXIVClassic_Lobby_Server.packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.dataobjects;

namespace FFXIVClassic_Map_Server.utils
{
    class ActorPropertyPacketUtil
    {
        Actor forActor;
        uint playerActorId;
        List<SubPacket> subPackets = new List<SubPacket>();
        SetActorPropetyPacket currentActorPropertyPacket;
        string currentTarget;

        public ActorPropertyPacketUtil(string firstTarget, Actor forActor, uint playerActorId)
        {
            currentActorPropertyPacket = new SetActorPropetyPacket(firstTarget);
            this.forActor = forActor;
            this.playerActorId = playerActorId;
            this.currentTarget = firstTarget;
        }

        public void addProperty(string property)
        {
            if (!currentActorPropertyPacket.addProperty(forActor, property))
            {
                currentActorPropertyPacket.setIsMore(true);
                currentActorPropertyPacket.addTarget();
                subPackets.Add(currentActorPropertyPacket.buildPacket(playerActorId, forActor.actorId));
                currentActorPropertyPacket = new SetActorPropetyPacket(currentTarget);
            }
        }

        public BasePacket done()
        {
            currentActorPropertyPacket.addTarget();
            currentActorPropertyPacket.setIsMore(false);
            subPackets.Add(currentActorPropertyPacket.buildPacket(playerActorId, forActor.actorId));
            return BasePacket.createPacket(subPackets, true, false);
        }

    }
}
