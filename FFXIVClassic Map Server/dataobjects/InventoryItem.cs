﻿using System;
using System.IO;

namespace FFXIVClassic_Map_Server.dataobjects
{
    class InventoryItem
    {
        public const byte DEALINGMODE_NONE = 0;
        public const byte DEALINGMODE_REFERENCED = 1;
        public const byte DEALINGMODE_PRICED = 2;

        public const byte TAG_EXCLUSIVE = 0x3;
        public const byte TAG_DEALING = 0xC9;
        public const byte TAG_ATTACHED = 0xCA;

        public const byte TYPE_SINGLE = 12;
        public const byte TYPE_STACK = 13;
        public const byte TYPE_SEEK_ITEM = 20;
        public const byte TYPE_SEEK_REPAIR = 30;

        public ulong uniqueId;
        public uint itemId;
        public int quantity = 1;
        public ushort slot;

        public byte dealingVal       = 0;
        public byte dealingMode      = DEALINGMODE_NONE;
        public uint dealingAttached1 = 0;
        public uint dealingAttached2 = 0;
        public uint dealingAttached3 = 0;

        public byte[] tags = new byte[4];
        public byte[] tagValues = new byte[4];

        public byte quality = 1;

        public ItemModifier modifiers;

        public class ItemModifier
        {
            public uint durability = 0;
            public ushort use = 0;
            public uint materiaId = 0;
            public uint materiaLife = 0;
            public byte mainQuality = 0;
            public byte[] subQuality = new byte[3];
            public uint polish = 0;
            public uint param1 = 0;
            public uint param2 = 0;
            public uint param3 = 0;
            public ushort spiritbind = 0;
            public byte[] materiaType = new byte[5];
            public byte[] materiaGrade = new byte[5];

            public ItemModifier()
            {                
            }

            public ItemModifier(MySql.Data.MySqlClient.MySqlDataReader reader)
            {
                durability = reader.GetUInt32("durability"); 
                mainQuality = reader.GetByte("mainQuality");
                subQuality[0] = reader.GetByte("subQuality1");
                subQuality[1] = reader.GetByte("subQuality2");
                subQuality[2] = reader.GetByte("subQuality3");
                param1 = reader.GetUInt32("param1");
                param2 = reader.GetUInt32("param2");
                param3 = reader.GetUInt32("param3");
                spiritbind = reader.GetUInt16("spiritbind");

                ushort materia1 = reader.GetUInt16("materia1");
                ushort materia2 = reader.GetUInt16("materia2");
                ushort materia3 = reader.GetUInt16("materia3");
                ushort materia4 = reader.GetUInt16("materia4");
                ushort materia5 = reader.GetUInt16("materia5");

                materiaType[0] = (byte)(materia1 & 0xFF);
                materiaGrade[0] = (byte)((materia1 >> 8) & 0xFF);
                materiaType[1] = (byte)(materia2 & 0xFF);
                materiaGrade[1] = (byte)((materia2 >> 8) & 0xFF);
                materiaType[2] = (byte)(materia3 & 0xFF);
                materiaGrade[2] = (byte)((materia3 >> 8) & 0xFF);
                materiaType[3] = (byte)(materia4 & 0xFF);
                materiaGrade[3] = (byte)((materia4 >> 8) & 0xFF);
                materiaType[4] = (byte)(materia5 & 0xFF);
                materiaGrade[4] = (byte)((materia5 >> 8) & 0xFF);
            }

            public void WriteBytes(BinaryWriter binWriter)
            {
                binWriter.Write((UInt32) durability);
                binWriter.Write((UInt16) use);
                binWriter.Write((UInt32) materiaId);
                binWriter.Write((UInt32) materiaLife);
                binWriter.Write((Byte)   mainQuality);
                binWriter.Write((Byte)   subQuality[0]);
                binWriter.Write((Byte)   subQuality[1]);
                binWriter.Write((Byte)   subQuality[2]);
                binWriter.Write((UInt32) polish);
                binWriter.Write((UInt32) param1);
                binWriter.Write((UInt32) param2);
                binWriter.Write((UInt32) param3);
                binWriter.Write((UInt16) spiritbind);
                binWriter.Write((Byte)   materiaType[0]);
                binWriter.Write((Byte)   materiaType[1]);
                binWriter.Write((Byte)   materiaType[2]);
                binWriter.Write((Byte)   materiaType[3]);
                binWriter.Write((Byte)   materiaType[4]);
                binWriter.Write((Byte)   materiaGrade[0]);
                binWriter.Write((Byte)   materiaGrade[1]);
                binWriter.Write((Byte)   materiaGrade[2]);
                binWriter.Write((Byte)   materiaGrade[3]);
                binWriter.Write((Byte)   materiaGrade[4]);
            }
        }

