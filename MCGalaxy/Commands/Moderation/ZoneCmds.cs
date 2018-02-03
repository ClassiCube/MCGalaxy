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
using MCGalaxy.Commands.Building;
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
            string opt = args[0];
            
            if (opt.CaselessEq("add")) {
                if (args.Length == 1) { Help(p); return; }
                CreateZone(p, args, 1);
            } else if (opt.CaselessEq("del")) {
                if (args.Length == 1) { Help(p); return; }
                DeleteZone(p, args);
            } else if (opt.CaselessEq("edit") || opt.CaselessEq("set")) {
                if (args.Length <= 2) { Help(p); return; }
                Zone zone = Matcher.FindZones(p, p.level, args[1]);
                if (zone == null) return;
                
                if (!zone.Access.CheckDetailed(p)) {
                    Player.Message(p, "Hence, you cannot edit this zone."); return;
                } else if (opt.CaselessEq("edit")) {
                    EditZone(p, args, zone);
                } else {
                    SetZoneProp(p, args, zone);
                }
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
            
            zone.RemoveFrom(lvl);
            Player.Message(p, "Zone " + zone.ColoredName + " %Sdeleted");
            lvl.Save(true);
        }
        
        void EditZone(Player p, string[] args, Zone zone) {
            if (!PermissionCmd.Do(p, args, 2, false, zone.Access)) return;
            p.level.Save(true);
        }
        
        void SetZoneProp(Player p, string[] args, Zone zone) {
            ColorDesc desc = default(ColorDesc);
            string opt = args[2];
            
            if (opt.CaselessEq("col")) {
                if (!CommandParser.GetHex(p, args[3], ref desc)) return;
                
                zone.Config.ShowColor = args[3];
                zone.ShowAll(p.level);
            } else if (opt.CaselessEq("alpha")) {
                byte alpha = 0;
                if (!CommandParser.GetByte(p, args[3], "Alpha", ref alpha)) return;
                
                zone.UnshowAll(p.level);
                zone.Config.ShowAlpha = alpha;
                zone.ShowAll(p.level);
            } else {
                Player.Message(p, "?????");
                return;
            }
            p.level.Save(true);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Zone add [name] <permissions>");
            Player.Message(p, "%HCreates a new zone, optionally also sets build permissions");
            Player.Message(p, "%T/Zone del [name]");
            Player.Message(p, "%HDeletes the given zone");
            Player.Message(p, "%T/Zone edit [name] [permissions]");
            Player.Message(p, "%HSets build permissions for the given zone");
            Player.Message(p, "%H  For syntax of permissions, see %T/Help PerBuild");
            Player.Message(p, "%T/Zone rename [old name] [new name]");
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
    
    public sealed class CmdZoneMark : Command {
        public override string name { get { return "ZoneMark"; } }
        public override string shortcut { get { return "ZMark"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("zm") }; }
        }
        
        public override void Use(Player p, string message) {
            if (message.Length == 0) { Help(p); return; }
            
            Zone z = Matcher.FindZones(p, p.level, message);
            if (z == null) return;
            
            if (!CmdMark.DoMark(p, z.MinX, z.MinY, z.MinZ)) {
                Player.Message(p, "Cannot mark, no selection in progress.");
            } else {
                CmdMark.DoMark(p, z.MaxX, z.MaxY, z.MaxZ);
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ZoneMark [name]");
            Player.Message(p, "%HUses corners of the given zone as a %T/Mark %Hfor selections");
        }
    }
}
