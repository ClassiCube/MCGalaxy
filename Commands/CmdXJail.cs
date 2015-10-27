/*
	Copyright 2011 MCGalaxy
		
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
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdXJail : Command
    {
        public override string name { get { return "xjail"; } }
        public override string shortcut { get { return "xj"; } }
        public override string type { get { return "other"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool museumUsable { get { return true; } }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/xjail <player> - Mutes <player>, freezes <player> and sends <player> to the XJail map (shortcut = /xj)");
            Player.SendMessage(p, "If <player> is already jailed, <player> will be spawned, unfrozen and unmuted");
            Player.SendMessage(p, "/xjail set - Sets the map to be used for xjail to your current map and sets jail to current location");
        }
        public override void Use(Player p, string message)
        {
            string dir = "extra/jail/";
            string jailMapFile = dir + "xjail.map.xjail";
            if (!Directory.Exists(dir)) { Directory.CreateDirectory(dir); }
            if (!File.Exists(jailMapFile))
            {
                using (StreamWriter SW = new StreamWriter(jailMapFile))
                {
                    SW.WriteLine(Server.mainLevel.name);
                }
            }
            if (message == "") { Help(p); return; }
            else
            {
                using (StreamReader SR = new StreamReader(jailMapFile))
                {
                    string xjailMap = SR.ReadLine();
                    SR.Close();
                    Command jail = Command.all.Find("jail");
                    if (message == "set")
                    {
                        if (!p.level.name.Contains("cMuseum"))
                        {
                            jail.Use(p, "create");
                            using (StreamWriter SW = new StreamWriter(jailMapFile))
                            {
                                SW.WriteLine(p.level.name);
                            }
                            Player.SendMessage(p, "The xjail map was set from '" + xjailMap + "' to '" + p.level.name + "'");
                            return;
                        }
                        else { Player.SendMessage(p, "You are in a museum!"); return; }
                    }
                    else
                    {
                        Player player = Player.Find(message);

                        if (player != null)
                        {
                            Command move = Command.all.Find("move");
                            Command spawn = Command.all.Find("spawn");
                            Command freeze = Command.all.Find("freeze");
                            Command mute = Command.all.Find("mute");
                            string playerFile = dir + player.name + "_temp.xjail";
                            if (!File.Exists(playerFile))
                            {
                                using (StreamWriter writeFile = new StreamWriter(playerFile))
                                {
                                    writeFile.WriteLine(player.level.name);
                                }
                                if (!player.muted) { mute.Use(p, message); }
                                if (!player.frozen) { freeze.Use(p, message); }
                                move.Use(p, message + " " + xjailMap);
                                while (player.Loading)
                                {
                                }
                                if (!player.jailed) { jail.Use(p, message); }
                                Player.GlobalMessage(player.color + player.DisplayName + Server.DefaultColor + " was XJailed!");
                                return;
                            }
                            else
                            {
                                using (StreamReader readFile = new StreamReader(playerFile))
                                {
                                    string playerMap = readFile.ReadLine();
                                    readFile.Close();
                                    File.Delete(playerFile);
                                    move.Use(p, message + " " + playerMap);
                                    while (player.Loading)
                                    {
                                    }
                                    mute.Use(p, message);
                                    jail.Use(p, message);
                                    freeze.Use(p, message);
                                    spawn.Use(player, "");
                                    Player.GlobalMessage(player.color + player.DisplayName + Server.DefaultColor + " was released from XJail!");
                                }
                                return;
                            }
                        }
                        else { Player.SendMessage(p, "Player not found"); return; }
                    }
                }
            }
        }
    }
}