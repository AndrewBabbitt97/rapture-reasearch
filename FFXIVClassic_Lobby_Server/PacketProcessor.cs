﻿using FFXIVClassic_Lobby_Server.common;
using FFXIVClassic_Lobby_Server.dataobjects;
using FFXIVClassic_Lobby_Server.packets;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFXIVClassic_Lobby_Server
{
    class PacketProcessor
    {
        List<ClientConnection> mConnections;
        Boolean isAlive = true;

        public PacketProcessor(List<ClientConnection> connectionList)
        {
            mConnections = connectionList;
        }

        public void update()
        {
            Console.WriteLine("Packet processing thread has started");
            while (isAlive)
            {
                lock (mConnections)
                {
                    foreach (ClientConnection client in mConnections)
                    {
                        //Receive packets
                        while (true)
                        {
                            if (client.incomingStream.Size < BasePacket.BASEPACKET_SIZE)
                                break;

                            try {
                                if (client.incomingStream.Size < BasePacket.BASEPACKET_SIZE)
                                    break;
                                BasePacketHeader header = BasePacket.getHeader(client.incomingStream.Peek(BasePacket.BASEPACKET_SIZE));

                                if (client.incomingStream.Size < header.packetSize)
                                    break;

                                BasePacket packet = new BasePacket(client.incomingStream.Get(header.packetSize));
                                processPacket(client, packet);

                            }
                            catch(OverflowException)
                            { break; }
                        }

                        //Send packets
                        while (client.sendPacketQueue.Count != 0)
                            client.flushQueuedSendPackets();
                    }
                }
            }
        }

        private void processPacket(ClientConnection client, BasePacket packet)
        {
            
            if ((packet.header.packetSize == 0x288) && (packet.data[0x34] == 'T'))		//Test Ticket Data
            {
                //Crypto handshake
                ProcessStartSession(client, packet);
                return;
            }
            
            BasePacket.decryptPacket(client.blowfish, ref packet);

            packet.debugPrintPacket();

            List<SubPacket> subPackets = packet.getSubpackets();
            foreach (SubPacket subpacket in subPackets)
            {                             

                switch (subpacket.header.opcode)
                {
                    case 0x03:
                        ProcessGetCharacters(client, subpacket);
                        break;
                    case 0x04:
                        ProcessSelectCharacter(client, subpacket);
                        break;
                    case 0x05:
                        ProcessSessionAcknowledgement(client, subpacket);
                        break;
                    case 0x0B:
                        ProcessModifyCharacter(client, subpacket);
                        break;  
                    case 0x0F:
                        //Mod Retainers
                    default:
                        Debug.WriteLine("Unknown command 0x{0:X} received.", subpacket.header.opcode);
                        break;
                }
            }
        }

        private void ProcessStartSession(ClientConnection client, BasePacket packet)
        {
            UInt32 clientTime = BitConverter.ToUInt32(packet.data, 0x74);

            //We assume clientTime is 0x50E0E812, but we need to generate a proper key later
            byte[] blowfishKey = { 0xB4, 0xEE, 0x3F, 0x6C, 0x01, 0x6F, 0x5B, 0xD9, 0x71, 0x50, 0x0D, 0xB1, 0x85, 0xA2, 0xAB, 0x43};
            client.blowfish = new Blowfish(blowfishKey);

            Console.WriteLine("Received encryption key: 0x{0:X}", clientTime);

            //Respond with acknowledgment
            BasePacket outgoingPacket = new BasePacket(HardCoded_Packets.g_secureConnectionAcknowledgment);
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
            client.queuePacket(outgoingPacket);
        }

        private void ProcessSessionAcknowledgement(ClientConnection client, SubPacket packet)
        {
            PacketStructs.SessionPacket sessionPacket = PacketStructs.toSessionStruct(packet.data);
            String sessionId = sessionPacket.session;
            String clientVersion = sessionPacket.version;

            Console.WriteLine("Got acknowledgment for secure session.");
            Console.WriteLine("SESSION ID: {0}", sessionId);
            Console.WriteLine("CLIENT VERSION: {0}", clientVersion);

            uint userId = Database.getUserIdFromSession(sessionId);
            client.currentUserId = userId;

            if (userId == 0)
            {
                client.disconnect();
                Console.WriteLine("Invalid session, kicking...");
            }

            Console.WriteLine("USER ID: {0}", userId);
            BasePacket outgoingPacket = new BasePacket("./packets/loginAck.bin");
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
            client.queuePacket(outgoingPacket);
        }

        private void ProcessGetCharacters(ClientConnection client, SubPacket packet)
        {   
	        Console.WriteLine("{0} => Get characters", client.currentUserId == 0 ? client.getAddress() : "User " + client.currentUserId);

            buildWorldLists(client, packet);

	        BasePacket outgoingPacket = new BasePacket("./packets/getCharsPacket.bin");
            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
	        client.queuePacket(outgoingPacket);
        }

        private void ProcessSelectCharacter(ClientConnection client, SubPacket packet)
        {
            uint characterId = 0;
            using (BinaryReader binReader = new BinaryReader(new MemoryStream(packet.data)))
            {
                binReader.BaseStream.Seek(0x8, SeekOrigin.Begin);
                characterId = binReader.ReadUInt32();
                binReader.Close();
            }

            Console.WriteLine("{0} => Select character id {1}", client.currentUserId == 0 ? client.getAddress() : "User " + client.currentUserId, characterId);	        

	        String serverIp = "141.117.162.99";
            ushort port = 54992;
            BitConverter.GetBytes(port);
	        BasePacket outgoingPacket = new BasePacket("./packets/selectChar.bin");

            //Write Character ID and Server info
            using (BinaryWriter binWriter = new BinaryWriter(new MemoryStream(outgoingPacket.data)))
            {
                binWriter.Seek(0x28, SeekOrigin.Begin);
                binWriter.Write(characterId);
                binWriter.Seek(0x78, SeekOrigin.Begin);
                binWriter.Write(System.Text.Encoding.ASCII.GetBytes(serverIp));
                binWriter.Seek(0x76, SeekOrigin.Begin);
                binWriter.Write(port);
                binWriter.Close();
            }

            BasePacket.encryptPacket(client.blowfish, outgoingPacket);
	        client.queuePacket(outgoingPacket);
        }

        private void ProcessModifyCharacter(ClientConnection client, SubPacket packet)
        {

            PacketStructs.CharacterRequestPacket charaReq = PacketStructs.toCharacterRequestStruct(packet.data);
            var slot = charaReq.slot;
            var code = charaReq.command;
            var name = charaReq.characterName;
            var worldId = charaReq.worldId;

            switch (code)
            {
                case 0x01://Reserve
                    var alreadyTaken = Database.reserveCharacter(client.currentUserId, slot, worldId, name);

                    if (alreadyTaken)
                    {
                        PacketStructs.ErrorPacket errorPacket = new PacketStructs.ErrorPacket();
                        errorPacket.sequence = charaReq.sequence;
                        errorPacket.errorId = 13005;
                        errorPacket.errorCode = 0;
                        errorPacket.statusCode = 0;
                        byte[] data = PacketStructs.StructureToByteArray(errorPacket);

                        SubPacket subpacket = new SubPacket(0x02, packet.header.sourceId, packet.header.targetId, data);
                        BasePacket basePacket = BasePacket.createPacket(subpacket, true, false);
                        basePacket.debugPrintPacket();
                        BasePacket.encryptPacket(client.blowfish, basePacket);
                        client.queuePacket(basePacket);

                        Console.WriteLine("User {0} => Error; name taken: \"{1}\"", client.currentUserId, charaReq.characterName);
                        return;
                    }

                    //Confirm Reserve
                    BasePacket confirmReservePacket = new BasePacket("./packets/chara/confirmReserve.bin");
                    BasePacket.encryptPacket(client.blowfish, confirmReservePacket);
                    client.queuePacket(confirmReservePacket);
                    Console.WriteLine("User {0} => Reserving character \"{1}\"", client.currentUserId, charaReq.characterName);
                    break;
                case 0x02://Make                    
                    Character character = Character.EncodedToCharacter(charaReq.characterInfoEncoded);
                    Database.makeCharacter(client.currentUserId, name, character);

                    //Confirm
                    BasePacket confirmMakePacket = new BasePacket("./packets/chara/confirmMake.bin");
                    BasePacket.encryptPacket(client.blowfish, confirmMakePacket);
                    client.queuePacket(confirmMakePacket);
                    Console.WriteLine("User {0} => Character created!", client.currentUserId);
                    break;
                case 0x03://Rename
                    break;
                case 0x04://Delete
                    Database.deleteCharacter(charaReq.characterId, charaReq.characterName);

                    //Confirm
                    BasePacket deleteConfirmPacket = new BasePacket("./packets/chara/confirmDelete.bin");
                    BasePacket.encryptPacket(client.blowfish, deleteConfirmPacket);
                    client.queuePacket(deleteConfirmPacket);
                    Console.WriteLine("User {0} => Character deleted \"{1}\"", client.currentUserId, charaReq.characterName);
                    break;
                case 0x06://Rename Retainer
                    break;
            }           
        }

        private void buildWorldLists(ClientConnection client, SubPacket packet)
        {
            List<World> serverList = Database.getServers();

            int serverCount = 0;
            int totalCount = 0;
            PacketStructs.WorldListPacket worldListPacket = new PacketStructs.WorldListPacket();
            uint isEndList = serverList.Count <= 6 ? (byte)(serverList.Count+1) : (byte)0;
            uint numWorlds = serverList.Count <= 6 ? (byte)serverList.Count : (byte)6;
            numWorlds <<= 8;

            worldListPacket.isEndListANDNumWorlds = (uint)(isEndList | numWorlds);
            worldListPacket.sequence = 0;
            worldListPacket.unknown1 = 0;
            worldListPacket.worlds = new PacketStructs.WorldListEntry[6];

            foreach (World world in serverList)
            {
                //Insert world entry
                PacketStructs.WorldListEntry entry = new PacketStructs.WorldListEntry();
                entry.id = world.id;
                entry.listPosition = world.listPosition;
                entry.population = world.population;
                entry.name = world.name;
                worldListPacket.worlds[serverCount] = entry;

                serverCount++;
                totalCount++;
                if (serverCount % 6 == 0 || totalCount >= serverList.Count)
                {
                    //Send this chunk of world list
                    byte[] data = PacketStructs.StructureToByteArray(worldListPacket);
                    SubPacket subpacket = new SubPacket(0x15, 0xe0006868, 0xe0006868, data);
                    BasePacket basePacket = BasePacket.createPacket(subpacket, true, false);
                    BasePacket.encryptPacket(client.blowfish, basePacket);
                    client.queuePacket(basePacket);

                    //Make new worldlist if still remaining
                    if (totalCount <= serverList.Count)
                    {
                        worldListPacket = new PacketStructs.WorldListPacket();
                        isEndList = serverList.Count <= 6 ? (byte)(serverList.Count + 1) : (byte)0;
                        numWorlds = serverList.Count <= 6 ? (byte)serverList.Count : (byte)6;
                        numWorlds <<= 8;
                        worldListPacket.isEndListANDNumWorlds = (uint)(isEndList | numWorlds);
                        worldListPacket.sequence = 0;
                        worldListPacket.unknown1 = 0;
                    }
                }
            }
        }

    }
}
