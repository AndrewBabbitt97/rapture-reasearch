﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.utils;

namespace FFXIVClassic_Map_Server.actors.chara.ai.controllers
{
    class BattleNpcController : Controller
    {
        private DateTime lastActionTime;
        private DateTime lastSpellCastTime;
        private DateTime lastSkillTime;
        private DateTime lastSpecialSkillTime; // todo: i dont think monsters have "2hr" cooldowns like ffxi
        private DateTime deaggroTime;
        private DateTime neutralTime;
        private DateTime waitTime;

        private bool firstSpell = true;
        private DateTime lastRoamUpdate;
        private DateTime battleStartTime;

        private new BattleNpc owner;
        public BattleNpcController(BattleNpc owner) :
            base(owner)
        {
            this.owner = owner;
            this.lastUpdate = DateTime.Now;
            this.waitTime = lastUpdate.AddSeconds(5);
        }

        public override void Update(DateTime tick)
        {
            lastUpdate = tick;
            // todo: handle aggro/deaggro and other shit here
            if (owner.aiContainer.IsEngaged())
            {
                DoCombatTick(tick);
            }
            else if (!owner.IsDead())
            {
                DoRoamTick(tick);
            }
        }

        public bool TryDeaggro()
        {
            if (owner.hateContainer.GetMostHatedTarget() == null || !owner.aiContainer.GetTargetFind().CanTarget(owner.target as Character))
            {
                return true;
            }
            else if (!owner.IsCloseToSpawn())
            {
                return true;
            }
            return false;
        }

        public override bool Engage(Character target)
        {
            // todo: check distance, last swing time, status effects
            var canEngage = this.owner.aiContainer.InternalEngage(target);
            if (canEngage)
            {
                //owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);

                // reset casting
                firstSpell = true;
                // todo: find a better place to put this?
                if (owner.GetState() != SetActorStatePacket.MAIN_STATE_ACTIVE)
                    owner.ChangeState(SetActorStatePacket.MAIN_STATE_ACTIVE);

                owner.moveState = 2;
                lastActionTime = DateTime.Now;
                battleStartTime = lastActionTime;
                // todo: adjust cooldowns with modifiers
            }
            return canEngage;
        }

        private bool TryEngage(Character target)
        {
            // todo:
            return true;
        }

        public override void Disengage()
        {
            var target = owner.target;
            base.Disengage();
            owner.statusEffects.RemoveStatusEffectsByFlags((uint)StatusEffectFlags.LoseOnDeath, true);
            // todo:
            lastActionTime = lastUpdate.AddSeconds(5);
            owner.isMovingToSpawn = true;
            owner.aiContainer.pathFind.SetPathFlags(PathFindFlags.None);
            owner.aiContainer.pathFind.PreparePath(owner.spawnX, owner.spawnY, owner.spawnZ, 1.5f, 10);
            neutralTime = lastActionTime;
            owner.hateContainer.ClearHate();
            owner.ResetMoveSpeeds();
            owner.moveState = 1;
            lua.LuaEngine.CallLuaBattleFunction(owner, "onDisengage", owner, target, Utils.UnixTimeStampUTC(lastUpdate));
        }

        public override void Cast(Character target, uint spellId)
        {
            // todo:
        }

        public override void Ability(Character target, uint abilityId)
        {
            // todo:
        }

        public override void RangedAttack(Character target)
        {
            // todo:
        }

        public override void MonsterSkill(Character target, uint mobSkillId)
        {
            // todo:
        }

        private void DoRoamTick(DateTime tick)
        {
            if (owner.hateContainer.GetHateList().Count > 0)
            {
                Engage(owner.hateContainer.GetMostHatedTarget());
                return;
            }
            //else if (owner.currentLockedTarget != 0)
            //{
            //    ChangeTarget(Server.GetWorldManager().GetActorInWorld(owner.currentLockedTarget).GetAsCharacter());
            //}

            if (tick >= waitTime)
            {
                // todo: aggro cooldown
                neutralTime = tick.AddSeconds(5);
                if (owner.aiContainer.pathFind.IsFollowingPath())
                {
                    owner.aiContainer.pathFind.FollowPath();
                    lastActionTime = tick.AddSeconds(-5);
                }
                else
                {
                    if (tick >= lastActionTime)
                    {
                        
                    }
                }
                // todo:
                waitTime = tick.AddSeconds(10);
                owner.OnRoam(tick);

                if (!owner.aiContainer.pathFind.IsFollowingPath())
                {
                    // will move on next tick
                    owner.aiContainer.pathFind.SetPathFlags(PathFindFlags.None);
                    owner.aiContainer.pathFind.PathInRange(owner.spawnX, owner.spawnY, owner.spawnZ, 1.5f, 20.0f);
                }
            }


            if (tick >= neutralTime)
            {
                foreach (var player in owner.zone.GetActorsAroundActor<Player>(owner, 50))
                {
                    if (!owner.isMovingToSpawn && owner.aiContainer.pathFind.AtPoint() && owner.aggroType != AggroType.None)
                    {
                        uint levelDifference = (uint)Math.Abs(owner.charaWork.parameterSave.state_mainSkillLevel - player.charaWork.parameterSave.state_mainSkillLevel);

                        if (levelDifference <= 10 || (owner.aggroType & AggroType.IgnoreLevelDifference) != 0 && CanAggroTarget(player))
                        {
                            owner.hateContainer.AddBaseHate(player);
                            break;
                        }
                    }
                }
            }

            if (owner.aiContainer.pathFind.IsFollowingPath())
            {
                owner.aiContainer.pathFind.FollowPath();
            }
        }

