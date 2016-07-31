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
            if (!ValidName(p, args[0], "player")) return;
            if (!ValidName(p, args[1], "player")) return;
            
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
            
            Swap(src, dst); Swap(dst, src);
            SwapGroups(src, dst);
        }
        
        const string format = "yyyy-MM-dd HH:mm:ss";
        void Swap(PlayerData src, PlayerData dst) {
            string first = src.FirstLogin.ToString(format);
            string last = src.LastLogin.ToString(format);
            const string syntax = "UPDATE Players SET totalBlocks=@0, totalCuboided=@1" +
            	", color=@2, totalDeaths=@3, FirstLogin=@4, IP=@5, totalKicked=@6, LastLogin=@7" +
            	", totalLogin=@8, Money=@9, Title=@10, title_color=@11, TimeSpent=@12 WHERE Name=@13";
            
            Database.Execute(syntax, src.Blocks, src.Cuboided, src.Color, src.Deaths,
                             first, src.IP, src.Kicks, last, src.Logins,
                             src.Money, src.Title, src.TitleColor, src.TotalTime, dst.Name);            
        }
        
        void SwapGroups(PlayerData src, PlayerData dst) {
            Group srcGroup = Group.findPlayerGroup(src.Name);
            Group dstGroup = Group.findPlayerGroup(dst.Name);
            
            srcGroup.playerList.Remove(src.Name);
            srcGroup.playerList.Add(dst.Name);
            srcGroup.playerList.Save();
            
            dstGroup.playerList.Remove(dst.Name);
            dstGroup.playerList.Add(src.Name);
            dstGroup.playerList.Save();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%T/infoswap [source] [other]");
            Player.SendMessage(p, "%HSwaps all the player's info from [source] to [other].");
            Player.SendMessage(p, "%HNote that both players must be offline for this to work.");
        }
    }
}
