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
        public override string name { get { return "infoswap"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }

        public override void Use(Player p, string text)
        {
            string[] args = text.SplitSpaces();
            if (args.Length != 2) { Help(p); return; }
            if (!Formatter.ValidName(p, args[0], "player")) return;
            if (!Formatter.ValidName(p, args[1], "player")) return;
            
            if (PlayerInfo.FindExact(args[0]) != null) {
                Player.Message(p, "\"{0}\" must be offline to use /infoswap.", args[0]); return;
            }
            if (PlayerInfo.FindExact(args[1]) != null) {
                Player.Message(p, "\"{0}\" must be offline to use /infoswap.", args[1]); return;
            }
            
            PlayerData src = PlayerInfo.FindData(args[0]);
            if (src == null) {
                Player.Message(p, "\"{0}\" was not found in the database.", args[0]); return;
            }
            PlayerData dst = PlayerInfo.FindData(args[1]);
            if (dst == null) {
                Player.Message(p, "\"{0}\" was not found in the database.", args[1]); return;
            }

            Group srcGroup = Group.findPlayerGroup(src.Name);
            Group dstGroup = Group.findPlayerGroup(dst.Name);
            if (p != null && srcGroup.Permission >= p.Rank) {
                Player.Message(p, "Cannot /infoswap for a player ranked equal or higher to yours."); return;
            }
            if (p != null && dstGroup.Permission >= p.Rank) {
                Player.Message(p, "Cannot /infoswap for a player ranked equal or higher to yours."); return;
            }
            
            SetData(src, dst.Name); SetData(dst, src.Name);
            SwapGroups(src, dst, srcGroup, dstGroup);
            
            Player.Message(p, "Successfully infoswapped {0} %Sand {1}",
                           PlayerInfo.GetColoredName(p, src.Name),
                           PlayerInfo.GetColoredName(p, dst.Name));
        }
        
        const string format = "yyyy-MM-dd HH:mm:ss";
        void SetData(PlayerData src, string dstName) {
            string first = src.FirstLogin.ToString(format);
            string last = src.LastLogin.ToString(format);
            long blocks = PlayerData.BlocksPacked(src.TotalPlaced, src.TotalModified);
            long cuboided = PlayerData.CuboidPacked(src.TotalDeleted, src.TotalDrawn);
            
            const string columns = "totalBlocks=@0, totalCuboided=@1, color=@2" +
                ", totalDeaths=@3, FirstLogin=@4, IP=@5, totalKicked=@6, LastLogin=@7" +
                ", totalLogin=@8, Money=@9, Title=@10, title_color=@11, TimeSpent=@12";
            Database.Backend.UpdateRows(
                "Players", columns, "WHERE Name=@13",
                blocks, cuboided, src.Color,
                src.Deaths, first, src.IP, src.kicks, last, src.Logins,
                src.Money, src.Title, src.TitleColor, src.TotalTime, dstName);
        }
        
        void SwapGroups(PlayerData src, PlayerData dst, Group srcGroup, Group dstGroup) {
            srcGroup.playerList.Remove(src.Name);
            srcGroup.playerList.Add(dst.Name);
            srcGroup.playerList.Save();
            
            dstGroup.playerList.Remove(dst.Name);
            dstGroup.playerList.Add(src.Name);
            dstGroup.playerList.Save();
        }
        
        public override void Help(Player p) {
            Player.Message(p, "%T/infoswap [source] [other]");
            Player.Message(p, "%HSwaps all the player's info from [source] to [other].");
            Player.Message(p, "%HNote that both players must be offline for this to work.");
        }
    }
}