        //Bare Minimum
        public InventoryItem(uint id, uint itemId)
        {
            this.uniqueId = id;
            this.itemId = itemId;
            this.quantity = 1;

            ItemData gItem = Server.GetItemGamedata(itemId);
            tags[1] = gItem.isExclusive ? (byte)0x3 : (byte)0x0;
        }

        //For check command
        public InventoryItem(InventoryItem item, ushort equipSlot)
        {
            this.uniqueId = item.uniqueId;
            this.itemId = item.itemId;
            this.quantity = item.quantity;
            this.slot = equipSlot;

            this.tags = item.tags;
            this.tagValues = item.tagValues;

            this.quality = item.quality;

            this.modifiers = item.modifiers;
        }

        public InventoryItem(uint uniqueId, uint itemId, int quantity, byte[] tags, byte[] tagValues, byte qualityNumber, ItemModifier modifiers = null)
        {
            this.uniqueId = uniqueId;
            this.itemId = itemId;
            this.quantity = quantity;
            
            if (tags != null)
                this.tags = tags;
            if (tagValues != null)
                this.tagValues = tagValues;

            this.quality = qualityNumber;
            this.modifiers = modifiers;
        }

        public byte[] ToPacketBytes()
        {           
            byte[] data = new byte[0x70];

            using (MemoryStream mem = new MemoryStream(data))
            {
                using (BinaryWriter binWriter = new BinaryWriter(mem))
                {
                    binWriter.Write((UInt64)uniqueId);
                    binWriter.Write((Int32)quantity);
                    binWriter.Write((UInt32)itemId);
                    binWriter.Write((UInt16)slot);

                    binWriter.Write((Byte)dealingVal);
                    binWriter.Write((Byte)dealingMode);

                    binWriter.Write((UInt32)dealingAttached1);
                    binWriter.Write((UInt32)dealingAttached2);
                    binWriter.Write((UInt32)dealingAttached3);

                    for (int i = 0; i < tags.Length; i++)
                        binWriter.Write((Byte) tags[i]);
                    for (int i = 0; i < tagValues.Length; i++)
                        binWriter.Write((Byte) tagValues[i]);

                    binWriter.Write((Byte)quality);
                                        
                    if (modifiers != null)
                    {
                        binWriter.Write((Byte)0x01);
                        modifiers.WriteBytes(binWriter);
                    }                  
                }                
            }

            return data;        
        }

        public void SetExclusive(bool isExclusive)
        {
            tags[0] = isExclusive ? TAG_EXCLUSIVE : (byte)0;
        }

        public void SetHasAttached(bool isAttached)
        {
            tags[0] = isAttached ? TAG_ATTACHED : (byte)0;
        }

        public void SetDealing(byte mode, uint price)
        {
            if (tags[0] == TAG_EXCLUSIVE)
                return;
                      
            tags[0] = TAG_DEALING;
            tagValues[0] = mode;      
            
            if (dealingMode != DEALINGMODE_REFERENCED)
            {
                dealingVal = 1;
                dealingMode = DEALINGMODE_PRICED;
                dealingAttached1 = 1;
                dealingAttached2 = itemId;
                dealingAttached3 = price; 
            }
        }

        public void SetAttachedToSlot(ushort package, ushort slot)
        {
            dealingVal = 1;
            dealingMode = DEALINGMODE_REFERENCED;
            dealingAttached1 = (uint)((ushort)package << 16) | slot;
            dealingAttached2 = 0;
            dealingAttached3 = 0;
        }        

    }
}
