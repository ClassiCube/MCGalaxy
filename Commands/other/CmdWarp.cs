/*
	Copyright 2011 MCForge
		
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
using System.Threading;
namespace MCGalaxy.Commands
{
    public sealed class CmdWarp : Command
    {
        public override string name { get { return "warp"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] {
                    new CommandPerm(LevelPermission.Operator, "+ can create warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can delete warps"),
                    new CommandPerm(LevelPermission.Operator, "+ can move/edit warps"),
                }; }
        }
        
        public override void Use(Player p, string message)
        {
            if (p == null) { MessageInGameOnly(p); return; }
            WarpList warps = WarpList.Global;
            string[] command = message.ToLower().Split(' ');
            string par0 = String.Empty;
            string par1 = String.Empty;
            string par2 = String.Empty;
            try
            {
                par0 = command[0];
                par1 = command[1];
                par2 = command[2];
            }
            catch { }
            if (par0 == "list" || par0 == "view" || par0 == "l" || par0 == "v")
            {
                Player.Message(p, "Warps:");
                foreach (Warp wr in warps.Items)
                {
                    if (LevelInfo.FindExact(wr.lvlname) != null)
                    {
                        Player.Message(p, wr.name + " : " + wr.lvlname);
                        Thread.Sleep(300); // I feel this is needed so that if there are a lot of warps, they do not immediatly go off the screen!
                    }
                }
                return;
            }

            if (par0 == "create" || par0 == "add" || par0 == "c" || par0 == "a")
            {
            	if (CheckExtraPerm(p, 1))
                {
                    if (par1 == null) { Player.Message(p, "You didn't specify a name for the warp!"); return; }
                    if (warps.Exists(par1)) { Player.Message(p, "Warp has already been created!!"); return; }
                    {
                        if (par2 == null) { warps.Create(par1, p); }
                        else { warps.Create(par1, PlayerInfo.Find(par2)); }
                    }
                    {
                        if (warps.Exists(par1))
                        {
                            Player.Message(p, "Warp created!");
                            return;
                        }
                        else
                        {
                            Player.Message(p, "Warp creation failed!!");
                            return;
                        }
                    }
                }
                else { MessageNeedExtra(p, "can create warps.", 1); return; }
            }

            if (par0 == "delete" || par0 == "remove" || par0 == "d" || par0 == "r")
            {
                if (CheckExtraPerm(p, 2))
                {
                    if (par1 == null) { Player.Message(p, "You didn't specify a warp to delete!"); return; }
                    if (!warps.Exists(par1)) { Player.Message(p, "Warp doesn't exist!!"); return; }
                    {
                        warps.Remove(par1, p);
                    }
                    {
                        if (!warps.Exists(par1))
                        {
                            Player.Message(p, "Warp deleted!");
                            return;
                        }
                        else
                        {
                            Player.Message(p, "Warp deletion failed!!");
                            return;
                        }
                    }
                }
                else { MessageNeedExtra(p, "can delete warps.", 2); return; }
            }

            if (par0 == "move" || par0 == "change" || par0 == "edit" || par0 == "m" || par0 == "e")
            {
                if (CheckExtraPerm(p, 3))
                {
                    if (par1 == null) { Player.Message(p, "You didn't specify a warp to be moved!"); return; }
                    if (!warps.Exists(par1)) { Player.Message(p, "Warp doesn't exist!!"); return; }
                    {
                        if (par2 == null) { warps.Update(par1, p); }
                        else { warps.Update(par1, PlayerInfo.Find(par2)); }
                    }
                    {
                        if (warps.Exists(par1))
                        {
                            Player.Message(p, "Warp moved!");
                            return;
                        }
                        else
                        {
                            Player.Message(p, "Warp moving failed!!");
                            return;
                        }
                    }
                }
                else { MessageNeedExtra(p, "can move warps.", 3); return; }
            }

            else
            {
                if (warps.Exists(par0))
                {
                    warps.Goto(par0, p);
                }
                else
                {
                    Player.Message(p, "That is not a command addition or a warp");
                    return;
                }
            }
        }
        public override void Help(Player p)
        {
            Player.Message(p, "/warp [name] - warp to that warp");
            Player.Message(p, "/warp list - list all the warps");
            if (CheckExtraPerm(p, 1))
                Player.Message(p, "/warp create [name] <player> - create a warp, if a <player> is given, it will be created where they are");
            if (CheckExtraPerm(p, 2))
                Player.Message(p, "/warp delete [name] - delete a warp");
            if (CheckExtraPerm(p, 3))
                Player.Message(p, "/warp move [name] <player> - move a warp, if a <player> is given, it will be created where they are");
        }
    }
}
