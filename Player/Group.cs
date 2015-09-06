/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace MCGalaxy
{
    /// <summary>
    /// This is the group object
    /// Where ranks and there data are stored
    /// </summary>
    public sealed class Group
    {
        public delegate void RankSet(Player p, Group newrank);
        [Obsolete("Please use OnPlayerRankSetEvent.Register()")]
        public static event RankSet OnPlayerRankSet;
        public delegate void GroupSave();
        [Obsolete("Please use OnGroupSaveEvent.Register()")]
        public static event GroupSave OnGroupSave;
        public delegate void GroupLoad();
        [Obsolete("Please use OnGroupLoadEvent.Register()")]
        public static event GroupLoad OnGroupLoad;
        public delegate void GroupLoaded(Group mGroup);
        [Obsolete("Please use OnGroupLoadedEvent.Register()")]
        public static event GroupLoaded OnGroupLoaded;
        public static bool cancelrank = false;
        //Move along...nothing to see here...
        internal static void because(Player p, Group newrank) { if (OnPlayerRankSet != null) { OnPlayerRankSet(p, newrank); } OnPlayerRankSetEvent.Call(p, newrank); }
        public string name;
        public string trueName;
        public string color;
        public byte OverseerMaps = 3;
        public LevelPermission Permission;
        public int maxBlocks;
        public long maxUndo;
        public CommandList commands;
        public string fileName;
        public PlayerList playerList;
        public string MOTD = String.Empty;

        /// <summary>
        /// Create a new group object
        /// </summary>
        public Group()
        {
            Permission = LevelPermission.Null;
        }

        /// <summary>
        /// Create a new group object
        /// </summary>
        /// <param name="Perm">The permission of the group</param>
        /// <param name="maxB">The maxblocks this group can cuboid</param>
        /// <param name="maxUn">The max undo this group can do</param>
        /// <param name="fullName">The group full name</param>
        /// <param name="newColor">The color of the group (Not including the &)</param>
        /// <param name="motd">the custom MOTD for the group</param>
        /// <param name="file">The file path where the current players of this group are stored</param>
        public Group(LevelPermission Perm, int maxB, long maxUn, string fullName, char newColor, string motd, string file, byte maps = 2)
        {
            Permission = Perm;
            maxBlocks = maxB;
            maxUndo = maxUn;
            trueName = fullName;
            name = trueName.ToLower();
            color = "&" + newColor;
            MOTD = motd;
            fileName = file;
            playerList = name != "nobody" ? PlayerList.Load(fileName, this) : new PlayerList();
            if (OnGroupLoaded != null)
                OnGroupLoaded(this);
            OnGroupLoadedEvent.Call(this);
        }
        /// <summary>
        /// Fill the commands that this group can use
        /// </summary>
        public void fillCommands()
        {
            CommandList _commands = new CommandList();
            GrpCommands.AddCommands(out _commands, Permission);
            commands = _commands;
        }
        /// <summary>
        /// Check to see if this group can excute cmd
        /// </summary>
        /// <param name="cmd">The command object to check</param>
        /// <returns>True if this group can use it, false if they cant</returns>
        public bool CanExecute(Command cmd) { return commands.Contains(cmd); }

        public static List<Group> GroupList = new List<Group>();
        public static Group standard;
        /// <summary>
        /// Load up all server groups
        /// </summary>
        public static void InitAll()
        {
            GroupList = new List<Group>();

            if (File.Exists("properties/ranks.properties"))
            {
                string[] lines = File.ReadAllLines("properties/ranks.properties");

                Group thisGroup = new Group();
                int gots = 0, version = 1;
                if (lines.Length > 0 && lines[0].StartsWith("#Version "))
                {
                    try { version = int.Parse(lines[0].Remove(0, 9)); }
                    catch { Server.s.Log("The ranks.properties version header is invalid! Ranks may fail to load!"); }
                }

                foreach (string s in lines)
                {
                    try
                    {
                        if (s == "" || s[0] == '#') continue;
                        if (s.Split('=').Length == 2)
                        {
                            string property = s.Split('=')[0].Trim();
                            string value = s.Split('=')[1].Trim();

                            if (thisGroup.name == "" && property.ToLower() != "rankname")
                            {
                                Server.s.Log("Hitting an error at " + s + " of ranks.properties");
                            }
                            else
                            {
                                switch (property.ToLower())
                                {
                                    case "rankname":
                                        gots = 0;
                                        thisGroup = new Group();

                                        if (value.ToLower() == "adv" || value.ToLower() == "op" || value.ToLower() == "super" || value.ToLower() == "nobody" || value.ToLower() == "noone")
                                            Server.s.Log("Cannot have a rank named \"" + value.ToLower() + "\", this rank is hard-coded.");
                                        else if (GroupList.Find(grp => grp.name == value.ToLower()) == null)
                                            thisGroup.trueName = value;
                                        else
                                            Server.s.Log("Cannot add the rank " + value + " twice");
                                        break;
                                    case "permission":
                                        int foundPermission;

                                        try
                                        {
                                            foundPermission = int.Parse(value);
                                        }
                                        catch { Server.s.Log("Invalid permission on " + s); break; }

                                        if (thisGroup.Permission != LevelPermission.Null)
                                        {
                                            Server.s.Log("Setting permission again on " + s);
                                            gots--;
                                        }

                                        bool allowed = GroupList.Find(grp => grp.Permission == (LevelPermission)foundPermission) == null;

                                        if (foundPermission > 119 || foundPermission < -50)
                                        {
                                            Server.s.Log("Permission must be between -50 and 119 for ranks");
                                            break;
                                        }

                                        if (allowed)
                                        {
                                            gots++;
                                            thisGroup.Permission = (LevelPermission)foundPermission;
                                        }
                                        else
                                        {
                                            Server.s.Log("Cannot have 2 ranks set at permission level " + value);
                                        }
                                        break;
                                    case "limit":
                                        int foundLimit;

                                        try
                                        {
                                            foundLimit = int.Parse(value);
                                        }
                                        catch { Server.s.Log("Invalid limit on " + s); break; }

                                        gots++;
                                        thisGroup.maxBlocks = foundLimit;
                                        break;
                                    case "maxundo":
                                        int foundMax;

                                        try
                                        {
                                            foundMax = int.Parse(value);
                                        }
                                        catch { Server.s.Log("Invalid maximum on " + s); break; }

                                        gots++;
                                        thisGroup.maxUndo = foundMax;
                                        break;
                                    case "color":
                                        char foundChar;

                                        try
                                        {
                                            foundChar = char.Parse(value);
                                        }
                                        catch { Server.s.Log("Incorrect color on " + s); break; }

                                        if ((foundChar >= '0' && foundChar <= '9') || (foundChar >= 'a' && foundChar <= 'f'))
                                        {
                                            gots++;
                                            thisGroup.color = foundChar.ToString(CultureInfo.InvariantCulture);
                                        }
                                        else
                                        {
                                            Server.s.Log("Invalid color code at " + s);
                                        }
                                        break;
                                    case "filename":
                                        if (value.Contains("\\") || value.Contains("/"))
                                        {
                                            Server.s.Log("Invalid filename on " + s);
                                            break;
                                        }

                                        gots++;
                                        thisGroup.fileName = value;
                                        break;
                                    case "motd":
                                        if (!String.IsNullOrEmpty(value))
                                            thisGroup.MOTD = value;
                                        gots++;
                                        break;
                                    case "osmaps":
                                        byte osmaps;
                                        if (byte.TryParse(value, out osmaps) == false)
                                        { osmaps = 2; }

                                        gots++;
                                        thisGroup.OverseerMaps = osmaps;
                                        break;
                                }

                                if ((gots >= 4 && version < 2) || (gots >= 5 && version < 3) || gots >= 6)
                                {
                                    if (version < 2)
                                    {
                                        if ((int)thisGroup.Permission >= 100)
                                            thisGroup.maxUndo = int.MaxValue;
                                        else if ((int)thisGroup.Permission >= 80)
                                            thisGroup.maxUndo = 5400;
                                    }

                                    GroupList.Add(new Group(thisGroup.Permission,
                                        thisGroup.maxBlocks,
                                        thisGroup.maxUndo,
                                        thisGroup.trueName,
                                        thisGroup.color[0],
                                        thisGroup.MOTD,
                                        thisGroup.fileName,
                                        thisGroup.OverseerMaps));
                                }
                            }
                        }
                        else
                        {
                            Server.s.Log("In ranks.properties, the line " + s + " is wrongly formatted");
                        }
                    }
                    catch (Exception e) { Server.s.Log("Encountered an error at line \"" + s + "\" in ranks.properties"); Server.ErrorLog(e); }
                }
            }

            if (GroupList.Find(grp => grp.Permission == LevelPermission.Banned) == null) GroupList.Add(new Group(LevelPermission.Banned, 1, 1, "Banned", '8', String.Empty, "banned.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Guest) == null) GroupList.Add(new Group(LevelPermission.Guest, 1, 120, "Guest", '7', String.Empty, "guest.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Builder) == null) GroupList.Add(new Group(LevelPermission.Builder, 400, 300, "Builder", '2', String.Empty, "builders.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.AdvBuilder) == null) GroupList.Add(new Group(LevelPermission.AdvBuilder, 1200, 900, "AdvBuilder", '3', String.Empty, "advbuilders.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Operator) == null) GroupList.Add(new Group(LevelPermission.Operator, 2500, 5400, "Operator", 'c', String.Empty, "operators.txt"));
            if (GroupList.Find(grp => grp.Permission == LevelPermission.Admin) == null) GroupList.Add(new Group(LevelPermission.Admin, 65536, int.MaxValue, "SuperOP", 'e', String.Empty, "uberOps.txt"));
            GroupList.Add(new Group(LevelPermission.Nobody, 65536, -1, "Nobody", '0', String.Empty, "nobody.txt"));

            bool swap = true; Group storedGroup;
            while (swap)
            {
                swap = false;
                for (int i = 0; i < GroupList.Count - 1; i++)
                    if (GroupList[i].Permission > GroupList[i + 1].Permission)
                    {
                        swap = true;
                        storedGroup = GroupList[i];
                        GroupList[i] = GroupList[i + 1];
                        GroupList[i + 1] = storedGroup;
                    }
            }

            if (Group.Find(Server.defaultRank) != null) standard = Group.Find(Server.defaultRank);
            else standard = Group.findPerm(LevelPermission.Guest);

            foreach (Player pl in Player.players)
            {
                pl.group = GroupList.Find(g => g.name == pl.group.name);
            }
            if (OnGroupLoad != null)
                OnGroupLoad();
            OnGroupLoadEvent.Call();
            saveGroups(GroupList);
        }
        /// <summary>
        /// Save givenList group
        /// </summary>
        /// <param name="givenList">The list of groups to save</param>
        public static void saveGroups(List<Group> givenList)
        {
            File.Create("properties/ranks.properties").Dispose();
            using (StreamWriter SW = File.CreateText("properties/ranks.properties"))
            {
                SW.WriteLine("#Version 3");
                SW.WriteLine("#RankName = string");
                SW.WriteLine("#     The name of the rank, use capitalization.");
                SW.WriteLine("#");
                SW.WriteLine("#Permission = num");
                SW.WriteLine("#     The \"permission\" of the rank. It's a number.");
                SW.WriteLine("#		There are pre-defined permissions already set. (for the old ranks)");
                SW.WriteLine("#		Banned = -20, Guest = 0, Builder = 30, AdvBuilder = 50, Operator = 80");
                SW.WriteLine("#		SuperOP = 100, Nobody = 120");
                SW.WriteLine("#		Must be greater than -50 and less than 120");
                SW.WriteLine("#		The higher the number, the more commands do (such as undo allowing more seconds)");
                SW.WriteLine("#Limit = num");
                SW.WriteLine("#     The command limit for the rank (can be changed in-game with /limit)");
                SW.WriteLine("#		Must be greater than 0 and less than 10000000");
                SW.WriteLine("#MaxUndo = num");
                SW.WriteLine("#     The undo limit for the rank, only applies when undoing others.");
                SW.WriteLine("#		Must be greater than 0 and less than " + int.MaxValue);
                SW.WriteLine("#Color = char");
                SW.WriteLine("#     A single letter or number denoting the color of the rank");
                SW.WriteLine("#	    Possibilities:");
                SW.WriteLine("#		    0, 1, 2, 3, 4, 5, 6, 7, 8, 9, a, b, c, d, e, f");
                SW.WriteLine("#FileName = string.txt");
                SW.WriteLine("#     The file which players of this rank will be stored in");
                SW.WriteLine("#		It doesn't need to be a .txt file, but you may as well");
                SW.WriteLine("#		Generally a good idea to just use the same file name as the rank name");
                SW.WriteLine("#MOTD = string");
                SW.WriteLine("#		Alternate MOTD players of the rank will see when joining the server.");
                SW.WriteLine("#		Leave blank to use the server MOTD.");
                SW.WriteLine("#OSMaps = num");
                SW.WriteLine("#		The number of maps the players will have in /os");
                SW.WriteLine("#		Defaults to 2 if invalid number (number has to be between 0-128");
                SW.WriteLine();
                SW.WriteLine();
                foreach (Group grp in givenList)
                {
                    if (grp.name != "nobody")
                    {
                        SW.WriteLine("RankName = " + grp.trueName);
                        SW.WriteLine("Permission = " + (int)grp.Permission);
                        SW.WriteLine("Limit = " + grp.maxBlocks);
                        SW.WriteLine("MaxUndo = " + grp.maxUndo);
                        SW.WriteLine("Color = " + grp.color[1]);
                        SW.WriteLine("MOTD = " + grp.MOTD);
                        SW.WriteLine("FileName = " + grp.fileName);
                        //SW.WriteLine("OSMaps = " + grp.OverseerMaps.ToString());
                        SW.WriteLine();
                    }
                }
            }
            if (OnGroupSave != null)
                OnGroupSave();
            OnGroupSaveEvent.Call();
        }
        /// <summary>
        /// Check to see if group /name/ exists
        /// </summary>
        /// <param name="name">The name of the group to search for</param>
        /// <returns>Return true if it does exists, returns false if it doesnt</returns>
        public static bool Exists(string name)
        {
            name = name.ToLower();
            return GroupList.Any(gr => gr.name == name);
        }
        /// <summary>
        /// Find the group with the name /name/
        /// </summary>
        /// <param name="name">The name of the group</param>
        /// <returns>The group object with that name</returns>
        public static Group Find(string name)
        {
            name = name.ToLower();

            if (name == "adv") name = "advbuilder";
            if (name == "op") name = "operator";
            if (name == "super" || (name == "admin" && !Group.Exists("admin"))) name = "superop";
            if (name == "noone") name = "nobody";

            return GroupList.FirstOrDefault(gr => gr.name == name.ToLower());
        }
        /// <summary>
        /// Find the group with the permission /Perm/
        /// </summary>
        /// <param name="Perm">The level permission to search for</param>
        /// <returns>The group object with that level permission</returns>
        public static Group findPerm(LevelPermission Perm)
        {
            return GroupList.FirstOrDefault(grp => grp.Permission == Perm);
        }

        /// <summary>
        /// Find the group with the permission /Perm/
        /// </summary>
        /// <param name="Perm">The level permission to search for</param>
        /// <returns>The group object with that level permission</returns>
        public static Group findPermInt(int Perm)
        {
            return GroupList.FirstOrDefault(grp => (int)grp.Permission == Perm);
        }

        /// <summary>
        /// Get the group name that player /playerName/ is in
        /// </summary>
        /// <param name="playerName">The player Name</param>
        /// <returns>The group name</returns>
        public static string findPlayer(string playerName)
        {
            foreach (Group grp in Group.GroupList.Where(grp => grp.playerList.Contains(playerName)))
            {
                return grp.name;
            }
            return Group.standard.name;
        }

        /// <summary>
        /// Find the group object that the player /playerName/ is in
        /// </summary>
        /// <param name="playerName">The player name</param>
        /// <returns>The group object that the player is in</returns>
        public static Group findPlayerGroup(string playerName)
        {
            foreach (Group grp in Group.GroupList.Where(grp => grp.playerList.Contains(playerName)))
            {
                return grp;
            }
            return Group.standard;
        }

        public static string concatList(bool includeColor = true, bool skipExtra = false, bool permissions = false)
        {
            string returnString = "";
            foreach (Group grp in Group.GroupList.Where(grp => !skipExtra || (grp.Permission > LevelPermission.Guest && grp.Permission < LevelPermission.Nobody)))
            {
                if (includeColor)
                {
                    returnString += ", " + grp.color + grp.name + Server.DefaultColor;
                }
                else if (permissions)
                {
                    returnString += ", " + ((int)grp.Permission).ToString(CultureInfo.InvariantCulture);
                }
                else
                    returnString += ", " + grp.name;
            }

            if (includeColor) returnString = returnString.Remove(returnString.Length - 2);

            return returnString.Remove(0, 2);
        }
    }

    public class GrpCommands
    {
        public class rankAllowance
        {
            public string commandName;
            public LevelPermission lowestRank;
            public List<LevelPermission> disallow = new List<LevelPermission>();
            public List<LevelPermission> allow = new List<LevelPermission>();
        }
        public static List<rankAllowance> allowedCommands;
        public static List<string> foundCommands = new List<string>();

        public static LevelPermission defaultRanks(string command)
        {
            Command cmd = Command.all.Find(command);

            return cmd != null ? cmd.defaultRank : LevelPermission.Null;
        }

        public static void fillRanks()
        {
            foundCommands = Command.all.commandNames();
            allowedCommands = new List<rankAllowance>();

            rankAllowance allowVar;

            foreach (Command cmd in Command.all.All())
            {
                allowVar = new rankAllowance();
                allowVar.commandName = cmd.name;
                allowVar.lowestRank = cmd.defaultRank;
                allowedCommands.Add(allowVar);
            }

            if (File.Exists("properties/command.properties"))
            {
                string[] lines = File.ReadAllLines("properties/command.properties");

                //if (lines.Length == 0) ; // this is useless?
                /*else */
                if (lines[0] == "#Version 2")
                {
                    string[] colon = new[] { " : " };
                    foreach (string line in lines)
                    {
                        allowVar = new rankAllowance();
                        if (line == "" || line[0] == '#') continue;
                        //Name : Lowest : Disallow : Allow
                        string[] command = line.Split(colon, StringSplitOptions.None);

                        if (!foundCommands.Contains(command[0]))
                        {
                            Server.s.Log("Incorrect command name: " + command[0]);
                            continue;
                        }
                        allowVar.commandName = command[0];

                        string[] disallow = new string[0];
                        if (command[2] != "")
                            disallow = command[2].Split(',');
                        string[] allow = new string[0];
                        if (command[3] != "")
                            allow = command[3].Split(',');

                        try
                        {
                            allowVar.lowestRank = (LevelPermission)int.Parse(command[1]);
                            foreach (string s in disallow) { allowVar.disallow.Add((LevelPermission)int.Parse(s)); }
                            foreach (string s in allow) { allowVar.allow.Add((LevelPermission)int.Parse(s)); }
                        }
                        catch
                        {
                            Server.s.Log("Hit an error on the command " + line);
                            continue;
                        }

                        int current = 0;
                        foreach (rankAllowance aV in allowedCommands)
                        {
                            if (command[0] == aV.commandName)
                            {
                                allowedCommands[current] = allowVar;
                                break;
                            }
                            current++;
                        }
                    }
                }
                else
                {
                    foreach (string line in lines.Where(line => line != "" && line[0] != '#'))
                    {
                        allowVar = new rankAllowance();
                        string key = line.Split('=')[0].Trim().ToLower();
                        string value = line.Split('=')[1].Trim().ToLower();

                        if (!foundCommands.Contains(key))
                        {
                            Server.s.Log("Incorrect command name: " + key);
                        }
                        else if (Level.PermissionFromName(value) == LevelPermission.Null)
                        {
                            Server.s.Log("Incorrect value given for " + key + ", using default value.");
                        }
                        else
                        {
                            allowVar.commandName = key;
                            allowVar.lowestRank = Level.PermissionFromName(value);

                            int current = 0;
                            foreach (rankAllowance aV in allowedCommands)
                            {
                                if (key == aV.commandName)
                                {
                                    allowedCommands[current] = allowVar;
                                    break;
                                }
                                current++;
                            }
                        }
                    }
                }
                Save(allowedCommands);
            }
            else Save(allowedCommands);

            foreach (Group grp in Group.GroupList)
            {
                grp.fillCommands();
            }
        }

        public static void Save(List<rankAllowance> givenList)
        {
            try
            {
                File.Create("properties/command.properties").Dispose();
                using (StreamWriter w = File.CreateText("properties/command.properties"))
                {
                    w.WriteLine("#Version 2");
                    w.WriteLine("#   This file contains a reference to every command found in the server software");
                    w.WriteLine("#   Use this file to specify which ranks get which commands");
                    w.WriteLine("#   Current ranks: " + Group.concatList(false, false, true));
                    w.WriteLine("#   Disallow and allow can be left empty, just make sure there's 2 spaces between the colons");
                    w.WriteLine("#   This works entirely on permission values, not names. Do not enter a rank name. Use it's permission value");
                    w.WriteLine("#   CommandName : LowestRank : Disallow : Allow");
                    w.WriteLine("#   gun : 60 : 80,67 : 40,41,55");
                    w.WriteLine("");
                    foreach (rankAllowance aV in givenList)
                    {
                        w.WriteLine(aV.commandName + " : " + (int)aV.lowestRank + " : " + getInts(aV.disallow) + " : " + getInts(aV.allow));
                    }
                }
            }
            catch
            {
                Server.s.Log("SAVE FAILED! command.properties");
            }
        }
        public static string getInts(List<LevelPermission> givenList)
        {
            string returnString = ""; bool foundOne = false;
            foreach (LevelPermission Perm in givenList)
            {
                foundOne = true;
                returnString += "," + (int)Perm;
            }
            if (foundOne) returnString = returnString.Remove(0, 1);
            return returnString;
        }
        public static void AddCommands(out CommandList commands, LevelPermission perm)
        {
            commands = new CommandList();

            foreach (rankAllowance aV in allowedCommands.Where(aV => (aV.lowestRank <= perm && !aV.disallow.Contains(perm)) || aV.allow.Contains(perm)))
                commands.Add(Command.all.Find(aV.commandName));
        }
    }
}