        private void DoCombatTick(DateTime tick)
        {
            HandleHate();

            // todo: magic/attack/ws cooldowns etc
            if (TryDeaggro())
            {
                Disengage();
                return;
            }

            Move();
            lua.LuaEngine.CallLuaBattleFunction(owner, "onCombatTick", owner, owner.target, Utils.UnixTimeStampUTC(tick));
        }

        private void Move()
        {
            if (!owner.aiContainer.CanFollowPath())
            {
                return;
            }

            if (owner.aiContainer.pathFind.IsFollowingScriptedPath())
            {
                owner.aiContainer.pathFind.FollowPath();
                return;
            }

            var targetPos = new Vector3(owner.target.positionX, owner.target.positionY, owner.target.positionZ);
            var distance = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, targetPos.X, targetPos.Y, targetPos.Z);

            if (distance > owner.GetAttackRange() - 0.2f || owner.aiContainer.CanFollowPath())
            {
                if (CanMoveForward(distance))
                {
                    if (!owner.aiContainer.pathFind.IsFollowingPath() && distance > 3)
                    {
                        // pathfind if too far otherwise jump to target
                        owner.aiContainer.pathFind.SetPathFlags(PathFindFlags.None);
                        owner.aiContainer.pathFind.PreparePath(targetPos, 1.5f, 5);
                    }
                    owner.aiContainer.pathFind.FollowPath();
                    if (!owner.aiContainer.pathFind.IsFollowingPath())
                    {
                        if (owner.target is Player)
                        {
                            foreach (var chara in owner.zone.GetActorsAroundActor<Character>(owner, 1))
                            {
                                if (chara == owner)
                                    continue;

                                float mobDistance = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, chara.positionX, chara.positionY, chara.positionZ);
                                if (mobDistance < 0.50f && (chara.updateFlags & ActorUpdateFlags.Position) == 0)
                                {
                                    owner.aiContainer.pathFind.PathInRange(targetPos, 1.3f, chara.GetAttackRange());
                                    break;
                                }
                            }
                        }
                        FaceTarget();
                    }
                }
            }
            else
            {
                FaceTarget();
            }
        }

        private void FaceTarget()
        {
            // todo: check if stunned etc
            if (owner.statusEffects.HasStatusEffectsByFlag(StatusEffectFlags.PreventAction))
            {
            }
            else
            {
                owner.LookAt(owner.target);
            }
        }

        private bool CanMoveForward(float distance)
        {
            // todo: check spawn leash and stuff
            if (!owner.IsCloseToSpawn())
            {
                return false;
            }
            if (owner.GetSpeed() == 0)
            {
                return false;
            }
            return true;
        }

        public bool CanAggroTarget(Character target)
        {
            if (owner.neutral || owner.aggroType == AggroType.None || owner.IsDead())
            {
                return false;
            }

            // todo: can mobs aggro mounted targets?
            if (target.IsDead() || target.currentMainState == SetActorStatePacket.MAIN_STATE_MOUNTED)
            {
                return false;
            }

            if (owner.aiContainer.IsSpawned() && !owner.aiContainer.IsEngaged() && CanDetectTarget(target))
            {
                return true;
            }
            return false;
        }

        public bool CanDetectTarget(Character target, bool forceSight = false)
        {
            if (owner.IsDead())
                return false;

            // todo: this should probably be changed to only allow detection at end of path?
            if (owner.aiContainer.pathFind.IsFollowingScriptedPath() || owner.aiContainer.pathFind.IsFollowingPath() && !owner.aiContainer.pathFind.AtPoint())
            {
                return false;
            }

            // todo: handle sight/scent/hp etc
            if (target.IsDead() || target.currentMainState == SetActorStatePacket.MAIN_STATE_MOUNTED)
                return false;

            float verticalDistance = Math.Abs(target.positionY - owner.positionY);
            if (verticalDistance > 8)
                return false;

            var distance = Utils.Distance(owner.positionX, owner.positionY, owner.positionZ, target.positionX, target.positionY, target.positionZ);

            bool detectSight = forceSight || (owner.aggroType & AggroType.Sight) != 0;
            bool hasSneak = false;
            bool hasInvisible = false;
            bool isFacing = owner.IsFacing(target);

            // todo: check line of sight and aggroTypes
            if (distance > 20)
            {
                return false;
            }

            // todo: seems ffxiv doesnt even differentiate between sneak/invis?
            {
                hasSneak = target.statusEffects.HasStatusEffectsByFlag((uint)StatusEffectFlags.Stealth);
                hasInvisible = hasSneak;
            }


            if ((owner.aggroType & AggroType.LowHp) != 0 && target.GetHPP() < 75)
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            if (detectSight && !hasInvisible && isFacing)
                return CanSeePoint(target.positionX, target.positionY, target.positionZ);

            return false;
        }

        public bool CanSeePoint(float x, float y, float z)
        {
            return NavmeshUtils.CanSee((Zone)owner.zone, owner.positionX, owner.positionY, owner.positionZ, x, y, z);
        }

        private void HandleHate()
        {
            ChangeTarget(owner.hateContainer.GetMostHatedTarget());
        }

        public override void ChangeTarget(Character target)
        {
            owner.target = target;
            owner.currentLockedTarget = target != null ? target.actorId : 0xC0000000;
            owner.currentTarget = target != null ? target.actorId : 0xC0000000;
            base.ChangeTarget(target);
        }
    }
}
