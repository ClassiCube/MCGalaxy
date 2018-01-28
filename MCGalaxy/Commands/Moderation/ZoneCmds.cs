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
using MCGalaxy.Commands.World;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Moderation {
    public sealed class CmdZone : Command {
        public override string name { get { return "Zone"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (message.Length == 0) { Help(p); return; }
            
            if (args[0].CaselessEq("add")) {
                if (args.Length == 1) { Help(p); return; }
                CreateZone(p, args, 1);
            } else if (args[0].CaselessEq("del")) {
                Player.Message(p, "Place a block where you would like to delete a zone.");
                p.MakeSelection(1, null, DeleteZone);
            } else {
                CreateZone(p, args, 0);
            }
        }
        
        void CreateZone(Player p, string[] args, int offset) {
            Zone z = new Zone(p.level); 
            z.Config.Name = args[offset];
            PermissionCmd.Do(p, args, offset + 1, false, z.Access);

            Player.Message(p, "Creating zone " + z.ColoredName);
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, z, AddZone);
        }
        
        bool AddZone(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Zone z = (Zone)state;
            z.MinX = (ushort)Math.Min(marks[0].X, marks[1].X);
            z.MinY = (ushort)Math.Min(marks[0].Y, marks[1].Y);
            z.MinZ = (ushort)Math.Min(marks[0].Z, marks[1].Z);
            z.MaxX = (ushort)Math.Max(marks[0].X, marks[1].X);
            z.MaxY = (ushort)Math.Max(marks[0].Y, marks[1].Y);
            z.MaxZ = (ushort)Math.Max(marks[0].Z, marks[1].Z);

            p.level.Zones.Add(z);
            p.level.Save(true);
            Player.Message(p, "Created zone " + z.ColoredName);
            return false;
        }

        bool DeleteZone(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Level lvl = p.level;
            bool foundDel = false;
            Vec3S32 P = marks[0];
            
            for (int i = 0; i < lvl.Zones.Count; i++) {
                Zone zn = lvl.Zones[i];
                if (P.X < zn.MinX || P.X > zn.MaxX || P.Y < zn.MinY || P.Y > zn.MaxY || P.Z < zn.MinZ || P.Z > zn.MaxZ) continue;
                
                if (!zn.Access.CheckDetailed(p)) {
                    Player.Message(p, "Hence, you cannot delete this zone.");
                    continue;
                }
                
                lvl.Zones.RemoveAt(i); i--;
                Player.Message(p, "Zone " + zn.ColoredName + " %sdeleted");
                foundDel = true;
            }
            
            if (!foundDel) Player.Message(p, "No zones found to delete.");
            return false;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Zone add [name] %H- Creates a zone only [name] can build in");
            Player.Message(p, "%T/Zone add [rank] %H- Creates a zone only [rank]+ can build in");
            Player.Message(p, "%T/Zone del %H- Deletes the zone clicked");
        }
    }
    
    public sealed class CmdZoneTest : Command {
        public override string name { get { return "ZoneTest"; } }
        public override string shortcut { get { return "ZTest"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }

        public override void Use(Player p, string message) {
            Player.Message(p, "Place or delete a block where you would like to check for zones.");
            p.MakeSelection(1, null, TestZone);
        }
        
        bool TestZone(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Vec3S32 P = marks[0];
            Level lvl = p.level;
            bool found = false;

            for (int i = 0; i < lvl.Zones.Count; i++) {
                Zone z = lvl.Zones[i];
                if (!z.Contains(P.X, P.Y, P.Z)) continue;
                found = true;
                
                AccessResult status = z.Access.Check(p);
                bool allowed = status == AccessResult.Allowed || status == AccessResult.Whitelisted;
                Player.Message(p, "  Zone {0} %S- {1}{2}", z.ColoredName, allowed ? "&a" : "&c", status );
            }
            
            if (!found) { Player.Message(p, "No zones affect this block."); }
            return true;
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ZoneTest %H- Lists all zones affecting a block");
        }
    }
    
    public sealed class CmdZoneList : Command {
        public override string name { get { return "ZoneList"; } }
        public override string shortcut { get { return "Zones"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }

        public override void Use(Player p, string message) {
            MultiPageOutput.Output(p, p.level.Zones, FormatZone, "ZoneList", "zones", message, true);
        }
        
        static string FormatZone(Zone zone) {
            return zone.ColoredName
                + " &b- (" + zone.MinX + ", " + zone.MinY + ", " + zone.MinZ
                + ") to (" + zone.MaxX + ", " + zone.MaxY + ", " + zone.MaxZ + ")";
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ZoneList %H- Lists all zones in current level");
        }
    }
}
