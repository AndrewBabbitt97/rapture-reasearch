﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Threading;
using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Map_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using System.IO;
using FFXIVClassic_Map_Server.packets.send.actor;
using FFXIVClassic_Map_Server;
using FFXIVClassic_Map_Server.packets.send;
using FFXIVClassic_Map_Server.dataobjects.chara;
using FFXIVClassic_Map_Server.Actors;
using FFXIVClassic_Map_Server.lua;

namespace FFXIVClassic_Lobby_Server
{
    class Server
    {
        public const int FFXIV_MAP_PORT     = 54992;
        public const int BUFFER_SIZE        = 0x400;
        public const int BACKLOG            = 100;
        public const string STATIC_ACTORS_PATH = "./staticactors.bin";

        private static Server mSelf;

        private Socket mServerSocket;

        private Dictionary<uint,ConnectedPlayer> mConnectedPlayerList = new Dictionary<uint,ConnectedPlayer>();
        private List<ClientConnection> mConnectionList = new List<ClientConnection>();
        private LuaEngine mLuaEngine = new LuaEngine();
        private WorldManager mWorldManager;
        private static StaticActors mStaticActors;
        private PacketProcessor mProcessor;
        private Thread mProcessorThread;
        private Thread mGameThread;

        public Server()
        {
            mSelf = this;
        }

        public static Server getServer()
        {
            return mSelf;
        }

