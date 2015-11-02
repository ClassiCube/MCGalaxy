/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl) Licensed under the
	Educational Community License, Version 2.0 (the "License"); you may
	not use this file except in compliance with the License. You may
	obtain a copy of the License at
	
	http://www.osedu.org/licenses/ECL-2.0
	
	Unless required by applicable law or agreed to in writing,
	software distributed under the License is distributed on an "AS IS"
	BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
	or implied. See the License for the specific language governing
	permissions and limitations under the License.
*/
namespace MCGalaxy.Commands
{
    public sealed class CmdPlayerBlock : Command
    {
        public override string name { get { return "playerblock"; } }
        public override string shortcut { get { return "pblock"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPlayerBlock() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            Player who = Player.Find(message.Split(' ')[0]);
            if (who == null) { Player.SendMessage(p, "Could not find player"); return; }
            try
            {
                if (message.Split(' ').Length == 2)
                {
                    who.blockCount = int.Parse(message.Split(' ')[1]);
                }
            }
            finally
            {
                p.SendMessage(who.color + who.name + Server.DefaultColor + " has " + who.blockCount.ToString() + " blocks.");
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/playerblock <player> [blocks] - sets <player>'s block count to [blocks]");
            Player.SendMessage(p, "/playerblock <player> - shows you <player>'s block count");
        }
    }
}
