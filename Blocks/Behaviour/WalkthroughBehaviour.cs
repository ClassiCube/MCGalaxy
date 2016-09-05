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
using System.Collections.Generic;
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.BlockBehaviour {    
    internal static class WalkthroughBehaviour {

        internal static bool Door(Player p, byte block, ushort x, ushort y, ushort z) {
            p.level.Blockchange(x, y, z, Block.DoorAirs(block));
            return true;
        }
        
        internal static bool Train(Player p, byte block, ushort x, ushort y, ushort z) {
            if (!p.onTrain) p.HandleDeath(Block.train);
            return true;
        }
        
        internal static bool Portal(Player p, byte block, ushort x, ushort y, ushort z, bool checkPos) {
            if (checkPos && p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            p.RevertBlock(x, y, z);
            try {
                DataTable Portals = Database.Backend.GetRows("Portals" + p.level.name, "*",
                                                             "WHERE EntryX=@0 AND EntryY=@1 AND EntryZ=@2", x, y, z);
                int last = Portals.Rows.Count - 1;
                if (last == -1) { Portals.Dispose(); return true; }
                byte rotX = p.rot[0], rotY = p.rot[1];
                
                DataRow row = Portals.Rows[last];
                string map = row["ExitMap"].ToString();
                if (p.level.name != map) {
                    if (p.level.permissionvisit > p.Rank) {
                        Player.Message(p, "You do not have the adequate rank to visit this map!"); return true;
                    }
                    
                    p.ignorePermission = true;
                    Level curLevel = p.level;
                    PlayerActions.ChangeMap(p, map);
                    if (curLevel == p.level) { Player.Message(p, "The map the portal goes to isn't loaded."); return true; }
                    p.ignorePermission = false;
                    p.BlockUntilLoad(10);
                }
                
                x = ushort.Parse(row["ExitX"].ToString());
                y = ushort.Parse(row["ExitY"].ToString());
                z = ushort.Parse(row["ExitZ"].ToString());
                PlayerActions.MoveCoords(p, x, y, z, rotX, rotY);
                Portals.Dispose();
            } catch {
                Player.Message(p, "Portal had no exit.");
            }
            return true;
        }
        
        internal static bool MessageBlock(Player p, byte block, ushort x, ushort y, ushort z, bool checkPos) {
            if (checkPos && p.level.PosToInt(x, y, z) == p.lastWalkthrough) return true;
            p.RevertBlock(x, y, z);
            try {
                DataTable Messages = Database.Backend.GetRows("Messages" + p.level.name, "*",
                                                              "WHERE X=@0 AND Y=@1 AND Z=@2", x, y, z);
                int last = Messages.Rows.Count - 1;
                if (last == -1) { Messages.Dispose(); return true; }
                string message = Messages.Rows[last]["Message"].ToString().Trim();
                message = message.Replace("\\'", "\'");
                message = message.Replace("@p", p.name);
                
                if (message != p.prevMsg || Server.repeatMessage) {
                    string text;
                    List<string> cmds = ParseMB(message, out text);
                    if (text != null) Player.Message(p, text);
                    
                    if (cmds.Count == 1) {
                        string[] parts = cmds[0].SplitSpaces(2);
                        p.HandleCommand(parts[0], parts.Length > 1 ? parts[1] : "");
                    } else if (cmds.Count > 0) {
                        p.HandleCommands(cmds);
                    }
                    p.prevMsg = message;
                }
            } catch {
                Player.Message(p, "No message was stored.");
            }
            return true;
        }
        
        internal static bool Checkpoint(Player p, byte block, ushort x, ushort y, ushort z) {
            p.useCheckpointSpawn = true;
            p.checkpointX = x; p.checkpointY = y; p.checkpointZ = z;
            p.checkpointRotX = p.rot[0]; p.checkpointRotY = p.rot[1];
            
            int index = p.level.PosToInt(x, y, z);
            if (index != p.lastCheckpointIndex) {
                int sendY = (p.pos[1] / 32) * 32 + 10;
                p.SpawnEntity(p, 0xFF, p.pos[0], (ushort)sendY, p.pos[2], p.rot[0], p.rot[1]);
                p.lastCheckpointIndex = index;
            }
            return true;
        }
        
        static string[] sep = { " |/" };
        const StringSplitOptions opts = StringSplitOptions.RemoveEmptyEntries;
        static List<string> empty = new List<string>();
        internal static List<string> ParseMB(string message, out string text) {
            if (message.IndexOf('|') == -1) return ParseSingle(message, out text);
            
            string[] parts = message.Split(sep, opts);
            List<string> cmds = ParseSingle(parts[0], out text);
            if (parts.Length == 1) return cmds;
            
            if (text != null) cmds = new List<string>();
            for (int i = 1; i < parts.Length; i++)
                cmds.Add(parts[i]);
            return cmds;
        }
        
        static List<string> ParseSingle(string message, out string text) {
            if (message[0] == '/') {
                text = null; return new List<string>(){ message.Substring(1) };
            } else {
                text = message; return empty;
            }
        }
    }
}
