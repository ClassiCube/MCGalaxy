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
using System;
using System.Globalization;
using System.IO;
using MCGalaxy.Util;

namespace MCGalaxy.Commands
{
    public sealed class CmdUndo : Command
    {
        public override string name { get { return "undo"; } }
        public override string shortcut { get { return "u"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdUndo() { }

        int MAX = -1; // This is the value changed to MAX in the Undo list, and used to allow everything undone.

        public override void Use(Player p, string message)
        {
            byte b; long seconds = -2; Player who = null; Player.UndoPos Pos; int CurrentPos = 0; bool undoPhysics = false; string whoName = String.Empty;
            if (p != null)
                p.RedoBuffer.Clear();
            if (message == "") {
                if (p == null) {
                    Player.SendMessage(null, "Console doesn't have an undo buffer."); return;
                }
                message = p.name.ToLower() + " 30";
            }
            string[] parts = message.Split(' ');

            try {
                if (parts.Length > 1) {
                    whoName = parts[0];
                    who = parts[0].ToLower() == "physics" ? null : Player.Find(parts[0]);
                    undoPhysics = parts[0].ToLower() == "physics";
                    message = parts[1].ToLower();
                } else {
                    who = (p == null || message.ToLower() == "physics") ? null : p;
                    undoPhysics = message.ToLower() == "physics";
                }
                //If user is undoing him/herself, then all is go.
                //If user is undoing someone else, then restrictions are used.
                if (p == who)
                    seconds = ((message.ToLower() != "all") ? long.Parse(message) : int.MaxValue);
                else
                    seconds = getAllowed(p, message.ToLower());
            } catch {
            	if (parts.Length > 1 && parts[1].ToLower() == "update") {
            		UndoFile.UpgradePlayerUndoFiles(whoName);
            		Player.SendMessage(p, "Updated undo files for " + whoName + " to the new binary format.");
            		return;
            	}
                Player.SendMessage(p, "Invalid seconds, or you're unable to use /xundo. Using 30 seconds."); //only run if seconds is an invalid number
                seconds = 30;
            }

            //At this point, we know the number is valid, and allowed for the particular person's group.
            if (who != null)
            {
                if (p != null)
                {
                    if (who.group.Permission > p.group.Permission && who != p) { Player.SendMessage(p, "Cannot undo a user of higher or equal rank"); return; }
                    if (who != p && (int)p.group.Permission < CommandOtherPerms.GetPerm(this, 1)) { Player.SendMessage(p, "Only an " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 1)).name + "+ may undo other people's actions"); return; }
                }
                Level saveLevel = null;
                for (CurrentPos = who.UndoBuffer.Count - 1; CurrentPos >= 0; --CurrentPos)
                {
                    try
                    {
                        Pos = who.UndoBuffer[CurrentPos];
                        Level foundLevel = Level.FindExact(Pos.mapName);
                        saveLevel = foundLevel;
                        b = foundLevel.GetTile(Pos.x, Pos.y, Pos.z);
                        if (Pos.timePlaced.AddSeconds(seconds) >= DateTime.Now)
                        {
                            if (b == Pos.newtype || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava)
                            {
                                foundLevel.Blockchange(Pos.x, Pos.y, Pos.z, Pos.type, true);

                                Pos.newtype = Pos.type; Pos.type = b;
                                if (p != null) p.RedoBuffer.Add(Pos);
                                who.UndoBuffer.RemoveAt(CurrentPos);
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch { }
                }
                try
                {
                    bool fake = false;
                    undoOfflineHelper(p, who.name, seconds, ref fake);
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                }

                if (p == who)
                {
                    Player.SendMessage(p, "Undid your actions for the past &b" + seconds + Server.DefaultColor + " seconds.");
                }
                else
                {
                    Player.GlobalChat(who, who.color + who.DisplayName + Server.DefaultColor + "'s actions for the past &b" + seconds + " seconds were undone.", false);
                    // Also notify console
                    Server.s.Log(who.name + "'s actions for the past " + seconds + " seconds were undone.");
                }
                // Don't forget to save the map;) in case someone unloads it without a manual blockchange:D
                if (saveLevel != null) saveLevel.Save(true);
                return;
            }
            else if (undoPhysics)
            {
                if (p != null && (int)p.@group.Permission < CommandOtherPerms.GetPerm(this, 2)) { Player.SendMessage(p, "Reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this, 2)).name + "+"); return; }
                if (p != null && !p.@group.CanExecute(Command.all.Find("physics"))) { Player.SendMessage(p, "You can only undo physics if you can use them."); return; }

                Command.all.Find("physics").Use(p, "0");
                Level.UndoPos uP;
                ushort x, y, z;

                if (p.level.UndoBuffer.Count != Server.physUndo)
                {
                    for (CurrentPos = p.level.currentUndo; CurrentPos >= 0; CurrentPos--)
                    {
                        try
                        {
                            uP = p.level.UndoBuffer[CurrentPos];
                            b = p.level.GetTile(uP.location);
                            if (uP.timePerformed.AddSeconds(seconds) < DateTime.Now)
                            {
                                break;
                            }
                            if (b == uP.newType || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava)
                            {
                                p.level.IntToPos(uP.location, out x, out y, out z);
                                p.level.Blockchange(p, x, y, z, uP.oldType);
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    for (CurrentPos = p.level.currentUndo; CurrentPos != p.level.currentUndo + 1; CurrentPos--)
                    {
                        try
                        {
                            if (CurrentPos < 0) CurrentPos = p.level.UndoBuffer.Count - 1;
                            uP = p.level.UndoBuffer[CurrentPos];
                            b = p.level.GetTile(uP.location);
                            if (uP.timePerformed.AddSeconds(seconds) < DateTime.Now)
                            {
                                break;
                            }
                            if (b == uP.newType || Block.Convert(b) == Block.water || Block.Convert(b) == Block.lava)
                            {
                                p.level.IntToPos(uP.location, out x, out y, out z);
                                p.level.Blockchange(p, x, y, z, uP.oldType, true);
                            }
                        }
                        catch { }
                    }
                }

                Player.GlobalMessage("Physics were undone &b" + seconds + Server.DefaultColor + " seconds");
                // Also notify console
                Player.SendMessage(null, "Physics were undone &b" + seconds + Server.DefaultColor + " seconds");
                // Don't forget to save the map;) in case someone unloads it without a manual blockchange:D
                p.level.Save(true);
            }
            else
            { // Here, who == null, meaning the user specified is offline
                if (p != null)
                {
                    if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this)) { Player.SendMessage(p, "Reserved for " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + "+"); return; }
                    // ^^^ is using the same as the 1st other permission for the this command because the only difference is that this is for offline players so it might aswell be the same!!
                }

                bool FoundUser = false;
                try
                {
                    UndoFile.UndoPlayer(p, whoName.ToLower(), seconds, ref FoundUser);

                    if (FoundUser)
                    {
                        Player.GlobalMessage(Server.FindColor(whoName) + whoName + Server.DefaultColor + "'s actions for the past &b" + seconds + Server.DefaultColor + " seconds were undone.");
                        // Also notify console
                        Server.s.Log(whoName + "'s actions for the past " + seconds + " seconds were undone.");
                        // Don't forget to save the map;) in case someone unloads it without a manual blockchange:D
                        p.level.Save(true);
                    }
                    else Player.SendMessage(p, "Could not find player specified.");
                }
                catch (Exception e)
                {
                    Server.ErrorLog(e);
                }
            }
        }

        private long getAllowed(Player p, string param)
        {
            long secs;
            if (param == "all" && p.group.CanExecute(Command.all.Find("xundo")))
                secs = (p.@group.maxUndo == MAX) ? int.MaxValue : p.group.maxUndo;
            else
                secs = long.Parse(param); //caught by try/catch in outer method

            if (secs == 0) secs = 5400;

            if (p != null && p.group.maxUndo != MAX && secs > p.group.maxUndo)
            {
                Player.SendMessage(p, p.group.name + "s may only undo up to " + p.group.maxUndo + " seconds.");
                return p.group.maxUndo;
            }
            return secs;
        }
        
        private void undoOfflineHelper(Player p, string whoName, long seconds, ref bool FoundUser)  {
        	UndoFile.UndoPlayer(p, whoName.ToLower(), seconds, ref FoundUser);
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/undo [player] [seconds] - Undoes the blockchanges made by [player] in the previous [seconds].");
            if (p == null || (p.group.maxUndo <= 500000 || p.group.maxUndo == 0))
                Player.SendMessage(p, "/undo [player] all - &cWill undo 68 years, 18 days, 15 hours, 28 minutes, 31 seconds for [player]");
            if (p == null || (p.group.maxUndo <= 1800 || p.group.maxUndo == 0))
                Player.SendMessage(p, "/undo [player] 0 - &cWill undo 30 minutes");
            Player.SendMessage(p, "/undo physics [seconds] - Undoes the physics for the current map");
        }
    }
}
