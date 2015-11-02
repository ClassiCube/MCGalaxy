/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace MCGalaxy
{
    /// <summary>
    /// These are extra permissions for certain commands
    /// </summary>
    public static class CommandOtherPerms
    {
        /// <summary>
        /// Restore the permissions to there defaults
        /// </summary>
        public static void AddDefaultPerms()
        {
            Add(Command.all.Find("ban"), (int)LevelPermission.AdvBuilder, "The lowest rank that can be banned");
            Add(Command.all.Find("zone"), (int)LevelPermission.Operator, "The lowest rank to delete zones", 1);
            Add(Command.all.Find("zone"), (int)LevelPermission.Operator, "The lowest rank to delete all zones", 2);
            Add(Command.all.Find("zone"), (int)LevelPermission.Operator, "The lowest rank to create zones", 3);
            Add(Command.all.Find("whowas"), (int)LevelPermission.AdvBuilder, "The lowest rank at which it shows the player who uses the command the other players ip, if they are whitelisted and if they are a developer");
            Add(Command.all.Find("whois"), (int)LevelPermission.AdvBuilder, "The lowest rank at which it shows the player who uses the command the other players ip, if they are whitelisted and if they are a developer");
            Add(Command.all.Find("warp"), (int)LevelPermission.Operator, "The lowest rank to create warps", 1);
            Add(Command.all.Find("warp"), (int)LevelPermission.Operator, "The lowest rank to delete warps", 2);
            Add(Command.all.Find("warp"), (int)LevelPermission.Operator, "The lowest rank to move/edit warps", 3);
            Add(Command.all.Find("undo"), (int)LevelPermission.Operator, "The lowest rank to undo other players actions", 1);
            Add(Command.all.Find("undo"), (int)LevelPermission.AdvBuilder, "The lowest rank to be able to undo physics", 2);
            Add(Command.all.Find("tnt"), (int)LevelPermission.Operator, "The lowest rank at which big tnt can be used", 1);
            Add(Command.all.Find("tnt"), (int)LevelPermission.Operator, "The lowest rank at which the user can allow/disallow tnt", 2);
            Add(Command.all.Find("tnt"), (int)LevelPermission.Operator, "The lowest rank at which nuke tnt can be used", 3);
            Add(Command.all.Find("store"), (int)LevelPermission.Operator, "The lowest rank at which a user can delete someone elses save");
            Add(Command.all.Find("rules"), (int)LevelPermission.Builder, "The lowest rank that can send rules to other players");
            Add(Command.all.Find("reveal"), (int)LevelPermission.Operator, "The lowest rank that can reveal to everyone");
            Add(Command.all.Find("report"), (int)LevelPermission.Operator, "The lowest rank at which the player can check,view and delete reports");
            Add(Command.all.Find("patrol"), (int)LevelPermission.Guest, "The highest rank to be patrolled");
            Add(Command.all.Find("news"), (int)LevelPermission.Operator, "The lowest rank that can send rules to everyone");
            Add(Command.all.Find("map"), (int)LevelPermission.Operator, "The lowest rank that can edt map optios");
            Add(Command.all.Find("faq"), (int)LevelPermission.Builder, "The lowest rank that can send rules to other players");
            Add(Command.all.Find("economy"), (int)LevelPermission.Operator, "The lowest rank that can setup economy");
            Add(Command.all.Find("chatroom"), (int)LevelPermission.AdvBuilder, "The lowest rank that can create chatrooms", 1);
            Add(Command.all.Find("chatroom"), (int)LevelPermission.AdvBuilder, "The lowest rank that can delete a chatroom if empty", 2);
            Add(Command.all.Find("chatroom"), (int)LevelPermission.Operator, "The lowest rank that can delete a chatroom", 3);
            Add(Command.all.Find("chatroom"), (int)LevelPermission.Operator, "The lowest rank that can spy on a chatroom", 4);
            Add(Command.all.Find("chatroom"), (int)LevelPermission.Operator, "The lowest rank that can force a player to join a chatroom", 5);
            Add(Command.all.Find("chatroom"), (int)LevelPermission.Operator, "The lowest rank that can kick a player from a chatroom", 6);
            Add(Command.all.Find("chatroom"), (int)LevelPermission.Operator, "The lowest rank that can send a global message to a chatroom (without any delay)", 7);
            Add(Command.all.Find("changelog"), (int)LevelPermission.Operator, "The lowest rank that can send the changelog to everybody");
            Add(Command.all.Find("countdown"), (int)LevelPermission.Operator, "The lowest rank that can send the rules to everybody", 1);
            Add(Command.all.Find("countdown"), (int)LevelPermission.Operator, "The lowest rank that can setup countdown (download, start, restart, enable, disable, cancel)", 2);
            Add(Command.all.Find("botset"), (int)LevelPermission.Operator, "The lowest rank that can set the bot to killer");
            Add(Command.all.Find("draw"), (int)Command.all.Find("draw").defaultRank, "The lowest rank that can use cones with /draw", 1);
            Add(Command.all.Find("draw"), (int)Command.all.Find("draw").defaultRank, "The lowest rank that can use pyramids with /draw", 2);
            Add(Command.all.Find("draw"), (int)Command.all.Find("draw").defaultRank, "The lowest rank that can use spheres with /draw", 3);
            Add(Command.all.Find("draw"), (int)Command.all.Find("draw").defaultRank, "The lowest rank that can use volcanos with /draw", 4);
            Add(Command.all.Find("title"), (int)LevelPermission.Operator, "The lowest rank that can change other's titles");
            Add(Command.core.Find("tntwars"), (int)LevelPermission.Operator, "The lowest rank that can use admin commands for tntwars", 1);
        }

        public static List<OtherPerms> list = new List<OtherPerms>();

        public class OtherPerms
        {
            public Command cmd;
            public int Permission;
            public string Description = "";
            public int number;
        }

        public static int GetPerm(Command cmd, int number = 1)
        {
            OtherPerms otpe = Find(cmd, number);
            return otpe.Permission;
        }

        public static OtherPerms Find(Command cmd, int number = 1){
            return list.FirstOrDefault(OtPe => OtPe.cmd == cmd && OtPe.number == number);
        }

        public static void Add(Command command, int Perm, string desc, int number = 1)
        {
            if (Perm > 120) { return; }
            OtherPerms otpe = new OtherPerms();
            otpe.cmd = command;
            otpe.Permission = Perm;
            otpe.Description = desc;
            otpe.number = number;
            list.Add(otpe);
        }

        public static void Edit(OtherPerms op, int perm)
        {
            if (perm > 120) { return; }
            OtherPerms otpe = op;
            list.Remove(op);
            otpe.Permission = perm;
            list.Add(otpe);
        }

        public static int GetMaxNumber(Command cmd)
        {
            int i = 1;
            bool stop = false;
            while (stop == false)
            {
                OtherPerms op = Find(cmd, i);
                if (op == null) { stop = true; }
                else { i++; }
            }
            return (i - 1);
        }

        public static void Save()
        {
            using (StreamWriter SW = new StreamWriter("properties/ExtraCommandPermissions.properties"))
            {
                SW.WriteLine("#     This file is used for setting up additional permissions that are needed in commands!!");
                SW.WriteLine("#");
                SW.WriteLine("#     LAYOUT:");
                SW.WriteLine("#     [commandname]:[additionalpermissionnumber]:[permissionlevel]:[description]");
                SW.WriteLine("#     I.E:");
                SW.WriteLine("#     countdown:2:80:The lowest rank that can setup countdown (download, start, restart, enable, disable, cancel)");
                SW.WriteLine("#");
                SW.WriteLine("#     Please also note that descriptions cannot contain ':' and permissions cannot be above 120");
                SW.WriteLine("#");
                foreach (OtherPerms otpe in list)
                {
                    try
                    {
                        SW.WriteLine(otpe.cmd.name + ":" + otpe.number + ":" + otpe.Permission + ":" + otpe.Description);
                    }
                    catch { Server.s.Log("Saving an additional command permission failed!!"); }
                }
                SW.Dispose();
            }
        }

        public static void Load()
        {
            if (list.Count == 0) { AddDefaultPerms(); }
            if (File.Exists("properties/ExtraCommandPermissions.properties"))
            {
                using (StreamReader SR = new StreamReader("properties/ExtraCommandPermissions.properties"))
                {
                    string line;
                    while (SR.EndOfStream == false)
                    {
                        line = SR.ReadLine();
                        try
                        {
                            if (!line.StartsWith("#") && line.Contains(':'))
                            {
                                string[] LINE = line.ToLower().Split(':');
                                OtherPerms OTPE = Find(Command.all.Find(LINE[0]), int.Parse(LINE[1]));
                                Edit(OTPE, int.Parse(LINE[2]));
                            }
                        }
                        catch { Server.s.Log("Loading an additional command permission failed!!"); }
                    }
                    SR.Dispose();
                }
            }
            else { Save(); Load(); }
        }
    }
}
