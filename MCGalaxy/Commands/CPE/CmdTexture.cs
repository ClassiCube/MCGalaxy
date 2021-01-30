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
using MCGalaxy.Network;

namespace MCGalaxy.Commands.CPE {
    public sealed class CmdTexture : Command2 {
        public override string name { get { return "Texture"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message, CommandData data) {
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces();
            string scope = args[0].ToLower();
            if (scope == "local")    scope = "level";
            if (scope == "localzip") scope = "levelzip";
            
            if (args.Length == 1) {
                if (scope == "level")
                    p.Message("Level terrain: "   + GetPath(p.level.Config.Terrain));
                else if (scope == "levelzip")
                    p.Message("Level tex pack: "  + GetPath(p.level.Config.TexturePack));
                else if (scope == "global")
                    p.Message("Global terrain: "  + GetPath(Server.Config.DefaultTerrain));
                else if (scope == "globalzip")
                    p.Message("Global tex pack: " + GetPath(Server.Config.DefaultTexture));
                else
                    Help(p);
                return;
            }
            
            string url = args[1];
            if (url.CaselessEq("normal") || url.CaselessEq("reset")) {
                url = "";
            } else {
                HttpUtil.FilterURL(ref url);
                
                if (!(url.EndsWith(".png") || url.EndsWith(".zip"))) {
                    p.Message("URL must end in .png (for terrain) or .zip (for texture pack)"); return;
                }
                if (url.Length > NetUtils.StringSize) {
                    p.Message("The URL must be " + NetUtils.StringSize + " characters or less."); return;
                }
            }

            if (scope == "global" || scope == "globalzip") {
                Server.Config.DefaultTerrain = "";
                Server.Config.DefaultTexture = "";
                
                if (url.Length == 0) {
                    p.Message("Reset server textures to default");
                } else if (url.CaselessEnds(".png")) {
                    Server.Config.DefaultTerrain = url;
                    p.Message("Set server's default terrain to " + url);
                } else if (url.CaselessEnds(".zip")) {
                    Server.Config.DefaultTexture = url;
                    p.Message("Set server's default texture pack to " + url);
                }
                UpdateGlobal(p);
            } else if (scope == "level" || scope == "levelzip") {
                if (!LevelInfo.Check(p, data.Rank, p.level, "set texture of this level")) return;
                p.level.Config.Terrain = "";
                p.level.Config.TexturePack = "";
                
                if (url.Length == 0) {
                    p.Message("Reset level textures to server default");
                } else if (url.CaselessEnds(".png")) {
                    p.level.Config.Terrain = url;
                    p.Message("Set level's terrain to " + url);
                } else if (url.CaselessEnds(".zip")) {
                    p.level.Config.TexturePack = url;
                    p.Message("Set level's texture pack to " + url);
                }
                UpdateLevel(p);
            } else {
                Help(p);
            }
        }
        
        static string GetPath(string url) { return url.Length == 0 ? "(none)" : url; }
        
        static void UpdateGlobal(Player p) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                pl.SendCurrentTextures();
            }
            SrvProperties.Save();
        }
        
        static void UpdateLevel(Player p) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != p.level) continue;
                pl.SendCurrentTextures();
            }
            p.level.SaveSettings();
        }
        
        public override void Help(Player p) {
            p.Message("&T/Texture global/level [url]");
            p.Message("&HChanges server default or current level's texture.");
            p.Message("&H[url] must end with .png (terrain) or .zip (texture pack)");
            p.Message("&HUsing 'reset' for [url] will reset the texture to default");
        }
    }
}
