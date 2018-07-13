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
using System.Collections.Generic;
using MCGalaxy.Blocks;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy {
    public partial class Player {
        
        class ExtEntry {
            public string ExtName;
            public byte ClientExtVersion, ServerExtVersion = 1;
            
            public ExtEntry(string extName) { ExtName = extName; }
            public ExtEntry(string extName, byte extVersion) {
                ExtName = extName; ServerExtVersion = extVersion;
            }
        }
        
        ExtEntry[] extensions = new ExtEntry[] {
            new ExtEntry(CpeExt.ClickDistance),    new ExtEntry(CpeExt.CustomBlocks),
            new ExtEntry(CpeExt.HeldBlock),        new ExtEntry(CpeExt.TextHotkey),
            new ExtEntry(CpeExt.ExtPlayerList, 2), new ExtEntry(CpeExt.EnvColors),
            new ExtEntry(CpeExt.SelectionCuboid),  new ExtEntry(CpeExt.BlockPermissions),
            new ExtEntry(CpeExt.ChangeModel),      new ExtEntry(CpeExt.EnvMapAppearance, 2),
            new ExtEntry(CpeExt.EnvWeatherType),   new ExtEntry(CpeExt.HackControl),
            new ExtEntry(CpeExt.EmoteFix),         new ExtEntry(CpeExt.MessageTypes),
            new ExtEntry(CpeExt.LongerMessages),   new ExtEntry(CpeExt.FullCP437),
            new ExtEntry(CpeExt.BlockDefinitions), new ExtEntry(CpeExt.BlockDefinitionsExt, 2),
            new ExtEntry(CpeExt.TextColors),       new ExtEntry(CpeExt.BulkBlockUpdate),
            new ExtEntry(CpeExt.EnvMapAspect),     new ExtEntry(CpeExt.PlayerClick),
            new ExtEntry(CpeExt.EntityProperty),   new ExtEntry(CpeExt.ExtEntityPositions),
            new ExtEntry(CpeExt.TwoWayPing),       new ExtEntry(CpeExt.InventoryOrder),
            new ExtEntry(CpeExt.InstantMOTD),      new ExtEntry(CpeExt.FastMap),
            new ExtEntry(CpeExt.ExtTextures), 
            #if TEN_BIT_BLOCKS
            new ExtEntry(CpeExt.ExtBlocks),
            #endif
        };
        
        ExtEntry FindExtension(string extName) {
            foreach (ExtEntry ext in extensions) {
                if (ext.ExtName.CaselessEq(extName)) return ext;
            }
            return null;
        }
        
        // these are checked very frequently, so avoid overhead of HasCpeExt
        public bool hasCustomBlocks, hasBlockDefs, hasTextColors, hasExtBlocks,
        hasChangeModel, hasExtList, hasCP437, hasTwoWayPing, hasBulkBlockUpdate, hasExtTexs;

        void AddExtension(string extName, int version) {
            ExtEntry ext = FindExtension(extName.Trim());
            if (ext == null) return;
            ext.ClientExtVersion = (byte)version;
            
            if (ext.ExtName == CpeExt.CustomBlocks) {
                if (version == 1) Send(Packet.CustomBlockSupportLevel(1));
                hasCustomBlocks = true;
                if (MaxRawBlock < Block.CpeMaxBlock) MaxRawBlock = Block.CpeMaxBlock;
            } else if (ext.ExtName == CpeExt.ChangeModel) {
                hasChangeModel = true;
            } else if (ext.ExtName == CpeExt.FullCP437) {
                hasCP437 = true;
            } else if (ext.ExtName == CpeExt.ExtPlayerList) {
                hasExtList = true;
            } else if (ext.ExtName == CpeExt.BlockDefinitions) {
                hasBlockDefs = true;
                if (MaxRawBlock < 255) MaxRawBlock = 255;
            } else if (ext.ExtName == CpeExt.TextColors) {
                hasTextColors = true;
                for (int i = 0; i < Colors.List.Length; i++) {
                    if (!Colors.List[i].IsModified()) continue;
                    Send(Packet.SetTextColor(Colors.List[i]));
                }
            } else if (ext.ExtName == CpeExt.ExtEntityPositions) {
                hasExtPositions = true;
            } else if (ext.ExtName == CpeExt.TwoWayPing) {
                hasTwoWayPing = true;
            } else if (ext.ExtName == CpeExt.BulkBlockUpdate) {
                hasBulkBlockUpdate = true;
            } else if (ext.ExtName == CpeExt.ExtTextures) {
                hasExtTexs = true;
            } 
            #if TEN_BIT_BLOCKS
            else if (ext.ExtName == CpeExt.ExtBlocks) {
                hasExtBlocks = true;
                if (MaxRawBlock < 767) MaxRawBlock = 767;
            }
            #endif
        }

        public bool Supports(string extName, int version = 1) {
            if (!hasCpe) return false;
            ExtEntry ext = FindExtension(extName);
            return ext != null && ext.ClientExtVersion == version;
        }
        
        string lastUrl = "";
        public void SendCurrentTextures() {
            BlockID side = ConvertBlock(level.Config.EdgeBlock);
            BlockID edge = ConvertBlock(level.Config.HorizonBlock);

            string url = GetTextureUrl();
            if (Supports(CpeExt.EnvMapAspect)) {
                // reset all other textures back to client default.
                if (url != lastUrl) Send(Packet.EnvMapUrl("", hasCP437));
                Send(Packet.EnvMapUrl(url, hasCP437));
            } else if (Supports(CpeExt.EnvMapAppearance, 2)) {
                // reset all other textures back to client default.
                if (url != lastUrl) {
                    Send(Packet.MapAppearanceV2("", (byte)side, (byte)edge, level.Config.EdgeLevel,
                                                level.Config.CloudsHeight, level.Config.MaxFogDistance, hasCP437));
                }
                Send(Packet.MapAppearanceV2(url, (byte)side, (byte)edge, level.Config.EdgeLevel,
                                            level.Config.CloudsHeight, level.Config.MaxFogDistance, hasCP437));
                lastUrl = url;
            } else if (Supports(CpeExt.EnvMapAppearance)) {
                url = level.Config.Terrain.Length == 0 ? ServerConfig.DefaultTerrain : level.Config.Terrain;
                Send(Packet.MapAppearance(url, (byte)side, (byte)edge, level.Config.EdgeLevel, hasCP437));
            }
        }
        
        public string GetTextureUrl() {
            string url = level.Config.TexturePack.Length == 0 ? level.Config.Terrain : level.Config.TexturePack;
            if (url.Length == 0) {
                url = ServerConfig.DefaultTexture.Length == 0 ? ServerConfig.DefaultTerrain : ServerConfig.DefaultTexture;
            }
            return url;
        }

        public void SendEnvColor(byte type, string hex) {
            ColorDesc c;
            if (Colors.TryParseHex(hex, out c)) {
                Send(Packet.EnvColor(type, c.R, c.G, c.B));
            } else {
                Send(Packet.EnvColor(type, -1, -1, -1));
            }
        }
        
        public void SendCurrentBlockPermissions() {
            if (!Supports(CpeExt.BlockPermissions)) return;           
            // Write the block permissions as one bulk TCP packet
            SendAllBlockPermissions();
        }
        
        void SendAllBlockPermissions() {
            int count = MaxRawBlock + 1;
            int size = hasExtBlocks ? 5 : 4;
            byte[] bulk = new byte[count * size];
            
            for (int i = 0; i < count; i++) {
                BlockID block = Block.FromRaw((BlockID)i);
                bool place  = group.Blocks[block] && level.CanPlace;
                bool delete = group.Blocks[block] && level.CanDelete;
                
                // Placing air is the same as deleting existing block at that position in the world
                if (block == Block.Air) place &= delete;
                Packet.WriteBlockPermission((BlockID)i, place, delete, hasExtBlocks, bulk, i * size);
            }
            Send(bulk);
        }
    }
    
    public static class CpeExt {
        public const string ClickDistance = "ClickDistance";
        public const string CustomBlocks = "CustomBlocks";
        public const string HeldBlock = "HeldBlock";
        public const string TextHotkey = "TextHotKey";
        public const string ExtPlayerList = "ExtPlayerList";
        public const string EnvColors = "EnvColors";
        public const string SelectionCuboid = "SelectionCuboid";
        public const string BlockPermissions = "BlockPermissions";
        public const string ChangeModel = "ChangeModel";
        public const string EnvMapAppearance = "EnvMapAppearance";
        public const string EnvWeatherType = "EnvWeatherType";
        public const string HackControl = "HackControl";
        public const string EmoteFix = "EmoteFix";
        public const string MessageTypes = "MessageTypes";
        public const string LongerMessages = "LongerMessages";
        public const string FullCP437 = "FullCP437";
        public const string BlockDefinitions = "BlockDefinitions";
        public const string BlockDefinitionsExt = "BlockDefinitionsExt";
        public const string TextColors = "TextColors";
        public const string BulkBlockUpdate = "BulkBlockUpdate";
        public const string EnvMapAspect = "EnvMapAspect";
        public const string PlayerClick = "PlayerClick";
        public const string EntityProperty = "EntityProperty";
        public const string ExtEntityPositions = "ExtEntityPositions";
        public const string TwoWayPing = "TwoWayPing";
        public const string InventoryOrder = "InventoryOrder";
        public const string InstantMOTD = "InstantMOTD";
        public const string FastMap = "FastMap";
        public const string ExtBlocks = "ExtendedBlocks";
        public const string ExtTextures = "ExtendedTextures";
    }
    
    public enum CpeMessageType : byte {
        Normal = 0, Status1 = 1, Status2 = 2, Status3 = 3,
        BottomRight1 = 11, BottomRight2 = 12, BottomRight3 = 13,
        Announcement = 100,
    }
    
    public enum EnvProp : byte {
        SidesBlock = 0, EdgeBlock = 1, EdgeLevel = 2,
        CloudsLevel = 3, MaxFog = 4, CloudsSpeed = 5,
        WeatherSpeed = 6, WeatherFade = 7, ExpFog = 8,
        SidesOffset = 9, SkyboxHorSpeed = 10, SkyboxVerSpeed = 11,
        
        Max,
    }
    
    public enum EntityProp : byte {
        RotX = 0, RotY = 1, RotZ = 2, ScaleX = 3, ScaleY = 4, ScaleZ = 5,
    }
}