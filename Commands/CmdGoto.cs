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
using System;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdGoto : Command
    {
        public override string name { get { return "goto"; } }
        public override string shortcut { get { return "g"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdGoto() { }

        public override void Use(Player p, string message)
        {
            if (p == null) { Player.SendMessage(p, "This command can only be used in-game!"); return; }
            if (message == "") { Help(p); return; }

            try
            {
                Level foundLevel = Level.Find(message);
                if (foundLevel != null)
                {
                    Level startLevel = p.level;

                    GC.Collect();

                    if (p.level == foundLevel) { Player.SendMessage(p, "You are already in \"" + foundLevel.name + "\"."); return; }
                    bool blacklisted = Player.BlacklistCheck(p.name, message);
                    if (blacklisted == true) { Player.SendMessage(p, "You are blacklisted from " + foundLevel.name + "."); return; }
                    if (!p.ignorePermission)
                        if (p.group.Permission < foundLevel.permissionvisit) { Player.SendMessage(p, "You're not allowed to go to " + foundLevel.name + "."); return; }
                    if (!p.ignorePermission)
                        if (p.group.Permission > foundLevel.pervisitmax) { if (!p.group.CanExecute(Command.all.Find("pervisitmax"))) { Player.SendMessage(p, "Your rank must be " + foundLevel.pervisitmax + " or lower to go there!"); return; } }
                    {
                        if (!File.Exists("text/lockdown/map/" + message + ""))
                        {

                            p.Loading = true;
                            foreach (Player pl in Player.players) if (p.level == pl.level && p != pl) p.SendDie(pl.id);
                            foreach (PlayerBot b in PlayerBot.playerbots) if (p.level == b.level) p.SendDie(b.id);

                            Player.GlobalDie(p, true);
                            p.level = foundLevel; p.SendUserMOTD(); p.SendMap();

                            GC.Collect();

                            ushort x = (ushort)((0.5 + foundLevel.spawnx) * 32);
                            ushort y = (ushort)((1 + foundLevel.spawny) * 32);
                            ushort z = (ushort)((0.5 + foundLevel.spawnz) * 32);

                            if (!p.hidden) Player.GlobalSpawn(p, x, y, z, foundLevel.rotx, foundLevel.roty, true, "");
                            else unchecked { p.SendPos((byte)-1, x, y, z, foundLevel.rotx, foundLevel.roty); }

                            foreach (Player pl in Player.players)
                                if (pl.level == p.level && p != pl && !pl.hidden)
                                    p.SendSpawn(pl.id, pl.color + pl.name, pl.pos[0], pl.pos[1], pl.pos[2], pl.rot[0], pl.rot[1]);

                            foreach (PlayerBot b in PlayerBot.playerbots)
                                if (b.level == p.level)
                                    p.SendSpawn(b.id, b.color + b.name, b.pos[0], b.pos[1], b.pos[2], b.rot[0], b.rot[1]);

                            if (!p.hidden) Player.GlobalChat(p, p.color + "*" + p.DisplayName + Server.DefaultColor + " went to &b" + foundLevel.name, false);
                                      

                            p.Loading = false;

                            bool skipUnload = false;
                            if (startLevel.unload && !startLevel.name.Contains("&cMuseum "))
                            {
                                foreach (Player pl in Player.players) if (pl.level == startLevel) { skipUnload = true; break; }
                                if (!skipUnload && Server.AutoLoad) startLevel.Unload(true);
                            }

                            if (Server.lava.active && !Server.lava.sendingPlayers && Server.lava.map == foundLevel)
                            {
                                if (Server.lava.roundActive)
                                {
                                    Server.lava.AnnounceRoundInfo(p);
                                    Server.lava.AnnounceTimeLeft(!Server.lava.flooded, true, p);
                                }
                                else
                                {
                                    Player.SendMessage(p, "Vote for the next map!");
                                    Player.SendMessage(p, "Choices: " + Server.lava.VoteString);
                                }
                            }

                            if (Server.zombie.GameInProgess())
                            {
                                if (p.level.name == Server.zombie.currentLevelName)
                                    Server.zombie.InfectedPlayerLogin(p);
                            }

                            if (p.level.name != Server.zombie.currentLevelName)
                            {
                                if(ZombieGame.alive.Contains(p))
                                ZombieGame.alive.Remove(p);
                                if (ZombieGame.infectd.Contains(p))
                                ZombieGame.infectd.Remove(p);
                            }
                            if (p.inTNTwarsMap)
                            {
                                p.canBuild = true;
                            }
                            if (TntWarsGame.Find(p.level) != null)
                            {
                                if (TntWarsGame.Find(p.level).GameStatus != TntWarsGame.TntWarsGameStatus.Finished && TntWarsGame.Find(p.level).GameStatus != TntWarsGame.TntWarsGameStatus.WaitingForPlayers)
                                {
                                    p.canBuild = false;
                                    Player.SendMessage(p, "TNT Wars: Disabled your building because you are in a TNT Wars map!");
                                }

                                p.inTNTwarsMap = true;
                            }
                            Server.IRC.Say(p.color + p.DisplayName + " %rwent to &8" + foundLevel.name, false, true);
                        }
                        else Player.SendMessage(p, "The level " + message + " is locked.");
                    }
                }
                else if (Server.AutoLoad)
                {
                    if (!File.Exists("levels/" + message + ".lvl"))
                    {
                        Player.SendMessage(p, "Level \"" + message + "\" doesn't exist! Did you mean...");
                        Command.all.Find("search").Use(p, "levels " + message);
                    }
                    else if (Level.Find(message) != null || Level.CheckLoadOnGoto(message))
                    {
                        Command.all.Find("load").Use(p, message);
                        foundLevel = Level.Find(message);
                        if (foundLevel != null) Use(p, message);
                    }
                    else
                        Player.SendMessage(p, "Level \"" + message + "\" cannot be loaded using /goto!");
                }
                else 
                { 
                    Player.SendMessage(p, "There is no level \"" + message + "\" loaded. Did you mean..");
                    Command.all.Find("search").Use(p, "levels " + message);
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch (Exception e) { Server.ErrorLog(e); }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/goto <mapname> - Teleports yourself to a different level.");
        }
    }
}
