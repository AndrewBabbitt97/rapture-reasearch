-- MySQL dump 10.13  Distrib 5.7.10, for Win64 (x86_64)
--
-- Host: localhost    Database: ffxiv_database
-- ------------------------------------------------------
-- Server version	5.7.10-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `server_zones`
--

DROP TABLE IF EXISTS `server_zones`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `server_zones` (
  `id` int(10) unsigned NOT NULL,
  `regionId` smallint(6) unsigned NOT NULL,
  `zoneName` varchar(255) DEFAULT NULL,
  `placeName` varchar(255) NOT NULL,
  `className` varchar(30) NOT NULL,
  `dayMusic` smallint(6) unsigned DEFAULT '0',
  `nightMusic` smallint(6) unsigned DEFAULT '0',
  `battleMusic` smallint(6) unsigned DEFAULT '0',
  `isIsolated` tinyint(1) DEFAULT '0',
  `isInn` tinyint(1) DEFAULT '0',
  `canRideChocobo` tinyint(1) DEFAULT '1',
  `canStealth` tinyint(1) DEFAULT '0',
  `isInstanceRaid` tinyint(1) unsigned DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `server_zones`
--

LOCK TABLES `server_zones` WRITE;
/*!40000 ALTER TABLE `server_zones` DISABLE KEYS */;
INSERT INTO `server_zones` VALUES (0,0,NULL,'--','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (128,101,'sea0Field01','Lower La Noscea','ZoneMasterSeaS0',60,60,21,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (129,101,'sea0Field02','Western La Noscea','ZoneMasterSeaS0',60,60,21,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (130,101,'sea0Field03','Eastern La Noscea','ZoneMasterSeaS0',60,60,21,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (131,101,'sea0Dungeon01','Mistbeard Cove','ZoneMasterSeaS0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (132,101,'sea0Dungeon02','Cassiopeia Hollow','ZoneMasterSeaS0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (133,101,'sea0Town01','Limsa Lominsa','ZoneMasterSeaS0',59,59,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (134,202,'sea0Market01','Market Wards','ZoneMasterMarketSeaS0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (135,101,'sea0Field04','Upper La Noscea','ZoneMasterSeaS0',60,60,21,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (137,101,NULL,'U\'Ghamaro Mines','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (138,101,NULL,'La Noscea','',60,60,21,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (139,101,NULL,'The Cieldalaes','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (140,101,NULL,'Sailors Ward','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (141,101,'sea0Field01a','Lower La Noscea','ZoneMasterSeaS0',60,60,21,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (143,102,'roc0Field01','Coerthas Central Highlands','ZoneMasterRocR0',55,55,15,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (144,102,'roc0Field02','Coerthas Eastern Highlands','ZoneMasterRocR0',55,55,15,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (145,102,'roc0Field03','Coerthas Eastern Lowlands','ZoneMasterRocR0',55,55,15,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (146,102,NULL,'Coerthas','',55,55,15,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (147,102,'roc0Field04','Coerthas Central Lowlands','ZoneMasterRocR0',55,55,15,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (148,102,'roc0Field05','Coerthas Western Highlands','ZoneMasterRocR0',55,55,15,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (150,103,'fst0Field01','Central Shroud','ZoneMasterFstF0',52,52,13,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (151,103,'fst0Field02','East Shroud','ZoneMasterFstF0',52,52,13,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (152,103,'fst0Field03','North Shroud','ZoneMasterFstF0',52,52,13,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (153,103,'fst0Field04','West Shroud','ZoneMasterFstF0',52,52,13,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (154,103,'fst0Field05','South Shroud','ZoneMasterFstF0',52,52,13,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (155,103,'fst0Town01','Gridania','ZoneMasterFstF0',51,51,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (156,103,NULL,'The Black Shroud','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (157,103,'fst0Dungeon01','The Mun-Tuy Cellars','ZoneMasterFstF0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (158,103,'fst0Dungeon02','The Tam-Tara Deepcroft','ZoneMasterFstF0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (159,103,'fst0Dungeon03','The Thousand Maws of Toto-Rak','ZoneMasterFstF0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (160,204,'fst0Market01','Market Wards','ZoneMasterMarketFstF0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (161,103,NULL,'Peasants Ward','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (162,103,NULL,'Central Shroud','',52,52,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (164,106,'fst0Battle01','Central Shroud','ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (165,106,'fst0Battle02','Central Shroud','ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (166,106,'fst0Battle03','Central Shroud','ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (167,106,'fst0Battle04','Central Shroud','ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (168,106,'fst0Battle05','Central Shroud','ZoneMasterBattleFstF0',0,0,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (170,104,'wil0Field01','Central Thanalan','ZoneMasterWilW0',68,68,25,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (171,104,'wil0Field02','Eastern Thanalan','ZoneMasterWilW0',68,68,25,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (172,104,'wil0Field03','Western Thanalan','ZoneMasterWilW0',68,68,25,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (173,104,'wil0Field04','Northern Thanalan','ZoneMasterWilW0',68,68,25,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (174,104,'wil0Field05','Southern Thanalan','ZoneMasterWilW0',68,68,25,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (175,104,'wil0Town01','Ul\'dah','ZoneMasterWilW0',66,66,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (176,104,'wil0Dungeon01','Nanawa Mines','ZoneMasterWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (177,207,'_jail','-','ZoneMasterJail',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (178,104,'wil0Dungeon02','Copperbell Mines','ZoneMasterWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (179,104,NULL,'Thanalan','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (180,205,'wil0Market01','Market Wards','ZoneMasterMarketWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (181,104,NULL,'Merchants Ward','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (182,104,NULL,'Central Thanalan','',68,68,25,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (184,107,'wil0Battle01','Ul\'dah','ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (185,107,'wil0Battle01','Ul\'dah','ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (186,104,'wil0Battle02','Ul\'dah','ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (187,104,'wil0Battle03','Ul\'dah','ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (188,104,'wil0Battle04','Ul\'dah','ZoneMasterBattleWilW0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (190,105,'lak0Field01','Mor Dhona','ZoneMasterLakL0',49,49,11,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (192,111,'ocn0Battle01','Rhotano Sea','ZoneMasterBattleOcnO0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (193,111,'ocn0Battle02','Rhotano Sea','ZoneMasterBattleOcnO0',7,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (194,111,'ocn0Battle03','Rhotano Sea','ZoneMasterBattleOcnO0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (195,111,'ocn0Battle04','Rhotano Sea','ZoneMasterBattleOcnO0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (196,111,'ocn0Battle05','Rhotano Sea','ZoneMasterBattleOcnO0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (198,111,'ocn0Battle06','Rhotano Sea','ZoneMasterBattleOcnO0',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (200,111,NULL,'Strait of Merlthor','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (201,111,NULL,'-','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (204,101,'sea0Field02a','Western La Noscea','',60,60,21,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (205,101,'sea0Field03a','Eastern La Noscea','',60,60,21,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (206,103,'fst0Town1a','Gridania','ZoneMasterFstF0',52,52,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (207,103,NULL,'North Shroud','',52,52,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (208,103,NULL,'South Shroud','',52,52,13,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (209,104,'wil0Town01a','Ul\'dah','ZoneMasterWilW0',66,66,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (210,104,NULL,'Eastern Thanalan','',68,68,25,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (211,104,NULL,'Western Thanalan','',68,68,25,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (230,101,'sea0Town01a','Limsa Lominsa','ZoneMasterSeaS0',59,59,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (231,102,'roc0Dungeon01','Dzemael Darkhold','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (232,101,'sea0Office01','Maelstrom Command','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (233,104,'wil0Office01','Hall of Flames','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (234,103,'fst0Office01','Adders\' Nest','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (235,101,NULL,'Shposhae','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (236,101,NULL,'Locke\'s Lie','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (237,101,NULL,'Turtleback Island','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (238,103,'fst0Field04','Thornmarch','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (239,102,NULL,'The Howling Eye','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (240,104,'wil0Field05a','The Bowl of Embers','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (244,209,'prv0Inn01','Inn Room','ZoneMasterPrvI0',61,61,0,0,1,0,0,0);
INSERT INTO `server_zones` VALUES (245,102,'roc0Dungeon02','The Aurum Vale','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (246,104,NULL,'Cutter\'s Cry','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (247,103,NULL,'North Shroud','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (248,101,NULL,'Western La Noscea','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (249,104,NULL,'Eastern Thanalan','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (250,102,NULL,'The Howling Eye','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (251,105,NULL,'Transmission Tower','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (252,102,NULL,'The Aurum Vale','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (253,102,NULL,'The Aurum Vale','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (254,104,NULL,'Cutter\'s Cry','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (255,104,NULL,'Cutter\'s Cry','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (256,102,NULL,'The Howling Eye','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (257,109,NULL,'Rivenroad','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (258,103,NULL,'North Shroud','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (259,103,NULL,'North Shroud','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (260,101,NULL,'Western La Noscea','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (261,101,NULL,'Western La Noscea','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (262,104,NULL,'Eastern Thanalan','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (263,104,NULL,'Eastern Thanalan','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (264,105,'lak0Field01','Transmission Tower','',0,0,0,0,0,1,0,0);
INSERT INTO `server_zones` VALUES (265,104,NULL,'The Bowl of Embers','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (266,105,'lak0Field01a','Mor Dhona','ZoneMasterLakL0',49,49,11,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (267,109,NULL,'Rivenroad','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (268,109,NULL,'Rivenroad','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (269,101,NULL,'Locke\'s Lie','',0,0,0,0,0,0,0,0);
INSERT INTO `server_zones` VALUES (270,101,NULL,'Turtleback Island','',0,0,0,0,0,0,0,0);
/*!40000 ALTER TABLE `server_zones` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2016-06-07 22:54:54