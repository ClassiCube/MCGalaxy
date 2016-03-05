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
namespace MCGalaxy.Commands
{
    public sealed class CmdTnt : Command
    {
        public override string name { get { return "tnt"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override CommandPerm[] OtherPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "The lowest rank at which big tnt can be used", 1),
                    new CommandPerm(LevelPermission.Operator, "The lowest rank at which the user can allow/disallow tnt", 2),
                    new CommandPerm(LevelPermission.Operator, "The lowest rank at which nuke tnt can be used", 3),
                }; }
        }

        public override void Use(Player p, string message) {
            if (message.Split(' ').Length > 1) { Help(p); return; }

            if (p.modeType == Block.smalltnt || p.modeType == Block.bigtnt || p.modeType == Block.nuketnt) {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                p.modeType = 0; Player.SendMessage(p, "TNT mode is now &cOFF" + Server.DefaultColor + ".");
            } else if (message.ToLower() == "small" || message == "") {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
                p.modeType = Block.smalltnt;
                Player.SendMessage(p, "TNT mode is now &aON" + Server.DefaultColor + ".");
            } else if (message.ToLower() == "big") {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 1)) {
                    p.modeType = Block.bigtnt;
                    Player.SendMessage(p, "TNT (Big) mode is now &aON" + Server.DefaultColor + ".");
                } else {
                    Player.SendMessage(p, "This mode is reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 1)).name + "+");
                }
            } else if (message.ToLower() == "nuke") {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 3)) {
                    p.modeType = Block.nuketnt;
                    Player.SendMessage(p, "TNT (Nuke) mode is now &aON" + Server.DefaultColor + ".");
                } else {
                    Player.SendMessage(p, "This mode is reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 3)).name + "+");
                }
            } else if (message.ToLower() == "allow") {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2)) {
                    p.allowTnt = true; Player.SendMessage(p, "&cTnt usage has now been enabled!");
                } else {
                    Player.SendMessage(p, "You must be " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 2)).name + "+ to use this command.");
                }                
                return;
            } else if (message.ToLower() == "disallow") {
                if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2)) {                 
                    p.allowTnt = false; Player.SendMessage(p, "&cTnt usage has now been disabled!");
                } else {
                    Player.SendMessage(p, "You must be " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 2)).name + "+ to use this command.");
                }               
                return;
            } else {
                Help(p);
            }
            p.painting = false;
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/tnt [small/big/nuke] - Creates exploding TNT (with Physics 3).");
            Player.SendMessage(p, "Big and Nuke TNT is reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 3)).name + "+");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this, 2)) {
                Player.SendMessage(p, "/tnt allow - Allows the use of tnt server-wide.");
                Player.SendMessage(p, "/tnt disallow - Disallows the use of tnt server-wide.");
            }
        }
    }
}
