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
        public int BlockDefinitions = 0;
        public int LongerMessages = 0;
        public int FullCP437 = 0;

        public void AddExtension(string Extension, int version)
        {
            lock (this)
            {
                switch (Extension.Trim())
                {
                    case "ClickDistance":
                        ClickDistance = version;
                        break;
                    case "CustomBlocks":
                        CustomBlocks = version;
                        if (version == 1)
                            SendCustomBlockSupportLevel(1);
                        break;
                    case "HeldBlock":
                        HeldBlock = version;
                        break;
                    case "TextHotKey":
                        TextHotKey = version;
                        break;
                    /*case "ExtPlayerList":
                        ExtPlayerList = version;
                        spawned = true;
                        if (version > 0)
                            Player.players.ForEach(delegate(Player p)
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
                                        unchecked { SendChangeModel((byte)-1, model); }
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
                    case "EnvColors":
                        SendCurrentEnvColors();
                        EnvColors = version;
                        break;
                    case "SelectionCuboid":
                        SelectionCuboid = version;
                        break;
                    case "BlockPermissions":
                        BlockPermissions = version;
                        break;
                    case "ChangeModel":
                        UpdateModels();
                        ChangeModel = version;
                        break;
                    case "EnvMapAppearance":
                        SendCurrentMapAppearance();
                        EnvMapAppearance = version;
                        break;
                    case "EnvWeatherType":
                        SendSetMapWeather(level.weather);
                        EnvWeatherType = version;
                        break;
                    case "HackControl":
                        HackControl = version;
                        break;
                    case "EmoteFix":
                        EmoteFix = version;
                        break;
                    case "MessageTypes":
                        MessageTypes = version;
                        break;
                    case "BlockDefinitions":
                        try
                        {

                            foreach (BlockDefinitions bd in Server.GlobalDefinitions)
                            {
                                SendBlockDefinitions(bd);
                                SendSetBlockPermission(bd.ID, 1, 1);
                            }
                            Server.s.Log("Player supports Block Definitions");
                        }
                        catch { }
                        BlockDefinitions = version;
                        break;
                     case "LongerMessages":
                        LongerMessages = version;
                        break;
                     case "FullCP437":
                        FullCP437 = version;
                        break;
                }
            }
        }

        public bool HasExtension(string Extension, int version = 1) {
            if(!hasCpe)
                return false;
            switch (Extension)
            {
                case "ClickDistance": return ClickDistance == version;
                case "CustomBlocks": return CustomBlocks == version;
                case "HeldBlock": return HeldBlock == version;
                case "TextHotKey": return TextHotKey == version;
                case "ExtPlayerList": return ExtPlayerList == version;
                case "EnvColors": return EnvColors == version;
                case "SelectionCuboid": return SelectionCuboid == version;
                case "BlockPermissions": return BlockPermissions == version;
                case "ChangeModel": return ChangeModel == version;
                case "EnvMapAppearance": return EnvMapAppearance == version;
                case "EnvWeatherType": return EnvWeatherType == version;
                case "HackControl": return HackControl == version;
                case "EmoteFix": return EmoteFix == version;
                case "MessageTypes": return MessageTypes == version;
                case "BlockDefinitions": return BlockDefinitions == version;
                case "LongerMessages": return LongerMessages == version;
                case "FullCP437": return FullCP437 == version;
                default: return false;
            }
        }
        
        public void UpdateModels() {
            Player.players.ForEach(
                p =>
                {
                    if (p.level == this.level)
                        if (p == this) {
                            SendChangeModel(0xFF, model);
                        } else {
                            SendChangeModel(p.id, p.model);
                            if (p.HasExtension("ChangeModel"))
                                p.SendChangeModel(this.id, model);
                    }
                });
        }
        
        public void SendCurrentMapAppearance() {
            string url = level.textureUrl == "" ? Server.defaultTextureUrl : level.textureUrl;
            SendSetMapAppearance(url, level.EdgeBlock, level.HorizonBlock, level.EdgeLevel);
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
    }
}