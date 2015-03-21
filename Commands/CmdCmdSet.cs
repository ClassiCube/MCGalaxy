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
    public sealed class CmdCmdSet : Command
    {
        public override string name { get { return "cmdset"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdCmdSet() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }

            string foundBlah = Command.all.FindShort(message.Split(' ')[0]);

            Command foundCmd;
            if (foundBlah == "") foundCmd = Command.all.Find(message.Split(' ')[0]);
            else foundCmd = Command.all.Find(foundBlah);

            if (foundCmd == null) { Player.SendMessage(p, "Could not find command entered"); return; }
            if (p != null && !p.group.CanExecute(foundCmd)) { Player.SendMessage(p, "This command is higher than your rank."); return; }

            LevelPermission newPerm = Level.PermissionFromName(message.Split(' ')[1]);
            if (newPerm == LevelPermission.Null) { Player.SendMessage(p, "Could not find rank specified"); return; }
            if (p != null && newPerm > p.group.Permission) { Player.SendMessage(p, "Cannot set to a rank higher than yourself."); return; }

            GrpCommands.rankAllowance newCmd = GrpCommands.allowedCommands.Find(rA => rA.commandName == foundCmd.name);
            newCmd.lowestRank = newPerm;
            GrpCommands.allowedCommands[GrpCommands.allowedCommands.FindIndex(rA => rA.commandName == foundCmd.name)] = newCmd;

            GrpCommands.Save(GrpCommands.allowedCommands);
            GrpCommands.fillRanks();
            Player.GlobalMessage("&d" + foundCmd.name + Server.DefaultColor + "'s permission was changed to " + Level.PermissionToName(newPerm));
            //if (p == null) ; // this is useless?
            //{
                Player.SendMessage(p, foundCmd.name + "'s permission was changed to " + Level.PermissionToName(newPerm));
                return;
            //}
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/cmdset [cmd] [rank] - Changes [cmd] rank to [rank]");
            Player.SendMessage(p, "Only commands you can use can be modified");
            Player.SendMessage(p, "Available ranks: " + Group.concatList());
        }
    }
}