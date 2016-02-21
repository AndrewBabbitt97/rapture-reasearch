﻿using MySql.Data.MySqlClient;
using Dapper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.utils;
using FFXIVClassic_Lobby_Server.packets;
using FFXIVClassic_Map_Server.packets.send.player;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Map_Server;
using FFXIVClassic_Map_Server.common.EfficientHashTables;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.packets.send.Actor.inventory;
using FFXIVClassic_Map_Server.actors.chara.player;

namespace FFXIVClassic_Lobby_Server
{

    class Database
    {

        public static uint getUserIdFromSession(String sessionId)
        {
            uint id = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand("SELECT * FROM sessions WHERE id = @sessionId AND expiration > NOW()", conn);
                    cmd.Parameters.AddWithValue("@sessionId", sessionId);
                    using (MySqlDataReader Reader = cmd.ExecuteReader())
                    {
                            while (Reader.Read())
                            {
                                id = Reader.GetUInt32("userId");
                            }                        
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }                
            }
            return id;
        }
     
        public static DBWorld getServer(uint serverId)
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                DBWorld world = null;
                try
                {
                    conn.Open();
                    world = conn.Query<DBWorld>("SELECT * FROM servers WHERE id=@ServerId", new {ServerId = serverId}).SingleOrDefault();                  
                }
                catch (MySqlException e)
                {                    
                }
                finally
                {
                    conn.Dispose();
                }

