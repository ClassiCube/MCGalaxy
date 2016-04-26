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
using System;
using System.Data;
using System.Text.RegularExpressions;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    
    public sealed class CmdMessageBlock : Command {
        
        public override string name { get { return "mb"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdMessageBlock() { }
        static char[] trimChars = { ' ' };

        public override void Use(Player p, string message) {
            if (message == "") { Help(p); return; }

            CatchPos cpos;
            cpos.message = null;
            string[] args = message.Split(trimChars, 2);
            switch (args[0].ToLower()) {
                    case "air": cpos.type = Block.MsgAir; break;
                    case "water": cpos.type = Block.MsgWater; break;
                    case "lava": cpos.type = Block.MsgLava; break;
                    case "black": cpos.type = Block.MsgBlack; break;
                    case "white": cpos.type = Block.MsgWhite; break;
                    case "show": ShowMessageBlocks(p); return;
                    default: cpos.type = Block.MsgWhite; cpos.message = message; break;
            }
            if (args.Length == 1) {
                Player.SendMessage(p, "You need to provide text to put in the messageblock."); return;
            }
            if (cpos.message == null)
                cpos.message = args[1];

            foreach (Command com in Command.all.commands) {
                if (com.defaultRank <= p.group.Permission && !com.type.Contains("mod")) continue;
                
                if (IsCommand(cpos.message, "/" + com.name)) {
                    p.SendMessage("You cannot use that command in a messageblock."); return;
                }
                if (com.shortcut != "" && IsCommand(cpos.message, "/" + com.shortcut)) {
                    p.SendMessage("You cannot use that command in a messageblock."); return;
                }
            }

            p.blockchangeObject = cpos;
            Player.SendMessage(p, "Place where you wish the message block to go."); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        bool IsCommand(string message, string cmd) {
            return message.CaselessEq(cmd) || message.CaselessStarts(cmd + " ");
        }

        void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type, byte extType) {
            p.ClearBlockchange();
            CatchPos cpos = (CatchPos)p.blockchangeObject;
            
            if (p.level.DoBlockchange(p, x, y, z, cpos.type, 0)) {
                Player.GlobalBlockchange(p.level, x, y, z, cpos.type, 0);
                UpdateDatabase(p, cpos, x, y, z);
                Player.SendMessage(p, "Message block created.");
            } else {
                Player.SendMessage(p, "Failed to create a message block.");
            }

            if (p.staticCommands)
                p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        
        void UpdateDatabase(Player p, CatchPos cpos, ushort x, ushort y, ushort z) {
            cpos.message = cpos.message.Replace("'", "\\'");
            cpos.message = Colors.EscapeColors(cpos.message);
            //safe against SQL injections because no user input is given here
            ParameterisedQuery query = ParameterisedQuery.Create();
            DataTable Messages = Database.fillData(query, "SELECT * FROM `Messages" + p.level.name + "` WHERE X=" + x + " AND Y=" + y + " AND Z=" + z);
            
            query.AddParam("@Message", cpos.message);
            if (Messages.Rows.Count == 0)
                Database.executeQuery(query, "INSERT INTO `Messages" + p.level.name + "` (X, Y, Z, Message) VALUES (" + x + ", " + y + ", " + z + ", @Message)");
            else
                Database.executeQuery(query, "UPDATE `Messages" + p.level.name + "` SET Message=@Message WHERE X=" + x + " AND Y=" + y + " AND Z=" + z);
            
            Messages.Dispose();
        }

        struct CatchPos { public string message; public byte type; }

        void ShowMessageBlocks(Player p) {
            p.showMBs = !p.showMBs;
            //safe against SQL injections because no user input is given here
            using (DataTable Messages = Database.fillData("SELECT * FROM `Messages" + p.level.name + "`")) {
                if (p.showMBs) {
                    for (int i = 0; i < Messages.Rows.Count; i++) {
                        DataRow row = Messages.Rows[i];
                        p.SendBlockchange(ushort.Parse(row["X"].ToString()), ushort.Parse(row["Y"].ToString()), ushort.Parse(row["Z"].ToString()), Block.MsgWhite);
                    }
                    Player.SendMessage(p, "Now showing &a" + Messages.Rows.Count + " %SMBs.");
                } else {
                    for (int i = 0; i < Messages.Rows.Count; i++) {
                        DataRow row = Messages.Rows[i];
                        p.RevertBlock(ushort.Parse(row["X"].ToString()), ushort.Parse(row["Y"].ToString()), ushort.Parse(row["Z"].ToString()));
                    }
                    Player.SendMessage(p, "Now hiding MBs.");
                }
            }
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "/mb [block] [message] - Places a message in your next block.");
            Player.SendMessage(p, "Valid blocks: white, black, air, water, lava");
            Player.SendMessage(p, "/mb show shows or hides MBs");
        }
    }
}
