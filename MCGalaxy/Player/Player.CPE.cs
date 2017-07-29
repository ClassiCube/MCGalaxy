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
using MCGalaxy.Network;

namespace MCGalaxy {
    public partial class Player {
        public int ClickDistance, CustomBlocks, HeldBlock, TextHotKey;
        public int ExtPlayerList, EnvColors, SelectionCuboid, BlockPermissions;
        public int ChangeModel, EnvMapAppearance, EnvWeatherType, HackControl;
        public int EmoteFix, MessageTypes, LongerMessages, FullCP437;
        public int BlockDefinitions, BlockDefinitionsExt, TextColors, BulkBlockUpdate;
        public int EnvMapAspect, PlayerClick, EntityProperty, ExtEntityPositions, TwoWayPing;

        // these are checked very frequently, so avoid overhead of HasCpeExt
        public bool hasCustomBlocks, hasBlockDefs,
        hasTextColors, hasChangeModel, hasExtList, hasCP437, hasTwoWayPing;

        public void AddExtension(string ext, int version) {
            switch (ext.Trim()) {
                case CpeExt.ClickDistance:
                    ClickDistance = version; break;
                case CpeExt.CustomBlocks:
                    CustomBlocks = version;
                    if (version == 1) Send(Packet.CustomBlockSupportLevel(1));
                    hasCustomBlocks = true; break;
                case CpeExt.HeldBlock:
                    HeldBlock = version; break;
                case CpeExt.TextHotkey:
                    TextHotKey = version; break;
                case CpeExt.ExtPlayerList:
                    ExtPlayerList = version;
                    hasExtList = version == 2; break;
                case CpeExt.EnvColors:
                    EnvColors = version; break;
                case CpeExt.SelectionCuboid:
                    SelectionCuboid = version; break;
                case CpeExt.BlockPermissions:
                    BlockPermissions = version; break;
                case CpeExt.ChangeModel:
                    ChangeModel = version;
                    hasChangeModel = true; break;
                case CpeExt.EnvMapAppearance:
                    EnvMapAppearance = version; break;
                case CpeExt.EnvWeatherType:
                    EnvWeatherType = version; break;
                case CpeExt.HackControl:
                    HackControl = version; break;
                case CpeExt.EmoteFix:
                    EmoteFix = version; break;
                case CpeExt.MessageTypes:
                    MessageTypes = version; break;
                case CpeExt.LongerMessages:
                    LongerMessages = version; break;
                case CpeExt.FullCP437:
                    FullCP437 = version;
                    hasCP437 = true; break;
                case CpeExt.BlockDefinitions:
                    BlockDefinitions = version;
                    hasBlockDefs = true; break;
                case CpeExt.BlockDefinitionsExt:
                    BlockDefinitionsExt = version; break;
                case CpeExt.TextColors:
                    hasTextColors = true;
                    TextColors = version;
                    
                    for (int i = 0; i < Colors.List.Length; i++) {
                        if (!Colors.List[i].IsModified()) continue;
                        Send(Packet.SetTextColor(Colors.List[i]));
                    }
                    break;
                case CpeExt.BulkBlockUpdate:
                    BulkBlockUpdate = version; break;
                case CpeExt.EnvMapAspect:
                    EnvMapAspect = version; break;
                case CpeExt.PlayerClick:
                    PlayerClick = version; break;
                case CpeExt.EntityProperty:
                    EntityProperty = version; break;
                case CpeExt.ExtEntityPositions:
                    ExtEntityPositions = version;
                    hasExtPositions = true; break;
                case CpeExt.TwoWayPing:
                    TwoWayPing = version; 
                    hasTwoWayPing = true; break;
            }
        }

        public bool HasCpeExt(string Extension, int version = 1) {
            if (!hasCpe) return false;
            switch (Extension) {
                    case CpeExt.ClickDistance: return ClickDistance == version;
                    case CpeExt.CustomBlocks: return CustomBlocks == version;
                    case CpeExt.HeldBlock: return HeldBlock == version;
                    case CpeExt.TextHotkey: return TextHotKey == version;
                    case CpeExt.ExtPlayerList: return ExtPlayerList == version;
                    case CpeExt.EnvColors: return EnvColors == version;
                    case CpeExt.SelectionCuboid: return SelectionCuboid == version;
                    case CpeExt.BlockPermissions: return BlockPermissions == version;
                    case CpeExt.ChangeModel: return ChangeModel == version;
                    case CpeExt.EnvMapAppearance: return EnvMapAppearance == version;
                    case CpeExt.EnvWeatherType: return EnvWeatherType == version;
                    case CpeExt.HackControl: return HackControl == version;
                    case CpeExt.EmoteFix: return EmoteFix == version;
                    case CpeExt.MessageTypes: return MessageTypes == version;
                    case CpeExt.LongerMessages: return LongerMessages == version;
                    case CpeExt.FullCP437: return FullCP437 == version;
                    case CpeExt.BlockDefinitions: return BlockDefinitions == version;
                    case CpeExt.BlockDefinitionsExt: return BlockDefinitionsExt == version;
                    case CpeExt.TextColors: return TextColors == version;
                    case CpeExt.BulkBlockUpdate: return BulkBlockUpdate == version;
                    case CpeExt.EnvMapAspect: return EnvMapAspect == version;
                    case CpeExt.PlayerClick: return PlayerClick == version;
                    case CpeExt.EntityProperty: return EntityProperty == version;
                    case CpeExt.ExtEntityPositions: return ExtEntityPositions == version;
                    case CpeExt.TwoWayPing: return TwoWayPing == version;
                    default: return false;
            }
        }
        