                return world;
            }
        }    

        public static List<Npc> getNpcList()
        {
            using (var conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                List<Npc> npcList = null;
                try
                {
                    conn.Open();
                    npcList = conn.Query<Npc>("SELECT * FROM npc_list").ToList();
                }
                catch (MySqlException e)
                {
                }
                finally
                {
                    conn.Dispose();
                }

                return npcList;
            }
        }

        public static void loadPlayerCharacter(Player player)
        {            
            string query;
            MySqlCommand cmd;            

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    //Load basic info                  
                    query = @"
                    SELECT 
                    name,
                    positionX,
                    positionY,
                    positionZ,
                    rotation,
                    actorState,
                    currentZoneId,
                    currentClassJob,                
                    gcCurrent,
                    gcLimsaRank,
                    gcGridaniaRank,
                    gcUldahRank,
                    currentTitle,
                    guardian,
                    birthDay,
                    birthMonth,
                    initialTown,
                    tribe,
                    currentParty,
                    restBonus,
                    achievementPoints
                    FROM characters WHERE id = @charId";                    

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.displayNameId = 0xFFFFFFFF;
                            player.customDisplayName = reader.GetString(0);
                            player.oldPositionX = player.positionX = reader.GetFloat(1);
                            player.oldPositionY = player.positionY = reader.GetFloat(2);
                            player.oldPositionZ = player.positionZ = reader.GetFloat(3);
                            player.oldRotation = player.rotation = reader.GetFloat(4);
                            player.currentMainState = reader.GetUInt16(5);
                            player.zoneId = reader.GetUInt32(6);
                            player.charaWork.parameterSave.state_mainSkill[0] = reader.GetByte(7);
                            player.gcCurrent = reader.GetByte(8);
                            player.gcRankLimsa = reader.GetByte(9);
                            player.gcRankGridania = reader.GetByte(10);
                            player.gcRankUldah = reader.GetByte(11);
                            player.currentTitle = reader.GetUInt32(12);
                            player.playerWork.guardian = reader.GetByte(13);
                            player.playerWork.birthdayDay = reader.GetByte(14);
                            player.playerWork.birthdayMonth = reader.GetByte(15);
                            player.playerWork.initialTown = reader.GetByte(16);
                            player.playerWork.tribe = reader.GetByte(17);
                            player.playerWork.restBonusExpRate = reader.GetInt32(19);
                            player.achievementPoints = reader.GetUInt32(20);
                        }
                    }

                    player.charaWork.parameterSave.state_mainSkillLevel = 49;

                    /*
                    //Get level of our classjob
                    //Load appearance
                    query = @"
                        SELECT 
                        baseId
                        FROM characters_appearance WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            
                        }

                    }
                    */
                   
                    //Get level of our classjob
                    //Load appearance
                    query = @"
                        SELECT 
                        hp,
                        hpMax,
                        mp,
                        mpMax                        
                        FROM characters_parametersave WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.charaWork.parameterSave.hp[0] = reader.GetInt16(0);
                            player.charaWork.parameterSave.hpMax[0] = reader.GetInt16(1);
                            player.charaWork.parameterSave.mp = reader.GetInt16(2);
                            player.charaWork.parameterSave.mpMax = reader.GetInt16(3);                            
                        }
                    }
                    
                    //Load appearance
                    query = @"
                        SELECT 
                        baseId,                       
                        size,
                        voice,
                        skinColor,
                        hairStyle,
                        hairColor,
                        hairHighlightColor,
                        eyeColor,
                        characteristics,
                        characteristicsColor,
                        faceType,
                        ears,
                        faceMouth,
                        faceFeatures,
                        faceNose,
                        faceEyeShape,
                        faceIrisSize,
                        faceEyebrows,
                        mainHand,
                        offHand,
                        head,
                        body,
                        legs,
                        hands,
                        feet,
                        waist,
                        leftFinger,
                        rightFinger,
                        leftEar,
                        rightEar
                        FROM characters_appearance WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            if (reader.GetUInt32(0) == 0xFFFFFFFF)
                                player.modelId = CharacterUtils.getTribeModel(player.playerWork.tribe);
                            else
                                player.modelId = reader.GetUInt32(0);
                            player.appearanceIds[Character.SIZE] = reader.GetByte(1);
                            player.appearanceIds[Character.COLORINFO] = (uint)(reader.GetUInt16(3) | (reader.GetUInt16(5) << 10) | (reader.GetUInt16(7) << 20));
                            player.appearanceIds[Character.FACEINFO] = PrimitiveConversion.ToUInt32(CharacterUtils.getFaceInfo(reader.GetByte(8), reader.GetByte(9), reader.GetByte(10), reader.GetByte(11), reader.GetByte(12), reader.GetByte(13), reader.GetByte(14), reader.GetByte(15), reader.GetByte(16), reader.GetByte(17)));
                            player.appearanceIds[Character.HIGHLIGHT_HAIR] = (uint)(reader.GetUInt16(6) | reader.GetUInt16(4) << 10);
                            player.appearanceIds[Character.VOICE] = reader.GetByte(2);
                            player.appearanceIds[Character.WEAPON1] = reader.GetUInt32(18);
                            player.appearanceIds[Character.WEAPON2] = reader.GetUInt32(19);
                            player.appearanceIds[Character.HEADGEAR] = reader.GetUInt32(20);
                            player.appearanceIds[Character.BODYGEAR] = reader.GetUInt32(21);
                            player.appearanceIds[Character.LEGSGEAR] = reader.GetUInt32(22);
                            player.appearanceIds[Character.HANDSGEAR] = reader.GetUInt32(23);
                            player.appearanceIds[Character.FEETGEAR] = reader.GetUInt32(24);
                            player.appearanceIds[Character.WAISTGEAR] = reader.GetUInt32(25);
                            player.appearanceIds[Character.R_EAR] = reader.GetUInt32(26);
                            player.appearanceIds[Character.L_EAR] = reader.GetUInt32(27);
                            player.appearanceIds[Character.R_FINGER] = reader.GetUInt32(28);
                            player.appearanceIds[Character.L_FINGER] = reader.GetUInt32(29);
                        }

                    }

                    //Load Status Effects
                    query = @"
                        SELECT 
                        statusId,
                        expireTime                     
                        FROM characters_statuseffect WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            player.charaWork.status[count] = reader.GetUInt16(0);
                            player.charaWork.statusShownTime[count] = reader.GetUInt32(1);
                        }
                    }

                    //Load Chocobo
                    query = @"
                        SELECT 
                        hasChocobo,
                        hasGoobbue,
                        chocoboAppearance,
                        chocoboName                             
                        FROM characters_chocobo WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            player.hasChocobo = reader.GetBoolean(0);
                            player.hasGoobbue = reader.GetBoolean(1);
                            player.chocoboAppearance = reader.GetByte(2);
                            player.chocoboName = reader.GetString(3);
                        }
                    }

                    //Load Timers
                    query = @"
                        SELECT 
                        thousandmaws,
                        dzemaeldarkhold,
                        bowlofembers_hard,                
                        bowlofembers,
                        thornmarch,
                        aurumvale,
                        cutterscry,
                        battle_aleport,
                        battle_hyrstmill,
                        battle_goldenbazaar,
                        howlingeye_hard,
                        howlingeye,
                        castrumnovum,
                        bowlofembers_extreme,
                        rivenroad,
                        rivenroad_hard,
                        behests,
                        companybehests,
                        returntimer,
                        skirmish
                        FROM characters_timers WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            for (int i = 0; i < player.timers.Length; i++)
                                player.timers[i] = reader.GetUInt32(i);
                        }
                    }
                   
                    //Load Hotbar
                    query = @"
                        SELECT 
                        hotbarSlot,
                        commandId,
                        recastTime                
                        FROM characters_hotbar WHERE characterId = @charId AND classId = @classId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@classId", player.charaWork.parameterSave.state_mainSkill[0]);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {                        
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.charaWork.command[index+32] = reader.GetUInt32(1);
                            player.charaWork.parameterSave.commandSlot_recastTime[index] = reader.GetUInt32(2);
                        }
                    }

                    //Load Scenario Quests
                    query = @"
                        SELECT 
                        slot,
                        questId                       
                        FROM characters_quest_scenario WHERE characterId = @charId";
                   
                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.playerWork.questScenario[index] = 0xA0F00000 | reader.GetUInt32(1);
                        }
                    }

                    //Load Local Guildleves
                    query = @"
                        SELECT 
                        slot,
                        questId,
                        abandoned,
                        completed  
                        FROM characters_quest_guildleve_local WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.playerWork.questGuildleve[index] = 0xA0F00000 | reader.GetUInt32(1);
                        }
                    }

                    //Load Regional Guildleve Quests
                    query = @"
                        SELECT 
                        slot,
                        guildleveId,
                        abandoned,
                        completed  
                        FROM characters_quest_guildleve_regional WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int index = reader.GetUInt16(0);
                            player.work.guildleveId[index] = reader.GetUInt16(1);
                            player.work.guildleveDone[index] = reader.GetBoolean(2);
                            player.work.guildleveChecked[index] = reader.GetBoolean(3);
                        }
                    }

                    //Load NPC Linkshell
                    query = @"
                        SELECT 
                        npcLinkshellId,
                        isCalling,
                        isExtra  
                        FROM characters_npclinkshell WHERE characterId = @charId";

                    cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int npcLSId = reader.GetUInt16(0);
                            player.playerWork.npcLinkshellChatCalling[npcLSId] = reader.GetBoolean(1);
                            player.playerWork.npcLinkshellChatExtra[npcLSId] = reader.GetBoolean(2);                            
                        }
                    }

                    player.getInventory(Inventory.NORMAL).initList(getInventory(player, 0, Inventory.NORMAL));
                    player.getInventory(Inventory.KEYITEMS).initList(getInventory(player, 0, Inventory.KEYITEMS));
                    player.getInventory(Inventory.CURRANCY).initList(getInventory(player, 0, Inventory.CURRANCY));
                    player.getInventory(Inventory.BAZAAR).initList(getInventory(player, 0, Inventory.BAZAAR));
                    player.getInventory(Inventory.MELDREQUEST).initList(getInventory(player, 0, Inventory.MELDREQUEST));
                    player.getInventory(Inventory.LOOT).initList(getInventory(player, 0, Inventory.LOOT));

                    player.getEquipment().SetEquipment(getEquipment(player));
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

        }

        public static List<Tuple<ushort, InventoryItem>> getEquipment(Player player)
        {
            List<Tuple<ushort, InventoryItem>> equipment = new List<Tuple<ushort, InventoryItem>>();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    equipSlot,
                                    itemSlot
                                    FROM characters_inventory_equipment                                    
                                    WHERE characterId = @charId ORDER BY equipSlot";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ushort equipSlot = reader.GetUInt16(0);
                            ushort itemSlot = reader.GetUInt16(1);
                            InventoryItem item = player.getInventory(Inventory.NORMAL).getItem(itemSlot);
                            equipment.Add(new Tuple<ushort, InventoryItem>(equipSlot, item));
                        }
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

            return equipment;
        }

        public static void equipItem(Player player, ushort equipSlot, ushort itemSlot)
        {

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    INSERT INTO characters_inventory_equipment                                    
                                    (characterId, equipSlot, itemSlot)
                                    VALUES
                                    (@characterId, @equipSlot, @itemSlot)
                                    ON DUPLICATE KEY UPDATE itemSlot=@itemSlot;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@equipSlot", equipSlot);
                    cmd.Parameters.AddWithValue("@itemSlot", itemSlot);

                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

        }

        public static void unequipItem(Player player, ushort equipSlot)
        {

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    DELETE FROM characters_inventory_equipment                                    
                                    WHERE characterId = @characterId AND equipSlot = @equipSlot;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@characterId", player.actorId);
                    cmd.Parameters.AddWithValue("@equipSlot", equipSlot);

                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

        }

        public static List<InventoryItem> getInventory(Player player, uint slotOffset, uint type)
        {
            List<InventoryItem> items = new List<InventoryItem>();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT
                                    serverItemId,
                                    itemId,
                                    quantity,
                                    slot,
                                    isUntradeable,
                                    quality,
                                    durability,
                                    spiritBind,
                                    materia1,
                                    materia2,
                                    materia3,
                                    materia4,
                                    materia5
                                    FROM characters_inventory
                                    INNER JOIN server_items ON serverItemId = server_items.id
                                    WHERE characterId = @charId AND inventoryType = @type AND slot >= @slot ORDER BY slot";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@slot", slotOffset);
                    cmd.Parameters.AddWithValue("@type", type);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {                        
                        while (reader.Read())
                        {
                            uint uniqueId = reader.GetUInt32(0);
                            uint itemId = reader.GetUInt32(1);
                            int quantity = reader.GetInt32(2);
                            ushort slot = reader.GetUInt16(3);

                            bool isUntradeable = reader.GetBoolean(4);
                            byte qualityNumber = reader.GetByte(5);

                            uint durability = reader.GetUInt32(6);
                            ushort spiritBind = reader.GetUInt16(7);

                            byte materia1 = reader.GetByte(8);
                            byte materia2 = reader.GetByte(9);
                            byte materia3 = reader.GetByte(10);
                            byte materia4 = reader.GetByte(11);
                            byte materia5 = reader.GetByte(12);

                            items.Add(new InventoryItem(uniqueId, itemId, quantity, slot, isUntradeable, qualityNumber, durability, spiritBind, materia1, materia2, materia3, materia4, materia5));
                        }
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

            return items;
        }

        public static InventoryItem addItem(Player player, uint itemId, int quantity, byte quality, bool isUntradeable, uint durability, ushort type)
        {
            InventoryItem insertedItem = null;

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    

                    string query = @"
                                    INSERT INTO server_items                                    
                                    (itemId, quality, isUntradeable, durability)
                                    VALUES
                                    (@itemId, @quality, @isUntradeable, @durability); 
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    string query2 = @"
                                    INSERT INTO characters_inventory
                                    (characterId, slot, inventoryType, serverItemId, quantity)
                                    SELECT @charId, IFNULL(MAX(SLOT)+1, 0), @inventoryType, LAST_INSERT_ID(), @quantity FROM characters_inventory WHERE characterId = @charId AND inventoryType = @inventoryType;
                                    ";

                    MySqlCommand cmd2 = new MySqlCommand(query2, conn);
                    
                    cmd.Parameters.AddWithValue("@itemId", itemId);
                    cmd.Parameters.AddWithValue("@quality", quality);
                    cmd.Parameters.AddWithValue("@isUntradeable", isUntradeable);
                    cmd.Parameters.AddWithValue("@durability", durability);

                    cmd2.Parameters.AddWithValue("@charId", player.actorId);
                    cmd2.Parameters.AddWithValue("@inventoryType", type);
                    cmd2.Parameters.AddWithValue("@quantity", quantity);

                    cmd.ExecuteNonQuery();
                    cmd2.ExecuteNonQuery();

                    insertedItem = new InventoryItem((uint)cmd.LastInsertedId, itemId, quantity, (ushort)player.getInventory(type).getNextEmptySlot(), isUntradeable, quality, durability, 0, 0, 0, 0, 0, 0);
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

            return insertedItem;
        }

        public static void setQuantity(Player player, uint slot, ushort type, int quantity)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                     string query = @"
                                    UPDATE characters_inventory
                                    SET quantity = @quantity
                                    WHERE characterId = @charId AND slot = @slot AND inventoryType = @type;
                                    ";
                    
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@quantity", quantity);
                    cmd.Parameters.AddWithValue("@slot", slot);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

        }

        public static void removeItem(Player player, ulong serverItemId, ushort type)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}; Allow User Variables=True", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT slot INTO @slotToDelete FROM characters_inventory WHERE serverItemId = @serverItemId;
                                    UPDATE characters_inventory
                                    SET slot = slot - 1
                                    WHERE characterId = @charId AND slot > @slotToDelete AND inventoryType = @type;

                                    DELETE FROM characters_inventory
                                    WHERE serverItemId = @serverItemId AND inventoryType = @type;

                                    DELETE FROM server_items
                                    WHERE id = @serverItemId;
                                    ";                    

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@serverItemId", serverItemId);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

        }

        public static void removeItem(Player player, ushort slot, ushort type)
        {
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}; Allow User Variables=True", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT serverItemId INTO @serverItemId FROM characters_inventory WHERE characterId = @charId AND slot = @slot;

                                    DELETE FROM characters_inventory
                                    WHERE characterId = @charId AND slot = @slot AND inventoryType = @type;

                                    DELETE FROM server_items
                                    WHERE id = @serverItemId;

                                    UPDATE characters_inventory
                                    SET slot = slot - 1
                                    WHERE characterId = @charId AND slot > @slot AND inventoryType = @type;
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    cmd.Parameters.AddWithValue("@slot", slot);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.ExecuteNonQuery();

                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

        }

        public static SubPacket getLatestAchievements(Player player)
        {
            uint[] latestAchievements = new uint[5];
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                 
                    //Load Last 5 Completed
                    string query = @"
                                    SELECT 
                                    characters_achievements.achievementId FROM characters_achievements 
                                    INNER JOIN gamedata_achievements ON characters_achievements.achievementId = gamedata_achievements.achievementId
                                    WHERE characterId = @charId AND rewardPoints <> 0 ORDER BY timeDone LIMIT 5";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        int count = 0;
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0);                           
                            latestAchievements[count++] = id;
                        }
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

            return SetLatestAchievementsPacket.buildPacket(player.actorId, latestAchievements);
        }

        public static SubPacket getAchievementsPacket(Player player)
        {
            SetCompletedAchievementsPacket cheevosPacket = new SetCompletedAchievementsPacket();

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();
                 
                    string query = @"
                                    SELECT packetOffsetId 
                                    FROM characters_achievements 
                                    INNER JOIN gamedata_achievements ON characters_achievements.achievementId = gamedata_achievements.achievementId
                                    WHERE characterId = @charId AND timeDone IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@charId", player.actorId);
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {                            
                            uint offset = reader.GetUInt32(0);

                            if (offset < 0 || offset >= cheevosPacket.achievementFlags.Length)
                            {
                                Log.error("SQL Error; achievement flag offset id out of range: " + offset);
                                continue;
                            }
                            cheevosPacket.achievementFlags[offset] = true;                             
                        }
                    }
                }
                catch (MySqlException e)
                { Console.WriteLine(e); }
                finally
                {
                    conn.Dispose();
                }
            }

            return cheevosPacket.buildPacket(player.actorId);
        }

    }
}
