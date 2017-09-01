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
using MCGalaxy.SQL;

namespace MCGalaxy.Commands.Maintenance {
    public sealed class CmdInfoSwap : Command {       
        public override string name { get { return "InfoSwap"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }

        public override void Use(Player p, string text) {
            string[] args = text.SplitSpaces();
            if (args.Length != 2) { Help(p); return; }
            if (!Formatter.ValidName(p, args[0], "player")) return;
            if (!Formatter.ValidName(p, args[1], "player")) return;
            
            if (PlayerInfo.FindExact(args[0]) != null) {
                Player.Message(p, "\"{0}\" must be offline to use %T/InfoSwap", args[0]); return;
            }
            if (PlayerInfo.FindExact(args[1]) != null) {
                Player.Message(p, "\"{0}\" must be offline to use %T/InfoSwap", args[1]); return;
            }
            
            string src = PlayerInfo.FindName(args[0]);
            if (src == null) {
                Player.Message(p, "\"{0}\" was not found in the database.", args[0]); return;
            }
            string dst = PlayerInfo.FindName(args[1]);
            if (dst == null) {
                Player.Message(p, "\"{0}\" was not found in the database.", args[1]); return;
            }

            Group srcGroup = Group.GroupIn(src);
            Group dstGroup = Group.GroupIn(dst);
            if (p != null && srcGroup.Permission >= p.Rank) {
                Player.Message(p, "Cannot %T/InfoSwap %Sfor a player ranked equal or higher to yours."); return;
            }
            if (p != null && dstGroup.Permission >= p.Rank) {
                Player.Message(p, "Cannot %T/InfoSwap %Sfor a player ranked equal or higher to yours."); return;
            }
                        
            SwapStats(src, dst);
            SwapGroups(src, dst, srcGroup, dstGroup);
            
            Player.Message(p, "Successfully infoswapped {0} %Sand {1}",
                           PlayerInfo.GetColoredName(p, src),
                           PlayerInfo.GetColoredName(p, dst));
        }
        
        void SwapStats(string src, string dst) {
            int tmpNum = new Random().Next(0, 10000000);
            string tmpName = "-tmp" + tmpNum + "-";
            
            Database.Backend.UpdateRows("Players", "Name=@1", "WHERE Name=@0", dst, tmpName); // PLAYERS[dst] = tmp
            Database.Backend.UpdateRows("Players", "Name=@1", "WHERE Name=@0", src, dst);     // PLAYERS[src] = dst
            Database.Backend.UpdateRows("Players", "Name=@1", "WHERE Name=@0", tmpName, src); // PLAYERS[tmp] = src
        }
        
        void SwapGroups(string src, string dst, Group srcGroup, Group dstGroup) {
            srcGroup.Players.Remove(src);
            srcGroup.Players.Add(dst);
            srcGroup.Players.Save();
            
            dstGroup.Players.Remove(dst);
            dstGroup.Players.Add(src);
            dstGroup.Players.Save();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/InfoSwap [source] [other]");
            Player.Message(p, "%HSwaps all the player's info from [source] to [other].");
            Player.Message(p, "%HNote that both players must be offline for this to work.");
        }
    }
}
