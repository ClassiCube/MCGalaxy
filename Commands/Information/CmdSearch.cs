/*
Copyright 2011-2014 MCGalaxy
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
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands
{
    public class CmdSearch : Command
    {
        public override string name { get { return "search"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Information; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
        public CmdSearch() { }

        public override void Use(Player p, string message)
        {
            if (message.Split(' ').Length < 2)
            {
                Help(p);
                return;
            }
            string type = message.Split(' ')[0];
            string keyword = message.Remove(0, (type.Length + 1));
            //
            if (type.ToLower().Contains("command") || type.ToLower().Contains("cmd"))
            {
                bool mode = true;
                string[] keywords = keyword.Split(' ');
                string[] found = null;
                if (keywords.Length == 1) { found = Commands.CommandKeywords.Find(keywords[0]); }
                else { found = Commands.CommandKeywords.Find(keywords); }
                if (found == null) { Player.SendMessage(p, "No commands found matching keyword(s): '" + message.Remove(0, (type.Length + 1)) + "'"); }
                else
                {
                    Player.SendMessage(p, "&bfound: ");
                    foreach (string s in found) { if (mode) { Player.SendMessage(p, "&2/" + s); } else { Player.SendMessage(p, "&9/" + s); } mode = (mode) ? false : true; }
                }
            }
            if (type.ToLower().Contains("block"))
            {
                string blocks = "";
                bool mode = true;
                for (byte i = 0; i < 255; i++)
                {
                    if (Block.Name(i).ToLower() != "unknown")
                    {
                        if (Block.Name(i).Contains(keyword))
                        {
                            if (mode) { blocks += Server.DefaultColor + ", &9" + Block.Name(i); }
                            else { blocks += Server.DefaultColor + ", &2" + Block.Name(i); }
                            mode = (mode) ? false : true;
                        }
                    }
                }
                if (blocks == "") { Player.SendMessage(p, "No blocks found containing &b" + keyword); }
                Player.SendMessage(p, blocks.Remove(0, 2));
            }
            if (type.ToLower().Contains("rank"))
            {
                string ranks = "";
                foreach (Group g in Group.GroupList)
                {
                    if (g.name.Contains(keyword))
                    {
                        ranks += g.color + g.name + "'";
                    }
                }
                if (ranks == "") { Player.SendMessage(p, "No ranks found containing &b" + keyword); }
                else { foreach (string r in ranks.Split('\'')) { Player.SendMessage(p, r); } }
            }
            if (type.ToLower().Contains("player"))
            {
                string players = "";
                Player[] online = PlayerInfo.Online;
                foreach (Player who in online)
                {
                    if (who.name.ToLower().Contains(keyword.ToLower()))
                    {
                        players += ", " + who.color + who.name;
                    }
                }
                if (players == "") { Player.SendMessage(p, "No usernames found containing &b" + keyword); }
                else { Player.SendMessage(p, players.Remove(0, 2)); }
            }
            if (type.ToLower().Contains("loaded"))
            {
                string levels = "";
                foreach (Level level in Server.levels)
                {
                    if (level.name.ToLower().Contains(keyword.ToLower()))
                    {
                        levels += ", " + level.name;
                    }
                }
                if (levels == "") { Player.SendMessage(p, "No loaded levels found containing &b" + keyword); }
                else { Player.SendMessage(p, levels.Remove(0, 2)); }
            }
            if (type.ToLower().Contains("levels"))
            {
                string searched = "";
                DirectoryInfo di = new DirectoryInfo("levels/");
                FileInfo[] fi = di.GetFiles("*.lvl");

                foreach (FileInfo file in fi)
                {
                    string level = file.Name.Replace(".lvl", "".ToLower());
                    if ((level.Contains(keyword.ToLower())))
                    {
                        searched += ", " + level;
                    }
                }

                if (searched == "") { Player.SendMessage(p, "No levels found containing &b" + keyword); }
                else { Player.SendMessage(p, searched.Remove(0, 2)); }
            }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "&b/search &2commands &a<keywords[more]> &e- finds commands with those keywords");
            Player.SendMessage(p, "&b/search &2blocks &a<keyword> &e- finds blocks with that keyword");
            Player.SendMessage(p, "&b/search &2ranks &a<keyword> &e- finds blocks with that keyword");
            Player.SendMessage(p, "&b/search &2players &a<keyword> &e- find players with that keyword");
            Player.SendMessage(p, "&b/search &2loaded &a<keyword> &e- finds loaded levels with that keyword");
            Player.SendMessage(p, "&b/search &2levels &a<keyword> &e- find all levels with that keyword");
        }
    }
}
