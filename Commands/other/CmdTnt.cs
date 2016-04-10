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
namespace MCGalaxy.Commands
{
    public sealed class CmdTnt : Command
    {
        public override string name { get { return "tnt"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "Lowest rank at which big tnt can be used"),
                    new CommandPerm(LevelPermission.Operator, "Lowest rank at which the user can allow/disallow tnt"),
                    new CommandPerm(LevelPermission.Operator, "Lowest rank at which nuke tnt can be used"),
                }; }
        }

        public override void Use(Player p, string message) {
            if (message.Split(' ').Length > 1) { Help(p); return; }

            if (p.modeType == Block.smalltnt || p.modeType == Block.bigtnt || p.modeType == Block.nuketnt) {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                p.modeType = 0; Player.SendMessage(p, "TNT mode is now &cOFF%S.");
            } else if (message.ToLower() == "small" || message == "") {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
                p.modeType = Block.smalltnt;
                Player.SendMessage(p, "TNT mode is now &aON%S.");
            } else if (message.ToLower() == "big") {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
            	if (CheckAdditionalPerm(p, 1)) {
                    p.modeType = Block.bigtnt;
                    Player.SendMessage(p, "TNT (Big) mode is now &aON%S.");
                } else {
                    MessageNeedPerms(p, "can use big TNT mode.", 1); return;
                }
            } else if (message.ToLower() == "nuke") {
                if (!p.allowTnt) {
                    Player.SendMessage(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
            	if (CheckAdditionalPerm(p, 3)) {
                    p.modeType = Block.nuketnt;
                    Player.SendMessage(p, "TNT (Nuke) mode is now &aON%S.");
                } else {
                    MessageNeedPerms(p, "can use nuke TNT mode.", 3); return;
                }
            } else if (message.ToLower() == "allow") {
            	if (CheckAdditionalPerm(p, 2)) {
                    p.allowTnt = true; Player.SendMessage(p, "&cTnt usage has now been enabled!");
                } else {
                    MessageNeedPerms(p, "can allow TNT usage.", 2); return;
                }                
                return;
            } else if (message.ToLower() == "disallow") {
            	if (CheckAdditionalPerm(p, 2)) {
                    p.allowTnt = false; Player.SendMessage(p, "&cTnt usage has now been disabled!");
                } else {
                    MessageNeedPerms(p, "can disallow TNT usage.", 2); return;
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
            if (CheckAdditionalPerm(p, 2)) {
                Player.SendMessage(p, "/tnt allow - Allows the use of tnt server-wide.");
                Player.SendMessage(p, "/tnt disallow - Disallows the use of tnt server-wide.");
            }
        }
    }
}
