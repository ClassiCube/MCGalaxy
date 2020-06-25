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

namespace MCGalaxy.Network {

    /// <summary> List of packet opcode bytes. (Packet identifiers) </summary>
    public static class Opcode {
        
        public const byte Handshake = 0;
        public const byte Ping = 1;
        public const byte LevelInitialise = 2;
        public const byte LevelDataChunk = 3;
        public const byte LevelFinalise = 4;
        public const byte SetBlockClient = 5;
        public const byte SetBlock = 6;
        public const byte AddEntity = 7;
        public const byte EntityTeleport = 8;
        public const byte RelPosAndOrientationUpdate = 9;
        public const byte RelPosUpdate = 10;
        public const byte OrientationUpdate = 11;
        public const byte RemoveEntity = 12;
        public const byte Message = 13;
        public const byte Kick = 14;
        public const byte SetPermission = 15;

        public const byte CpeExtInfo = 16;
        public const byte CpeExtEntry = 17;
        public const byte CpeSetClickDistance = 18;
        public const byte CpeCustomBlockSupportLevel = 19;
        public const byte CpeHoldThis = 20;
        public const byte CpeSetTextHotkey = 21;
        public const byte CpeExtAddPlayerName = 22;
        public const byte CpeExtAddEntity = 23;
        public const byte CpeExtRemovePlayerName = 24;
        public const byte CpeEnvColors = 25;
        public const byte CpeMakeSelection = 26;
        public const byte CpeRemoveSelection = 27;
        public const byte CpeSetBlockPermission = 28;
        public const byte CpeChangeModel = 29;
        public const byte CpeEnvSetMapApperance = 30;
        public const byte CpeEnvWeatherType = 31;
        public const byte CpeHackControl = 32;
        public const byte CpeExtAddEntity2 = 33;
        public const byte CpePlayerClick = 34;
        public const byte CpeDefineBlock = 35;
        public const byte CpeUndefineBlock = 36;
        public const byte CpeDefineBlockExt = 37;
        public const byte CpeBulkBlockUpdate = 38;
        public const byte CpeSetTextColor = 39;
        public const byte CpeSetMapEnvUrl = 40;
        public const byte CpeSetMapEnvProperty = 41;
        public const byte CpeSetEntityProperty = 42;
        public const byte CpeTwoWayPing = 43;
        public const byte CpeSetInventoryOrder = 44;
        public const byte CpeSetHotbar = 45;
        public const byte CpeSetSpawnpoint = 46;
        public const byte CpeVelocityControl = 47;
        public const byte CpeDefineEffect = 48;
        public const byte CpeSpawnEffect = 49;
        public const byte CpeDefineModel = 50;
        public const byte CpeDefineModelPart = 51;
        public const byte CpeUndefineModel = 52;
    }
}