        #region Socket Handling
        public bool startServer()
        {
            mStaticActors = new StaticActors(STATIC_ACTORS_PATH);

            mWorldManager = new WorldManager(this);
            mWorldManager.LoadZoneList();
            mWorldManager.LoadZoneEntranceList();
            mWorldManager.LoadNPCs();

            IPEndPoint serverEndPoint = new System.Net.IPEndPoint(IPAddress.Parse(ConfigConstants.OPTIONS_BINDIP), FFXIV_MAP_PORT);

            try{
                mServerSocket = new System.Net.Sockets.Socket(serverEndPoint.Address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);         
            }
            catch (Exception e)
            {
                throw new ApplicationException("Could not create socket, check to make sure not duplicating port", e);
            }
            try
            {
                mServerSocket.Bind(serverEndPoint);
                mServerSocket.Listen(BACKLOG);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured while binding socket, check inner exception", e);
            }
            try
            {
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
            catch (Exception e)
            {
                throw new ApplicationException("Error occured starting listeners, check inner exception", e);
            }

            Console.Write("Game server has started @ ");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("{0}:{1}", (mServerSocket.LocalEndPoint as IPEndPoint).Address, (mServerSocket.LocalEndPoint as IPEndPoint).Port);
            Console.ForegroundColor = ConsoleColor.Gray;

            mProcessor = new PacketProcessor(this, mConnectedPlayerList, mConnectionList);

            //mGameThread = new Thread(new ThreadStart(mProcessor.update));
            //mGameThread.Start();
            return true;
        }

        private void acceptCallback(IAsyncResult result)
        {
            ClientConnection conn = null;
            Socket socket = (System.Net.Sockets.Socket)result.AsyncState;
           
            try
            {

                conn = new ClientConnection();
                conn.socket = socket.EndAccept(result);
                conn.buffer = new byte[BUFFER_SIZE];

                lock (mConnectionList)
                {
                    mConnectionList.Add(conn);
                }

                Log.conn(String.Format("Connection {0}:{1} has connected.", (conn.socket.RemoteEndPoint as IPEndPoint).Address, (conn.socket.RemoteEndPoint as IPEndPoint).Port));
                //Queue recieving of data from the connection
                conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                //Queue the accept of the next incomming connection
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
            catch (SocketException)
            {
                if (conn != null)
                {
                
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
            catch (Exception)
            {
                if (conn != null)
                {                   
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
                mServerSocket.BeginAccept(new AsyncCallback(acceptCallback), mServerSocket);
            }
        }

        public static Actor getStaticActors(uint id)
        {
            return mStaticActors.getActor(id);
        }

        public static Actor getStaticActors(string name)
        {
            return mStaticActors.findStaticActor(name);
        }

        /// <summary>
        /// Receive Callback. Reads in incoming data, converting them to base packets. Base packets are sent to be parsed. If not enough data at the end to build a basepacket, move to the beginning and prepend.
        /// </summary>
        /// <param name="result"></param>
        private void receiveCallback(IAsyncResult result)
        {
            ClientConnection conn = (ClientConnection)result.AsyncState;            

            try
            {
                int bytesRead = conn.socket.EndReceive(result);
                if (bytesRead > 0)
                {
                    int offset = 0;

                    //Build packets until can no longer or out of data
                    while(true)
                    {                        
                        BasePacket basePacket = buildPacket(ref offset, conn.buffer, bytesRead);
                        //If can't build packet, break, else process another
                        if (basePacket == null)                        
                            break;                        
                        else                        
                            mProcessor.processPacket(conn, basePacket);                        
                    }
                    
                    //Not all bytes consumed, transfer leftover to beginning
                    if (offset < bytesRead)                    
                        Array.Copy(conn.buffer, offset, conn.buffer, 0, bytesRead - offset);

                    //Build any queued subpackets into basepackets and send
                    conn.flushQueuedSendPackets();
                    
                    if (offset < bytesRead)                    
                        //Need offset since not all bytes consumed
                        conn.socket.BeginReceive(conn.buffer, bytesRead - offset, conn.buffer.Length - (bytesRead - offset), SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                    else                        
                        //All bytes consumed, full buffer available
                        conn.socket.BeginReceive(conn.buffer, 0, conn.buffer.Length, SocketFlags.None, new AsyncCallback(receiveCallback), conn);
                }
                else
                {
                    Log.conn(String.Format("{0} has disconnected.", conn.owner == 0 ? conn.getAddress() : "User " + conn.owner));
                  
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }
                }
            }
            catch (SocketException)
            {                
                if (conn.socket != null)
                {
                    Log.conn(String.Format("{0} has disconnected.", conn.owner == 0 ? conn.getAddress() : "User " + conn.owner));
                    
                    lock (mConnectionList)
                    {
                        mConnectionList.Remove(conn);
                    }          
                }
            }
        }

        /// <summary>
        /// Builds a packet from the incoming buffer + offset. If a packet can be built, it is returned else null.
        /// </summary>
        /// <param name="offset">Current offset in buffer.</param>
        /// <param name="buffer">Incoming buffer.</param>
        /// <returns>Returns either a BasePacket or null if not enough data.</returns>
        public BasePacket buildPacket(ref int offset, byte[] buffer, int bytesRead)
        {
            BasePacket newPacket = null;

            //Too small to even get length
            if (bytesRead <= offset)
                return null;

            ushort packetSize = BitConverter.ToUInt16(buffer, offset);

            //Too small to whole packet
            if (bytesRead < offset + packetSize)
                return null;

            if (buffer.Length < offset + packetSize)
                return null;

            try
            {
                newPacket = new BasePacket(buffer, ref offset);
            }
            catch (OverflowException)
            {
                return null;
            }

            return newPacket;
        }

        #endregion

        public void sendPacket(ConnectedPlayer client, string path)
        {
            BasePacket packet = new BasePacket(path);
    
            if (client != null)
            {
                packet.replaceActorID(client.actorID);
                client.queuePacket(packet);
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    packet.replaceActorID(entry.Value.actorID);
                    entry.Value.queuePacket(packet);
                }
            }
        }

        public void testCodePacket(uint id, uint value, string target)
        {
            SetActorPropetyPacket changeProperty = new SetActorPropetyPacket(target);

            changeProperty.setTarget(target);
            changeProperty.addInt(id, value);
            changeProperty.addTarget();

            foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
            {
                SubPacket changePropertyPacket = changeProperty.buildPacket((entry.Value.actorID), (entry.Value.actorID));
                
                BasePacket packet = BasePacket.createPacket(changePropertyPacket, true, false);
                packet.debugPrintPacket();

                entry.Value.queuePacket(packet);               
            }
        }

        public void doMusic(ConnectedPlayer client, string music)
        {
            ushort musicId;
            
            if (music.ToLower().StartsWith("0x"))
                musicId = Convert.ToUInt16(music, 16);
            else
                musicId = Convert.ToUInt16(music);

            if (client != null)
                client.queuePacket(BasePacket.createPacket(SetMusicPacket.buildPacket(client.actorID, musicId, 1), true, false));
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    BasePacket musicPacket = BasePacket.createPacket(SetMusicPacket.buildPacket(entry.Value.actorID, musicId, 1), true, false);
                    entry.Value.queuePacket(musicPacket);
                }
            }
        }

        public void doWarp(ConnectedPlayer client, string entranceId)
        {            
            uint id;

            try
            {
                if (entranceId.ToLower().StartsWith("0x"))
                    id = Convert.ToUInt32(entranceId, 16);
                else
                    id = Convert.ToUInt32(entranceId);
            }
            catch(FormatException e)
            {return;}

            FFXIVClassic_Map_Server.WorldManager.ZoneEntrance ze = mWorldManager.getZoneEntrance(id);

            if (ze == null)
                return;

            if (client != null)
                mWorldManager.DoZoneChange(client.getActor(), ze.zoneId, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, 0.0f);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    mWorldManager.DoZoneChange(entry.Value.getActor(), ze.zoneId, ze.spawnType, ze.spawnX, ze.spawnY, ze.spawnZ, 0.0f);
                }
            }
        }

