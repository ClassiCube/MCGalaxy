/*
    Copyright 2011 MCGalaxy
        
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
using System.IO;
using MCGalaxy.Util;

namespace MCGalaxy.Commands {
    
    public sealed class CmdHighlight : Command {
        
        public override string name { get { return "highlight"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdHighlight() { }

        public override void Use(Player p, string message) {
            long seconds;
            bool FoundUser = false;
            if (p == null) { MessageInGameOnly(p); return; }
            if (message == "") message = p.name + " 300";
            string[] args = message.Split(' ');
            string name = args[0];

            if (args.Length >= 2) {
                if (!long.TryParse(args[1], out seconds)) {
                    Player.SendMessage(p, "Invalid seconds."); return;
                }
            } else if (long.TryParse(args[0], out seconds)) {
                args[0] = p.name;
            } else {
                seconds = 300;
            }
            if (seconds <= 0) seconds = 5400;

            Player who = PlayerInfo.Find(name);
            if (who != null) {
                FoundUser = true;
                HighlightOnline(p, seconds, who);
            }

            UndoFile.HighlightPlayer(p, name.ToLower(), seconds, ref FoundUser);
            if (FoundUser) {
                Player.SendMessage(p, "Now highlighting &b" + seconds +  " %Sseconds for " + Server.FindColor(name) + name);
                Player.SendMessage(p, "&cUse /reload to un-highlight");
            } else {
                Player.SendMessage(p, "Could not find player specified.");
            }
        }

        static void HighlightOnline(Player p, long seconds, Player who) {
            for (int i = who.UndoBuffer.Count - 1; i >= 0; --i) {
                try {
                    Player.UndoPos undo = who.UndoBuffer[i];
                    Level foundLevel = LevelInfo.FindExact(undo.mapName);
                    if (foundLevel != p.level) continue;
                    
                    byte b = foundLevel.GetTile(undo.x, undo.y, undo.z);
                    DateTime time = Server.StartTime.AddSeconds(undo.timeDelta + seconds);
                    if (time < DateTime.UtcNow) break;
                    
                    if (b == undo.newtype || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava) {
                        if (b == Block.air || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava)
                            p.SendBlockchange(undo.x, undo.y, undo.z, Block.red);
                        else
                            p.SendBlockchange(undo.x, undo.y, undo.z, Block.green);
                    }
                } catch { }
            }
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/highlight [player] [seconds] - Highlights blocks modified by [player] in the last [seconds]");
            Player.SendMessage(p, "/highlight [player] 0 - Will highlight 30 minutes");
            Player.SendMessage(p, "&c/highlight cannot be disabled, you must use /reload to un-highlight");
        }
    }
}
