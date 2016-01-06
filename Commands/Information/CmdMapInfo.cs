/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.IO;

namespace MCGalaxy.Commands {
	
    public sealed class CmdMapInfo : Command {
		
        public override string name { get { return "mapinfo"; } }
        public override string shortcut { get { return "status"; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public CmdMapInfo() { }

        public override void Use(Player p, string message)
        {
            Level lvl = message == "" ? p.level : Level.Find(message);
            if (lvl == null) { Player.SendMessage(p, "Could not find specified level."); return; }

            Player.SendMessage(p, "&b" + lvl.name + Server.DefaultColor + ": Width=" + lvl.Width + " Height=" + lvl.Height + " Depth=" + lvl.Length);
            string physicsState = CmdPhysics.states[lvl.physics];
            Player.SendMessage(p, "Physics are " + physicsState + Server.DefaultColor + " on &b" + lvl.name);

            Player.SendMessage(p, "Build rank = " + Group.findPerm(lvl.permissionbuild).color + Group.findPerm(lvl.permissionbuild).trueName + Server.DefaultColor +
                               " : Visit rank = " + Group.findPerm(lvl.permissionvisit).color + Group.findPerm(lvl.permissionvisit).trueName);

            Player.SendMessage(p, "BuildMax Rank = " + Group.findPerm(lvl.perbuildmax).color + Group.findPerm(lvl.perbuildmax).trueName + Server.DefaultColor + 
                               " : VisitMax Rank = " + Group.findPerm(lvl.pervisitmax).color + Group.findPerm(lvl.pervisitmax).trueName);

            string gunStatus = lvl.guns ? "&aonline" : "&coffline";
            Player.SendMessage(p, "&cGuns &eare " + gunStatus + " &eon " + lvl.name + ".");

            if (Directory.Exists(Server.backupLocation + "/" + lvl.name)) {
                int latestBackup = Directory.GetDirectories(Server.backupLocation + "/" + lvl.name).Length;
                Player.SendMessage(p, "Latest backup: &a" + latestBackup + Server.DefaultColor + " at &a" + Directory.GetCreationTime(@Server.backupLocation + "/" + lvl.name + "/" + latestBackup).ToString("yyyy-MM-dd HH:mm:ss")); // + Directory.GetCreationTime(@Server.backupLocation + "/" + latestBackup + "/").ToString("yyyy-MM-dd HH:mm:ss"));
            } else  {
                Player.SendMessage(p, "No backups for this map exist yet.");
            }
            
            if (lvl.textureUrl != "") {
                Player.SendMessage(p, "TexturePack: %b" + lvl.textureUrl);
            } else if (lvl == Server.mainLevel && Server.defaultTextureUrl != "") {
                Player.SendMessage(p, "TexturePack: " + Server.defaultTextureUrl);
            } else {
                Player.SendMessage(p, "No textures for this map exist yet.");
            }
            
            const string format = "Colors: Fog {0}, Sky {1}, Clouds {2}, Sunlight {3}, Shadowlight {4}";
            Player.SendMessage(p, String.Format(format, Color(lvl.FogColor), Color(lvl.SkyColor), Color(lvl.CloudColor),
                                                Color(lvl.LightColor), Color(lvl.ShadowColor)));
            if (lvl.EdgeLevel != -1) { Player.SendMessage(p, "Water Level: %b" + lvl.EdgeLevel.ToString()); }
            else { Player.SendMessage(p, "Water Level: %bdefault"); }
            if (lvl.EdgeBlock != Block.blackrock) { Player.SendMessage(p, "Edge Block: %b" + lvl.EdgeBlock.ToString()); }
            else { Player.SendMessage(p, "Edge Block: %bdefault"); }
            if (lvl.HorizonBlock != Block.water) { Player.SendMessage(p, "Horizon Block: %b" + lvl.HorizonBlock.ToString()); }
            else { Player.SendMessage(p, "Horizon Block: %bdefault"); }
        }
        
        static string Color(string src) {
            return (src == null || src == "-1") ? "%bnone%e" : "%b" + src + "%e";
        }
        
        public override void Help(Player p)  {
            Player.SendMessage(p, "/mapinfo <map> - Display details of <map>");
        }
    }
}
