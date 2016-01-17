/*
    Copyright 2011 MCGalaxy
        
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

namespace MCGalaxy.Commands {
    
    public sealed class CmdTexture : Command {
        
        public override string name { get { return "texture"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdTexture() { }

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }
            string[] args = message.Split(' ');
            string scope = args[0].ToLower();
            
            if (args.Length == 1) {
            	if (scope == "level")
            		Player.SendMessage(p, "Level terrain: " + GetPath(p.level.terrainUrl));
            	else if (scope == "levelzip")
                    Player.SendMessage(p, "Level tex pack: " + GetPath(p.level.texturePackUrl));
                else if (scope == "global")
            		Player.SendMessage(p, "Global terrain: " + GetPath(Server.defaultTerrainUrl));
            	else if (scope == "globalzip")
            		Player.SendMessage(p, "Global tex pack: " + GetPath(Server.defaultTexturePackUrl));
            	else
            		Help(p);
            	return; 
            }
            
            string url = args[1];            
            if (url.ToLower() == "normal" || url.ToLower() == "reset") {
                url = "";
            } else if (!(url.StartsWith("http://") || url.StartsWith("https://"))) {
                p.SendMessage("Please use http:// or https:// in front of your URL"); return;
            }
            
            if ((scope == "global" || scope == "level") && !(url == "" || url.EndsWith(".png"))) {
                p.SendMessage("The terrain URL must end in a .png"); return;
            }
            if ((scope == "globalzip" || scope == "levelzip") && !(url == "" || url.EndsWith(".zip"))) {
                p.SendMessage("The texture pack URL must end in a .zip"); return;
            }
            
            if (scope == "global") {
                Server.defaultTerrainUrl = url;
                p.SendMessage("Set server's default terrain to " + args[1]);
                UpdateGlobally(p, false);
            } else if (scope == "level") {
                p.level.terrainUrl = url;
                p.SendMessage("Set level's terrain to " + args[1]);
                UpdateLevel(p, false);
            } else if (scope == "globalzip") {
                Server.defaultTexturePackUrl = url;
                p.SendMessage("Set server's default texture pack to " + args[1]);
                UpdateGlobally(p, true);
            } else if (scope == "levelzip") {
                p.level.texturePackUrl = url;
                p.SendMessage("Set level's texture pack to " + args[1]);
                UpdateLevel(p, true);
            }
        }
        
        static string GetPath(string url) {
        	return url == "" ? "(none)" : url;
        }
        
        void UpdateGlobally(Player p, bool zip) {
            foreach (Player pl in Player.players) {
                bool hasExt = pl.HasCpeExt(CpeExt.EnvMapAppearance) || pl.HasCpeExt(CpeExt.EnvMapAppearance, 2);
                string url = zip ? pl.level.texturePackUrl : pl.level.terrainUrl;
                if (hasExt && url == "")
                    pl.SendCurrentMapAppearance();
            }
            SrvProperties.Save("properties/server.properties");
        }
        
        void UpdateLevel(Player p, bool zip) {
            foreach (Player pl in Player.players) {
                bool hasExt = pl.HasCpeExt(CpeExt.EnvMapAppearance) || pl.HasCpeExt(CpeExt.EnvMapAppearance, 2);
                if (hasExt && pl.level == p.level)
                    pl.SendCurrentMapAppearance();
            }
            p.level.Save();
            Level.SaveSettings(p.level);
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/texture [scope] [url]");
            Player.SendMessage(p, "%H  global/globalzip scope: Changes server's default texture.");
            Player.SendMessage(p, "%H  level/levelzip scope: Changes current level's texture.");
            Player.SendMessage(p, "%HUsing 'reset' as a url will reset the texture to default.");
            Player.SendMessage(p, "%HNote: zip texture packs are not supported by all clients.");
        }
    }
}
