/*
    Copyright 2011 MCGalaxy
        
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
using System.Data;
using System.Text.RegularExpressions;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdMessageBlock : Command
    {
        public override string name { get { return "mb"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdMessageBlock() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }

            CatchPos cpos;
            cpos.message = "";

            try
            {
                switch (message.Split(' ')[0])
                {
                    case "air": cpos.type = Block.MsgAir; break;
                    case "water": cpos.type = Block.MsgWater; break;
                    case "lava": cpos.type = Block.MsgLava; break;
                    case "black": cpos.type = Block.MsgBlack; break;
                    case "white": cpos.type = Block.MsgWhite; break;
                    case "show": showMBs(p); return;
                    default: cpos.type = Block.MsgWhite; cpos.message = message; break;
                }
            }
            catch { cpos.type = Block.MsgWhite; cpos.message = message; }

            string current = "";
            string cmdparts = "";

            /*Fix by alecdent*/
            try
            {
                foreach (var com in Command.all.commands)
                {
                    if (com.type.Contains("mod"))
                    {
                        current = "/" + com.name;

                        cmdparts = message.Split(' ')[0].ToLower().ToString();
                        if (cmdparts[0] == '/')
                        {
                            if (current == cmdparts.ToLower())
                            {
                                p.SendMessage("You can't use that command in your messageblock!");
                                return;
                            }
                        }

                        cmdparts = message.Split(' ')[1].ToLower().ToString();
                        if (cmdparts[0] == '/')
                        {
                            if (current == cmdparts.ToLower())
                            {
                                p.SendMessage("You can't use that command in your messageblock!");
                                return;
                            }
                        }
                        if (com.shortcut != "")
                        {
                            current = "/" + com.name;

                            cmdparts = message.Split(' ')[0].ToLower().ToString();
                            if (cmdparts[0] == '/')
                            {
                                if (current == cmdparts.ToLower())
                                {
                                    p.SendMessage("You can't use that command in your messageblock!");
                                    return;
                                }
                            }

                            cmdparts = message.Split(' ')[1].ToLower().ToString();
                            if (cmdparts[0] == '/')
                            {
                                if (current == cmdparts.ToLower())
                                {
                                    p.SendMessage("You can't use that command in your messageblock!");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch { }

            if (cpos.message == "") cpos.message = message.Substring(message.IndexOf(' ') + 1);
            p.blockchangeObject = cpos;

            Player.SendMessage(p, "Place where you wish the message block to go."); p.ClearBlockchange();
            p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/mb [block] [message] - Places a message in your next block.");
            Player.SendMessage(p, "Valid blocks: white, black, air, water, lava");
            Player.SendMessage(p, "/mb show shows or hides MBs");
        }

        public void Blockchange1(Player p, ushort x, ushort y, ushort z, byte type)
        {
            p.ClearBlockchange();
            CatchPos cpos = (CatchPos)p.blockchangeObject;

            cpos.message = cpos.message.Replace("'", "\\'");
            cpos.message = Chat.EscapeColours(cpos.message);
            //safe against SQL injections because no user input is given here
            DataTable Messages = Database.fillData("SELECT * FROM `Messages" + p.level.name + "` WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z);
            Database.AddParams("@Message", cpos.message);
            if (Messages.Rows.Count == 0)
            {
                Database.executeQuery("INSERT INTO `Messages" + p.level.name + "` (X, Y, Z, Message) VALUES (" + (int)x + ", " + (int)y + ", " + (int)z + ", @Message)");
            }
            else
            {
                Database.executeQuery("UPDATE `Messages" + p.level.name + "` SET Message=@Message WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z);
            }
            Messages.Dispose();
            Player.SendMessage(p, "Message block placed.");
            p.level.Blockchange(p, x, y, z, cpos.type);
            p.SendBlockchange(x, y, z, cpos.type);

            if (p.staticCommands) p.Blockchange += new Player.BlockchangeEventHandler(Blockchange1);
        }

        struct CatchPos { public string message; public byte type; }

        public void showMBs(Player p)
        {
            try
            {
                p.showMBs = !p.showMBs;
                //safe against SQL injections because no user input is given here
                using (DataTable Messages = Database.fillData("SELECT * FROM `Messages" + p.level.name + "`"))
                {
                    int i;

                    if (p.showMBs)
                    {
                        for (i = 0; i < Messages.Rows.Count; i++)
                            p.SendBlockchange(ushort.Parse(Messages.Rows[i]["X"].ToString()), ushort.Parse(Messages.Rows[i]["Y"].ToString()), ushort.Parse(Messages.Rows[i]["Z"].ToString()), Block.MsgWhite);
                        Player.SendMessage(p, "Now showing &a" + i.ToString() + Server.DefaultColor + " MBs.");
                    }
                    else
                    {
                        for (i = 0; i < Messages.Rows.Count; i++)
                            p.SendBlockchange(ushort.Parse(Messages.Rows[i]["X"].ToString()), ushort.Parse(Messages.Rows[i]["Y"].ToString()), ushort.Parse(Messages.Rows[i]["Z"].ToString()), p.level.GetTile(ushort.Parse(Messages.Rows[i]["X"].ToString()), ushort.Parse(Messages.Rows[i]["Y"].ToString()), ushort.Parse(Messages.Rows[i]["Z"].ToString())));
                        Player.SendMessage(p, "Now hiding MBs.");
                    }
                }
            }
            catch (Exception e)
            {
                Server.ErrorLog(e);
            }
        }
    }
}
