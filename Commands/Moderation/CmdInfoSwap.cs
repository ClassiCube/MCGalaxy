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
using MCGalaxy.SQL;

namespace MCGalaxy.Commands {
    
    public sealed class CmdInfoSwap : Command {
        
        public override string name { get { return "infoswap"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Nobody; } }

        public override void Use(Player p, string text) {
            string[] args = text.Split(' ');
            if (args.Length != 2) { Help(p); return; }
            if (!Player.ValidName(args[0])) {
                Player.SendMessage(p, "\"" + args[0] + "\" is not a valid player name."); return;
            }
            if (!Player.ValidName(args[1])) {
                Player.SendMessage(p, "\"" + args[1] + "\" is not a valid player name."); return;
            }
            
            if (PlayerInfo.FindExact(args[0]) != null) {
                Player.SendMessage(p, "\"" + args[0] + "\" must be offline to use /infoswap."); return;
            }
            if (PlayerInfo.FindExact(args[1]) != null) {
                Player.SendMessage(p, "\"" + args[1] + "\" must be offline to use /infoswap."); return;
            }
            
            OfflinePlayer src = PlayerInfo.FindOffline(args[0], true);
            if (src == null) {
                Player.SendMessage(p, "\"" + args[0] + "\" was not found in the database."); return;
            }
            OfflinePlayer dst = PlayerInfo.FindOffline(args[1], true);
            if (dst == null) {
                Player.SendMessage(p, "\"" + args[1] + "\" was not found in the database."); return;
            }
            
            Swap(src, dst); Swap(dst, src);
            SwapGroups(src, dst);
        }
        
        const string format = "yyyy-MM-dd HH:mm:ss";
        void Swap(OfflinePlayer src, OfflinePlayer dst) {
            ParameterisedQuery query = ParameterisedQuery.Create();
            query.AddParam("@Name", dst.name);
            query.AddParam("@Blocks", src.blocks);
            query.AddParam("@Color", Colors.Name(src.color));
            query.AddParam("@Deaths", src.deaths);
            query.AddParam("@First", DateTime.Parse(src.firstLogin).ToString(format));
            query.AddParam("@IP", src.ip);
            query.AddParam("@Kicks", src.kicks);
            query.AddParam("@Last", DateTime.Parse(src.lastLogin).ToString(format));
            query.AddParam("@Logins", src.logins);
            query.AddParam("@Money", src.money);
            query.AddParam("@Title", src.title);
            query.AddParam("@TColor", Colors.Name(src.titleColor));
            query.AddParam("@Time", src.totalTime);
            
            Database.executeQuery(query, "UPDATE Players SET totalBlocks=@Blocks,color=@Color,"
                                  + "totalDeaths=@Deaths,FirstLogin=@First,IP=@IP,"
                                  + "totalKicked=@Kicks,LastLogin=@Last,totalLogin=@Logins,"
                                  + "Money=@Money,Title=@Title,title_color=@TColor,TimeSpent=@Time"
                                  + " WHERE Name=@Name");
        }
        
        void SwapGroups(OfflinePlayer src, OfflinePlayer dst) {
            Group srcGroup = Group.findPlayerGroup(src.name);
            Group dstGroup = Group.findPlayerGroup(dst.name);
            
            srcGroup.playerList.Remove(src.name);
            srcGroup.playerList.Add(dst.name);
            srcGroup.playerList.Save();
            
            dstGroup.playerList.Remove(dst.name);
            dstGroup.playerList.Add(src.name);
            dstGroup.playerList.Save();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/infoswap [source] [other]");
            Player.SendMessage(p, "%HSwaps all the player's info from [source] to [other].");
            Player.SendMessage(p, "%HNote that both players must be offline for this to work.");
        }
    }
}
