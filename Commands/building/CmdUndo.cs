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
using System.Collections.Generic;
using MCGalaxy.Util;

namespace MCGalaxy.Commands
{
    public sealed class CmdUndo : Command
    {
        public override string name { get { return "undo"; } }
        public override string shortcut { get { return "u"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "The lowest rank to undo other players actions", 1),
                    new CommandPerm(LevelPermission.AdvBuilder, "The lowest rank to be able to undo physics", 2),
                }; }
        }

        public override void Use(Player p, string message) {
            if (p != null) p.RedoBuffer.Clear();
            int ignored = 0;
            if (message == "") {
                if (p == null) { Player.SendMessage(null, "Console doesn't have an undo buffer."); return; }
                message = p.name.ToLower() + " 30";
            } else if (p != null && int.TryParse(message, out ignored)) {
                message = p.name.ToLower() + " " + message;
            }
            
            string[] parts = message.Split(' ');
            bool undoPhysics = parts[0].ToLower() == "physics";
            Player who = undoPhysics ? null : PlayerInfo.Find(parts[0]);
            long seconds = GetSeconds(p, who, parts.Length > 1 ? parts[1] : "30");
            
            if (parts.Length > 1 && parts[1].ToLower() == "update") {
                UndoFile.UpgradePlayerUndoFiles(parts[0]);
                Player.SendMessage(p, "Updated undo files for " + parts[0] + " to the new binary format.");
                return;
            }

            if (who != null)
                UndoOnlinePlayer(p, seconds, parts[0], who);
            else if (undoPhysics)
                UndoLevelPhysics(p, seconds);
            else
                UndoOfflinePlayer(p, seconds, parts[0]);
        }

        const int undoMax = -1; // allows everything to be undone.
        long GetSeconds(Player p, Player who, string param) {
            long secs;
            if (param.ToLower() == "all") {
                secs = (p.group.maxUndo == undoMax || p == who) ? int.MaxValue : p.group.maxUndo;
            } else if (!long.TryParse(param, out secs)) {
                Player.SendMessage(p, "Invalid seconds, using 30 seconds.");
                return 30;
            }

            if (secs == 0) secs = 5400;
            if (p != null && p != who && p.group.maxUndo != undoMax && secs > p.group.maxUndo) {
                Player.SendMessage(p, p.group.name + "s may only undo up to " + p.group.maxUndo + " seconds.");
                return p.group.maxUndo;
            }
            return secs;
        }
        
        void UndoOnlinePlayer(Player p, long seconds, string whoName, Player who) {
            if (p != null) {
                if (who.group.Permission > p.group.Permission && who != p) {
                    Player.SendMessage(p, "Cannot undo a user of higher or equal rank"); return;
                }
                if (who != p && (int)p.group.Permission < CommandOtherPerms.GetPerm(this, 1)) {
                    Player.SendMessage(p, "Only an " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 1)).name + "+ may undo other people's actions"); return;
                }
            }
            
            Level saveLevel = null;
            PerformUndo(p, seconds, who.UndoBuffer, ref saveLevel);
            bool foundUser = false;
            UndoFile.UndoPlayer(p, whoName.ToLower(), seconds, ref foundUser);

            if (p == who) {
                Player.SendMessage(p, "Undid your actions for the past &b" + seconds + " %Sseconds.");
            } else {
                Player.SendChatFrom(who, who.color + who.DisplayName + "%S's actions for the past &b" + seconds + " seconds were undone.", false);
            }
            Server.s.Log(who.name + "'s actions for the past " + seconds + " seconds were undone.");
            if (saveLevel != null) saveLevel.Save(true);
        }
        
        void UndoOfflinePlayer(Player p, long seconds, string whoName) {
            if (p != null && (int)p.group.Permission < CommandOtherPerms.GetPerm(this)) {
                Player.SendMessage(p, "Reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + "+"); return;
            }

            bool foundUser = false;
            UndoFile.UndoPlayer(p, whoName.ToLower(), seconds, ref foundUser);

            if (foundUser) {
                Player.GlobalMessage(Server.FindColor(whoName) + whoName + "%S's actions for the past &b" + seconds + " %Sseconds were undone.");
                Server.s.Log(whoName + "'s actions for the past " + seconds + " seconds were undone.");
                p.level.Save(true);
            } else {
                Player.SendMessage(p, "Could not find player specified.");
            }
        }
        
        void UndoLevelPhysics(Player p, long seconds) {
            if (p != null && (int)p.group.Permission < CommandOtherPerms.GetPerm(this, 2)) {
                Player.SendMessage(p, "Reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 2)).name + "+"); return;
            }
            if (p != null && !p.group.CanExecute("physics")) {
                Player.SendMessage(p, "You can only undo physics if you can use /physics."); return;
            }
            Command.all.Find("physics").Use(p, "0");

            if (p.level.UndoBuffer.Count != Server.physUndo) {
                int count = p.level.currentUndo;
                for (int i = count; i >= 0; i--) {
                    try {
                        if (!CheckBlockPhysics(p, seconds, i, p.level.UndoBuffer[i])) break;
                    } catch { }
                }
            } else {
                int count = p.level.currentUndo;
                for (int i = count; i >= 0; i--) {
                    try {
                        if (!CheckBlockPhysics(p, seconds, i, p.level.UndoBuffer[i])) break;
                    } catch { }
                }
                for (int i = p.level.UndoBuffer.Count - 1; i > count; i--) {
                    try {
                        if (!CheckBlockPhysics(p, seconds, i, p.level.UndoBuffer[i])) break;
                    } catch { }
                }
            }

            Player.GlobalMessage("Physics were undone &b" + seconds + " %Sseconds");
            Server.s.Log( "Physics were undone &b" + seconds + " %Sseconds");
            p.level.Save(true);
        }
        
        static void PerformUndo(Player p, long seconds, UndoCache cache, ref Level saveLvl) {
            UndoCacheNode node = cache.Tail;
            if (node == null) return;
            
            while (node != null) {
                Level lvl = LevelInfo.FindExact(node.MapName);
                if (lvl == null) { node = node.Prev; continue; }
                saveLvl = lvl;
                List<UndoCacheItem> items = node.Items;
                BufferedBlockSender buffer = new BufferedBlockSender(lvl);
                
                for (int i = items.Count - 1; i >= 0; i--) {
                    UndoCacheItem item = items[i];
                    ushort x, y, z;
                    node.Unpack(item.Index, out x, out y, out z);
                    DateTime time = node.BaseTime.AddTicks((item.TimeDelta + seconds) * TimeSpan.TicksPerSecond);
                    if (time < DateTime.UtcNow) { buffer.CheckIfSend(true); return; }
                    
                    byte b = lvl.GetTile(x, y, z);
                    byte newTile = 0, newExtTile = 0;
                    item.GetNewExtBlock(out newTile, out newExtTile);
                    if (b == newTile || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                        Player.UndoPos uP = default(Player.UndoPos);
                        byte extType = 0;
                        if (b == Block.custom_block) extType = lvl.GetExtTile(x, y, z);
                        byte tile = 0, extTile = 0;
                        item.GetExtBlock(out tile, out extTile);
                    
                        if (lvl.DoBlockchange(p, x, y, z, tile, extTile)) {
                            buffer.Add(lvl.PosToInt(x, y, z), tile, extTile);
                            buffer.CheckIfSend(false);
                        }
                        
                        uP.newtype = tile; uP.newExtType = extTile;
                        uP.type = b; uP.extType = extType;
                        uP.x = x; uP.y = y; uP.z = z;
                        uP.mapName = node.MapName;
                        time = node.BaseTime.AddTicks(item.TimeDelta * TimeSpan.TicksPerSecond);
                        uP.timeDelta = (int)time.Subtract(Server.StartTime).TotalSeconds;
                        if (p != null) p.RedoBuffer.Add(lvl, uP);
                    }                   
                }
                buffer.CheckIfSend(true);
                node = node.Prev;
            }
        }
        
        bool CheckBlockPhysics(Player p, long seconds, int i, Level.UndoPos undo) {
            byte b = p.level.GetTile(undo.location);
            DateTime time = Server.StartTime.AddTicks(undo.timeDelta * TimeSpan.TicksPerSecond);
            if (time.AddTicks(seconds * TimeSpan.TicksPerSecond) < DateTime.UtcNow) return false;
            
            if (b == undo.newType || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                ushort x, y, z;
                p.level.IntToPos(undo.location, out x, out y, out z);
                int undoIndex = p.level.currentUndo;
                p.level.currentUndo = i;                
                p.level.currentUndo = undoIndex;
                p.level.Blockchange(x, y, z, undo.oldType, true, "", undo.oldExtType, false);
            }
            return true;
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/undo [player] [seconds] - Undoes the blockchanges made by [player] in the previous [seconds].");
            if (p == null || (p.group.maxUndo <= 500000 || p.group.maxUndo == 0))
                Player.SendMessage(p, "/undo [player] all - &cWill undo 68 years, 18 days, 15 hours, 28 minutes, 31 seconds for [player]");
            if (p == null || (p.group.maxUndo <= 1800 || p.group.maxUndo == 0))
                Player.SendMessage(p, "/undo [player] - &cWill undo 30 minutes");
            Player.SendMessage(p, "/undo physics [seconds] - Undoes the physics for the current map");
        }
    }
}
