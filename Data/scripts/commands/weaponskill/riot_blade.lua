require("global");
require("weaponskill");

function onSkillPrepare(caster, target, skill)
    return 0;
end;

function onSkillStart(caster, target, skill)
    return 0;
end;

--Chance to decrease defense when executed from behind the target
function onPositional(caster, target, skill)
    skill.statusChance = 0.75;
    skill.statusMagnitude = 100;
end;

function onSkillFinish(caster, target, skill, action, actionContainer)
    --calculate ws damage
    action.amount = skill.basePotency;

    --DoAction handles rates, buffs, dealing damage
    action.DoAction(caster, target, skill, actionContainer);

    --Try to apply status effect
    action.TryStatus(caster, target, skill, actionContainer, true);
end;