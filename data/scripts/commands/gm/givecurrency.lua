require("global");

properties = {
    permissions = 0,
    parameters = "sss",
    description = "adds <currency> to self or <target>.",
}

function onTrigger(player, argc, currency, name, lastName)
    local sender = "[givecurrency] ";
    
    if name then
        if lastName then
            player = GetWorldManager():GetPCInWorld(name.." "..lastName) or nil;
        else
            player = GetWorldManager():GetPCInWorld(name) or nil;
        end;
    end;
    
    if player then
        currency = tonumber(currency) or nil;
        qty = 1;
        location = INVENTORY_CURRENCY;
        
        local added = player:GetInventory(location):AddItem(currency, qty);
        local messageID = MESSAGE_TYPE_SYSTEM_ERROR;
        local message = "unable to add currency";
        
        if currency and added then
            message = string.format("added currency %u to %s", currency, player:GetName());
        end
        player:SendMessage(messageID, sender, message);
        print(message);
    else
        print(sender.."unable to add currency, ensure player name is valid.");
    end;
end;