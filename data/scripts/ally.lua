require ("global")
require ("magic")
require ("weaponskill")

allyGlobal =
{
}

function allyGlobal.onSpawn(ally, target)

end

function allyGlobal.onEngage(ally, target)

end

function allyGlobal.onAttack(ally, target, damage)

end

function allyGlobal.onDamageTaken(ally, attacker, damage)

end

function allyGlobal.onCombatTick(ally, target, tick, contentGroupCharas)
    allyGlobal.HelpPlayers(ally, contentGroupCharas)
end

function allyGlobal.onDeath(ally, player, lastAttacker)

end

function allyGlobal.onDespawn(ally)

end

function allyGlobal.HelpPlayers(ally, contentGroupCharas, pickRandomTarget)
    if contentGroupCharas then
        print("assssss")
            if chara then
                -- probably a player, or another ally
                -- todo: queue support actions, heal, try pull hate off player etc
                if chara:IsPlayer() then
                    -- do stuff
                    if not ally:IsEngaged() then
                        if chara:IsEngaged() then
                            print("ass")
                            allyGlobal.EngageTarget(ally, chara.target, nil)
                            return true
                        end
                    end
                elseif chara:IsMonster() and chara:IsEngaged() then
                    allyGlobal.EngageTarget(ally, chara, nil)
                    return true
                end
            end
        end
    end
end

function allyGlobal.HealPlayer(ally, player)

end

function allyGlobal.SupportAction(ally, player)

end

function allyGlobal.EngageTarget(ally, target, contentGroupCharas)
    if contentGroupCharas then
        for chara in contentGroupCharas do
            if chara.IsMonster() then
                if chara.allegiance ~= ally.allegiance then
                    ally:Engage(chara)
                end
            end
        end
    elseif target then
        ally:Engage(target)
        ally.hateContainer:AddBaseHate(target);
    end
end