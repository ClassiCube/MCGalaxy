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
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ZRemove", "del"), new CommandAlias("ZDelete", "del"),
                    new CommandAlias("ZAdd"), new CommandAlias("ZEdit", "edit") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] args = message.SplitSpaces();
            if (message.Length == 0) { Help(p); return; }
            
            if (args[0].CaselessEq("add")) {
                if (args.Length == 1) { Help(p); return; }
                CreateZone(p, args, 1);
            } else if (args[0].CaselessEq("del")) {
                if (args.Length == 1) { Help(p); return; }
                DeleteZone(p, args);
            } else if (args[0].CaselessEq("edit")) {
                if (args.Length < 3) { Help(p); return; }
                EditZone(p, args);
            } else {
                CreateZone(p, args, 0);
            }
        }
        
        void CreateZone(Player p, string[] args, int offset) {
            if (p.level.FindZoneExact(args[offset]) != null) {
                Player.Message(p, "A zone with that name already exists. Use %T/zedit %Sto change it.");
                return;
            }
            
            Zone z = new Zone(p.level);
            z.Config.Name = args[offset];
            if (!PermissionCmd.Do(p, args, offset + 1, false, z.Access)) return;

            Player.Message(p, "Creating zone " + z.ColoredName);
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, z, AddZone);
        }
        
        bool AddZone(Player p, Vec3S32[] marks, object state, ExtBlock block) {
            Zone zone = (Zone)state;
            zone.MinX = (ushort)Math.Min(marks[0].X, marks[1].X);
            zone.MinY = (ushort)Math.Min(marks[0].Y, marks[1].Y);
            zone.MinZ = (ushort)Math.Min(marks[0].Z, marks[1].Z);
            zone.MaxX = (ushort)Math.Max(marks[0].X, marks[1].X);
            zone.MaxY = (ushort)Math.Max(marks[0].Y, marks[1].Y);
            zone.MaxZ = (ushort)Math.Max(marks[0].Z, marks[1].Z);

            zone.AddTo(p.level);
            p.level.Save(true);
            Player.Message(p, "Created zone " + zone.ColoredName);
            return false;
        }
        
        void DeleteZone(Player p, string[] args) {
            Level lvl = p.level;
            Zone zone = Matcher.FindZones(p, lvl, args[1]);
            if (zone == null) return;
            if (!zone.Access.CheckDetailed(p)) {
                Player.Message(p, "Hence, you cannot delete this zone."); return;
            }
            
            lvl.Zones.Remove(zone);
            Player.Message(p, "Zone " + zone.ColoredName + " %Sdeleted");
            lvl.Save();
        }
        
        void EditZone(Player p, string[] args) {
            Level lvl = p.level;
            Zone zone = Matcher.FindZones(p, lvl, args[1]);
            if (zone == null) return;
            if (!zone.Access.CheckDetailed(p)) {
                Player.Message(p, "Hence, you cannot edit this zone."); return;
            }
            
            if (args[2].CaselessEq("col")) {
                ColorDesc desc = default(ColorDesc);
                if (!CommandParser.GetHex(p, args[3], ref desc)) return;
                
                zone.Config.ShowColor = args[3];
                zone.ShowAll(lvl);
            } else if (args[2].CaselessEq("alpha")) {
                if (!CommandParser.GetByte(p, args[3], "Alpha", ref zone.Config.ShowAlpha)) return;
                zone.ShowAll(lvl);
            } else if (!PermissionCmd.Do(p, args, 2, false, zone.Access)) {
                return;
            }
            lvl.Save(true);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Zone add [name] %H- Creates a new zone");
            Player.Message(p, "%T/Zone del [name] %H- Deletes the given zone");
            Player.Message(p, "%T/Zone edit [name] [args] %H- Edits/Updates the given zone");
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

            Zone[] zones = lvl.Zones.Items;
            for (int i = 0; i < zones.Length; i++) {
                Zone z = zones[i];
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
            Zone[] zones = p.level.Zones.Items;
            MultiPageOutput.Output(p, zones, FormatZone, "ZoneList", "zones", message, true);
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
