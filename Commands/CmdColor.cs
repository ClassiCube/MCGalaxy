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
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    public sealed class CmdColor : Command {
        public override string name { get { return "color"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdColor() { }

        public override void Use(Player p, string message) {
            if ( message == "" || message.Split(' ').Length > 2 ) { Help(p); return; }
            int pos = message.IndexOf(' ');
            if ( pos != -1 ) {
                Player who = Player.Find(message.Substring(0, pos));

                if ( p != null && who.group.Permission > p.group.Permission ) { Player.SendMessage(p, "You cannot change the color of someone ranked higher than you!"); return; }

                if ( who == null ) { Player.SendMessage(p, "There is no player \"" + message.Substring(0, pos) + "\"!"); return; }

                if ( message.Substring(pos + 1) == "del" ) {
                    Database.AddParams("@Name", who.name);
                    Database.executeQuery("UPDATE Players SET color = '' WHERE name = @Name");
                    Player.GlobalChat(who, who.color + "*" + Name(who.name) + " color reverted to " + who.group.color + "their group's default" + Server.DefaultColor + ".", false);
                    who.color = who.group.color;

                    Player.GlobalDie(who, false);
                    Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
                    who.SetPrefix();
                    return;
                }
                string color = c.Parse(message.Substring(pos + 1));
                if ( color == "" ) { Player.SendMessage(p, "There is no color \"" + message + "\"."); }
                else if ( color == who.color ) { Player.SendMessage(p, who.name + " already has that color."); }
                else {
                    //Player.GlobalChat(who, p.color + "*" + p.name + "&e changed " + who.color + Name(who.name) +
                    //                  " color to " + color +
                    //                  c.Name(color) + "&e.", false);
                    Database.AddParams("@Color", c.Name(color));
                    Database.AddParams("@Name", who.name);
                    Database.executeQuery("UPDATE Players SET color = @Color WHERE name = @Name");

                    Player.GlobalChat(who, who.color + "*" + Name(who.name) + " color changed to " + color + c.Name(color) + Server.DefaultColor + ".", false);
                    if ( p == null ) {
                        Player.SendMessage(p, "*" + Name(who.name) + " color was changed to " + c.Name(color) + ".");
                    }
                    who.color = color;

                    Player.GlobalDie(who, false);
                    Player.GlobalSpawn(who, who.pos[0], who.pos[1], who.pos[2], who.rot[0], who.rot[1], false);
                    who.SetPrefix();
                }
            }
            else {
                if ( p != null ) {
                    if ( message == "del" ) {
                        Database.AddParams("@Name", p.name);
                        Database.executeQuery("UPDATE Players SET color = '' WHERE name = @Name");

                        Player.GlobalChat(p, p.color + "*" + Name(p.name) + " color reverted to " + p.group.color + "their group's default" + Server.DefaultColor + ".", false);
                        p.color = p.group.color;

                        Player.GlobalDie(p, false);
                        Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                        p.SetPrefix();
                        return;
                    }
                    string color = c.Parse(message);
                    if ( color == "" ) { Player.SendMessage(p, "There is no color \"" + message + "\"."); }
                    else if ( color == p.color ) { Player.SendMessage(p, "You already have that color."); }
                    else {
                        Database.AddParams("@Color", c.Name(color));
                        Database.AddParams("@Name", p.name);
                        Database.executeQuery("UPDATE Players SET color = @Color WHERE name = @Name");

                        Player.GlobalChat(p, p.color + "*" + Name(p.name) + " color changed to " + color + c.Name(color) + Server.DefaultColor + ".", false);
                        p.color = color;

                        Player.GlobalDie(p, false);
                        Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                        p.SetPrefix();
                    }
                }
            }
        }
        public override void Help(Player p) {
            Player.SendMessage(p, "/color [player] <color/del>- Changes the nick color.  Using 'del' removes color.");
            Player.SendMessage(p, "&0black &1navy &2green &3teal &4maroon &5purple &6gold &7silver");
            Player.SendMessage(p, "&8gray &9blue &alime &baqua &cred &dpink &eyellow &fwhite");
        }
        static string Name(string name) {
            string ch = name[name.Length - 1].ToString().ToLower();
            if ( ch == "s" || ch == "x" ) { return name + Server.DefaultColor + "'"; }
            else { return name + Server.DefaultColor + "'s"; }
        }
    }
}