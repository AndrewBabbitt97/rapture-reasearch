﻿using FFXIVClassic_Map_Server;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.actors.area;
using FFXIVClassic_Map_Server.actors.chara.npc;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic.Common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.lua;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server.packets.send.login;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFXIVClassic_Map_Server.actors.group;
using FFXIVClassic_Map_Server.packets.send.group;
using FFXIVClassic_Map_Server.packets.WorldPackets.Receive;
using FFXIVClassic_Map_Server.packets.WorldPackets.Send.Group;
using System.Threading;
using System.Diagnostics;
using FFXIVClassic_Map_Server.actors.director;
using FFXIVClassic_Map_Server.actors.chara.player;

namespace FFXIVClassic_Map_Server
{
    class WorldManager
    {
        private DebugProg debug = new DebugProg();
        private WorldMaster worldMaster = new WorldMaster();
        private Dictionary<uint, Zone> zoneList;
        private Dictionary<uint, List<SeamlessBoundry>> seamlessBoundryList;
        private Dictionary<uint, ZoneEntrance> zoneEntranceList;
        private Dictionary<uint, ActorClass> actorClasses = new Dictionary<uint,ActorClass>();
        private Dictionary<ulong, Party> currentPlayerParties = new Dictionary<ulong, Party>(); //GroupId, Party object

        private Server mServer;

        private const int MILIS_LOOPTIME = 10;
        private Timer mZoneTimer;

        //Zone Server Groups
        public Dictionary<ulong, Group> mContentGroups = new Dictionary<ulong, Group>();
        public Dictionary<ulong, RelationGroup> mRelationGroups = new Dictionary<ulong, RelationGroup>();
        public Dictionary<ulong, TradeGroup> mTradeGroups = new Dictionary<ulong, TradeGroup>();
        private Object groupLock = new Object();
        private Object tradeLock = new Object();
        public ulong groupIndexId = 1;

        public WorldManager(Server server)
        {
            mServer = server;
        }

        public void LoadZoneList()
        {
            zoneList = new Dictionary<uint, Zone>();
            int count1 = 0;
            int count2 = 0;
            
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    zoneName,
                                    regionId,
                                    classPath,
                                    dayMusic,
                                    nightMusic,
                                    battleMusic,
                                    isIsolated,
                                    isInn,
                                    canRideChocobo,
                                    canStealth,
                                    isInstanceRaid
                                    FROM server_zones
                                    WHERE zoneName IS NOT NULL and serverIp = @ip and serverPort = @port";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    cmd.Parameters.AddWithValue("@ip", ConfigConstants.OPTIONS_BINDIP);
                    cmd.Parameters.AddWithValue("@port", ConfigConstants.OPTIONS_PORT);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Zone zone = new Zone(reader.GetUInt32(0), reader.GetString(1), reader.GetUInt16(2), reader.GetString(3), reader.GetUInt16(4), reader.GetUInt16(5), reader.GetUInt16(6), reader.GetBoolean(7), reader.GetBoolean(8), reader.GetBoolean(9), reader.GetBoolean(10), reader.GetBoolean(11));
                            zoneList[zone.actorId] = zone;
                            count1++;
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

            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    parentZoneId,
                                    privateAreaName,
                                    privateAreaType,
                                    className,
                                    dayMusic,
                                    nightMusic,
                                    battleMusic
                                    FROM server_zones_privateareas
                                    WHERE privateAreaName IS NOT NULL";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint parentZoneId = reader.GetUInt32("parentZoneId");

                            if (zoneList.ContainsKey(parentZoneId))
                            {
                                Zone parent = zoneList[parentZoneId];
                                PrivateArea privArea = new PrivateArea(parent, reader.GetUInt32("id"), reader.GetString("className"), reader.GetString("privateAreaName"), reader.GetUInt32("privateAreaType"), reader.GetUInt16("dayMusic"), reader.GetUInt16("nightMusic"), reader.GetUInt16("battleMusic"));
                                parent.AddPrivateArea(privArea);
                            }
                            else
                                continue;
      
                            count2++;
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

            Program.Log.Info(String.Format("Loaded {0} zones and {1} private areas.", count1, count2));
        }

        public void LoadZoneEntranceList()
        {
            zoneEntranceList = new Dictionary<uint, ZoneEntrance>();
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    id,
                                    zoneId,
                                    spawnType,
                                    spawnX,
                                    spawnY,
                                    spawnZ,
                                    spawnRotation,
                                    privateAreaName
                                    FROM server_zones_spawnlocations";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32(0);
                            string privArea = null;

                            if (!reader.IsDBNull(7))
                                privArea = reader.GetString(7);

                            ZoneEntrance entance = new ZoneEntrance(reader.GetUInt32(1), privArea, 1, reader.GetByte(2), reader.GetFloat(3), reader.GetFloat(4), reader.GetFloat(5), reader.GetFloat(6));
                            zoneEntranceList[id] = entance;
                            count++;
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

            Program.Log.Info(String.Format("Loaded {0} zone spawn locations.", count));
        }

