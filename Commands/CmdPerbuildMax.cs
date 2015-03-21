/*
    Written by Jack1312
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
namespace MCGalaxy.Commands
{
    public sealed class CmdPerbuildMax : Command
    {
        public override string name { get { return "perbuildmax"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "pbm"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdPerbuildMax() { }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/perbuildmax [Level] [Rank] - Sets the highest rank able to visit [Level].");
        }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            int number = message.Split(' ').Length;
            if (number > 2 || number < 1) { Help(p); return; }
            if (number == 1)
            {
                LevelPermission Perm = Level.PermissionFromName(message);
                if (Perm == LevelPermission.Null) { Player.SendMessage(p, "Not a valid rank"); return; }
                if (p.level.perbuildmax > p.group.Permission)
                {
                    if (p.level.perbuildmax != LevelPermission.Nobody)
                    {
                        Player.SendMessage(p, "You cannot change the perbuildmax of a level with a perbuildmax higher than your rank.");
                        return;
                    }
                }
                p.level.perbuildmax = Perm;
                Level.SaveSettings(p.level);
                Server.s.Log(p.level.name + " buildmax permission changed to " + message + ".");
                Player.GlobalMessageLevel(p.level, "buildmax permission changed to " + message + ".");
            }
            else
            {
                int pos = message.IndexOf(' ');
                string t = message.Substring(0, pos).ToLower();
                string s = message.Substring(pos + 1).ToLower();
                LevelPermission Perm = Level.PermissionFromName(s);
                if (Perm == LevelPermission.Null) { Player.SendMessage(p, "Not a valid rank"); return; }

                Level level = Level.Find(t);
                if (level.perbuildmax > p.group.Permission)
                {
                    if (level.perbuildmax != LevelPermission.Nobody)
                    {
                        Player.SendMessage(p, "You cannot change the perbuildmax of a level with a perbuildmax higher than your rank.");
                        return;
                    }
                }
                if (level != null)
                {
                    level.perbuildmax = Perm;
                    Level.SaveSettings(level);
                    Server.s.Log(level.name + " buildmax permission changed to " + s + ".");
                    Player.GlobalMessageLevel(level, "buildmax permission changed to " + s + ".");
                    if (p != null)
                        if (p.level != level) { Player.SendMessage(p, "buildmax permission changed to " + s + " on " + level.name + "."); }
                    return;
                }
                else
                    Player.SendMessage(p, "There is no level \"" + s + "\" loaded.");
            }
        }
    }
}