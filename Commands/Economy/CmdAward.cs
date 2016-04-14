/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy.Commands
{
    public sealed class CmdAward : Command
    {
        public override string name { get { return "award"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdAward() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }

            bool give = true;
            if (message.Split(' ')[0].ToLower() == "give")
            {
                give = true;
                message = message.Substring(message.IndexOf(' ') + 1);
            }
            else if (message.Split(' ')[0].ToLower() == "take")
            {
                give = false;
                message = message.Substring(message.IndexOf(' ') + 1);
            }
            
            string foundPlayer = message.Split(' ')[0];
            Player who = PlayerInfo.Find(message);
            if (who != null) foundPlayer = who.name;
            string awardName = message.Substring(message.IndexOf(' ') + 1);
            if (!Awards.ExistsAward(awardName))
            {
                Player.SendMessage(p, "The award you entered doesn't exist");
                Player.SendMessage(p, "Use /awards for a list of awards");
                return;
            }

            if (give)
            {
                if (Awards.GiveAward(foundPlayer, awardName))
                {
                    Player.GlobalMessage(Server.FindColor(foundPlayer) + foundPlayer + " %Swas awarded: &b" + awardName);
                }
                else
                {
                    Player.SendMessage(p, "The player already has that award!");
                }
            }
            else
            {
                if (Awards.TakeAward(foundPlayer, awardName))
                {
                    Player.GlobalMessage(Server.FindColor(foundPlayer) + foundPlayer + " %Shad their &b" + awardName + " %Saward removed");
                }
                else
                {
                    Player.SendMessage(p, "The player didn't have the award you tried to take");
                }
            }

            Awards.Save();
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/award <give/take> [player] [award] - Gives [player] the [award]");
            Player.SendMessage(p, "If no Give or Take is given, Give is used");
            Player.SendMessage(p, "[award] needs to be the full award's name. Not partial");
        }
    }
}
