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
using MCGalaxy.DB;
using MCGalaxy.Events.PlayerDBEvents;
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdInfoSwap : Command2 {       
        public override string name { get { return "InfoSwap"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }

        public override void Use(Player p, string text, CommandData data) {
            string[] args = text.SplitSpaces();
            if (args.Length != 2) { Help(p); return; }
            
            string src = GetName(p, args[0]), dst = GetName(p, args[1]);
            if (src == null || dst == null) return;

            Group srcGroup = Group.GroupIn(src), dstGroup = Group.GroupIn(dst);
            if (!CheckRank(p, data, src, srcGroup.Permission, "&T/InfoSwap&S", false)) return;
            if (!CheckRank(p, data, dst, dstGroup.Permission, "&T/InfoSwap&S", false)) return;
                        
            SwapStats(src, dst);
            SwapGroups(src, dst, srcGroup, dstGroup);
            OnInfoSwapEvent.Call(src, dst);
            
            p.Message("Successfully infoswapped {0} &Sand {1}",
                      p.FormatNick(src), p.FormatNick(dst));
        }
        
        static string GetName(Player p, string name) {
            if (!Formatter.ValidName(p, name, "player")) return null;
            if (PlayerInfo.FindExact(name) != null) {
                p.Message("\"{0}\" must be offline to use &T/InfoSwap", name); return null;
            }
            
            string match = PlayerDB.FindName(name);
            if (match == null) {
                p.Message("\"{0}\" was not found in the database.", name); return null;
            }
            return match;
        }
        
        static void SwapStats(string src, string dst) {
            int tmpNum = new Random().Next(0, 10000000);
            string tmpName = "-tmp" + tmpNum + "-";
            
            Database.UpdateRows("Players", "Name=@1", "WHERE Name=@0", dst, tmpName); // PLAYERS[dst] = tmp
            Database.UpdateRows("Players", "Name=@1", "WHERE Name=@0", src, dst);     // PLAYERS[src] = dst
            Database.UpdateRows("Players", "Name=@1", "WHERE Name=@0", tmpName, src); // PLAYERS[tmp] = src
        }
        
        static void SwapGroups(string src, string dst, Group srcGroup, Group dstGroup) {
            srcGroup.Players.Remove(src);
            srcGroup.Players.Add(dst);
            srcGroup.Players.Save();
            
            dstGroup.Players.Remove(dst);
            dstGroup.Players.Add(src);
            dstGroup.Players.Save();
        }
        
        public override void Help(Player p) {
            p.Message("&T/InfoSwap [source] [other]");
            p.Message("&HSwaps all the player's info from [source] to [other].");
            p.Message("&HNote that both players must be offline for this to work.");
        }
    }
}
