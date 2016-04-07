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
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.BlockBehaviour {
    
    internal static class WalkthroughBehaviour {

        internal static bool Portal(Player p, byte block, ushort x, ushort y, ushort z, bool checkPos) {
            if (checkPos && p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            p.RevertBlock(x, y, z);
            try {
                //safe against SQL injections because no user input is given here
                DataTable Portals = Database.fillData("SELECT * FROM `Portals" + p.level.name + "` WHERE EntryX=" + (int)x + " AND EntryY=" + (int)y + " AND EntryZ=" + (int)z);
                int last = Portals.Rows.Count - 1;
                if (last == -1) { Portals.Dispose(); return true; }
                
                DataRow row = Portals.Rows[last];
                if (p.level.name != row["ExitMap"].ToString()) {
                    if (p.level.permissionvisit > p.group.Permission) {
                        Player.SendMessage(p, "You do not have the adequate rank to visit this map!"); return true;
                    }
                    
                    p.ignorePermission = true;
                    Level curLevel = p.level;
                    Command.all.Find("goto").Use(p, row["ExitMap"].ToString());
                    if (curLevel == p.level) { Player.SendMessage(p, "The map the portal goes to isn't loaded."); return true; }
                    p.ignorePermission = false;
                }
                p.BlockUntilLoad(10);
                Command.all.Find("move").Use(p, p.name + " " + row["ExitX"].ToString() + " " + row["ExitY"].ToString() + " " + row["ExitZ"].ToString());
                Portals.Dispose();
            } catch {
                Player.SendMessage(p, "Portal had no exit.");
            }
            return true;
        }
        
        static char[] trimChars = { ' ' };
        internal static bool MessageBlock(Player p, byte block, ushort x, ushort y, ushort z, bool checkPos) {
            if (checkPos && p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            p.RevertBlock(x, y, z);
            try {
                //safe against SQL injections because no user input is given here
                DataTable Messages = Database.fillData("SELECT * FROM `Messages" + p.level.name + "` WHERE X=" + (int)x + " AND Y=" + (int)y + " AND Z=" + (int)z);
                int last = Messages.Rows.Count - 1;
                if (last == -1) { Messages.Dispose(); return true; }
                
                string message = Messages.Rows[last]["Message"].ToString().Trim();
                message = message.Replace("\\'", "\'");
                if ( message != p.prevMsg || Server.repeatMessage ) {
                    if ( message.StartsWith("/") ) {
                        string[] parts = message.Remove(0, 1).Split(trimChars, 2);
                        p.HandleCommand(parts[0], parts.Length > 1 ? parts[1] : "");
                    } else {
                        Player.SendMessage(p, message);
                    }
                    p.prevMsg = message;
                }
            } catch {
                Player.SendMessage(p, "No message was stored.");
            }
            return true;
        }
        
        internal static bool Checkpoint(Player p, byte block, ushort x, ushort y, ushort z) {
            p.useCheckpointSpawn = true;
            p.checkpointX = x; p.checkpointY = y; p.checkpointZ = z;
            int index = p.level.PosToInt(x, y, z);
            if (index != p.lastCheckpointIndex) {
            	int sendY = (p.pos[1] / 32) * 32 + 10;
            	p.SpawnEntity(p, 0xFF, p.pos[0], (ushort)sendY, p.pos[2], p.rot[0], p.rot[1]);
                p.lastCheckpointIndex = index;
            }
            return true;
        }
        
        internal static bool Door(Player p, byte block, ushort x, ushort y, ushort z) {
            p.level.Blockchange(x, y, z, Block.DoorAirs(block));
            return true;
        }
    }
}
