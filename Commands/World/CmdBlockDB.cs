/*
    Copyright 2011 MCForge
    
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
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {    
    public sealed class CmdBlockDB : Command {
        
        public override string name { get { return "blockdb"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public override CommandAlias[] Aliases {
            get { return new [] { new CommandAlias("clearblockchanges", "clear"), 
                    new CommandAlias("cbc", "clear") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.Split(' ');
            if (args.Length == 1 && p == null) {
                Player.Message(p, "You must provide a map name when running the command from console."); return;
            }
            args[0] = args[0].ToLower();
            
            Level lvl = p == null ? null : p.level;
            if (args.Length > 1) {
                lvl = LevelInfo.FindOrShowMatches(p, args[1]);
                if (lvl == null) return;
            }
            
            if (args[0] == "clear") {
            	Player.Message(p, "Clearing &cALL %Sblock changes for &d{0}...", lvl.name);
                if (Server.useMySQL)
                    Database.executeQuery("TRUNCATE TABLE `Block" + lvl.name + "`");
                else
                    Database.executeQuery("DELETE FROM `Block" + lvl.name + "`");
                Player.Message(p, "Cleared &cALL %Sblock changes for &d" + lvl.name);
            } else if (args[0] == "disable") {
                lvl.UseBlockDB = false;
                Player.Message(p, "&cDisabled %Srecording further block changesfor &d" + lvl.name);
                Level.SaveSettings(lvl);
            } else if (args[0] == "enable") {
                lvl.UseBlockDB = true;
                Player.Message(p, "&aEnabled %Srecording further block changes for &d" + lvl.name);
                Level.SaveSettings(lvl);
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/blockdb clear [map]");
            Player.Message(p, "%HClears the BlockDB (block changes stored in /about) for [map]");
            Player.Message(p, "%T/blockdb disable [map]");
            Player.Message(p, "%HDisables recording block changes to the BlockDB for [map]");
            Player.Message(p, "%T/blockdb enable [map]");
            Player.Message(p, "%HEnables %Hrecording block changes to the BlockDB for [map]");
            Player.Message(p, "%HIf no map name is given, uses your current map.");
            Player.Message(p, "%CUse these commands with great caution!");
        }
    }
}