        public void LoadSeamlessBoundryList()
        {
            seamlessBoundryList = new Dictionary<uint, List<SeamlessBoundry>>();
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    *
                                    FROM server_seamless_zonechange_bounds";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32("id");
                            uint regionId = reader.GetUInt32("regionId");
                            uint zoneId1 = reader.GetUInt32("zoneId1");
                            uint zoneId2 = reader.GetUInt32("zoneId2");

                            float z1_x1 = reader.GetFloat("zone1_boundingbox_x1");
                            float z1_y1 = reader.GetFloat("zone1_boundingbox_y1");
                            float z1_x2 = reader.GetFloat("zone1_boundingbox_x2");
                            float z1_y2 = reader.GetFloat("zone1_boundingbox_y2");

                            float z2_x1 = reader.GetFloat("zone2_boundingbox_x1");
                            float z2_y1 = reader.GetFloat("zone2_boundingbox_y1");
                            float z2_x2 = reader.GetFloat("zone2_boundingbox_x2");
                            float z2_y2 = reader.GetFloat("zone2_boundingbox_y2");

                            float m_x1 = reader.GetFloat("merge_boundingbox_x1");
                            float m_y1 = reader.GetFloat("merge_boundingbox_y1");
                            float m_x2 = reader.GetFloat("merge_boundingbox_x2");
                            float m_y2 = reader.GetFloat("merge_boundingbox_y2");
                            
                            if (!seamlessBoundryList.ContainsKey(regionId))
                                seamlessBoundryList.Add(regionId, new List<SeamlessBoundry>());

                            seamlessBoundryList[regionId].Add(new SeamlessBoundry(regionId, zoneId1, zoneId2, z1_x1, z1_y1, z1_x2, z1_y2, z2_x1, z2_y1, z2_x2, z2_y2, m_x1, m_y1, m_x2, m_y2));

                            count++;
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

            Program.Log.Info(String.Format("Loaded {0} region seamless boundries.", count));
        }

        public void LoadActorClasses()
        {            
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    gamedata_actor_class.id,
                                    classPath,                                    
                                    displayNameId,
                                    propertyFlags,
                                    eventConditions,
                                    pushCommand,
                                    pushCommandSub,
                                    pushCommandPriority
                                    FROM gamedata_actor_class
                                    LEFT JOIN gamedata_actor_pushcommand
                                    ON gamedata_actor_class.id = gamedata_actor_pushcommand.id
                                    WHERE classPath <> ''
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            uint id = reader.GetUInt32("id");
                            string classPath = reader.GetString("classPath");
                            uint nameId = reader.GetUInt32("displayNameId");
                            string eventConditions = null;

                            uint propertyFlags = reader.GetUInt32("propertyFlags");

                            if (!reader.IsDBNull(4))
                                eventConditions = reader.GetString("eventConditions");
                            else
                                eventConditions = "{}";

                            ushort pushCommand = 0;
                            ushort pushCommandSub = 0;
                            byte pushCommandPriority = 0;

                            if (!reader.IsDBNull(reader.GetOrdinal("pushCommand")))
                            {
                                pushCommand = reader.GetUInt16("pushCommand");
                                pushCommandSub = reader.GetUInt16("pushCommandSub");
                                pushCommandPriority = reader.GetByte("pushCommandPriority");
                            }

                            ActorClass actorClass = new ActorClass(id, classPath, nameId, propertyFlags, eventConditions, pushCommand, pushCommandSub, pushCommandPriority);
                            actorClasses.Add(id, actorClass);
                            count++;
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

            Program.Log.Info(String.Format("Loaded {0} actor classes.", count));
        }

