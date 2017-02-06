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

namespace MCGalaxy {
    public partial class Player {
        public int ClickDistance, CustomBlocks, HeldBlock, TextHotKey;
        public int ExtPlayerList, EnvColors, SelectionCuboid, BlockPermissions;
        public int ChangeModel, EnvMapAppearance, EnvWeatherType, HackControl;
        public int EmoteFix, MessageTypes, LongerMessages, FullCP437;
        public int BlockDefinitions, BlockDefinitionsExt, TextColors, BulkBlockUpdate;
        public int EnvMapAspect, PlayerClick, EntityProperty;

        // these are checked frequently, so avoid overhead of HasCpeExt
        public bool hasCustomBlocks, hasBlockDefs,
        hasTextColors, hasChangeModel, hasExtList;

        public void AddExtension(string ext, int version) {
            switch (ext.Trim()) {
                case CpeExt.ClickDistance:
                    ClickDistance = version; break;
                case CpeExt.CustomBlocks:
                    CustomBlocks = version;
                    if (version == 1)
                        SendRaw(Opcode.CpeCustomBlockSupportLevel, 1);
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
                    UpdateModels();
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
                    FullCP437 = version; break;
                case CpeExt.BlockDefinitions:
                    BlockDefinitions = version;
                    hasBlockDefs = true; break;
                case CpeExt.BlockDefinitionsExt:
                    BlockDefinitionsExt = version; break;
                case CpeExt.TextColors:
                    hasTextColors = true;
                    TextColors = version;
                    
                    for (int i = 0; i < Colors.ExtColors.Length; i++) {
                        if (Colors.ExtColors[i].Undefined) continue;
                        Colors.SendSetTextColor(this, Colors.ExtColors[i]);
                    } break;
                case CpeExt.BulkBlockUpdate:
                    BulkBlockUpdate = version; break;
                case CpeExt.EnvMapAspect:
                    EnvMapAspect = version; break;
                case CpeExt.PlayerClick:
                    PlayerClick = version; break;
                case CpeExt.EntityProperty:
                    EntityProperty = version; break;
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
                    default: return false;
            }
        }
        
        public void UpdateModels() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != level) continue;
                if (p == this) {
                    if (model != "humanoid") SendChangeModel(Entities.SelfID, model);
                    continue;
                }
                
                if (p.model != "humanoid") SendChangeModel(p.id, p.model);
                if (p.hasChangeModel && model != "humanoid")
                    p.SendChangeModel(id, model);
            }
            
            PlayerBot[] bots = PlayerBot.Bots.Items;
            foreach (PlayerBot b in bots) {
                if (level != b.level) continue;
                if (b.model != "humanoid") SendChangeModel(b.id, b.model);
            }
        }
        
        string lastUrl = "";
        public void SendCurrentMapAppearance() {
            byte side = (byte)level.EdgeBlock, edge = (byte)level.HorizonBlock;
            if (side >= Block.CpeCount && !hasBlockDefs)
                side = level.GetFallback(side);
            if (edge >= Block.CpeCount && !hasBlockDefs)
                edge = level.GetFallback(edge);
            
            if (HasCpeExt(CpeExt.EnvMapAspect)) {
                string url = GetTextureUrl();
                // reset all other textures back to client default.
                if (url != lastUrl) Send(Packet.EnvMapUrl(""));
                Send(Packet.EnvMapUrl(url));
                
                Send(Packet.EnvMapProperty(EnvProp.SidesBlock, side));
                Send(Packet.EnvMapProperty(EnvProp.EdgeBlock, edge));
                Send(Packet.EnvMapProperty(EnvProp.EdgeLevel, level.EdgeLevel));
                Send(Packet.EnvMapProperty(EnvProp.CloudsLevel, level.CloudsHeight));
                Send(Packet.EnvMapProperty(EnvProp.MaxFog, level.MaxFogDistance));
                Send(Packet.EnvMapProperty(EnvProp.CloudsSpeed, level.CloudsSpeed));
                Send(Packet.EnvMapProperty(EnvProp.WeatherSpeed, level.WeatherSpeed));
                Send(Packet.EnvMapProperty(EnvProp.ExpFog, level.ExpFog ? 1 : 0));
            } else if (HasCpeExt(CpeExt.EnvMapAppearance, 2)) {
                string url = GetTextureUrl();
                // reset all other textures back to client default.
                if (url != lastUrl) {
                    Send(Packet.MapAppearanceV2("", side, edge, level.EdgeLevel, level.CloudsHeight, level.MaxFogDistance));
                }
                Send(Packet.MapAppearanceV2(url, side, edge, level.EdgeLevel, level.CloudsHeight, level.MaxFogDistance));
                lastUrl = url;
            } else if (HasCpeExt(CpeExt.EnvMapAppearance)) {
                string url = level.terrainUrl == "" ? Server.defaultTerrainUrl : level.terrainUrl;
                Send(Packet.MapAppearance(url, side, edge, level.EdgeLevel));
            }
        }
        
        public string GetTextureUrl() {
            string url = level.texturePackUrl == "" ? level.terrainUrl : level.texturePackUrl;
            if (url == "")
                url = Server.defaultTextureUrl == "" ? Server.defaultTerrainUrl : Server.defaultTextureUrl;
            return url;
        }
        
        public void SendCurrentEnvColors() {
            SendEnvColor(0, level.SkyColor);
            SendEnvColor(1, level.CloudColor);
            SendEnvColor(2, level.FogColor);
            SendEnvColor(3, level.ShadowColor);
            SendEnvColor(4, level.LightColor);
        }
        
        public void SendEnvColor(byte type, string hex) {
            if (String.IsNullOrEmpty(hex)) {
                Send(Packet.EnvColor(type, -1, -1, -1)); return;
            }
            
            try {
                CustomColor c = Colors.ParseHex(hex);
                Send(Packet.EnvColor(type, c.R, c.G, c.B));
            } catch (ArgumentException) {
                Send(Packet.EnvColor(type, -1, -1, -1));
            }
        }
        
        public void SendCurrentBlockPermissions() {
            // Write the block permissions as one bulk TCP packet
            int count = NumBlockPermissions();
            byte[] bulk = new byte[4 * count];
            WriteBlockPermissions(bulk);
            Send(bulk);
        }
        
        int NumBlockPermissions() {
            int count = hasCustomBlocks ? Block.CpeCount : Block.OriginalCount;
            if (!hasBlockDefs) return count;

            for (int i = Block.CpeCount; i < 256; i++) {
                if (level.CustomBlockDefs[i] == null) continue;
                count++;
            }
            return count;
        }
        
        void WriteBlockPermissions(byte[] bulk) {
            int coreCount = hasCustomBlocks ? Block.CpeCount : Block.OriginalCount;
            for (byte i = 0; i < coreCount; i++) {
                bool place = Block.canPlace(this, i) && level.CanPlace;
                bool delete = Block.canPlace(this, i) && level.CanDelete;
                Packet.WriteBlockPermission(i, place, delete, bulk, i * 4);
            }
            
            if (!hasBlockDefs) return;
            int j = coreCount * 4;
            
            for (int i = Block.CpeCount; i < 256; i++) {
                if (level.CustomBlockDefs[i] == null) continue;               
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
    }
    
    public enum EntityProp : byte {
        RotX = 0, RotY = 1, RotZ = 2,
    }
}