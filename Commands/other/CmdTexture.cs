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
            if (args.Length != 2) { Help(p); return; }
            string scope = args[0].ToLower();
            string url = args[1];
            
            if (url.ToLower() == "normal" || url.ToLower() == "reset") {
                url = "";
            } else {
                if (!url.StartsWith("http://") && !args[1].StartsWith("https://")) {
                    p.SendMessage("Please use http:// or https:// in front of your URL"); return;
                }
                if (!url.EndsWith(".png")) {
                    p.SendMessage("Please make sure your URL ends in .png"); return;
                }
            }
            
            if (scope == "global") {
                Server.defaultTextureUrl = url;
                p.SendMessage("Set server's default texture to " + args[1]);
                foreach (Player pl in Player.players) {
                    if (pl.HasCpeExt(CpeExt.EnvMapAppearance) && pl.level.textureUrl == "") {
                        pl.SendSetMapAppearance(Server.defaultTextureUrl, p.level.EdgeBlock, p.level.HorizonBlock, p.level.EdgeLevel);
                    }
                }
                SrvProperties.Save("properties/server.properties");
            } else if (scope == "level") {
                p.level.textureUrl = url;
                p.SendMessage("Set level's texture to " + args[1]);
                
                foreach (Player pl in Player.players) {
                    if (pl.HasCpeExt(CpeExt.EnvMapAppearance) && pl.level == p.level) {
                        pl.SendSetMapAppearance(p.level.textureUrl, p.level.EdgeBlock, p.level.HorizonBlock, p.level.EdgeLevel);
                    }
                }
                p.level.Save();
                Level.SaveSettings(p.level);
            }
        }
        
        public override void Help(Player p) {
            p.SendMessage("/texture global [texture url] - Changes the server default texture");
            p.SendMessage("/texture level [texture url] - Changes current level's texture");
            p.SendMessage("Using 'reset' for the texture url will reset the texture to default.");
        }
    }
}
