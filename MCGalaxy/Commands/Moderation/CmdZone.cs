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
using MCGalaxy.Commands.Moderation;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdZone : Command {
        public override string name { get { return "zone"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can delete zones"),
                    new CommandPerm(LevelPermission.Operator, "+ can delete all zones"),
                    new CommandPerm(LevelPermission.Operator, "+ can create zones"),
                }; }
        }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ozone", "map"), new CommandAlias("oz", "map") }; }
        }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (message == "") {
                Player.Message(p, "Place a block where you would like to check for zones.");
                p.MakeSelection(1, null, CheckZone);
            } else if (args[0].CaselessEq("add")) {
                if (!CheckAdd(p, args, "zone add")) return;

                Player.Message(p, "Place or break two blocks to determine the edges.");
                Player.Message(p, "Zone for: &b" + args[1] + ".");
                p.MakeSelection(2, args[1], AddZone);
            } else if (args[0].CaselessEq("map")) {
                if (!CheckAdd(p, args, "zone map")) return;

                ZoneAll(p.level, args[1]);
                Player.Message(p, "Added zone for &b" + args[1]);
            } else if (args[0].CaselessEq("del") && args.Length > 1 && args[1].CaselessEq("all")) {
                if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
                DeleteAll(p);
            } else if (args[0].CaselessEq("del")) {
                if (!CheckExtraPerm(p, 1)) { MessageNeedExtra(p, 1); return; }
                
                if (p.canBuild) { //Checks if player can build there
                    Player.Message(p, "Place a block where you would like to delete a zone.");
                    p.MakeSelection(1, null, DeleteZone);
                } else { //if they cant, it warns them, the ops and logs it on the server!
                    Player.Message(p, "You can't delete a zone which is above your rank!");
                    Chat.MessageOps(p.name + " tried to delete a zone that is above their rank!");
                    Logger.Log(LogType.SuspiciousActivity, "{0} tried to delete a zone that is above their rank!", p.name);
                }
            } else {
                Help(p);
            }
        }
        
        internal static void ZoneAll(Level lvl, string owner) {
            Level.Zone zn = default(Level.Zone);
            zn.bigX = (ushort)(lvl.Width - 1);
            zn.bigY = (ushort)(lvl.Height - 1);
            zn.bigZ = (ushort)(lvl.Length - 1);
            zn.Owner = owner;
            
            lvl.ZoneList.Add(zn);
            LevelDB.CreateZone(lvl.name, zn);
        }
        
        internal static void DeleteAll(Player p) {
            DeleteWhere(p, zone => true);
        }

        internal static void DeleteWhere(Player p, Predicate<Level.Zone> filter) {
            int count = p.level.ZoneList.Count, removed = 0;
            for (int i = count - 1; i >= 0; i--) {
                Level.Zone zone = p.level.ZoneList[i];
                if (!filter(zone)) continue;
                LevelDB.DeleteZone(p.level.name, zone);
                
                removed++;
                Player.Message(p, "Zone deleted for &b" + zone.Owner);
                p.level.ZoneList.Remove(p.level.ZoneList[i]);
            }
            Player.Message(p, "Removed {0} zone{1}.", removed, count == 1 ? "s" : "");
        }
        
        bool CheckAdd(Player p, string[] args, string cmd) {
            if (!CheckExtraPerm(p, 3)) { MessageNeedExtra(p, 3); return false; }
            if (args.Length == 1) { Help(p); return false; }
            if (!Formatter.ValidName(p, args[1], "player or rank")) return false;
            
            string reason = args.Length > 2 ? args[2] : "";
            args[1] = FindZoneOwner(p, cmd, args[1], ref reason);
            return args[1] != null;
        }
        
        bool CheckZone(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Vec3S32 P = marks[0];
            string zoneMsg = p.level.FindZoneOwners(p, (ushort)P.X, (ushort)P.Y, (ushort)P.Z);
            Player.Message(p, zoneMsg);
            return true;
        }

        bool DeleteZone(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Level lvl = p.level;
            bool foundDel = false;
            Vec3S32 P = marks[0];
            
            for (int i = 0; i < lvl.ZoneList.Count; i++) {
                Level.Zone zn = lvl.ZoneList[i];
                if (P.X < zn.smallX || P.X > zn.bigX || P.Y < zn.smallY || P.Y > zn.bigY || P.Z < zn.smallZ || P.Z > zn.bigZ)
                    continue;
                
                if (zn.Owner.Length >= 3 && zn.Owner.StartsWith("grp")) {
                    Group grp = Group.Find(zn.Owner.Substring(3));
                    if (grp != null && p.Rank < grp.Permission) continue;
                } else if (zn.Owner != "" && !zn.Owner.CaselessEq(p.name)) {
                    Group group = Group.findPlayerGroup(zn.Owner);
                    if (p.Rank < group.Permission) continue;
                }
                
                LevelDB.DeleteZone(lvl.name, zn);
                lvl.ZoneList.RemoveAt(i); i--;
                Player.Message(p, "Zone deleted for &b" + zn.Owner);
                foundDel = true;
            }
            
            if (!foundDel) Player.Message(p, "No zones found to delete.");
            return false;
        }
        
        bool AddZone(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Level.Zone Zn;
            Zn.smallX = (ushort)Math.Min(marks[0].X, marks[1].X);
            Zn.smallY = (ushort)Math.Min(marks[0].Y, marks[1].Y);
            Zn.smallZ = (ushort)Math.Min(marks[0].Z, marks[1].Z);
            Zn.bigX = (ushort)Math.Max(marks[0].X, marks[1].X);
            Zn.bigY = (ushort)Math.Max(marks[0].Y, marks[1].Y);
            Zn.bigZ = (ushort)Math.Max(marks[0].Z, marks[1].Z);
            Zn.Owner = (string)state;

            p.level.ZoneList.Add(Zn);
            LevelDB.CreateZone(p.level.name, Zn);
            Player.Message(p, "Added zone for &b" + (string)state);
            return false;
        }
        
        internal static string FindZoneOwner(Player p, string cmd, string name, ref string reason) {
            if (Group.Find(name) != null)
                return "grp" + Group.Find(name).name;
            return ModActionCmd.FindName(p, "zone", cmd, "", name, ref reason);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/zone add [name] %H- Creates a zone only [name] can build in");
            Player.Message(p, "%T/zone add [rank] %H- Creates a zone only [rank]+ can build in");
            Player.Message(p, "%T/zone map [name/rank] %H- /zone add across the entire map");       
            Player.Message(p, "%T/zone del %H- Deletes the zone clicked");
        }
    }
}
