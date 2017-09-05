﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.actor.battle;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Map_Server.actors.chara.ai.state
{
    class MagicState : State
    {

        private BattleCommand spell;
        private Vector3 startPos;

        public MagicState(Character owner, Character target, ushort spellId) :
            base(owner, target)
        {
            this.startPos = owner.GetPosAsVector3();
            this.startTime = DateTime.Now;
            this.spell = Server.GetWorldManager().GetBattleCommand(spellId);
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicPrepare", owner, target, spell);

            if (returnCode == 0 && owner.CanCast(target, spell))
            {
                OnStart();
            }
            else
            {
                errorResult = new BattleAction(owner.actorId, 32553, 0);
                interrupt = true;
            }
        }

        public override void OnStart()
        {
            var returnCode = lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicStart", owner, target, spell);

            if (returnCode != 0)
            {
                interrupt = true;
                errorResult = new BattleAction(target.actorId, (ushort)(returnCode == -1 ? 32553 : returnCode), 0, 0, 0, 1);
            }
            else
            {
                // todo: check within attack range
                float[] baseCastDuration = { 1.0f, 0.25f };

                float spellSpeed = spell.castTimeSeconds;

                // command casting duration
                if (owner is Player)
                {
                    // todo: modify spellSpeed based on modifiers and stuff
                    ((Player)owner).SendStartCastbar(spell.id, Utils.UnixTimeStampUTC(DateTime.Now.AddSeconds(spellSpeed)));               
                }
                owner.SendChant(0xF, 0x0);
                owner.DoBattleAction(spell.id, 0x6F000002, new BattleAction(target.actorId, 30128, 1, 0, 1)); //You begin casting (6F000002: BLM, 6F000003: WHM)     
            }
        }

        public override bool Update(DateTime tick)
        {
            if (spell != null)
            {
                TryInterrupt();

                if (interrupt)
                {
                    OnInterrupt();
                    return true;
                }

                // todo: check weapon delay/haste etc and use that
                var actualCastTime = spell.castTimeSeconds;

                if ((tick - startTime).TotalSeconds >= spell.castTimeSeconds)
                {
                    OnComplete();
                    return true;
                }
                return false;
            }
            return true;
        }

        public override void OnInterrupt()
        {
            // todo: send paralyzed/sleep message etc.
            if (errorResult != null)
            {
                owner.SendChant(0, 0);
                owner.DoBattleAction(spell.id, errorResult.animation, errorResult);
                errorResult = null;
            }
        }

        public override void OnComplete()
        {
            spell.targetFind.FindWithinArea(target, spell.validTarget, spell.aoeTarget);
            isCompleted = true;

            var targets = spell.targetFind.GetTargets();
            BattleAction[] actions = new BattleAction[targets.Count];
            var i = 0;
            foreach (var chara in targets)
            {
                var action = new BattleAction(target.actorId, spell.worldMasterTextId, spell.battleAnimation, 0, (byte)HitDirection.None, 1);
                action.amount = (ushort)lua.LuaEngine.CallLuaBattleCommandFunction(owner, spell, "magic", "onMagicFinish", owner, chara, spell, action);
                actions[i++] = action;
            }

            // todo: this is fuckin stupid, probably only need *one* error packet, not an error for each action
            var errors = (BattleAction[])actions.Clone();

            owner.OnCast(this, actions, ref errors);
            owner.DoBattleAction(spell.id, spell.battleAnimation, actions);
        }

        public override void TryInterrupt()
        {
            if (interrupt)
                return;

            if (owner.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction))
            {
                // todo: sometimes paralyze can let you attack, get random percentage of actually letting you attack
                var list = owner.statusEffects.GetStatusEffectsByFlag((uint)StatusEffectFlags.PreventAction);
                uint effectId = 0;
                if (list.Count > 0)
                {
                    // todo: actually check proc rate/random chance of whatever effect
                    effectId = list[0].GetStatusEffectId();
                }
                interrupt = true;
                return;
            }

            if (HasMoved())
            {
                errorResult = new BattleAction(owner.actorId, 30211, 0);
                errorResult.animation = 0x7F000002;
                interrupt = true;
                return;
            }

            interrupt = !CanCast();
        }

        private bool CanCast()
        {
            return owner.CanCast(target, spell) && spell.IsValidTarget(owner, target) && !HasMoved();
        }

        private bool HasMoved()
        {
            return (owner.GetPosAsVector3() != startPos);
        }

        public override void Cleanup()
        {
            owner.SendChant(0, 0);

            if (owner is Player)
            {
                ((Player)owner).SendEndCastbar();
            }
            owner.aiContainer.UpdateLastActionTime();
        }

        public BattleCommand GetSpell()
        {
            return spell;
        }
    }
}
