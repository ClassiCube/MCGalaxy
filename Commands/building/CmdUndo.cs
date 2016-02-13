/*
    Copyright 2011 MCGalaxy
        
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
using System.Globalization;
using System.IO;
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
        public CmdUndo() { }

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
            for (int i = who.UndoBuffer.Count - 1; i >= 0; --i) {
                try {
                    Player.UndoPos Pos = who.UndoBuffer[i];
                    if (!CheckBlockPlayer(p, seconds, Pos, ref saveLevel)) break;
                } catch { }
            }
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
            if (p != null && !p.group.CanExecute(Command.all.Find("physics"))) {
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
        
        bool CheckBlockPlayer(Player p, long seconds, Player.UndoPos undo, ref Level saveLevel) {
            Level lvl = LevelInfo.FindExact(undo.mapName);
            saveLevel = lvl;
            byte b = lvl.GetTile(undo.x, undo.y, undo.z);
            DateTime time = Server.StartTime.AddSeconds(undo.timeDelta + seconds);
            if (time < DateTime.UtcNow) return false;
            
            if (b == undo.newtype || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                undo.newtype = undo.type; undo.newExtType = undo.extType;
                byte extType = 0;
                if (b == Block.custom_block)
                    extType = lvl.GetExtTile(undo.x, undo.y, undo.z);
                
                lvl.Blockchange(p, undo.x, undo.y, undo.z, undo.type, undo.extType);
                undo.type = b; undo.extType = extType;
                if (p != null) p.RedoBuffer.Add(undo);
            }
            return true;
        }
        
        bool CheckBlockPhysics(Player p, long seconds, int i, Level.UndoPos undo) {
            byte b = p.level.GetTile(undo.location);
            if (undo.timePerformed.AddSeconds(seconds) < DateTime.Now)
                return false;
            
            if (b == undo.newType || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                ushort x, y, z;
                int undoIndex = p.level.currentUndo;
                p.level.currentUndo = i;
                p.level.IntToPos(undo.location, out x, out y, out z);
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