        string lastUrl = "";
        public void SendCurrentMapAppearance() {
            byte side = (byte)level.Config.EdgeBlock, edge = (byte)level.Config.HorizonBlock;
            if (!hasBlockDefs) side = level.RawFallback(side);
            if (!hasBlockDefs) edge = level.RawFallback(edge);
            
            if (HasCpeExt(CpeExt.EnvMapAspect)) {
                string url = GetTextureUrl();
                // reset all other textures back to client default.
                if (url != lastUrl) Send(Packet.EnvMapUrl("", hasCP437));
                Send(Packet.EnvMapUrl(url, hasCP437));
                
                Send(Packet.EnvMapProperty(EnvProp.SidesBlock, side));
                Send(Packet.EnvMapProperty(EnvProp.EdgeBlock, edge));
                
                Send(Packet.EnvMapProperty(EnvProp.EdgeLevel, level.Config.EdgeLevel));
                Send(Packet.EnvMapProperty(EnvProp.SidesOffset, level.Config.SidesOffset));
                Send(Packet.EnvMapProperty(EnvProp.CloudsLevel, level.Config.CloudsHeight));
                
                Send(Packet.EnvMapProperty(EnvProp.MaxFog, level.Config.MaxFogDistance));
                Send(Packet.EnvMapProperty(EnvProp.CloudsSpeed, level.Config.CloudsSpeed));
                Send(Packet.EnvMapProperty(EnvProp.WeatherSpeed, level.Config.WeatherSpeed));
                Send(Packet.EnvMapProperty(EnvProp.ExpFog, level.Config.ExpFog ? 1 : 0));
            } else if (HasCpeExt(CpeExt.EnvMapAppearance, 2)) {
                string url = GetTextureUrl();
                // reset all other textures back to client default.
                if (url != lastUrl) {
                    Send(Packet.MapAppearanceV2("", side, edge, level.Config.EdgeLevel, 
                	                            level.Config.CloudsHeight, level.Config.MaxFogDistance, hasCP437));
                }
                Send(Packet.MapAppearanceV2(url, side, edge, level.Config.EdgeLevel, 
                                            level.Config.CloudsHeight, level.Config.MaxFogDistance, hasCP437));
                lastUrl = url;
            } else if (HasCpeExt(CpeExt.EnvMapAppearance)) {
                string url = level.Config.Terrain == "" ? ServerConfig.DefaultTerrain : level.Config.Terrain;
                Send(Packet.MapAppearance(url, side, edge, level.Config.EdgeLevel, hasCP437));
            }
        }
        
        public string GetTextureUrl() {
            string url = level.Config.TexturePack == "" ? level.Config.Terrain : level.Config.TexturePack;
            if (url == "")
                url = ServerConfig.DefaultTexture == "" ? ServerConfig.DefaultTerrain : ServerConfig.DefaultTexture;
            return url;
        }
        
        public void SendCurrentEnvColors() {
            SendEnvColor(0, level.Config.SkyColor);
            SendEnvColor(1, level.Config.CloudColor);
            SendEnvColor(2, level.Config.FogColor);
            SendEnvColor(3, level.Config.ShadowColor);
            SendEnvColor(4, level.Config.LightColor);
        }
        
        public void SendEnvColor(byte type, string hex) {
            if (String.IsNullOrEmpty(hex)) {
                Send(Packet.EnvColor(type, -1, -1, -1)); return;
            }
            
            try {
                ColorDesc c = Colors.ParseHex(hex);
                Send(Packet.EnvColor(type, c.R, c.G, c.B));
            } catch (ArgumentException) {
                Send(Packet.EnvColor(type, -1, -1, -1));
            }
        }
        
        public void SendCurrentBlockPermissions() {
            if (!HasCpeExt(CpeExt.BlockPermissions)) return;
            
            // Write the block permissions as one bulk TCP packet
            int count = NumBlockPermissions();
            byte[] bulk = new byte[4 * count];
            WriteBlockPermissions(bulk);
            Send(bulk);
        }
        
        int NumBlockPermissions() {
            int count = hasCustomBlocks ? Block.CpeCount : Block.OriginalCount;
            if (!hasBlockDefs) return count;

            return count + (Block.Count - Block.CpeCount);
        }
        
        void WriteBlockPermissions(byte[] bulk) {
            int coreCount = hasCustomBlocks ? Block.CpeCount : Block.OriginalCount;
            for (byte i = 0; i < coreCount; i++) {
                bool place = BlockPerms.UsableBy(this, i) && level.CanPlace;
                bool delete = BlockPerms.UsableBy(this, i) && level.CanDelete;
                Packet.WriteBlockPermission(i, place, delete, bulk, i * 4);
            }
            
            if (!hasBlockDefs) return;
            int j = coreCount * 4;
            
            for (int i = Block.CpeCount; i < Block.Count; i++) {
                Packet.WriteBlockPermission((byte)i, level.CanPlace, level.CanDelete, bulk, j);
                j += 4;
            }
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
        SidesOffset = 9,
    }
    
    public enum EntityProp : byte {
        RotX = 0, RotY = 1, RotZ = 2,
    }
}