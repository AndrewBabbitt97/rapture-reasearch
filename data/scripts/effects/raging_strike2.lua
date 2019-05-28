require("modifiers")
require("battleutils")


function onCommandStart(effect, attacker, command, actionContainer)
    --Random guess
    command.enmityModifier += 0.25;
end

function onHit(effect, attacker, defender, action, actionContainer)
    if skill.id == 27259 then
        --Effect stacks up to 3 times
        if effect.GetTier() < 3 then
            effect.SetTier(effect.GetTier() + 1);
        end
    end
end

function onMiss(effect, attacker, defender, action, actionContainer)
    if skill.id == 27259 then
        effect.SetTier(0);
    end
end