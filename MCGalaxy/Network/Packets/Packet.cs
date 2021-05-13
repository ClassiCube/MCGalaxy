/*
    Copyright 2015 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using MCGalaxy.Blocks;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Network {

    /// <summary> Constructors for all classic and CPE packets. </summary>
    public static class Packet {
        
        #region Classic
        
        public static byte[] Motd(Player p, string motd) {
            byte[] buffer = new byte[131];
            buffer[0] = Opcode.Handshake;
            buffer[1] = Server.version;
            
            if (motd.Length > NetUtils.StringSize) {
                NetUtils.Write(motd, buffer, 2, p.hasCP437);
                NetUtils.Write(motd.Substring(NetUtils.StringSize), buffer, 66, p.hasCP437);
            } else {
                NetUtils.Write(Server.Config.Name, buffer, 2, p.hasCP437);
                NetUtils.Write(motd, buffer, 66, p.hasCP437);
            }

            buffer[130] = p.UserType();
            return buffer;
        }
        
        public static byte[] Ping() { return new byte[] { Opcode.Ping }; }
        
        public static byte[] LevelInitalise() { return new byte[] { Opcode.LevelInitialise }; }        
        public static byte[] LevelInitaliseExt(int volume) {
            byte[] buffer = new byte[5];
            buffer[0] = Opcode.LevelInitialise;
            NetUtils.WriteI32(volume, buffer, 1);
            return buffer;
        }
        
        public static byte[] LevelFinalise(ushort width, ushort height, ushort length) {
            byte[] buffer = new byte[7];
            buffer[0] = Opcode.LevelFinalise;
            NetUtils.WriteU16(width,  buffer, 1);
            NetUtils.WriteU16(height, buffer, 3);
            NetUtils.WriteU16(length, buffer, 5);
            return buffer;
        }
        
        public static byte[] AddEntity(byte entityID, string name, Position pos,
                                       Orientation rot, bool hasCP437, bool extPos) {
            byte[] buffer = new byte[74 + (extPos ? 6 : 0)];
            buffer[0] = Opcode.AddEntity;
            buffer[1] = entityID;
            NetUtils.Write(name, buffer, 2, hasCP437);
            
            int offset = NetUtils.WritePos(pos, buffer, 66, extPos);
            buffer[66 + offset] = rot.RotY;
            buffer[67 + offset] = rot.HeadX;
            return buffer;
        }
        
        public static byte[] Teleport(byte entityID, Position pos, Orientation rot, bool extPos) {
            byte[] buffer = new byte[10 + (extPos ? 6 : 0)];
            buffer[0] = Opcode.EntityTeleport;
            buffer[1] = entityID;
            
            int offset = NetUtils.WritePos(pos, buffer, 2, extPos);
            buffer[2 + offset] = rot.RotY;
            buffer[3 + offset] = rot.HeadX;
            return buffer;
        }
        
        public static byte[] RemoveEntity(byte entityID) {
            return new byte[] { Opcode.RemoveEntity, entityID };
        }
        
        public static byte[] BlankMessage() { return Message("", 0, false); }
        
        public static byte[] Message(string message, CpeMessageType type, bool hasCp437) {
            byte[] buffer = new byte[66];
            WriteMessage(message, (byte)type, hasCp437, buffer, 0);
            return buffer;
        }
        
        public static void WriteMessage(string message, byte type, bool hasCp437, byte[] buffer, int index) {
            buffer[index++] = Opcode.Message;
            buffer[index++] = type;
            NetUtils.Write(message, buffer, index, hasCp437);
        }
        
        public static byte[] Kick(string message, bool cp437) {
            byte[] buffer = new byte[65];
            buffer[0] = Opcode.Kick;
            NetUtils.Write(message, buffer, 1, cp437);
            return buffer;
        }

        public static byte[] UserType(byte type) {
            return new byte[] { Opcode.SetPermission, type };
        }
        
        #endregion
        
        
        #region CPE
        
        public static byte[] ExtInfo(byte extsCount) {
            byte[] buffer = new byte[67];
            buffer[0] = Opcode.CpeExtInfo;
            NetUtils.Write(Server.SoftwareNameVersioned, buffer, 1, false);
            NetUtils.WriteI16((short)extsCount, buffer, 65);
            return buffer;
        }
        
        public static byte[] ExtEntry(string name, int version) {
            byte[] buffer = new byte[69];
            buffer[0] = Opcode.CpeExtEntry;
            NetUtils.Write(name, buffer, 1, false);
            NetUtils.WriteI32(version, buffer, 65);
            return buffer;
        }
        
        public static byte[] ClickDistance(short distance) {
            byte[] buffer = new byte[3];
            buffer[0] = Opcode.CpeSetClickDistance;
            NetUtils.WriteI16(distance, buffer, 1);
            return buffer;
        }
        
        public static byte[] CustomBlockSupportLevel(byte level) {
            byte[] buffer = new byte[2];
            buffer[0] = Opcode.CpeCustomBlockSupportLevel;
            buffer[1] = level;
            return buffer;
        }
        
        public static byte[] HoldThis(BlockID raw, bool locked, bool extBlocks) {
            byte[] buffer = new byte[extBlocks ? 4 : 3];
            buffer[0] = Opcode.CpeHoldThis;
            NetUtils.WriteBlock(raw, buffer, 1, extBlocks);
            buffer[extBlocks ? 3 : 2] = (byte)(locked ? 1 : 0);
            return buffer;
        }
        
        public static byte[] TextHotKey(string label, string input, int keycode,
                                        byte mods, bool hasCP437) {
            // per spec, \n should cause client to automatically send hotkey
            input = input.Replace('\n', '◙');
        	
            byte[] buffer = new byte[134];
            buffer[0] = Opcode.CpeSetTextHotkey;
            NetUtils.Write(label, buffer, 1, hasCP437);
            NetUtils.Write(input, buffer, 65, hasCP437);
            NetUtils.WriteI32(keycode, buffer, 129);
            buffer[133] = mods;
            return buffer;
        }
        
        public static byte[] ExtAddEntity(byte entityID, string skin, string name, bool hasCP437) {
            byte[] buffer = new byte[130];
            buffer[0] = Opcode.CpeExtAddEntity;
            buffer[1] = entityID;
            NetUtils.Write(skin, buffer, 2, hasCP437);
            NetUtils.Write(name, buffer, 66, hasCP437);
            return buffer;
        }
        
        public static byte[] ExtAddPlayerName(byte nameID, string listName, string displayName,
                                              string grp, byte grpRank, bool hasCP437) {
            byte[] buffer = new byte[196];
            buffer[0] = Opcode.CpeExtAddPlayerName;
            NetUtils.WriteI16(nameID, buffer, 1);
            NetUtils.Write(listName, buffer, 3, hasCP437);
            NetUtils.Write(displayName, buffer, 67, hasCP437);
            NetUtils.Write(grp, buffer, 131, hasCP437);
            buffer[195] = grpRank;
            return buffer;
        }
        
        public static byte[] ExtRemovePlayerName(byte nameID) {
            byte[] buffer = new byte[3];
            buffer[0] = Opcode.CpeExtRemovePlayerName;
            NetUtils.WriteI16(nameID, buffer, 1);
            return buffer;
        }

        public static byte[] EnvColor(byte type, short r, short g, short b) {
            byte[] buffer = new byte[8];
            buffer[0] = Opcode.CpeEnvColors;
            buffer[1] = type;
            NetUtils.WriteI16(r, buffer, 2);
            NetUtils.WriteI16(g, buffer, 4);
            NetUtils.WriteI16(b, buffer, 6);
            return buffer;
        }
        
        public static byte[] MakeSelection(byte selID, string label, Vec3U16 p1, Vec3U16 p2,
                                           short r, short g, short b, short opacity, bool hasCP437) {
            byte[] buffer = new byte[86];
            buffer[0] = Opcode.CpeMakeSelection;
            buffer[1] = selID;
            NetUtils.Write(label, buffer, 2, hasCP437);
            
            NetUtils.WriteU16(p1.X, buffer, 66);
            NetUtils.WriteU16(p1.Y, buffer, 68);
            NetUtils.WriteU16(p1.Z, buffer, 70);
            NetUtils.WriteU16(p2.X, buffer, 72);
            NetUtils.WriteU16(p2.Y, buffer, 74);
            NetUtils.WriteU16(p2.Z, buffer, 76);
            
            NetUtils.WriteI16(r, buffer, 78);
            NetUtils.WriteI16(g, buffer, 80);
            NetUtils.WriteI16(b, buffer, 82);
            NetUtils.WriteI16(opacity, buffer, 84);
            return buffer;
        }
        
        public static byte[] DeleteSelection(byte selID) {
            byte[] buffer = new byte[2];
            buffer[0] = Opcode.CpeRemoveSelection;
            buffer[1] = selID;
            return buffer;
        }
        
        public static byte[] BlockPermission(BlockID raw, bool place, bool delete, bool extBlocks) {
            byte[] buffer = new byte[extBlocks ? 5 : 4];
            WriteBlockPermission(raw, place, delete, extBlocks, buffer, 0);
            return buffer;
        }
        
        public static void WriteBlockPermission(BlockID raw, bool place, bool delete, bool extBlocks, byte[] buffer, int index) {
            buffer[index++] = Opcode.CpeSetBlockPermission;
            NetUtils.WriteBlock(raw, buffer, index, extBlocks);
            index += extBlocks ? 2 : 1;
            buffer[index++] = place  ? (byte)1 : (byte)0;
            buffer[index++] = delete ? (byte)1 : (byte)0;
        }
        
        public static byte[] ChangeModel(byte entityID, string model, bool hasCP437) {
            byte[] buffer = new byte[66];
            buffer[0] = Opcode.CpeChangeModel;
            buffer[1] = entityID;
            NetUtils.Write(model, buffer, 2, hasCP437);
            return buffer;
        }
        
        public static byte[] MapAppearance(string url, byte side, byte edge, int sideLevel,
                                           bool hasCP437) {
            byte[] buffer = new byte[69];
            WriteMapAppearance(buffer, url, side, edge, sideLevel, hasCP437);
            return buffer;
        }
        
        public static byte[] MapAppearanceV2(string url, byte side, byte edge, int sideLevel,
                                             int cloudHeight, int maxFog, bool hasCP437) {
            byte[] buffer = new byte[73];
            WriteMapAppearance(buffer, url, side, edge, sideLevel, hasCP437);
            NetUtils.WriteI16((short)cloudHeight, buffer, 69);
            NetUtils.WriteI16((short)maxFog, buffer, 71);
            return buffer;
        }
        
        static void WriteMapAppearance(byte[] buffer, string url, byte side, byte edge,
                                       int sideLevel, bool hasCP437) {
            buffer[0] = Opcode.CpeEnvSetMapApperance;
            NetUtils.Write(url, buffer, 1, hasCP437);
            buffer[65] = side;
            buffer[66] = edge;
            NetUtils.WriteI16((short)sideLevel, buffer, 67);
        }

        public static byte[] EnvWeatherType(byte type) { // 0 - sunny; 1 - raining; 2 - snowing
            byte[] buffer = new byte[2];
            buffer[0] = Opcode.CpeEnvWeatherType;
            buffer[1] = type;
            return buffer;
        }
        
        public static byte[] HackControl(bool canFly, bool canNoclip,
                                         bool canSpeed, bool canRespawn,
                                         bool can3rdPerson, short maxJumpHeight) {
            byte[] buffer = new byte[8];
            buffer[0] = Opcode.CpeHackControl;
            buffer[1] = (byte)(canFly ? 1 : 0);
            buffer[2] = (byte)(canNoclip ? 1 : 0);
            buffer[3] = (byte)(canSpeed ? 1 : 0);
            buffer[4] = (byte)(canRespawn ? 1 : 0);
            buffer[5] = (byte)(can3rdPerson ? 1 : 0);
            NetUtils.WriteI16(maxJumpHeight, buffer, 6);
            return buffer;
        }
        
        public static byte[] ExtAddEntity2(byte entityID, string skinName, string displayName,
                                           Position pos, Orientation rot, bool hasCP437, bool extPos) {
            byte[] buffer = new byte[138 + (extPos ? 6 : 0)];
            buffer[0] = Opcode.CpeExtAddEntity2;
            buffer[1] = entityID;
            NetUtils.Write(displayName, buffer,  2, hasCP437);
            NetUtils.Write(skinName,    buffer, 66, hasCP437);
            
            int offset = NetUtils.WritePos(pos, buffer, 130, extPos);
            buffer[130 + offset] = rot.RotY;
            buffer[131 + offset] = rot.HeadX;
            return buffer;
        }
        
        public static byte[] SetTextColor(ColorDesc col) {
            byte[] buffer = new byte[6];
            buffer[0] = Opcode.CpeSetTextColor;
            buffer[1] = col.R; buffer[2] = col.G; buffer[3] = col.B; buffer[4] = col.A;
            buffer[5] = col.Index;
            return buffer;
        }

        
        public static byte[] EnvMapUrl(string url, bool hasCP437) {
            byte[] buffer = new byte[65];
            buffer[0] = Opcode.CpeSetMapEnvUrl;
            NetUtils.Write(url, buffer, 1, hasCP437);
            return buffer;
        }
        
        public static byte[] EnvMapProperty(EnvProp prop, int value) {
            byte[] buffer = new byte[6];
            buffer[0] = Opcode.CpeSetMapEnvProperty;
            buffer[1] = (byte)prop;
            NetUtils.WriteI32(value, buffer, 2);
            return buffer;
        }
        
        public static byte[] EntityProperty(byte entityID, EntityProp prop, int value) {
            byte[] buffer = new byte[7];
            buffer[0] = Opcode.CpeSetEntityProperty;
            buffer[1] = entityID;
            buffer[2] = (byte)prop;
            NetUtils.WriteI32(value, buffer, 3);
            return buffer;
        }
        
        public static byte[] TwoWayPing(bool serverToClient, ushort data) {
            byte[] buffer = new byte[4];
            buffer[0] = Opcode.CpeTwoWayPing;
            buffer[1] = (byte)(serverToClient ? 1 : 0);
            NetUtils.WriteU16(data, buffer, 2);
            return buffer;
        }
        
        public static byte[] SetInventoryOrder(BlockID rawId, BlockID rawOrder, bool extBlocks) {
            byte[] buffer = new byte[extBlocks ? 5 : 3];
            buffer[0] = Opcode.CpeSetInventoryOrder;
            NetUtils.WriteBlock(rawId, buffer, 1, extBlocks);
            NetUtils.WriteBlock(rawOrder, buffer, extBlocks ? 3 : 2, extBlocks);
            return buffer;
        }
        
        public static byte[] SetHotbar(BlockID rawId, byte slot, bool extBlocks) {
            byte[] buffer = new byte[extBlocks ? 4 : 3];
            buffer[0] = Opcode.CpeSetHotbar;
            NetUtils.WriteBlock(rawId, buffer, 1, extBlocks);
            buffer[extBlocks ? 3 : 2] = slot;
            return buffer;
        }
        
        public static byte[] SetSpawnpoint(Position pos, Orientation rot, bool extPos) {
            byte[] buffer = new byte[extPos ? 15 : 9];
            buffer[0] = Opcode.CpeSetSpawnpoint;
            NetUtils.WritePos(pos, buffer, 1, extPos);
            int offset = extPos ? 13 : 7;
            buffer[offset] = rot.RotY;
            buffer[1+offset] = rot.HeadX;
            return buffer;
        }
        
        /// <summary> Velocity is considered in terms of the 
        /// force needed to reach [num] block height if applied to Y velocity.
        /// e.g. 0 1.233 0 would give an upward force identical to a default clientside jump.
        /// </summary>
        public static byte[] VelocityControl(float x, float y, float z, byte xMode, byte yMode, byte zMode) {
            byte[] buffer = new Byte[16];
            buffer[0] = Opcode.CpeVelocityControl;
            NetUtils.WriteI32((int)(x * 10000), buffer, 1);
            NetUtils.WriteI32((int)(y * 10000), buffer, 5);
            NetUtils.WriteI32((int)(z * 10000), buffer, 9);
            buffer[13] = xMode;
            buffer[14] = yMode;
            buffer[15] = zMode;
            return buffer;
        }
        
        public static byte[] DefineEffect(byte effectID,
                                            byte U1, byte V1, byte U2, byte V2,
                                            byte tintRed, byte tintGreen, byte tintBlue,
                                            byte frameCount, byte particleCount,
                                            byte size, float sizeVariation,
                                            float spread, float speed, float gravity,
                                            float baseLifetime, float lifetimeVariation,
                                            bool expireUponTouchingGround, bool collidesSolid, bool collidesLiquid, bool collidesLeaves,
                                            bool fullBright) {
            byte[] buffer = new byte[36];
            buffer[0] = Opcode.CpeDefineEffect;           
            buffer[1] = effectID;
            
            buffer[2] = U1;
            buffer[3] = V1;
            buffer[4] = U2;
            buffer[5] = V2;
            
            buffer[6] = tintRed;
            buffer[7] = tintGreen;
            buffer[8] = tintBlue;
            
            buffer[9]  = frameCount;
            buffer[10] = particleCount;
            buffer[11] = size;
            
            NetUtils.WriteI32((int)(sizeVariation * 10000),     buffer, 12);         
            NetUtils.WriteU16((ushort)(spread * 32),            buffer, 16);
            NetUtils.WriteI32((int)(speed * 10000),             buffer, 18);
            NetUtils.WriteI32((int)(gravity * 10000),           buffer, 22);
            NetUtils.WriteI32((int)(baseLifetime * 10000),      buffer, 26);
            NetUtils.WriteI32((int)(lifetimeVariation * 10000), buffer, 30);
            byte flags = 0;
            if (expireUponTouchingGround) flags |= 1 << 0;
            if (collidesSolid)            flags |= 1 << 1;
            if (collidesLiquid)           flags |= 1 << 2;
            if (collidesLeaves)           flags |= 1 << 3;
            
            buffer[34] = flags;
            buffer[35] = fullBright ? (byte)1 : (byte)0;
            return buffer;
        }
        
        public static byte[] SpawnEffect(byte effectId, float x, float y, float z, float originX, float originY, float originZ) {
            byte[] buffer = new byte[26];
            buffer[0] = Opcode.CpeSpawnEffect;
            buffer[1] = effectId;
            
            NetUtils.WriteI32((int)(x * 32),       buffer,  2);
            NetUtils.WriteI32((int)(y * 32),       buffer,  6);
            NetUtils.WriteI32((int)(z * 32),       buffer, 10);
            NetUtils.WriteI32((int)(originX * 32), buffer, 14);
            NetUtils.WriteI32((int)(originY * 32), buffer, 18);
            NetUtils.WriteI32((int)(originZ * 32), buffer, 22);
            return buffer;
        }

        public const int MaxCustomModels = 64;
        public const int MaxCustomModelParts = 64;
        public const int MaxCustomModelAnims = 4;
        public static byte[] DefineModel(byte modelId, CustomModel customModel) {
            // 116 = 1 + 1 + 64 + 1 + 2*4 + 3*4 + 2*3*4 + 2*2 + 1
            byte[] buffer = new byte[116];
            int i = 0;
            buffer[i++] = Opcode.CpeDefineModel;
            buffer[i++] = modelId;

            // write model name
            NetUtils.Write(customModel.name, buffer, i, false);
            i += NetUtils.StringSize;

            // write bool flags
            byte flags = 0;
            flags |= (byte)((customModel.bobbing ? 1 : 0) << 0);
            flags |= (byte)((customModel.pushes ? 1 : 0) << 1);
            flags |= (byte)((customModel.usesHumanSkin ? 1 : 0) << 2);
            flags |= (byte)((customModel.calcHumanAnims ? 1 : 0) << 3);
            buffer[i++] = flags;

            // write nameY, eyeY
            NetUtils.WriteF32(customModel.nameY, buffer, i);
            i += 4;
            NetUtils.WriteF32(customModel.eyeY, buffer, i);
            i += 4;

            // write collisionBounds
            NetUtils.WriteF32(customModel.collisionBounds.X, buffer, i);
            i += 4;
            NetUtils.WriteF32(customModel.collisionBounds.Y, buffer, i);
            i += 4;
            NetUtils.WriteF32(customModel.collisionBounds.Z, buffer, i);
            i += 4;

            // write pickingBoundsAABB
            NetUtils.WriteF32(customModel.pickingBoundsMin.X, buffer, i);
            i += 4;
            NetUtils.WriteF32(customModel.pickingBoundsMin.Y, buffer, i);
            i += 4;
            NetUtils.WriteF32(customModel.pickingBoundsMin.Z, buffer, i);
            i += 4;

            NetUtils.WriteF32(customModel.pickingBoundsMax.X, buffer, i);
            i += 4;
            NetUtils.WriteF32(customModel.pickingBoundsMax.Y, buffer, i);
            i += 4;
            NetUtils.WriteF32(customModel.pickingBoundsMax.Z, buffer, i);
            i += 4;

            // write uScale, vScale
            NetUtils.WriteU16(customModel.uScale, buffer, i);
            i += 2;
            NetUtils.WriteU16(customModel.vScale, buffer, i);
            i += 2;

            // write # CustomModelParts
            buffer[i++] = customModel.partCount;

            return buffer;
        }

        public static byte[] DefineModelPart(byte modelId, CustomModelPart part) {
            // v1: 104 = (1 + 1 + 3*4 + 3*4 + 6*(2*2 + 2*2) + 3*4 + 3*4) + 1 + 4 + 1
            byte[] buffer = new byte[104];
            int i = WriteDefineModelPart(buffer, modelId, part);

            // ignore animations
            i++;
            i += 4;

            // write bool flags
            byte flags = 0;
            flags |= (byte)((part.fullbright ? 1 : 0) << 0);
            flags |= (byte)((part.firstPersonArm ? 1 : 0) << 1);

            buffer[i++] = flags;

            return buffer;
        }
        
        public static byte[] DefineModelPartV2(byte modelId, CustomModelPart part) {
            // v2: 167 = (1 + 1 + 3*4 + 3*4 + 6*(2*2 + 2*2) + 3*4 + 3*4) + 4*(1 + 4*4) + 1
            byte[] buffer = new byte[167];
            int i = WriteDefineModelPart(buffer, modelId, part);

            for (int j = 0; j < MaxCustomModelAnims; j++) {
                var anim = part.anims[j];

                buffer[i++] = (byte)(
                    ((byte)anim.type & 0x3F) | ((byte)anim.axis << 6)
                );

                NetUtils.WriteF32(anim.a, buffer, i);
                i += 4;
                NetUtils.WriteF32(anim.b, buffer, i);
                i += 4;
                NetUtils.WriteF32(anim.c, buffer, i);
                i += 4;
                NetUtils.WriteF32(anim.d, buffer, i);
                i += 4;
            }

            // write bool flags
            byte flags = 0;
            flags |= (byte)((part.fullbright ? 1 : 0) << 0);
            flags |= (byte)((part.firstPersonArm ? 1 : 0) << 1);

            buffer[i++] = flags;

            return buffer;
        }

        public static int WriteDefineModelPart(byte[] buffer, byte modelId, CustomModelPart part) {
            int i = 0;
            buffer[i++] = Opcode.CpeDefineModelPart;
            buffer[i++] = modelId;

            /* write min, max vec3 coords */
            NetUtils.WriteF32(part.min.X, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.min.Y, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.min.Z, buffer, i);
            i += 4;

            NetUtils.WriteF32(part.max.X, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.max.Y, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.max.Z, buffer, i);
            i += 4;

            /* write u, v coords for our 6 faces */
            for (int j = 0; j < 6; j++) {
                NetUtils.WriteU16(part.u1[j], buffer, i);
                i += 2;
                NetUtils.WriteU16(part.v1[j], buffer, i);
                i += 2;

                NetUtils.WriteU16(part.u2[j], buffer, i);
                i += 2;
                NetUtils.WriteU16(part.v2[j], buffer, i);
                i += 2;
            }

            /* write rotation origin point */
            NetUtils.WriteF32(part.rotationOrigin.X, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.rotationOrigin.Y, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.rotationOrigin.Z, buffer, i);
            i += 4;

            /* write rotation angles */
            NetUtils.WriteF32(part.rotation.X, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.rotation.Y, buffer, i);
            i += 4;
            NetUtils.WriteF32(part.rotation.Z, buffer, i);
            i += 4;

            return i;
        }

        public static byte[] UndefineModel(byte modelId) {
            byte[] buffer = new byte[2];
            int i = 0;
            buffer[i++] = Opcode.CpeUndefineModel;
            buffer[i++] = modelId;

            return buffer;
        }

        #endregion
        
        
        #region Block definitions
        
        public static byte[] DefineBlock(BlockDefinition def, bool hasCP437, 
                                         bool extBlocks, bool extTexs) {
            byte[] buffer = new byte[(extBlocks ? 81 : 80) + (extTexs ? 3 : 0)];
            int i = 0;
            buffer[i++] = Opcode.CpeDefineBlock;
            MakeDefineBlockStart(def, buffer, ref i, false, hasCP437, extBlocks, extTexs);
            buffer[i++] = def.Shape;
            MakeDefineBlockEnd(def, ref i, buffer);
            return buffer;
        }
        
        public static byte[] UndefineBlock(BlockDefinition def, bool extBlocks) {
            byte[] buffer = new byte[extBlocks ? 3 : 2];
            buffer[0] = Opcode.CpeUndefineBlock;
            NetUtils.WriteBlock(def.RawID, buffer, 1, extBlocks);
            return buffer;
        }
        
        public static byte[] DefineBlockExt(BlockDefinition def, bool uniqueSideTexs, bool hasCP437, 
                                            bool extBlocks, bool extTexs) {
            byte[] buffer = new byte[(extBlocks ? 86 : 85) + (uniqueSideTexs ? 3 : 0) + (extTexs ? 6 : 0)];
            int i = 0;
            buffer[i++] = Opcode.CpeDefineBlockExt;
            MakeDefineBlockStart(def, buffer, ref i, uniqueSideTexs, hasCP437, extBlocks, extTexs);
            buffer[i++] = def.MinX; buffer[i++] = def.MinZ; buffer[i++] = def.MinY;
            buffer[i++] = def.MaxX; buffer[i++] = def.MaxZ; buffer[i++] = def.MaxY;
            MakeDefineBlockEnd(def, ref i, buffer);
            return buffer;
        }
        
        static void WriteTex(byte[] buffer, ref int i, ushort value, bool extTexs) {
            if (extTexs) {
                NetUtils.WriteU16(value, buffer, i); i += 2;
            } else {
                buffer[i++] = (byte)value;
            }
        }
        
        static void MakeDefineBlockStart(BlockDefinition def, byte[] buffer, ref int i, bool uniqueSideTexs, 
                                         bool hasCP437, bool extBlocks, bool extTexs) {
            // speed = 2^((raw - 128) / 64);
            // therefore raw = 64log2(speed) + 128
            byte rawSpeed = (byte)(64 * Math.Log(def.Speed, 2) + 128);
            NetUtils.WriteBlock(def.RawID, buffer, i, extBlocks);
            i += extBlocks ? 2 : 1;
            NetUtils.Write(def.Name, buffer, i, hasCP437);
            i += NetUtils.StringSize;
            buffer[i++] = def.CollideType;
            buffer[i++] = rawSpeed;
            
            WriteTex(buffer, ref i, def.TopTex, extTexs);
            if (uniqueSideTexs) {
                WriteTex(buffer, ref i, def.LeftTex,  extTexs); WriteTex(buffer, ref i, def.RightTex, extTexs);
                WriteTex(buffer, ref i, def.FrontTex, extTexs); WriteTex(buffer, ref i, def.BackTex,  extTexs);
            } else {
                WriteTex(buffer, ref i, def.RightTex, extTexs);
            }            
            WriteTex(buffer, ref i, def.BottomTex, extTexs);
            
            buffer[i++] = (byte)(def.BlocksLight ? 0 : 1);
            buffer[i++] = def.WalkSound;
            buffer[i++] = (byte)(def.FullBright ? 1 : 0);
        }
        
        static void MakeDefineBlockEnd(BlockDefinition def, ref int i, byte[] buffer) {
            buffer[i++] = def.BlockDraw;
            buffer[i++] = def.FogDensity;
            buffer[i++] = def.FogR; buffer[i++] = def.FogG; buffer[i++] = def.FogB;
        }
        #endregion
    }
}
