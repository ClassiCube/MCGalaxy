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
using System.Text;

namespace MCGalaxy {

    /// <summary> Represents which ranks are allowed (and which are disallowed) to use an item. </summary>
    public class ItemPerms {
        public virtual string ItemName { get { return ""; } }
        public LevelPermission MinRank;
        public List<LevelPermission> Allowed, Disallowed;
        
        public ItemPerms(LevelPermission min, List<LevelPermission> allowed,
                         List<LevelPermission> disallowed) {
            Init(min, allowed, disallowed);
        }
        
        protected void Init(LevelPermission min, List<LevelPermission> allowed,
                            List<LevelPermission> disallowed) {
            MinRank = min; Allowed = allowed; Disallowed = disallowed;
        }
        
        protected void CopyTo(ItemPerms perms) {
            perms.MinRank    = MinRank;
            perms.Allowed    = Allowed    == null ? null : new List<LevelPermission>(Allowed);
            perms.Disallowed = Disallowed == null ? null : new List<LevelPermission>(Disallowed);
        }
        
        public bool UsableBy(LevelPermission perm) {
            return (perm >= MinRank || (Allowed != null && Allowed.Contains(perm)))
                && (Disallowed == null || !Disallowed.Contains(perm));
        }
        
        public void Describe(StringBuilder builder) {
            builder.Append(Group.GetColoredName(MinRank) + "%S+");
            
            if (Allowed != null && Allowed.Count > 0) {
                foreach (LevelPermission perm in Allowed) {
                    builder.Append(", " + Group.GetColoredName(perm));
                }
                builder.Append("%S");
            }
            
            if (Disallowed != null && Disallowed.Count > 0) {
                builder.Append( " (except ");
                foreach (LevelPermission perm in Disallowed) {
                    builder.Append(Group.GetColoredName(perm) + ", ");
                }
                builder.Remove(builder.Length - 2, 2);
                builder.Append("%S)");
            }
        }
        
        public string Describe() {
            StringBuilder builder = new StringBuilder();
            Describe(builder);
            return builder.ToString();
        }
        
        
        protected static void WriteHeader(StreamWriter w, string type, string name, string example) {
            w.WriteLine("#Version 2");
            w.WriteLine("#   This file list the ranks that can use " + type);
            w.WriteLine("#   Disallow and allow can be left empty");
            w.WriteLine("#   Works entirely on rank permission values, not rank names");
            w.WriteLine("#");
            w.WriteLine("#   Layout: " + name + " : LowestRank : Disallow : Allow");
            w.WriteLine("#   " + example + " : 60 : 80,67 : 40,41,55");
            w.WriteLine("");
        }
        
        protected string Serialise() {
            return ItemName + " : " + (int)MinRank + " : "
                + JoinPerms(Disallowed) + " : " + JoinPerms(Allowed);
        }
        
        static string JoinPerms(List<LevelPermission> list) {
            if (list == null || list.Count == 0) return "";
            return list.Join(p => ((int)p).ToString(), ",");
        }
        
        protected static void Deserialise(string[] args, int idx, out LevelPermission min,
                                          out List<LevelPermission> allowed, 
                                          out List<LevelPermission> disallowed) {
            min = (LevelPermission)int.Parse(args[idx]);
            disallowed = ExpandPerms(args[idx + 1]);
            allowed = ExpandPerms(args[idx + 2]);
        }
        
        static List<LevelPermission> ExpandPerms(string input) {
            if (input == null || input.Length == 0) return null;
            
            List<LevelPermission> perms = new List<LevelPermission>();
            foreach (string perm in input.SplitComma()) {
                perms.Add((LevelPermission)int.Parse(perm));
            }
            return perms;
        }
    }
}
