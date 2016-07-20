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
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can use big TNT"),
                    new CommandPerm(LevelPermission.Operator, "+ can allow/disallow tnt"),
                    new CommandPerm(LevelPermission.Operator, "+ can use nuke TNT"),
                }; }
        }

        public override void Use(Player p, string message) {
            if (message.Split(' ').Length > 1) { Help(p); return; }

            if (p.modeType == Block.smalltnt || p.modeType == Block.bigtnt || p.modeType == Block.nuketnt) {
                if (!p.allowTnt) {
                    Player.Message(p, "Tnt usage is not allowed at the moment!"); return;
                }
                p.modeType = 0; Player.Message(p, "TNT mode is now &cOFF%S.");
            } else if (message.ToLower() == "small" || message == "") {
                if (!p.allowTnt) {
                    Player.Message(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
                p.modeType = Block.smalltnt;
                Player.Message(p, "TNT mode is now &aON%S.");
            } else if (message.ToLower() == "big") {
                if (!p.allowTnt) {
                    Player.Message(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
            	if (CheckExtraPerm(p, 1)) {
                    p.modeType = Block.bigtnt;
                    Player.Message(p, "TNT (Big) mode is now &aON%S.");
                } else {
                    MessageNeedExtra(p, "can use big TNT mode.", 1); return;
                }
            } else if (message.ToLower() == "nuke") {
                if (!p.allowTnt) {
                    Player.Message(p, "Tnt usage is not allowed at the moment!"); return;
                }
                
            	if (CheckExtraPerm(p, 3)) {
                    p.modeType = Block.nuketnt;
                    Player.Message(p, "TNT (Nuke) mode is now &aON%S.");
                } else {
                    MessageNeedExtra(p, "can use nuke TNT mode.", 3); return;
                }
            } else if (message.ToLower() == "allow") {
            	if (CheckExtraPerm(p, 2)) {
                    p.allowTnt = true; Player.Message(p, "&cTnt usage has now been enabled!");
                } else {
                    MessageNeedExtra(p, "can allow TNT usage.", 2); return;
                }                
                return;
            } else if (message.ToLower() == "disallow") {
            	if (CheckExtraPerm(p, 2)) {
                    p.allowTnt = false; Player.Message(p, "&cTnt usage has now been disabled!");
                } else {
                    MessageNeedExtra(p, "can disallow TNT usage.", 2); return;
                }               
                return;
            } else {
                Help(p);
            }
            p.painting = false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "/tnt [small/big/nuke] - Creates exploding TNT (with Physics 3).");
            Player.Message(p, "Big and Nuke TNT is reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 3)).name + "+");
            if (CheckExtraPerm(p, 2)) {
                Player.Message(p, "/tnt allow - Allows the use of tnt server-wide.");
                Player.Message(p, "/tnt disallow - Disallows the use of tnt server-wide.");
            }
        }
    }
}
