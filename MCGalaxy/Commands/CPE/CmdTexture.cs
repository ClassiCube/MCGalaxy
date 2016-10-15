/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.Commands.World {  
    public sealed class CmdTexture : Command {        
        public override string name { get { return "texture"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            string scope = args[0].ToLower();
            if (scope == "local") scope = "level";
            if (scope == "localzip") scope = "levelzip";
            
            if (args.Length == 1) {
                if (scope == "level")
                    Player.Message(p, "Level terrain: " + GetPath(p.level.terrainUrl));
                else if (scope == "levelzip")
                    Player.Message(p, "Level tex pack: " + GetPath(p.level.texturePackUrl));
                else if (scope == "global")
                    Player.Message(p, "Global terrain: " + GetPath(Server.defaultTerrainUrl));
                else if (scope == "globalzip")
                    Player.Message(p, "Global tex pack: " + GetPath(Server.defaultTextureUrl));
                else
                    Help(p);
                return; 
            }
            
            string url = args[1];
            if (url.CaselessEq("normal") || url.CaselessEq("reset")) {
                url = "";
            } else if (!(url.StartsWith("http://") || url.StartsWith("https://"))) {
                Player.Message(p, "Please use http:// or https:// in front of your URL"); return;
            }
            
            if ((scope == "global" || scope == "level") && !(url == "" || url.EndsWith(".png"))) {
                Player.Message(p, "The terrain URL must end in a .png"); return;
            }
            if ((scope == "globalzip" || scope == "levelzip") && !(url == "" || url.EndsWith(".zip"))) {
                Player.Message(p, "The texture pack URL must end in a .zip"); return;
            }
            if (url.Length > 64) { p.SendMessage("The URL must be 64 characters or less."); return; }
            
            if (scope == "global") {
                Server.defaultTerrainUrl = url;
                Player.Message(p, "Set server's default terrain to " + args[1]);
                UpdateGlobally(p, false);
            } else if (scope == "level") {
                p.level.terrainUrl = url;
                Player.Message(p, "Set level's terrain to " + args[1]);
                UpdateLevel(p);
            } else if (scope == "globalzip") {
                Server.defaultTextureUrl = url;
                Player.Message(p, "Set server's default texture pack to " + args[1]);
                UpdateGlobally(p, true);
            } else if (scope == "levelzip") {
                p.level.texturePackUrl = url;
                Player.Message(p, "Set level's texture pack to " + args[1]);
                UpdateLevel(p);
            } else {
                Help(p);
            }
        }
        
        static string GetPath(string url) { return url == "" ? "(none)" : url; }
        
        void UpdateGlobally(Player p, bool zip) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                string url = zip ? pl.level.texturePackUrl : pl.level.terrainUrl;
                if (url == "") pl.SendCurrentMapAppearance();
            }
            SrvProperties.Save();
        }
        
        void UpdateLevel(Player p) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.level != p.level) continue;
                pl.SendCurrentMapAppearance();
            }
            p.level.Save();
            Level.SaveSettings(p.level);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/texture [scope] [url]");
            Player.Message(p, "%H  global/globalzip scope: Changes server's default texture.");
            Player.Message(p, "%H  level/levelzip scope: Changes current level's texture.");
            Player.Message(p, "%HUsing 'reset' as a url will reset the texture to default.");
            Player.Message(p, "%HNote: zip texture packs are not supported by all clients.");
        }
    }
}