        public void LoadSpawnLocations()
        {
            int count = 0;
            using (MySqlConnection conn = new MySqlConnection(String.Format("Server={0}; Port={1}; Database={2}; UID={3}; Password={4}", ConfigConstants.DATABASE_HOST, ConfigConstants.DATABASE_PORT, ConfigConstants.DATABASE_NAME, ConfigConstants.DATABASE_USERNAME, ConfigConstants.DATABASE_PASSWORD)))
            {
                try
                {
                    conn.Open();

                    string query = @"
                                    SELECT 
                                    actorClassId,  
                                    uniqueId,                                  
                                    zoneId,      
                                    privateAreaName,                              
                                    privateAreaLevel,
                                    positionX,
                                    positionY,
                                    positionZ,
                                    rotation,
                                    actorState,
                                    animationId,
                                    customDisplayName
                                    FROM server_spawn_locations                                    
                                    ";

                    MySqlCommand cmd = new MySqlCommand(query, conn);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {                            
                            uint zoneId = reader.GetUInt32("zoneId");
                            uint classId = reader.GetUInt32("actorClassId");
                            if (!actorClasses.ContainsKey(classId))
                                continue;
                            if (!zoneList.ContainsKey(zoneId))
                                continue;
                            Zone zone = zoneList[zoneId];
                            if (zone == null)
                                continue;

                            string customName = null;
                            if (!reader.IsDBNull(11))
                                customName = reader.GetString("customDisplayName");
                            string uniqueId = reader.GetString("uniqueId");                          
                            string privAreaName = reader.GetString("privateAreaName");
                            uint privAreaLevel = reader.GetUInt32("privateAreaLevel");
                            float x = reader.GetFloat("positionX");
                            float y = reader.GetFloat("positionY");
                            float z = reader.GetFloat("positionZ");
                            float rot = reader.GetFloat("rotation");
                            ushort state = reader.GetUInt16("actorState");
                            uint animId = reader.GetUInt32("animationId");
                            
                            SpawnLocation spawn = new SpawnLocation(classId, uniqueId, zoneId, privAreaName, privAreaLevel, x, y, z, rot, state, animId);

                            zone.AddSpawnLocation(spawn);

                            count++;
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

            Program.Log.Info(String.Format("Loaded {0} spawn(s).", count));
        }

        public void SpawnAllActors()
        {
            Program.Log.Info("Spawning actors...");
            foreach (Zone z in zoneList.Values)
                z.SpawnAllActors(true);
        }

        //Moves the actor to the new zone if exists. No packets are sent nor position changed. Merged zone is removed.
        public void DoSeamlessZoneChange(Player player, uint destinationZoneId)
        {
            Area oldZone;

            if (player.zone != null)
            {
                oldZone = player.zone;
                oldZone.RemoveActorFromZone(player);
            }

            //Add player to new zone and update
            Zone newZone = GetZone(destinationZoneId);

            //This server does not contain that zoneId
            if (newZone == null)
                return;

            newZone.AddActorToZone(player);

            player.zone = newZone;
            player.zoneId = destinationZoneId;

            player.zone2 = null;
            player.zoneId2 = 0;

            player.SendSeamlessZoneInPackets();

            player.SendMessage(0x20, "", "Doing Seamless Zone Change");

            LuaEngine.GetInstance().CallLuaFunction(player, newZone, "onZoneIn", true);
        }

        //Adds a second zone to pull actors from. Used for an improved seamless zone change.
        public void MergeZones(Player player, uint mergedZoneId)
        {
            //Add player to new zone and update
            Zone mergedZone = GetZone(mergedZoneId);

            //This server does not contain that zoneId
            if (mergedZone == null)
                return;

            mergedZone.AddActorToZone(player);

            player.zone2 = mergedZone;
            player.zoneId2 = mergedZone.actorId;

            player.SendMessage(0x20, "", "Merging Zones");

            LuaEngine.GetInstance().CallLuaFunction(player, mergedZone, "onZoneIn", true);
        }

        //Checks all seamless bounding boxes in region to see if player needs to merge or zonechange
        public void SeamlessCheck(Player player)
        {
            //Check if you are in a seamless bounding box
            //WorldMaster.DoSeamlessCheck(this) -- Return 

            /*
             * Find what bounding box in region I am in
             * ->If none, ignore
             * ->If zone box && is my zone, ignore
             * ->If zone box && is not my zone, DoSeamlessZoneChange
             * ->If merge box, MergeZones
             */

            if (player.zone == null)
                return;

            uint regionId = player.zone.regionId;

            if (!seamlessBoundryList.ContainsKey(regionId))
                return;

            foreach (SeamlessBoundry bounds in seamlessBoundryList[regionId])
            {
                if (CheckPosInBounds(player.positionX, player.positionZ, bounds.zone1_x1, bounds.zone1_y1, bounds.zone1_x2, bounds.zone1_y2))
                {
                    if (player.zoneId == bounds.zoneId1 && player.zoneId2 == 0)
                        return;

                    DoSeamlessZoneChange(player, bounds.zoneId1);
                }
                else if (CheckPosInBounds(player.positionX, player.positionZ, bounds.zone2_x1, bounds.zone2_y1, bounds.zone2_x2, bounds.zone2_y2))
                {
                    if (player.zoneId == bounds.zoneId2 && player.zoneId2 == 0)
                        return;

                    DoSeamlessZoneChange(player, bounds.zoneId2);
                }
                else if (CheckPosInBounds(player.positionX, player.positionZ, bounds.merge_x1, bounds.merge_y1, bounds.merge_x2, bounds.merge_y2))
                {
                    uint merged;
                    if (player.zoneId == bounds.zoneId1)
                        merged = bounds.zoneId2;
                    else
                        merged = bounds.zoneId1;

                    //Already merged
                    if (player.zoneId2 == merged)
                        return;

                    MergeZones(player, merged);
                }
            }
        }

        public bool CheckPosInBounds(float x, float y, float x1, float y1, float x2, float y2)
        {
            bool xIsGood = false;
            bool yIsGood = false;

            if ((x1 < x && x < x2) || (x1 > x && x > x2))
                xIsGood = true;

            if ((y1 < y && y < y2) || (y1 > y && y > y2))
                yIsGood = true;

            return xIsGood && yIsGood;
        }

        //Moves actor to new zone, and sends packets to spawn at the given zone entrance
        public void DoZoneChange(Player player, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Program.Log.Error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];
            DoZoneChange(player, ze.zoneId, ze.privateAreaName, ze.privateAreaType, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation);
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneChange(Player player, uint destinationZoneId, string destinationPrivateArea, int destinationPrivateAreaType, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {       
            //Add player to new zone and update
            Area newArea;

            if (destinationPrivateArea == null)
                newArea = GetZone(destinationZoneId);
            else //Add check for -1 if it is a instance
                newArea = GetZone(destinationZoneId).GetPrivateArea(destinationPrivateArea, (uint)destinationPrivateAreaType);

            //This server does not contain that zoneId
            if (newArea == null)
            {
                Program.Log.Debug("Request to change to zone not on this server by: {0}.", player.customDisplayName);
                RequestWorldServerZoneChange(player, destinationZoneId, spawnType, spawnX, spawnY, spawnZ, spawnRotation);
                return;
            }

            player.playerSession.LockUpdates(true);

            Area oldZone = player.zone;
            //Remove player from currentZone if transfer else it's login
            if (player.zone != null)
            {
                oldZone.RemoveActorFromZone(player);
            }

            newArea.AddActorToZone(player);

            //Update player actor's properties
            player.zoneId = newArea is PrivateArea ? ((PrivateArea)newArea).GetParentZone().actorId : newArea.actorId;

            player.privateArea = newArea is PrivateArea ? ((PrivateArea)newArea).GetPrivateAreaName() : null;
            player.privateAreaType = newArea is PrivateArea ? ((PrivateArea)newArea).GetPrivateAreaType() : 0;
            player.zone = newArea;
            player.positionX = spawnX;
            player.positionY = spawnY;
            player.positionZ = spawnZ;
            player.rotation = spawnRotation;

            //Delete any GL directors
            GuildleveDirector glDirector = player.GetGuildleveDirector();
            if (glDirector != null)
                player.RemoveDirector(glDirector);

            //Delete content if have
            if (player.currentContentGroup != null)
            {
                player.currentContentGroup.RemoveMember(player.actorId);
                player.SetCurrentContentGroup(null);

                if (oldZone is PrivateAreaContent)
                    ((PrivateAreaContent)oldZone).CheckDestroy();
            }                 

            //Send packets
            player.playerSession.QueuePacket(DeleteAllActorsPacket.BuildPacket(player.actorId));
            player.playerSession.QueuePacket(_0xE2Packet.BuildPacket(player.actorId, 0x2));
            player.SendZoneInPackets(this, spawnType);
            player.playerSession.ClearInstance();
            player.SendInstanceUpdate();

            player.playerSession.LockUpdates(false);

            //Send "You have entered an instance" if it's a Private Area
            if (newArea is PrivateArea)
                player.SendGameMessage(GetActor(), 34108, 0x20);

            LuaEngine.GetInstance().CallLuaFunction(player, newArea, "onZoneIn", true);
        }

        //Moves actor within zone to spawn position
        public void DoPlayerMoveInZone(Player player, uint zoneEntrance)
        {
            if (!zoneEntranceList.ContainsKey(zoneEntrance))
            {
                Program.Log.Error("Given zone entrance was not found: " + zoneEntrance);
                return;
            }

            ZoneEntrance ze = zoneEntranceList[zoneEntrance];

            if (ze.zoneId != player.zoneId)
                return;

            DoPlayerMoveInZone(player, ze.spawnX, ze.spawnY, ze.spawnZ, ze.spawnRotation, ze.spawnType);
        }

        //Moves actor within the zone
        public void DoPlayerMoveInZone(Player player, float spawnX, float spawnY, float spawnZ, float spawnRotation, byte spawnType = 0xF)
        {            
            //Remove player from currentZone if transfer else it's login
            if (player.zone != null)
            {
                player.playerSession.LockUpdates(true);
                player.zone.RemoveActorFromZone(player);                
                player.zone.AddActorToZone(player);

                //Update player actor's properties;
                player.positionX = spawnX;
                player.positionY = spawnY;
                player.positionZ = spawnZ;
                player.rotation = spawnRotation;

                //Send packets
                player.playerSession.QueuePacket(_0xE2Packet.BuildPacket(player.actorId, 0x10));
                player.playerSession.QueuePacket(player.CreateSpawnTeleportPacket(spawnType));

                player.playerSession.LockUpdates(false);
                player.SendInstanceUpdate();
            }            
        }

        //Moves actor to new zone, and sends packets to spawn at the given coords.
        public void DoZoneChangeContent(Player player, PrivateAreaContent contentArea, float spawnX, float spawnY, float spawnZ, float spawnRotation, ushort spawnType = SetActorPositionPacket.SPAWNTYPE_WARP_DUTY)
        {
            //Content area was null
            if (contentArea == null)
            {
                Program.Log.Debug("Request to change to content area not on this server by: {0}.", player.customDisplayName);
                return;
            }

            player.playerSession.LockUpdates(true);

            Area oldZone = player.zone;
            //Remove player from currentZone if transfer else it's login
            if (player.zone != null)
            {
                oldZone.RemoveActorFromZone(player);
            }

            contentArea.AddActorToZone(player);

            //Update player actor's properties
            player.zoneId = contentArea.GetParentZone().actorId;

            player.privateArea = contentArea.GetPrivateAreaName();
            player.privateAreaType = contentArea.GetPrivateAreaType();
            player.zone = contentArea;
            player.positionX = spawnX;
            player.positionY = spawnY;
            player.positionZ = spawnZ;
            player.rotation = spawnRotation;

            //Send "You have entered an instance" if it's a Private Area
            player.SendGameMessage(GetActor(), 34108, 0x20);

            //Send packets
            player.playerSession.QueuePacket(DeleteAllActorsPacket.BuildPacket(player.actorId));
            player.playerSession.QueuePacket(_0xE2Packet.BuildPacket(player.actorId, 0x10));
            player.SendZoneInPackets(this, spawnType);
            player.playerSession.ClearInstance();
            player.SendInstanceUpdate();

            player.playerSession.LockUpdates(false);
        
            LuaEngine.GetInstance().CallLuaFunction(player, contentArea, "onZoneIn", true);
        }

        //Session started, zone into world
        public void DoZoneIn(Player player, bool isLogin, ushort spawnType)
        {
            //Add player to new zone and update
            Area playerArea;
            if (player.privateArea != null)
                playerArea = GetPrivateArea(player.zoneId, player.privateArea, player.privateAreaType);
            else
                playerArea = GetZone(player.zoneId);

            //This server does not contain that zoneId
            if (playerArea == null)
                return;

            //Set the current zone and add player
            player.zone = playerArea;

            playerArea.AddActorToZone(player);
            
            //Send packets            
            if (!isLogin)
            {
                player.playerSession.QueuePacket(DeleteAllActorsPacket.BuildPacket(player.actorId));
                player.playerSession.QueuePacket(_0xE2Packet.BuildPacket(player.actorId, 0x2));
                //player.SendZoneInPackets(this, spawnType);
            }

            player.SendZoneInPackets(this, spawnType);

            player.destinationZone = 0;
            player.destinationSpawnType = 0;
            Database.SavePlayerPosition(player);

            player.playerSession.LockUpdates(false);

            LuaEngine.GetInstance().CallLuaFunction(player, playerArea, "onZoneIn", true);
        }

        public void ReloadZone(uint zoneId)
        {
            if (!zoneList.ContainsKey(zoneId))
                return;

            Zone zone = zoneList[zoneId];
            //zone.clear();
            //LoadNPCs(zone.actorId);

        }

        public ContentGroup CreateContentGroup(Director director, params Actor[] actors)
        {
            if (director == null)
                return null;

            lock (groupLock)
            {
                uint[] initialMembers = null;

                if (actors != null)
                {
                    initialMembers = new uint[actors.Length];
                    for (int i = 0; i < actors.Length; i++)
                        initialMembers[i] = actors[i].actorId;
                }

                groupIndexId = groupIndexId | 0x3000000000000000;

                ContentGroup contentGroup = new ContentGroup(groupIndexId, director, initialMembers);
                mContentGroups.Add(groupIndexId, contentGroup);
                groupIndexId++;
                if (initialMembers != null && initialMembers.Length != 0)
                    contentGroup.SendAll();

                return contentGroup;
            }
        }

        public ContentGroup CreateContentGroup(Director director, List<Actor> actors)
        {
            if (director == null)
                return null;

            lock (groupLock)
            {
                uint[] initialMembers = null;

                if (actors != null)
                {
                    initialMembers = new uint[actors.Count];
                    for (int i = 0; i < actors.Count; i++)
                        initialMembers[i] = actors[i].actorId;
                }

                groupIndexId = groupIndexId | 0x3000000000000000;

                ContentGroup contentGroup = new ContentGroup(groupIndexId, director, initialMembers);
                mContentGroups.Add(groupIndexId, contentGroup);
                groupIndexId++;
                if (initialMembers != null && initialMembers.Length != 0)
                    contentGroup.SendAll();

                return contentGroup;
            }
        }

        public ContentGroup CreateGLContentGroup(Director director, List<Actor> actors)
        {
            if (director == null)
                return null;

            lock (groupLock)
            {
                uint[] initialMembers = null;

                if (actors != null)
                {
                    initialMembers = new uint[actors.Count];
                    for (int i = 0; i < actors.Count; i++)
                        initialMembers[i] = actors[i].actorId;
                }

                groupIndexId = groupIndexId | 0x2000000000000000;

                GLContentGroup contentGroup = new GLContentGroup(groupIndexId, director, initialMembers);
                mContentGroups.Add(groupIndexId, contentGroup);
                groupIndexId++;
                if (initialMembers != null && initialMembers.Length != 0)
                    contentGroup.SendAll();

                return contentGroup;
            }
        }

        public void DeleteContentGroup(ulong groupId)
        {
            lock (groupLock)
            {
                if (mContentGroups.ContainsKey(groupId) && mContentGroups[groupId] is ContentGroup)
                {
                    ContentGroup group = (ContentGroup)mContentGroups[groupId];
                    mContentGroups.Remove(groupId);
                }
            }
        }

        public RelationGroup CreateRelationGroup(Actor inviter, Actor invitee, ulong groupType)
        {
            lock (groupLock)
            {                
                groupIndexId = groupIndexId | 0x0000000000000000;

                RelationGroup group = new RelationGroup(groupIndexId, inviter.actorId, invitee.actorId, 0, groupType);
                mRelationGroups.Add(groupIndexId, group);
                groupIndexId++;

                group.SendGroupPacketsAll(inviter.actorId, invitee.actorId);

                return group;
            }
        }

        public RelationGroup GetRelationGroup(uint actorId)
        {
            lock (groupLock)
            {
                foreach (RelationGroup relation in mRelationGroups.Values)
                {
                    if (relation.GetHost() == actorId || relation.GetOther() == actorId)
                        return relation;
                }
                return null;
            }
        }

        public void DeleteRelationGroup(ulong groupid)
        {
            lock (groupLock)
            {
                if (mRelationGroups.ContainsKey(groupid))
                    mRelationGroups.Remove(groupid);
            }
        }

        public TradeGroup CreateTradeGroup(Player inviter, Player invitee)
        {            
            lock (groupLock)
            {
                groupIndexId = groupIndexId | 0x0000000000000000;

                TradeGroup group = new TradeGroup(groupIndexId, inviter.actorId, invitee.actorId);
                mTradeGroups.Add(groupIndexId, group);
                groupIndexId++;

                group.SendGroupPacketsAll(inviter.actorId, invitee.actorId);

                inviter.SendGameMessage(GetActor(), 25101, 0x20, (object)invitee); //You request to trade with X
                invitee.SendGameMessage(GetActor(), 25037, 0x20, (object)inviter); //X wishes to trade with you

                return group;
            }
        }

        public TradeGroup GetTradeGroup(uint actorId)
        {
            lock (groupLock)
            {
                foreach (TradeGroup group in mTradeGroups.Values)
                {
                    if (group.GetHost() == actorId || group.GetOther() == actorId)
                        return (TradeGroup)group;
                }
                return null;
            }
        }

        public void DeleteTradeGroup(ulong groupid)
        {
            lock (groupLock)
            {
                if (mTradeGroups.ContainsKey(groupid))
                {
                    TradeGroup group = mTradeGroups[groupid];
                    group.SendDeletePackets(group.GetHost(), group.GetOther());
                    mTradeGroups.Remove(groupid);
                }
            }
        }

        public void TradeTEST(Player player)
        {
            player.KickEventSpecial(Server.GetStaticActors("TradeExecuteCommand"), 0, "commandContent", null, null, null, 16, null, null, null, null, null);
        }

        public void AcceptTrade(Player invitee)
        {
            TradeGroup group = GetTradeGroup(invitee.actorId);

            if (group == null)
            {
                invitee.SendMessage(0x20, "", "MASSIVE ERROR: No tradegroup found!!!");
                return;
            }

            Player inviter = (Player)invitee.GetZone().FindActorInArea(group.GetHost());

            //DeleteTradeGroup(group.groupIndex);

            inviter.StartTradeTransaction(invitee);
            invitee.StartTradeTransaction(inviter);

            inviter.KickEventSpecial(Server.GetStaticActors("TradeExecuteCommand"), 0, "commandContent", null, null, null, 16, null, null, null, null, null);
            invitee.KickEventSpecial(Server.GetStaticActors("TradeExecuteCommand"), 0, "commandContent", null, null, null, 16, null, null, null, null, null);
        }

        public void CancelTradeTooFar(Player inviter)
        {
            TradeGroup group = GetTradeGroup(inviter.actorId);

            if (group == null)
            {
                inviter.SendMessage(0x20, "", "MASSIVE ERROR: No tradegroup found!!!");
                return;
            }

            Player invitee = (Player)inviter.GetZone().FindActorInArea(group.GetOther());

            inviter.SendGameMessage(GetActor(), 25042, 0x20); //You cancel the trade.
            if (invitee != null)
                invitee.SendGameMessage(GetActor(), 25042, 0x20); //The trade has been canceled.

            DeleteTradeGroup(group.groupIndex);
        }

        public void CancelTrade(Player inviter)
        {
            TradeGroup group = GetTradeGroup(inviter.actorId);

            if (group == null)
            {
                inviter.SendMessage(0x20, "", "MASSIVE ERROR: No tradegroup found!!!");
                return;
            }

            Player invitee = (Player)inviter.GetZone().FindActorInArea(group.GetOther());            

            inviter.SendGameMessage(GetActor(), 25041, 0x20); //You cancel the trade.
            if (invitee != null)
                invitee.SendGameMessage(GetActor(), 25040, 0x20); //The trade has been canceled.

            DeleteTradeGroup(group.groupIndex);
        }

        public void RefuseTrade(Player invitee)
        {
            TradeGroup group = GetTradeGroup(invitee.actorId);

            if (group == null)
            {
                invitee.SendMessage(0x20, "", "MASSIVE ERROR: No tradegroup found!!!");
                return;
            }

            Player inviter = (Player)invitee.GetZone().FindActorInArea(group.GetHost());

            if (inviter != null)
                inviter.SendGameMessage(GetActor(), 25038, 0x20); //Your trade request fails

            DeleteTradeGroup(group.groupIndex);
        }

        public void SwapTradedItems(Player p1, Player p2)
        {
            lock (tradeLock)
            {
                if (p1.IsTradeAccepted() && p2.IsTradeAccepted())
                {
                    //move items around

                    p1.FinishTradeTransaction();
                    p2.FinishTradeTransaction();
                }
            }
        }

        public InventoryItem CreateItem(uint itemId, int amount, byte quality = 1, InventoryItem.ItemModifier modifiers = null)
        {
            return Database.CreateItem(itemId, amount, quality, modifiers);
        }

        public bool BazaarBuyOperation(Player bazaar, Player buyer, InventoryItem itemToBuy, int quantity, int cost)
        {
            if (bazaar == null || buyer == null || itemToBuy == null)
                return false;

            if (quantity <= 0)
                return false;

            if (cost < 0)
                return false;

            if (itemToBuy.GetBazaarMode() == InventoryItem.TYPE_SINGLE || itemToBuy.GetBazaarMode() == InventoryItem.TYPE_MULTI || itemToBuy.GetBazaarMode() == InventoryItem.TYPE_STACK)
            {
                itemToBuy.ChangeQuantity(-quantity);
                buyer.AddItem(itemToBuy.itemId, quantity, itemToBuy.quality);
                buyer.GetItemPackage(Inventory.CURRENCY_CRYSTALS).RemoveItem(1000001, cost);
            }
           
            if (itemToBuy.quantity == 0)
                Database.ClearBazaarEntry(bazaar, itemToBuy);

            bazaar.CheckBazaarFlags();

            return true;
        }

        public bool BazaarSellOperation(Player bazaar, Player buyer, InventoryItem reward, int rewardQuantity, InventoryItem seek, int seekQuantity)
        {
            if (bazaar == null || buyer == null || reward == null || seek == null)
                return false;

            if (rewardQuantity <= 0 || seekQuantity <= 0)
                return false;

            if (reward.GetBazaarMode() == InventoryItem.TYPE_SEEK_ITEM)
            {
                InventoryItem seekBazaar = bazaar.GetItemPackage(Inventory.BAZAAR).GetItemAttachedTo(reward);
                bazaar.RemoveItem(reward, rewardQuantity);
                bazaar.RemoveItem(seekBazaar, seekQuantity);
                bazaar.AddItem(seekBazaar);
                bazaar.AddItem(seek.itemId, seekQuantity, seek.quality);

                buyer.RemoveItem(seek, seekQuantity);
                buyer.AddItem(reward);
            }

            Database.ClearBazaarEntry(bazaar, reward);

            bazaar.CheckBazaarFlags();

            return true;
        }

        public void AddToBazaar(Player player, InventoryItem reward, InventoryItem seek, int rewardAmount, int seekAmount, byte bazaarMode)
        {
            bool succ = false;

            if (bazaarMode == InventoryItem.TYPE_SINGLE || bazaarMode == InventoryItem.TYPE_MULTI || bazaarMode == InventoryItem.TYPE_STACK)
                succ = Database.CreateBazaarEntry(player, reward, seek, rewardAmount, 0, bazaarMode, seekAmount);
            else
                succ = Database.CreateBazaarEntry(player, reward, seek, rewardAmount, seekAmount, bazaarMode);

            if (succ)
            {                
                if (bazaarMode != InventoryItem.TYPE_SINGLE && bazaarMode != InventoryItem.TYPE_MULTI && bazaarMode != InventoryItem.TYPE_STACK)
                {
                    reward.SetDealingAttached(bazaarMode, seek.uniqueId);
                    seek.SetHasAttached(true);
                    player.GetItemPackage(Inventory.BAZAAR).StartSendUpdate();
                    player.GetItemPackage(Inventory.BAZAAR).AddItem(reward);
                    player.GetItemPackage(Inventory.BAZAAR).AddItem(seek);
                    reward.SetAttachedIndex(Inventory.BAZAAR, seek.slot);
                    player.GetItemPackage(Inventory.BAZAAR).DoneSendUpdate();
                }
                else
                {
                    reward.SetDealing(bazaarMode, seekAmount);
                    player.GetItemPackage(Inventory.BAZAAR).StartSendUpdate();
                    player.GetItemPackage(Inventory.BAZAAR).AddItem(reward);
                    player.GetItemPackage(Inventory.BAZAAR).DoneSendUpdate();
                }
                
            }

            player.CheckBazaarFlags();
        }

        
        public void RemoveFromBazaar(Player player, InventoryItem rewardRef)
        {
            ushort attachedItemPackage = (ushort)((rewardRef.dealingAttached1 >> 16) & 0xFF);
            ushort attachedSlot = (ushort) (rewardRef.dealingAttached1 & 0xFF);

            InventoryItem seekRef = rewardRef.IsSelling() ? null : player.GetItemPackage(attachedItemPackage).GetItemAtSlot(attachedSlot);           

            Database.ClearBazaarEntry(player, rewardRef);

            player.GetItemPackage(Inventory.BAZAAR).RemoveItem(rewardRef);

            bool isSelling = rewardRef.IsSelling();
            rewardRef.SetNormal();

            if (seekRef != null)
                player.GetItemPackage(Inventory.BAZAAR).RemoveItem(seekRef);

            player.AddItem(rewardRef);

            if (!isSelling)
            {
                seekRef.SetNormal();
                player.AddItem(seekRef);
            }

            player.CheckBazaarFlags();
        }
        /*
        public void TransactionBazaar(Player owner, Player other, InventoryItem reward, InventoryItem seek, int rewardAmount, int seekAmount)
        {
            Database.ClearBazaarEntry(owner, reward, seek);

            //Remove Bazaar Items from owner
            owner.GetInventory(Inventory.BAZAAR).RemoveItem(reward);
            owner.GetInventory(Inventory.BAZAAR).RemoveItem(seek);

            //Remove Seek item from other
            if (seek.GetItemData().IsMoney())
                other.GetInventory(Inventory.CURRENCY_CRYSTALS).RemoveItem(seek.itemId, seekAmount);
            else
                other.GetInventory(Inventory.NORMAL).RemoveItem(seek.itemId, seekAmount);

            //Add reward to other, seek to owner
            if (reward.GetItemData().IsMoney())
                other.GetInventory(Inventory.CURRENCY_CRYSTALS).AddItem(reward.itemId, rewardAmount);
            else
                other.GetInventory(Inventory.NORMAL).AddItem(reward);

            if (seek.GetItemData().IsMoney())
                owner.GetInventory(Inventory.CURRENCY_CRYSTALS).AddItem(seek.itemId, seekAmount);
            else
                other.GetInventory(Inventory.NORMAL).AddItem(seek);
        }*/

        public bool SendGroupInit(Session session, ulong groupId)
        {
            if (mContentGroups.ContainsKey(groupId))
            {
                mContentGroups[groupId].SendInitWorkValues(session);
                return true;
            }
            else if (mTradeGroups.ContainsKey(groupId))
            {
                mTradeGroups[groupId].SendInitWorkValues(session);
                return true;
            }
            return false;
        }
        
        public void RequestWorldLinkshellCreate(Player player, string name, ushort crest)
        {
            SubPacket packet = CreateLinkshellPacket.BuildPacket(player.playerSession, name, crest, player.actorId);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellCrestModify(Player player, string name, ushort crest)
        {
            SubPacket packet = ModifyLinkshellPacket.BuildPacket(player.playerSession, 1, name, null, crest, 0);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellDelete(Player player, string name)
        {
            SubPacket packet = DeleteLinkshellPacket.BuildPacket(player.playerSession, name);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellRankChange(Player player, string lsname, string memberName, byte newRank)
        {
            SubPacket packet = LinkshellRankChangePacket.BuildPacket(player.playerSession, memberName, lsname, newRank);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellInviteMember(Player player, string lsname, uint invitedActorId)
        {
            SubPacket packet = LinkshellInvitePacket.BuildPacket(player.playerSession, invitedActorId, lsname);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellCancelInvite(Player player)
        {
            SubPacket packet = LinkshellInviteCancelPacket.BuildPacket(player.playerSession);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellLeave(Player player, string lsname)
        {
            SubPacket packet = LinkshellLeavePacket.BuildPacket(player.playerSession, lsname, null, false);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellKick(Player player, string lsname, string kickedName)
        {
            SubPacket packet = LinkshellLeavePacket.BuildPacket(player.playerSession, lsname, kickedName, true);
            player.QueuePacket(packet);
        }

        public void RequestWorldLinkshellChangeActive(Player player, string lsname)
        {
            SubPacket packet = LinkshellChangePacket.BuildPacket(player.playerSession, lsname);
            player.QueuePacket(packet);
        }

        private void RequestWorldServerZoneChange(Player player, uint destinationZoneId, byte spawnType, float spawnX, float spawnY, float spawnZ, float spawnRotation)
        {
            ZoneConnection zc = Server.GetWorldConnection();
            zc.RequestZoneChange(player.playerSession.id, destinationZoneId, spawnType, spawnX, spawnY, spawnZ, spawnRotation);
        }

        //World server sent a party member list synch packet to the zone server. Add and update players that may be a part of it.
        public void PartyMemberListRecieved(PartySyncPacket syncPacket)
        {
            lock (currentPlayerParties)
            {
                Party group;

                //If no members on this server, get out or clean
                if (!currentPlayerParties.ContainsKey(syncPacket.partyGroupId) && syncPacket.memberActorIds.Length == 0)
                    return;
                else if (!currentPlayerParties.ContainsKey(syncPacket.partyGroupId) && syncPacket.memberActorIds.Length == 0)
                    NoMembersInParty(currentPlayerParties[syncPacket.partyGroupId]);

                //Get or create group
                if (!currentPlayerParties.ContainsKey(syncPacket.partyGroupId))
                {
                    group = new Party(syncPacket.partyGroupId, syncPacket.owner);
                    currentPlayerParties.Add(syncPacket.partyGroupId, group);
                }
                else
                    group = currentPlayerParties[syncPacket.partyGroupId];

                group.SetLeader(syncPacket.owner);
                group.members = syncPacket.memberActorIds.ToList();

                //Add group to everyone
                for (int i = 0; i < group.members.Count; i++ )
                {
                    uint member = group.members[i];
                    Session session = Server.GetServer().GetSession(member);

                    if (session == null)
                        continue;

                    Player player = session.GetActor();
                    if (player == null)
                        continue;
                    player.SetParty(group);
                }
            }
        }

        //Player was removed from the party either due to leaving it or leaving the server. Remove if empty.
        public void NoMembersInParty(Party party)
        {
            if (currentPlayerParties.ContainsKey(party.groupIndex))
                currentPlayerParties.Remove(party.groupIndex);
        }

        public void CreateInvitePartyGroup(Player player, string name)
        {
            SubPacket invitePacket = PartyInvitePacket.BuildPacket(player.playerSession, name);
            player.QueuePacket(invitePacket);
        }
        public void CreateInvitePartyGroup(Player player, uint actorId)
        {
            SubPacket invitePacket = PartyInvitePacket.BuildPacket(player.playerSession, actorId);
            player.QueuePacket(invitePacket);
        }

        public void GroupInviteResult(Player player, uint groupType, uint result)
        {
            SubPacket groupInviteResultPacket = GroupInviteResultPacket.BuildPacket(player.playerSession, groupType, result);
            player.QueuePacket(groupInviteResultPacket);
        }
                
        public void StartZoneThread()
        {
            mZoneTimer = new Timer(ZoneThreadLoop, null, 0, MILIS_LOOPTIME);
            Program.Log.Info("Zone Loop has started");
        }
        
        public void ZoneThreadLoop(Object state)
        {          
            lock (zoneList)
            {
                foreach (Area area in zoneList.Values)
                    area.Update(MILIS_LOOPTIME);
            }            
        }

        public Player GetPCInWorld(string name)
        {
            if (Server.GetServer().GetSession(name) != null)
                return Server.GetServer().GetSession(name).GetActor();
            else
                return null;
        }

        public Player GetPCInWorld(uint charId)
        {
            if (Server.GetServer().GetSession(charId) != null)
                return Server.GetServer().GetSession(charId).GetActor();
            else
                return null;
        }

        public Actor GetActorInWorld(uint charId)
        {
            foreach (Zone zone in zoneList.Values)
            {
                Actor a = zone.FindActorInZone(charId);
                if (a != null)
                    return a;                
            }
            return null;
        }

        public Actor GetActorInWorldByUniqueId(string uid)
        {
            foreach (Zone zone in zoneList.Values)
            {
                Actor a = zone.FindActorInZoneByUniqueID(uid);
                if (a != null)
                    return a;
            }
            return null;
        }

        public Zone GetZone(uint zoneId)
        {
            if (!zoneList.ContainsKey(zoneId))
                return null;
            return zoneList[zoneId];
        }

        public PrivateArea GetPrivateArea(uint zoneId, string privateArea, uint privateAreaType)
        {
            if (!zoneList.ContainsKey(zoneId))
                return null;

            return zoneList[zoneId].GetPrivateArea(privateArea, privateAreaType);
        }

        public WorldMaster GetActor()
        {
            return worldMaster;
        }

        public DebugProg GetDebugActor()
        {
            return debug;
        }

        public class ZoneEntrance
        {
            public uint zoneId;
            public string privateAreaName;
            public int privateAreaType;
            public byte spawnType;
            public float spawnX;
            public float spawnY;
            public float spawnZ;
            public float spawnRotation;

            public ZoneEntrance(uint zoneId, string privateAreaName, int privateAreaType, byte spawnType, float x, float y, float z, float rot)
            {
                this.zoneId = zoneId;
                this.privateAreaName = privateAreaName;
                this.privateAreaType = privateAreaType;
                this.spawnType = spawnType;
                this.spawnX = x;
                this.spawnY = y;
                this.spawnZ  = z;
                this.spawnRotation = rot;
            }
        }

        public ZoneEntrance GetZoneEntrance(uint entranceId)
        {
            if (zoneEntranceList.ContainsKey(entranceId))
                return zoneEntranceList[entranceId];
            else
                return null;
        }

        public ActorClass GetActorClass(uint id)
        {
            if (actorClasses.ContainsKey(id))
                return actorClasses[id];
            else
                return null;
        }        
    }
}
