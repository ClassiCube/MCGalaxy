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
    public sealed class CmdPermissionBuild : Command
    {
        public override string name { get { return "perbuild"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPermissionBuild() { }

        public override void Use(Player p, string message)
        {
            if (p != null)
            {
                if (message == "") { Help(p); return; }
                int number = message.Split(' ').Length;
                if (number > 2 || number < 1) { Help(p); return; }
                if (number == 1)
                {
                    LevelPermission Perm = Level.PermissionFromName(message);
                    if (Perm == LevelPermission.Null) { Player.SendMessage(p, "Not a valid rank"); return; }
                    if (p.level.permissionbuild > p.group.Permission)
                    {
                        Player.SendMessage(p, "You cannot change the perbuild of a level with a perbuild higher than your rank.");
                        return;
                    }
                    p.level.permissionbuild = Perm;
                    Level.SaveSettings(p.level);
                    Server.s.Log(p.level.name + " build permission changed to " + message + ".");
                    Player.GlobalMessageLevel(p.level, "build permission changed to " + message + ".");
                }
                else
                {
                    int pos = message.IndexOf(' ');
                    string t = message.Substring(0, pos).ToLower();
                    string s = message.Substring(pos + 1).ToLower();
                    LevelPermission Perm = Level.PermissionFromName(s);
                    if (Perm == LevelPermission.Null) { Player.SendMessage(p, "Not a valid rank"); return; }

                    Level level = Level.Find(t);
                    if (level != null)
                    {
                        if (level.permissionbuild > p.group.Permission)
                        {
                            Player.SendMessage(p, "You cannot change the perbuild of a level with a perbuild higher than your rank.");
                            return;
                        }
                        level.permissionbuild = Perm;
                        Level.SaveSettings(level);
                        Server.s.Log(level.name + " build permission changed to " + s + ".");
                        Player.GlobalMessageLevel(level, "build permission changed to " + s + ".");
                        if (p != null)
                            if (p.level != level) { Player.SendMessage(p, "build permission changed to " + s + " on " + level.name + "."); }
                        return;
                    }
                    else
                        Player.SendMessage(p, "There is no level \"" + s + "\" loaded.");
                }
            }
            else
            {
                    string[] args = message.Split(' ');

                    LevelPermission Perm = Level.PermissionFromName(args[1]);
                    if (Perm == LevelPermission.Null) { Player.SendMessage(p, "Not a valid rank"); return; }

                    Level level = Level.Find(args[0]);
                    if (level != null)
                    {
                        level.permissionbuild = Perm;
                        Level.SaveSettings(level);
                        Server.s.Log(level.name + " build permission changed to " + args[1] + ".");
                        Player.GlobalMessageLevel(level, "build permission changed to " + args[1] + ".");
                        if (p != null)
                            if (p.level != level) { Player.SendMessage(p, "build permission changed to " + args[1] + " on " + level.name + "."); }
                        return;
                    }
                    else
                        Server.s.Log("There is no level \"" + args[1] + "\" loaded.");
                }

        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/PerBuild <map> <rank> - Sets build permission for a map.");
        }
    }
}
