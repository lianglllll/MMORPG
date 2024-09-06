/*
Navicat MySQL Data Transfer

Source Server         : 本机
Source Server Version : 50724
Source Host           : localhost:3306
Source Database       : mmorpg

Target Server Type    : MYSQL
Target Server Version : 50724
File Encoding         : 65001

Date: 2024-09-06 22:30:31
*/

SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for character
-- ----------------------------
DROP TABLE IF EXISTS `character`;
CREATE TABLE `character` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `JobId` int(11) NOT NULL,
  `Name` varchar(255) DEFAULT NULL,
  `Hp` int(11) NOT NULL,
  `Mp` int(11) NOT NULL,
  `Level` int(11) NOT NULL,
  `Exp` bigint(11) NOT NULL,
  `SpaceId` int(11) NOT NULL,
  `X` int(11) NOT NULL,
  `Y` int(11) NOT NULL,
  `Z` int(11) NOT NULL,
  `Gold` bigint(20) NOT NULL,
  `PlayerId` int(11) NOT NULL,
  `Knapsack` blob,
  `EquipsData` blob,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=22 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of character
-- ----------------------------
INSERT INTO `character` VALUES ('8', '0', '常世万法仙君', '100', '100', '1', '0', '0', '7758', '-359224', '1042', '0', '3', 0x080A, '');
INSERT INTO `character` VALUES ('9', '1', '斩神长老', '415', '600', '13', '6740', '1', '300279', '14', '197698', '6640', '3', 0x080A120608E90710AE03, 0x121008ED07100118FFFFFFFFFFFFFFFFFF01);
INSERT INTO `character` VALUES ('13', '1', '21', '355', '500', '1', '0', '0', '0', '1198', '0', '0', '7', null, null);
INSERT INTO `character` VALUES ('15', '1', '1122', '360', '425', '1', '10', '1', '288454', '14', '210898', '5', '4', 0x080A, 0x121008ED07100118FFFFFFFFFFFFFFFFFF01);
INSERT INTO `character` VALUES ('19', '4', '哦哈哟', '0', '600', '11', '11665', '1', '312262', '24', '185290', '63255', '3', 0x080A, 0x121008ED07100118FFFFFFFFFFFFFFFFFF01);
INSERT INTO `character` VALUES ('20', '4', '爷傲、奈我何', '18811', '600', '6', '2595', '1', '288853', '24', '200874', '335', '4', 0x080A120508E9071001, 0x121008ED07100118FFFFFFFFFFFFFFFFFF01);
INSERT INTO `character` VALUES ('21', '4', '222', '48093', '600', '1', '0', '1', '282312', '24', '167288', '0', '6', 0x080A, '');

-- ----------------------------
-- Table structure for user
-- ----------------------------
DROP TABLE IF EXISTS `user`;
CREATE TABLE `user` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `Username` varchar(255) DEFAULT NULL,
  `Password` varchar(255) DEFAULT NULL,
  `Coin` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Records of user
-- ----------------------------
INSERT INTO `user` VALUES ('3', '1', '1', '0');
INSERT INTO `user` VALUES ('4', '2', '2', '0');
INSERT INTO `user` VALUES ('6', '3', '3', '0');
INSERT INTO `user` VALUES ('7', '12', '12', '0');
INSERT INTO `user` VALUES ('8', '123', '123', '0');
INSERT INTO `user` VALUES ('9', '222', '222', '0');
