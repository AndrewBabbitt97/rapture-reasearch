﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.actors;
using FFXIVClassic_Map_Server.actors.chara;
using FFXIVClassic_Map_Server.actors.chara.ai;
using FFXIVClassic_Map_Server.actors.chara.ai.controllers;
using FFXIVClassic_Map_Server.packets.send.actor;

namespace FFXIVClassic_Map_Server.Actors
{
    [Flags]
    enum AggroType
    {
        None,
        Sight,
        Scent,
        LowHp,
        IgnoreLevelDifference
    }

    class BattleNpc : Npc
    {
        public HateContainer hateContainer;
        public AggroType aggroType;

        public BattleNpc(int actorNumber, ActorClass actorClass, string uniqueId, Area spawnedArea, float posX, float posY, float posZ, float rot,
            ushort actorState, uint animationId, string customDisplayName)
            : base(actorNumber, actorClass, uniqueId, spawnedArea, posX, posY, posZ, rot, actorState, animationId, customDisplayName)  
        {
            this.aiContainer = new AIContainer(this, new BattleNpcController(this), new PathFind(this), new TargetFind(this));

            this.currentSubState = SetActorStatePacket.SUB_STATE_MONSTER;
            //this.currentMainState = SetActorStatePacket.MAIN_STATE_ACTIVE;

            //charaWork.property[2] = 1;
            //npcWork.hateType = 1;

            this.hateContainer = new HateContainer(this);
            this.allegiance = CharacterTargetingAllegiance.BattleNpcs;
        }

        public override void Update(DateTime tick)
        {
            // todo:
            this.statusEffects.Update(tick);
        }
    }
}