        public void doWarp(ConnectedPlayer client, string zone, string sx, string sy, string sz)
        {
            uint zoneId;
            float x,y,z;

            if (zone.ToLower().StartsWith("0x"))
                zoneId = Convert.ToUInt32(zone, 16);
            else
                zoneId = Convert.ToUInt32(zone);
            
            if (mWorldManager.GetZone(zoneId) == null)
            {
                if (client != null)
                    client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", "Zone does not exist or setting isn't valid."), true, false));
                Log.error("Zone does not exist or setting isn't valid.");
            }

            x = Single.Parse(sx);
            y = Single.Parse(sy);
            z = Single.Parse(sz);

            if (client != null)
                mWorldManager.DoZoneChange(client.getActor(), zoneId, 0x2, x, y, z, 0.0f);
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    mWorldManager.DoZoneChange(entry.Value.getActor(), zoneId, 0x2, x, y, z, 0.0f);
                }
            }
        }

        public LuaEngine GetLuaEngine()
        {
            return mLuaEngine;
        }

        public WorldManager GetWorldManager()
        {
            return mWorldManager;
        }


        public void printPos(ConnectedPlayer client)
        {
            if (client != null)
            {
                Player p = client.getActor();
                client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Position: {1}, {2}, {3}, {4}", p.customDisplayName, p.positionX, p.positionY, p.positionZ, p.rotation)), true, false));
            }
            else
            {
                foreach (KeyValuePair<uint, ConnectedPlayer> entry in mConnectedPlayerList)
                {
                    Player p = entry.Value.getActor();
                    Log.info(String.Format("{0} position: {1}, {2}, {3}, {4}", p.customDisplayName, p.positionX, p.positionY, p.positionZ, p.rotation));
                }
            }
        }

        internal void doCommand(string input, ConnectedPlayer client)
        {
            input.Trim();

            String[] split = input.Split(' ');

            if (split.Length >= 1)
            {
                if (split[0].Equals("mypos"))
                {
                    try
                    {
                        printPos(client);
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not load packet: " + e);
                    }
                }
                else if (split[0].Equals("resetzone"))
                {
                    Log.info(String.Format("Got request to reset zone: {0}", client.getActor().zoneId));
                    if (client != null)
                    {
                        client.getActor().zone.clear();
                        client.getActor().zone.addActorToZone(client.getActor());
                        client.getActor().sendInstanceUpdate();
                        client.queuePacket(BasePacket.createPacket(SendMessagePacket.buildPacket(client.actorID, client.actorID, SendMessagePacket.MESSAGE_TYPE_GENERAL_INFO, "", String.Format("Resting zone {0}...", client.getActor().zoneId)), true, false));
                    }
                    mWorldManager.reloadZone(client.getActor().zoneId);                    
                }
            }
            if (split.Length >= 2)
            {
                if (split[0].Equals("sendpacket"))
                {
                    try
                    {
                        sendPacket(client, "./packets/" + split[1]);
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not load packet: " + e);
                    }
                }
                else if (split[0].Equals("music"))
                {
                    try
                    {
                        doMusic(client, split[1]);
                    }
                    catch (Exception e)
                    {
                        Log.error("Could not change music: " + e);
                    }
                }
                else if (split[0].Equals("warp"))
                {
                    doWarp(client, split[1]);
                }
            }
            if (split.Length >= 5)
            {
                if (split[0].Equals("warp"))
                {
                    doWarp(client, split[1], split[2], split[3], split[4]);
                }
                else if (split[0].Equals("property"))
                {
                    testCodePacket(Utils.MurmurHash2(split[1], 0), Convert.ToUInt32(split[2], 16), split[3]);
                }
            }                  
        }
    }

}
