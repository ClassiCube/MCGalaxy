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
using System.IO;
using MCGalaxy.Events;

namespace MCGalaxy {
    /// <summary> This is the group object, where ranks and their data are stored </summary>
    public sealed partial class Group {
        
        public delegate void GroupSave();
        [Obsolete("Please use OnGroupSaveEvent.Register()")]
        public static event GroupSave OnGroupSave;
        public delegate void GroupLoad();
        [Obsolete("Please use OnGroupLoadEvent.Register()")]
        public static event GroupLoad OnGroupLoad;
        public delegate void GroupLoaded(Group mGroup);
        [Obsolete("Please use OnGroupLoadedEvent.Register()")]
        public static event GroupLoaded OnGroupLoaded;
        
        public static Group BannedRank { get { return Find(LevelPermission.Banned); } }
        public static Group GuestRank { get { return Find(LevelPermission.Guest); } }
        public static Group NobodyRank { get { return Find(LevelPermission.Nobody); } }
        public static Group standard;
        
        public static void Register(Group grp) {
            GroupList.Add(grp);
            grp.LoadPlayers();
            
            if (OnGroupLoaded != null)
                OnGroupLoaded(grp);
            OnGroupLoadedEvent.Call(grp);
        }

        public static List<Group> GroupList = new List<Group>();
        static readonly object saveLock = new object();
        
        /// <summary> Load all server groups </summary>
        public static void InitAll() {
            GroupList = new List<Group>();
            if (File.Exists(Paths.RankPropsFile)) {
                GroupProperties.InitAll();
            } else {
                // Add some default ranks
                Register(new Group(LevelPermission.Builder, 400, 300, "Builder", '2', mapGenLimit));
                Register(new Group(LevelPermission.AdvBuilder, 1200, 900, "AdvBuilder", '3', mapGenLimit));
                Register(new Group(LevelPermission.Operator, 2500, 5400, "Operator", 'c', mapGenLimit));
                Register(new Group(LevelPermission.Admin, 65536, int.MaxValue, "SuperOP", 'e', mapGenLimitAdmin));
            }

            if (BannedRank == null)
                Register(new Group(LevelPermission.Banned, 1, 1, "Banned", '8', mapGenLimit));
            if (GuestRank == null)
                Register(new Group(LevelPermission.Guest, 1, 120, "Guest", '7', mapGenLimit));
            if (NobodyRank == null)
                Register(new Group(LevelPermission.Nobody, 65536, -1, "Nobody", '0', mapGenLimitAdmin));
            
            GroupList.Sort((a, b) => a.Permission.CompareTo(b.Permission));
            standard = Find(ServerConfig.DefaultRankName);
            if (standard == null) standard = GuestRank;

            if (OnGroupLoad != null) OnGroupLoad();
            OnGroupLoadEvent.Call();
            SaveList(GroupList);
        }
        
        /// <summary> Save givenList group </summary>
        /// <param name="givenList">The list of groups to save</param>
        public static void SaveList(List<Group> givenList) {
            lock (saveLock)
                GroupProperties.SaveGroups(givenList);
            
            if (OnGroupSave != null) OnGroupSave();
            OnGroupSaveEvent.Call();
        }
        
        
        /// <summary> Find the group which has the given name. </summary>
        public static Group Find(string name) {
            MapName(ref name);
            
            const StringComparison comp = StringComparison.OrdinalIgnoreCase;
            foreach (Group grp in GroupList) {
                if (grp.Name.Equals(name, comp)) return grp;
            }
            return null;
        }
        
        internal static void MapName(ref string name) {
            if (name.CaselessEq("op")) name = "operator";
        }
        
        /// <summary> Finds the group which has the given permission level. </summary>
        public static Group Find(LevelPermission perm) {
            return GroupList.Find(grp => grp.Permission == perm);
        }

        /// <summary> Find the group which has the given permission level. </summary>
        public static Group Find(int perm) {
            return GroupList.Find(grp => (int)grp.Permission == perm);
        }

        /// <summary> Find the group that the given player is a member of. </summary>
        /// <param name="playerName"> The player name. </param>
        public static Group GroupIn(string playerName) {
            foreach (Group grp in GroupList) {
                if (grp.Players.Contains(playerName)) return grp;
            }
            return standard;
        }

        
        public static string GetColoredName(LevelPermission perm) {
            Group grp = Find(perm);
            if (grp != null) return grp.ColoredName;
            return Colors.white + ((int)perm);
        }
        
        public static string GetColoredName(string rankName) {
            Group grp = Find(rankName);
            if (grp != null) return grp.ColoredName;
            return Colors.white + rankName;
        }
        
        public static string GetColor(LevelPermission perm) {
            Group grp = Find(perm);
            if (grp != null) return grp.Color;
            return Colors.white;
        }
    }
}
