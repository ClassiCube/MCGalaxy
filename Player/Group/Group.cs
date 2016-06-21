/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
        public string ColoredName { get { return color + trueName; } }
        public byte OverseerMaps = 3;
        public string prefix = "";
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
        public Group(LevelPermission Perm, int maxB, long maxUn, string fullName, char newColor, string motd, string file, byte maps = 3, string prefix = "")
        {
            Permission = Perm;
            maxBlocks = maxB;
            maxUndo = maxUn;
            trueName = fullName;
            name = trueName.ToLower();
            color = "&" + newColor;
            MOTD = motd;
            fileName = file;
            OverseerMaps = maps;
            this.prefix = prefix;
            playerList = name != "nobody" ? PlayerList.Load(fileName) : new PlayerList();
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
        
        public bool CanExecute(string cmdName) { 
            return commands.Contains(Command.all.Find(cmdName));
        }
        	
        /// <summary> Check to see if this group can excute cmd </summary>
        /// <param name="cmd">The command object to check</param>
        /// <returns>True if this group can use it, false if they cant</returns>
        public bool CanExecute(Command cmd) { return commands.Contains(cmd); }

        public static List<Group> GroupList = new List<Group>();
        public static Group standard;
        
        /// <summary> Load up all server groups </summary>
        public static void InitAll() {
            GroupList = new List<Group>();

            if (File.Exists("properties/ranks.properties"))
                GroupProperties.InitAll();

            if (findPerm(LevelPermission.Banned) == null) GroupList.Add(new Group(LevelPermission.Banned, 1, 1, "Banned", '8', String.Empty, "banned.txt"));
            if (findPerm(LevelPermission.Guest) == null) GroupList.Add(new Group(LevelPermission.Guest, 1, 120, "Guest", '7', String.Empty, "guest.txt"));
            if (findPerm(LevelPermission.Builder) == null) GroupList.Add(new Group(LevelPermission.Builder, 400, 300, "Builder", '2', String.Empty, "builders.txt"));
            if (findPerm(LevelPermission.AdvBuilder) == null) GroupList.Add(new Group(LevelPermission.AdvBuilder, 1200, 900, "AdvBuilder", '3', String.Empty, "advbuilders.txt"));
            if (findPerm(LevelPermission.Operator) == null) GroupList.Add(new Group(LevelPermission.Operator, 2500, 5400, "Operator", 'c', String.Empty, "operators.txt"));
            if (findPerm(LevelPermission.Admin) == null) GroupList.Add(new Group(LevelPermission.Admin, 65536, int.MaxValue, "SuperOP", 'e', String.Empty, "uberOps.txt"));
            GroupList.Add(new Group(LevelPermission.Nobody, 65536, -1, "Nobody", '0', String.Empty, "nobody.txt"));            
            GroupList.Sort((a, b) => a.Permission.CompareTo(b.Permission));

            if (Group.Find(Server.defaultRank) != null) standard = Group.Find(Server.defaultRank);
            else standard = Group.findPerm(LevelPermission.Guest);

            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players)
                pl.group = GroupList.Find(g => g.name == pl.group.name);
            if (OnGroupLoad != null)
                OnGroupLoad();
            OnGroupLoadEvent.Call();
            saveGroups(GroupList);
        }
        
        /// <summary> Save givenList group </summary>
        /// <param name="givenList">The list of groups to save</param>
        public static void saveGroups(List<Group> givenList) {
            GroupProperties.SaveGroups(givenList);
            if (OnGroupSave != null)
                OnGroupSave();
            OnGroupSaveEvent.Call();
        }
        
        /// <summary> Check whether a group with that name exists. </summary>
        public static bool Exists(string name) {
            name = name.ToLower();
            return GroupList.Any(gr => gr.name == name);
        }
        
        /// <summary> Find the group which has the given name. </summary>
        public static Group Find(string name) {
            name = name.ToLower();
            MapName(ref name);
            return GroupList.Find(gr => gr.name == name);
        }
        
        /// <summary> Find the group(s) which contain the given name. </summary>
        public static Group FindMatches(Player p, string name, out int matches) {
            name = name.ToLower();
            MapName(ref name);
            return Extensions.FindMatches<Group>(p, name, out matches, 
                                                 GroupList, g => true, g => g.name, "ranks");
        }
        
        /// <summary> Find the group(s) which contain the given name. </summary>
        public static Group FindOrShowMatches(Player p, string name) {
        	int matches = 0; return FindMatches(p, name, out matches);
        }
        
        static void MapName(ref string name) {
        	if (name == "adv") name = "advbuilder";
            if (name == "op") name = "operator";
            if (name == "super" || (name == "admin" && !Group.Exists("admin"))) name = "superop";
            if (name == "noone") name = "nobody";
        }
        
        /// <summary> Finds the group with has the given permission level. </summary>
        public static Group findPerm(LevelPermission Perm) {
            return GroupList.Find(grp => grp.Permission == Perm);
        }

        /// <summary> Find the group which has the given permission level. </summary>
        public static Group findPermInt(int Perm) {
            return GroupList.Find(grp => (int)grp.Permission == Perm);
        }

        /// <summary> Get the group name that player /playerName/ is in </summary>
        /// <param name="playerName">The player Name</param>
        /// <returns>The group name</returns>
        public static string findPlayer(string playerName) {
            foreach (Group grp in Group.GroupList.Where(grp => grp.playerList.Contains(playerName))) {
                return grp.name;
            }
            return Group.standard.name;
        }

        /// <summary> Find the group object that the player /playerName/ is in </summary>
        /// <param name="playerName">The player name</param>
        /// <returns>The group object that the player is in</returns>
        public static Group findPlayerGroup(string playerName) {
            foreach (Group grp in Group.GroupList.Where(grp => grp.playerList.Contains(playerName))) {
                return grp;
            }
            return Group.standard;
        }

        public static string concatList(bool includeColor = true, bool skipExtra = false, bool permissions = false) {
            string returnString = "";
            foreach (Group grp in Group.GroupList.Where(grp => !skipExtra || (grp.Permission > LevelPermission.Guest && grp.Permission < LevelPermission.Nobody)))
            {
                if (includeColor)
                    returnString += ", " + grp.color + grp.name + Server.DefaultColor;
                else if (permissions)
                    returnString += ", " + ((int)grp.Permission).ToString(CultureInfo.InvariantCulture);
                else
                    returnString += ", " + grp.name;
            }

            if (includeColor) returnString = returnString.Remove(returnString.Length - 2);

            return returnString.Remove(0, 2);
        }
        
        /// <summary> Returns whether the given player is in the banned rank. </summary>
        public static bool IsBanned(string name) {
            Group grp = findPerm(LevelPermission.Banned);
            return grp != null && grp.playerList.Contains(name);
        }
    }
}
