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
using MCGalaxy.Commands.CPE;
using MCGalaxy.Commands.World;
using MCGalaxy.Maths;
using BlockID = System.UInt16;

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
            string[] args = message.SplitSpaces(4);
            if (message.Length == 0) { Help(p); return; }
            string opt = args[0];
            
            if (IsCreateCommand(opt)) {
                if (args.Length == 1) { Help(p); return; }
                CreateZone(p, args, 1);
            } else if (IsDeleteCommand(opt)) {
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
            if (!PermissionCmd.Do(p, args, offset + 1, false, z.Access, p.level)) return;

            Player.Message(p, "Creating zone " + z.ColoredName);
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, "Selecting region for %SNew zone", z, AddZone);
        }
        
        bool AddZone(Player p, Vec3S32[] marks, object state, BlockID block) {
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
            PermissionCmd.Do(p, args, 2, false, zone.Access, p.level);
        }
        
        void SetZoneProp(Player p, string[] args, Zone zone) {
            ColorDesc desc = default(ColorDesc);
            if (args.Length < 4) { 
                Player.Message(p, "No value provided. See %T/Help zone properties");
                return;
            }
            
            string opt = args[2], value = args[3];           
            Predicate<Player> selector = pl => pl.ZoneIn == zone;
            if (opt.CaselessEq("alpha")) {
                float alpha = 0;
                if (!CommandParser.GetReal(p, value, "Alpha", ref alpha, 0, 1)) return;
                
                zone.UnshowAll(p.level);
                zone.Config.ShowAlpha = (byte)(alpha * 255);
                zone.ShowAll(p.level);
            } else if (opt.CaselessEq("col")) {
                if (!CommandParser.GetHex(p, value, ref desc)) return;
                
                zone.Config.ShowColor = value;
                zone.ShowAll(p.level);
            } else if (opt.CaselessEq("motd")) {
                zone.Config.MOTD = value;
                OnChangedZone(zone);
            } else if (CmdEnvironment.Handle(p, selector, opt, value, zone.Config, "zone " + zone.ColoredName)) {
                OnChangedZone(zone);
            } else {
                Help(p, "properties"); return;
            }
            p.level.Save(true);
        }
        
        void OnChangedZone(Zone zone) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (pl.ZoneIn == zone) pl.OnChangedZone();
            }
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/Zone add [name] <permissions>");
            Player.Message(p, "%HCreates a new zone, optionally also sets build permissions");
            Player.Message(p, "%T/Zone del [name]");
            Player.Message(p, "%HDeletes the given zone");
            Player.Message(p, "%T/Zone edit [name] [permissions]");
            Player.Message(p, "%HSets build permissions for the given zone");
            Player.Message(p, "%H  For syntax of permissions, see %T/Help PerBuild");
            Player.Message(p, "%T/Zone set [name] [property] [value]");
            Player.Message(p, "%HSets a property of this zone. See %T/Help zone properties");
        }
        
        public override void Help(Player p, string message) {
            if (message.CaselessEq("properties")) {
                Player.Message(p, "%T/Zone set [name] alpha [value]");
                Player.Message(p, "%HSets how solid the box shown around the zone is");
                Player.Message(p, "%H0 - not shown at all, 0.5 - half solid, 1 - fully solid");
                Player.Message(p, "%T/Zone set [name] col [hex color]");
                Player.Message(p, "%HSets the color of the box shown around the zone");
                Player.Message(p, "%T/Zone set [name] motd [value]");
                Player.Message(p, "%HSets the MOTD applied when in the zone. See %T/Help map motd");
                Player.Message(p, "%T/Zone set [name] [env property] [value]");
                Player.Message(p, "%HSets an env setting applied when in the zone. See %T/Help env");
            } else {
                base.Help(p, message);
            }
        }
    }
    
    public sealed class CmdZoneTest : Command {
        public override string name { get { return "ZoneTest"; } }
        public override string shortcut { get { return "ZTest"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        
        public override void Use(Player p, string message) {
            Player.Message(p, "Place or delete a block where you would like to check for zones.");
            p.MakeSelection(1, "Selecting point for %SZone check", null, TestZone);
        }
        
        bool TestZone(Player p, Vec3S32[] marks, object state, BlockID block) {
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
        public override bool UseableWhenFrozen { get { return true; } }
        
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
