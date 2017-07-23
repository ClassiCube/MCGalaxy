/*
    Copyright 2011 MCForge
    Created by Techjar (Jordan S.)
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using MCGalaxy.Games;
using MCGalaxy.Maths;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdLavaSurvival : Command {
        public override string name { get { return "lavasurvival"; } }
        public override string shortcut { get { return "ls"; } }
        public override string type { get { return CommandTypes.Games; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] ExtraPerms {
            get { return new[] { new CommandPerm(LevelPermission.Admin, "+ can setup lava survival"),
                    new CommandPerm(LevelPermission.Operator, "+ can start/stop lava survival") }; }
        }
        
        public override void Use(Player p, string message)  {
            if (message == "") { Help(p); return; }
            string[] args = message.ToLower().SplitSpaces();

            switch (args[0]) {
                case "go": HandleGo(p, args); return;
                case "info": HandleInfo(p, args); return;
                case "start": HandleStart(p, args); return;
                case "stop": HandleStop(p, args); return;
                case "end": HandleEnd(p, args); return;
                case "setup": HandleSetup(p, args); return;
            }
            Help(p);
        }
        
        void HandleGo(Player p, string[] args) {
            if (p == null) { Player.Message(p, "/{0} go can only be used in-game.", name); return; }
            if (!Server.lava.active) { Player.Message(p, "There is no Lava Survival game right now."); return; }
            PlayerActions.ChangeMap(p, Server.lava.map.name);
        }
        
        void HandleInfo(Player p, string[] args) {
            if (!Server.lava.active) { Player.Message(p, "There is no Lava Survival game right now."); return; }
            if (!Server.lava.roundActive) { Player.Message(p, "The round of Lava Survival hasn't started yet."); return; }
            Server.lava.AnnounceRoundInfo(p, p == null);
            Server.lava.AnnounceTimeLeft(!Server.lava.flooded, true, p, p == null);
        }
        
        void HandleStart(Player p, string[] args) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            string map = args.Length > 1 ? args[1] : "";
            switch (Server.lava.Start(map)) {
                case 0: Chat.MessageGlobal("Lava Survival has started! Join the fun with /ls go"); return;
                case 1: Player.Message(p, "There is already an active Lava Survival game."); return;
                case 2: Player.Message(p, "You must have at least 3 configured maps to play Lava Survival."); return;
                case 3: Player.Message(p, "The specified map doesn't exist."); return;
                default: Player.Message(p, "An unknown error occurred."); return;
            }
        }
        
        void HandleStop(Player p, string[] args) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            switch (Server.lava.Stop()) {
                case 0: Chat.MessageGlobal("Lava Survival has ended! We hope you had fun!"); return;
                case 1: Player.Message(p, "There isn't an active Lava Survival game."); return;
                default: Player.Message(p, "An unknown error occurred."); return;
            }
        }
        
        void HandleEnd(Player p, string[] args) {
            if (!CheckExtraPerm(p, 2)) { MessageNeedExtra(p, 2); return; }
            
            if (!Server.lava.active) { Player.Message(p, "There isn't an active Lava Survival game."); return; }
            if (Server.lava.roundActive) Server.lava.EndRound();
            else if (Server.lava.voteActive) Server.lava.EndVote();
            else Player.Message(p, "There isn't an active round or vote to end.");
        }

        void HandleSetup(Player p, string[] args) {
            if (!CheckExtraPerm(p, 1)) { MessageNeedExtra(p, 1); return; }
            if (p == null) { Player.Message(p, "/{0} setup can only be used in-game.", name); return; }            
            if (args.Length < 2) { SetupHelp(p); return; }
            if (Server.lava.active) { Player.Message(p, "You cannot configure Lava Survival while a game is active."); return; }
            
            switch (args[1]) {
                case "map": HandleSetupMap(p, args); return;
                case "block": HandleSetupBlock(p, args); return;
                case "safe": HandleSetupSafeZone(p, args); return;
                case "safezone": HandleSetupSafeZone(p, args); return;
                case "settings": HandleSetupSettings(p, args); return;
                case "mapsettings": HandleSetupMapSettings(p, args); return;
            }
            SetupHelp(p);
        }
        
        void HandleSetupMap(Player p, string[] args) {
            if (args.Length < 3) { SetupHelp(p, "map"); return; }
            Level lvl = Matcher.FindLevels(p, args[2]);
            if (lvl == null) return;
            if (lvl == Server.mainLevel) { Player.Message(p, "You cannot use the main map for Lava Survival."); return; }
            
            if (Server.lava.HasMap(lvl.name)) {
                Server.lava.RemoveMap(lvl.name);
                lvl.Config.PhysicsOverload = 1500;
                lvl.Config.AutoUnload = true;
                lvl.Config.LoadOnGoto = true;
                Player.Message(p, "Map {0} %Shas been removed.", lvl.ColoredName);
            } else {
                Server.lava.AddMap(lvl.name);

                LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(lvl.name);
                settings.blockFlood = new Vec3U16((ushort)(lvl.Width / 2), (ushort)(lvl.Height - 1), (ushort)(lvl.Length / 2));
                settings.blockLayer = new Vec3U16(0, (ushort)(lvl.Height / 2), 0);
                ushort x = (ushort)(lvl.Width / 2), y = (ushort)(lvl.Height / 2), z = (ushort)(lvl.Length / 2);
                settings.safeZone = new Vec3U16[] { new Vec3U16((ushort)(x - 3), y, (ushort)(z - 3)), new Vec3U16((ushort)(x + 3), (ushort)(y + 4), (ushort)(z + 3)) };
                Server.lava.SaveMapSettings(settings);

                lvl.Config.PhysicsOverload = 1000000;
                lvl.Config.AutoUnload = false;
                lvl.Config.LoadOnGoto = false;
                Player.Message(p, "Map {0} %Shas been added.", lvl.ColoredName);
            }
            Level.SaveSettings(lvl);
        }
        
        void HandleSetupBlock(Player p, string[] args) {
            if (!Server.lava.HasMap(p.level.name)) { Player.Message(p, "Add the map before configuring it."); return; }
            if (args.Length < 3) { SetupHelp(p, "block"); return; }

            if (args[2] == "flood") {
                Player.Message(p, "Place or destroy the block you want to be the total flood block spawn point.");
                p.MakeSelection(1, null, SetFloodPos);
            } else if (args[2] == "layer") {
                Player.Message(p, "Place or destroy the block you want to be the layer flood base spawn point.");
                p.MakeSelection(1, null, SetFloodLayerPos);
            } else {
                SetupHelp(p, "block");
            }
        }
        
        void HandleSetupSafeZone(Player p, string[] args) {
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, null, SetSafeZone);
        }
        
        void HandleSetupSettings(Player p, string[] args) {
            if (args.Length < 3) {
                Player.Message(p, "Maps: &b" + Server.lava.Maps.Join(", "));
                Player.Message(p, "Start on server startup: " + (Server.lava.startOnStartup ? "&aON" : "&cOFF"));
                Player.Message(p, "Send AFK to main: " + (Server.lava.sendAfkMain ? "&aON" : "&cOFF"));
                Player.Message(p, "Vote count: &b" + Server.lava.voteCount);
                Player.Message(p, "Vote time: &b" + Server.lava.voteTime + " minute" + (Server.lava.voteTime == 1 ? "" : "s"));
                return;
            }

            try {
                switch (args[2]) {
                    case "sendafkmain":
                        Server.lava.sendAfkMain = !Server.lava.sendAfkMain;
                        Player.Message(p, "Send AFK to main: " + (Server.lava.sendAfkMain ? "&aON" : "&cOFF"));
                        break;
                    case "votecount":
                        Server.lava.voteCount = (byte)Utils.Clamp(int.Parse(args[3]), 2, 10);
                        Player.Message(p, "Vote count: &b" + Server.lava.voteCount);
                        break;
                    case "votetime":
                        Server.lava.voteTime = double.Parse(args[3]);
                        Player.Message(p, "Vote time: &b" + Server.lava.voteTime + "minute" + (Server.lava.voteTime == 1 ? "" : "s"));
                        break;
                    default:
                        SetupHelp(p, "settings"); return;
                }
            } catch { Player.Message(p, "INVALID INPUT"); return; }
            Server.lava.SaveSettings();
        }
        
        void HandleSetupMapSettings(Player p, string[] args) {
            if (!Server.lava.HasMap(p.level.name)) { Player.Message(p, "Add the map before configuring it."); return; }
            LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
            if (args.Length < 4) {
                Player.Message(p, "Fast lava chance: &b" + settings.fast + "%");
                Player.Message(p, "Killer lava/water chance: &b" + settings.killer + "%");
                Player.Message(p, "Destroy blocks chance: &b" + settings.destroy + "%");
                Player.Message(p, "Water flood chance: &b" + settings.water + "%");
                Player.Message(p, "Layer flood chance: &b" + settings.layer + "%");
                Player.Message(p, "Layer height: &b" + settings.layerHeight + " block" + (settings.layerHeight == 1 ? "" : "s"));
                Player.Message(p, "Layer count: &b" + settings.layerCount);
                Player.Message(p, "Layer time: &b" + settings.layerInterval + " minute" + (settings.layerInterval == 1 ? "" : "s"));
                Player.Message(p, "Round time: &b" + settings.roundTime + " minute" + (settings.roundTime == 1 ? "" : "s"));
                Player.Message(p, "Flood time: &b" + settings.floodTime + " minute" + (settings.floodTime == 1 ? "" : "s"));
                Player.Message(p, "Flood position: &b" + settings.blockFlood.ToString(", "));
                Player.Message(p, "Layer position: &b" + settings.blockLayer.ToString(", "));
                Player.Message(p, "Safe zone: &b({0}) ({1})", settings.safeZone[0].ToString(", "), settings.safeZone[1].ToString(", "));
                return;
            }

            try {
                switch (args[2]) {
                    case "fast":
                        settings.fast = (byte)Utils.Clamp(int.Parse(args[3]), 0, 100);
                        Player.Message(p, "Fast lava chance: &b" + settings.fast + "%");
                        break;
                    case "killer":
                        settings.killer = (byte)Utils.Clamp(int.Parse(args[3]), 0, 100);
                        Player.Message(p, "Killer lava/water chance: &b" + settings.killer + "%");
                        break;
                    case "destroy":
                        settings.destroy = (byte)Utils.Clamp(int.Parse(args[3]), 0, 100);
                        Player.Message(p, "Destroy blocks chance: &b" + settings.destroy + "%");
                        break;
                    case "water":
                        settings.water = (byte)Utils.Clamp(int.Parse(args[3]), 0, 100);
                        Player.Message(p, "Water flood chance: &b" + settings.water + "%");
                        break;
                    case "layer":
                        settings.layer = (byte)Utils.Clamp(int.Parse(args[3]), 0, 100);
                        Player.Message(p, "Layer flood chance: &b" + settings.layer + "%");
                        break;
                    case "layerheight":
                        settings.layerHeight = int.Parse(args[3]);
                        Player.Message(p, "Layer height: &b" + settings.layerHeight + " block" + (settings.layerHeight == 1 ? "" : "s"));
                        break;
                    case "layercount":
                        settings.layerCount = int.Parse(args[3]);
                        Player.Message(p, "Layer count: &b" + settings.layerCount);
                        break;
                    case "layertime":
                        settings.layerInterval = double.Parse(args[3]);
                        Player.Message(p, "Layer time: &b" + settings.layerInterval + " minute" + (settings.layerInterval == 1 ? "" : "s"));
                        break;
                    case "roundtime":
                        settings.roundTime = double.Parse(args[3]);
                        Player.Message(p, "Round time: &b" + settings.roundTime + " minute" + (settings.roundTime == 1 ? "" : "s"));
                        break;
                    case "floodtime":
                        settings.floodTime = double.Parse(args[3]);
                        Player.Message(p, "Flood time: &b" + settings.floodTime + " minute" + (settings.floodTime == 1 ? "" : "s"));
                        break;
                    default:
                        SetupHelp(p, "mapsettings"); return;
                }
            } catch { Player.Message(p, "INVALID INPUT"); return; }
            Server.lava.SaveMapSettings(settings);
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/ls go %H- Join the fun!");
            Player.Message(p, "%T/ls info %H- View current round info and time.");
            
            if (CheckExtraPerm(p, 1))  {
                Player.Message(p, "%T/ls start <map> %H- Starts Lava Survival, optionally on the given map.");
                Player.Message(p, "%T/ls stop %H- Stops the current Lava Survival game.");
                Player.Message(p, "%T/ls end %H- End the current round or vote.");
            }
            if (CheckExtraPerm(p, 2)) {
                Player.Message(p, "%T/ls setup %H- Setup lava survival, use it for more info.");
            }
        }

        void SetupHelp(Player p, string mode = "") {
            switch (mode) {
                case "map":
                    Player.Message(p, "Add or remove maps in Lava Survival.");
                    Player.Message(p, "<mapname> - Adds or removes <mapname>.");
                    break;
                case "block":
                    Player.Message(p, "View or set the block spawn positions.");
                    Player.Message(p, "flood - Set the position for the total flood block.");
                    Player.Message(p, "layer - Set the position for the layer flood base.");
                    break;
                case "settings":
                    Player.Message(p, "View or change the settings for Lava Survival.");
                    Player.Message(p, "sendafkmain - Toggle sending AFK users to the main map when the map changes.");
                    Player.Message(p, "votecount <2-10> - Set how many maps will be in the next map vote.");
                    Player.Message(p, "votetime <time> - Set how long until the next map vote ends.");
                    break;
                case "mapsettings":
                    Player.Message(p, "View or change the settings for a Lava Survival map.");
                    Player.Message(p, "fast <0-100> - Set the percent chance of fast lava.");
                    Player.Message(p, "killer <0-100> - Set the percent chance of killer lava/water.");
                    Player.Message(p, "destroy <0-100> - Set the percent chance of the lava/water destroying blocks.");
                    Player.Message(p, "water <0-100> - Set the percent chance of a water instead of lava flood.");
                    Player.Message(p, "layer <0-100> - Set the percent chance of the lava/water flooding in layers.");
                    Player.Message(p, "layerheight <height> - Set the height of each layer.");
                    Player.Message(p, "layercount <count> - Set the number of layers to flood.");
                    Player.Message(p, "layertime <time> - Set the time interval for another layer to flood.");
                    Player.Message(p, "roundtime <time> - Set how long until the round ends.");
                    Player.Message(p, "floodtime <time> - Set how long until the map is flooded.");
                    break;
                default:
                    Player.Message(p, "Commands to setup Lava Survival.");
                    Player.Message(p, "map <name> - Add or remove maps in Lava Survival.");
                    Player.Message(p, "block <mode> - Set the block spawn positions.");
                    Player.Message(p, "safezone - Set the safe zone, which is an area that can't be flooded.");
                    Player.Message(p, "settings <setting> [value] - View or change the settings for Lava Survival.");
                    Player.Message(p, "mapsettings <setting> [value] - View or change the settings for a Lava Survival map.");
                    break;
            }
        }
        

        bool SetFloodPos(Player p, Vec3S32[] m, object state, ExtBlock block) {
            LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
            settings.blockFlood = (Vec3U16)m[0];
            Server.lava.SaveMapSettings(settings);

            Player.Message(p, "Position set! &b({0}, {1}, {2})", m[0].X, m[0].Y, m[0].Z);
            return false;
        }
                
        bool SetFloodLayerPos(Player p, Vec3S32[] m, object state, ExtBlock block) {
            LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
            settings.blockLayer = (Vec3U16)m[0];
            Server.lava.SaveMapSettings(settings);

            Player.Message(p, "Position set! &b({0}, {1}, {2})", m[0].X, m[0].Y, m[0].Z);
            return false;
        }
        
        bool SetSafeZone(Player p, Vec3S32[] m, object state, ExtBlock block) {
            Vec3S32 min = Vec3S32.Min(m[0], m[1]);
            Vec3S32 max = Vec3S32.Max(m[0], m[1]);

            LavaSurvival.MapSettings settings = Server.lava.LoadMapSettings(p.level.name);
            settings.safeZone = new Vec3U16[] { (Vec3U16)min, (Vec3U16)max };
            Server.lava.SaveMapSettings(settings);

            Player.Message(p, "Safe zone set! &b({0}, {1}, {2}) ({3}, {4}, {5})",
                           min.X, min.Y, min.Z, max.X, max.Y, max.Z);
            return false;
        }
    }
}
