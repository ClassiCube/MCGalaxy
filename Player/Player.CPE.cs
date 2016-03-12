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
using System.Drawing;

namespace MCGalaxy
{
    public partial class Player
    {
        public int ClickDistance = 0;
        public int CustomBlocks = 0;
        public int HeldBlock = 0;
        public int TextHotKey = 0;
        public int ExtPlayerList = 0;
        public int EnvColors = 0;
        public int SelectionCuboid = 0;
        public int BlockPermissions = 0;
        public int ChangeModel = 0;
        public int EnvMapAppearance = 0;
        public int EnvWeatherType = 0;
        public int HackControl = 0;
        public int EmoteFix = 0;
        public int MessageTypes = 0;        
        public int LongerMessages = 0;
        public int FullCP437 = 0;
        public int BlockDefinitions = 0;
        public int BlockDefinitionsExt = 0;
        public int TextColors = 0;
        public int BulkBlockUpdate = 0;

        public void AddExtension(string Extension, int version)
        {
            switch (Extension.Trim())
            {
                case CpeExt.ClickDistance:
                    ClickDistance = version; break;
                case CpeExt.CustomBlocks:
                    CustomBlocks = version;
                    if (version == 1) SendCustomBlockSupportLevel(1);
                    hasCustomBlocks = true;
                    break;
                case CpeExt.HeldBlock:
                    HeldBlock = version; break;
                case CpeExt.TextHotkey:
                    TextHotKey = version; break;
                    
                    /*case "ExtPlayerList":
                        ExtPlayerList = version;
                        spawned = true;
                        if (version > 0)
                            PlayerInfo.players.ForEach(delegate(Player p)
                            {
                                if (p.HasExtension("ExtPlayerList", 2) && p != this)
                                {
                                    p.SendExtAddPlayerName(id, name, group, color + name);
                                }
                                if (HasExtension("ExtPlayerList", 2))
                                {
                                    SendExtAddPlayerName(p.id, p.name, p.group, p.color + p.name);
                                }
                            });

                        try
                        {
                            ushort x = (ushort)((0.5 + level.spawnx) * 32);
                            ushort y = (ushort)((1 + level.spawny) * 32);
                            ushort z = (ushort)((0.5 + level.spawnz) * 32);
                            pos = new ushort[3] { x, y, z }; rot = new byte[2] { level.rotx, level.roty };

                            GlobalSpawn(this, x, y, z, rot[0], rot[1], true);
                            foreach (Player p in players)
                            {
                                if (p.level == level && p != this && !p.hidden)
                                    SendSpawn(p.id, p.color + p.name, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], p.DisplayName, p.SkinName);
                                if (HasExtension("ChangeModel"))
                                {
                                    if (p == this)
                                        SendChangeModel(0xFF, model);
                                    else SendChangeModel(p.id, p.model);
                                }
                            }
                            foreach (PlayerBot pB in PlayerBot.playerbots)
                            {
                                if (pB.level == level)
                                    SendSpawn(pB.id, pB.color + pB.name, pB.pos[0], pB.pos[1], pB.pos[2], pB.rot[0], pB.rot[1], pB.name, pB.name);
                            }

                        }
                        catch (Exception e)
                        {
                            Server.ErrorLog(e);
                            Server.s.Log("Error spawning player \"" + name + "\"");
                        }
                        break;*/
                case CpeExt.EnvColors:
                    EnvColors = version; break;
                case CpeExt.SelectionCuboid:
                    SelectionCuboid = version; break;
                case CpeExt.BlockPermissions:
                    BlockPermissions = version; break;
                case CpeExt.ChangeModel:
                    UpdateModels();
                    ChangeModel = version; break;
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
            }
        }

        public bool HasCpeExt(string Extension, int version = 1) {
            if (!hasCpe) return false;
            switch (Extension)
            {
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
                    default: return false;
            }
        }
        
        public void UpdateModels() {
        	Player[] players = PlayerInfo.Online; 
            foreach (Player p in players) {
                if (p.level == this.level)
                    if (p == this) {
                    SendChangeModel(0xFF, model);
                } else {
                    SendChangeModel(p.id, p.model);
                    if (p.HasCpeExt(CpeExt.ChangeModel))
                        p.SendChangeModel(this.id, model);
                }
            }
        }
        
        public void SendCurrentMapAppearance() {
        	byte edgeBlock = level.EdgeBlock, horBlock = level.HorizonBlock;
        	if (edgeBlock >= Block.CpeCount && !hasBlockDefs)
        	    edgeBlock = level.GetFallback(edgeBlock);
        	if (horBlock >= Block.CpeCount && !hasBlockDefs)
        	    horBlock = level.GetFallback(horBlock);  
        	    
            if (EnvMapAppearance == 2) {
                string url = level.texturePackUrl == "" ? level.terrainUrl : level.texturePackUrl;
                if (url == "") 
                    url = Server.defaultTexturePackUrl == "" ? Server.defaultTerrainUrl : Server.defaultTexturePackUrl;
                
                // reset all other textures back to client default.
                SendSetMapAppearanceV2("", edgeBlock, horBlock, level.EdgeLevel, level.CloudsHeight, level.MaxFogDistance);
                if (url != "")
                    SendSetMapAppearanceV2(url, edgeBlock, horBlock, level.EdgeLevel, level.CloudsHeight, level.MaxFogDistance);
            } else {
                string url = level.terrainUrl == "" ? Server.defaultTerrainUrl : level.terrainUrl;
                SendSetMapAppearance(url, edgeBlock, horBlock, level.EdgeLevel);
            }
        }
        
        public void SendCurrentEnvColors() {
            SendEnvColor(0, level.SkyColor);
            SendEnvColor(1, level.CloudColor);
            SendEnvColor(2, level.FogColor);
            SendEnvColor(3, level.ShadowColor);
            SendEnvColor(4, level.LightColor);
        }
        
        void SendEnvColor(byte type, string src) {
            try {
                Color col = System.Drawing.ColorTranslator.FromHtml("#" + src.ToUpper());
                SendEnvColor(type, col.R, col.G, col.B);
            } catch {
                SendEnvColor(type, -1, -1, -1);
            }
        }
        
        public void SendCurrentBlockPermissions() {
            byte count = hasCustomBlocks ? Block.CpeCount : Block.OriginalCount;
            for (byte i = 0; i < count; i++) {
                bool canPlace = Block.canPlace(this, i);
                bool canDelete = canPlace;
                
                if (!level.Buildable) canPlace = false;
                if (!level.Deletable) canDelete = false;
                SendSetBlockPermission(i, canPlace, canDelete);
            }
            
            if (!hasBlockDefs) return;
            for (int i = count; i < 256; i++) {
                if (level.CustomBlockDefs[i] == null) continue;
                SendSetBlockPermission((byte)i, level.Buildable, level.Deletable);
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
    }
}