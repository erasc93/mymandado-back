-- MySQL dump 10.13  Distrib 9.5.0, for Linux (x86_64)
--
-- Host: localhost    Database: mymandado
-- ------------------------------------------------------
-- Server version	9.5.0

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- GTID state at the beginning of the backup 
--


--
-- Current Database: `mymandado`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `mymandado` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `mymandado`;

--
-- Table structure for table `CART`
--

DROP TABLE IF EXISTS `CART`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CART` (
  `car_id` int NOT NULL AUTO_INCREMENT,
  `car_crtnb` int NOT NULL DEFAULT '0',
  `car_usrid` int NOT NULL,
  `car_name` varchar(30) DEFAULT NULL,
  `car_desc` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`car_id`),
  UNIQUE KEY `car_usrid` (`car_usrid`,`car_crtnb`),
  CONSTRAINT `cart_ibfk_1` FOREIGN KEY (`car_usrid`) REFERENCES `USERS` (`usr_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=84 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CART`
--

LOCK TABLES `CART` WRITE;
UNLOCK TABLES;

--
-- Table structure for table `CART_ITEMS`
--

DROP TABLE IF EXISTS `CART_ITEMS`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CART_ITEMS` (
  `crt_id` int NOT NULL AUTO_INCREMENT,
  `crt_usrid` int NOT NULL,
  `crt_crtnb` int NOT NULL,
  `crt_prdid` int NOT NULL,
  `crt_qtty` int NOT NULL DEFAULT '0',
  `crt_isdone` tinyint(1) DEFAULT '0',
  PRIMARY KEY (`crt_id`),
  UNIQUE KEY `crt_usrid` (`crt_usrid`,`crt_crtnb`,`crt_prdid`),
  KEY `crt_prdid` (`crt_prdid`),
  CONSTRAINT `cart_items_ibfk_1` FOREIGN KEY (`crt_usrid`, `crt_crtnb`) REFERENCES `CART` (`car_usrid`, `car_crtnb`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `cart_items_ibfk_2` FOREIGN KEY (`crt_prdid`) REFERENCES `PRODUCTS` (`prd_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=231 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CART_ITEMS`
--

LOCK TABLES `CART_ITEMS` WRITE;
UNLOCK TABLES;

--
-- Table structure for table `CART_SHARES`
--

DROP TABLE IF EXISTS `CART_SHARES`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `CART_SHARES` (
  `csh_id` int NOT NULL AUTO_INCREMENT,
  `csh_carid` int NOT NULL,
  `csh_usrid` int NOT NULL,
  PRIMARY KEY (`csh_id`),
  UNIQUE KEY `csh_carid` (`csh_carid`,`csh_usrid`),
  KEY `csh_usrid` (`csh_usrid`),
  CONSTRAINT `cart_shares_ibfk_1` FOREIGN KEY (`csh_carid`) REFERENCES `CART` (`car_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `cart_shares_ibfk_2` FOREIGN KEY (`csh_usrid`) REFERENCES `USERS` (`usr_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `CART_SHARES`
--

LOCK TABLES `CART_SHARES` WRITE;
UNLOCK TABLES;

--
-- Table structure for table `PRODUCTS`
--

DROP TABLE IF EXISTS `PRODUCTS`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PRODUCTS` (
  `prd_id` int NOT NULL AUTO_INCREMENT,
  `prd_name` varchar(30) NOT NULL,
  `prd_unit` varchar(10) DEFAULT NULL,
  PRIMARY KEY (`prd_id`),
  UNIQUE KEY `prd_name` (`prd_name`)
) ENGINE=InnoDB AUTO_INCREMENT=138 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PRODUCTS`
--

LOCK TABLES `PRODUCTS` WRITE;
UNLOCK TABLES;

--
-- Table structure for table `PRODUCTS_EXCLUSIONS`
--

DROP TABLE IF EXISTS `PRODUCTS_EXCLUSIONS`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `PRODUCTS_EXCLUSIONS` (
  `xld_userid` int NOT NULL,
  `xld_prdid` int NOT NULL,
  KEY `fk_exclude_user` (`xld_userid`),
  KEY `fk_exclude_prd` (`xld_prdid`),
  CONSTRAINT `fk_exclude_prd` FOREIGN KEY (`xld_prdid`) REFERENCES `PRODUCTS` (`prd_id`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `fk_exclude_user` FOREIGN KEY (`xld_userid`) REFERENCES `USERS` (`usr_id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `PRODUCTS_EXCLUSIONS`
--

LOCK TABLES `PRODUCTS_EXCLUSIONS` WRITE;
UNLOCK TABLES;

--
-- Table structure for table `USERS`
--

DROP TABLE IF EXISTS `USERS`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `USERS` (
  `usr_id` int NOT NULL AUTO_INCREMENT,
  `usr_role` int NOT NULL DEFAULT '0',
  `usr_name` varchar(30) NOT NULL,
  `usr_hashword` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`usr_id`),
  UNIQUE KEY `usr_name` (`usr_name`)
) ENGINE=InnoDB AUTO_INCREMENT=51 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `USERS`
--

LOCK TABLES `USERS` WRITE;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2026-01-24 22:11:27
