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
namespace MCGalaxy.Commands
{
    public sealed class CmdMap : Command
    {
        public override string name { get { return "map"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "mod"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMap() { }
        public static bool gettinginfo = false;

        public override void Use(Player p, string message)
        {
            if (message == "") message = p.level.name;

            Level foundLevel;

            if (message.IndexOf(' ') == -1)
            {
                foundLevel = Level.Find(message);
                if (foundLevel == null)
                {
                    if (p != null)
                    {
                        foundLevel = p.level;
                    }
                }
                else
                {
                    gettinginfo = true;
                    Player.SendMessage(p, "MOTD: &b" + foundLevel.motd);
                    Player.SendMessage(p, "Finite mode: " + FoundCheck(foundLevel, foundLevel.finite));
                    Player.SendMessage(p, "Random flow: " + FoundCheck(foundLevel, foundLevel.randomFlow));
                    Player.SendMessage(p, "Animal AI: " + FoundCheck(foundLevel, foundLevel.ai));
                    Player.SendMessage(p, "Edge water: " + FoundCheck(foundLevel, foundLevel.edgeWater));
                    Player.SendMessage(p, "Grass growing: " + FoundCheck(foundLevel, foundLevel.GrassGrow));
                    Player.SendMessage(p, "Tree growing: " + FoundCheck(foundLevel, foundLevel.growTrees));
                    Player.SendMessage(p, "Leaf decay: " + FoundCheck(foundLevel, foundLevel.leafDecay));
                    Player.SendMessage(p, "Physics speed: &b" + foundLevel.speedPhysics);
                    Player.SendMessage(p, "Physics overload: &b" + foundLevel.overload);
                    Player.SendMessage(p, "Survival death: " + FoundCheck(foundLevel, foundLevel.Death) + "(Fall: " + foundLevel.fall + ", Drown: " + foundLevel.drown + ")");
                    Player.SendMessage(p, "Killer blocks: " + FoundCheck(foundLevel, foundLevel.Killer));
                    Player.SendMessage(p, "Unload: " + FoundCheck(foundLevel, foundLevel.unload));
                    Player.SendMessage(p, "Load on /goto: " + FoundCheck(foundLevel, foundLevel.loadOnGoto));
                    Player.SendMessage(p, "Auto physics: " + FoundCheck(foundLevel, foundLevel.rp));
                    Player.SendMessage(p, "Instant building: " + FoundCheck(foundLevel, foundLevel.Instant));
                    Player.SendMessage(p, "RP chat: " + FoundCheck(foundLevel, !foundLevel.worldChat));
                    Player.SendMessage(p, "Guns: " + FoundCheck(foundLevel, foundLevel.guns));
                    gettinginfo = false;
                    return;
                }
            }
            else
            {
                foundLevel = Level.Find(message.Split(' ')[0]);

                if (foundLevel == null || message.Split(' ')[0].ToLower() == "ps" || message.Split(' ')[0].ToLower() == "rp") foundLevel = p.level;
                else message = message.Substring(message.IndexOf(' ') + 1);
            }

            if (p != null)
                if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this)) { Player.SendMessage(p, "Setting map options is reserved to " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + "+"); return; }

            string foundStart;
            if (message.IndexOf(' ') == -1) foundStart = message.ToLower();
            else foundStart = message.Split(' ')[0].ToLower();

            try
            {
                if (foundLevel == null) Player.SendMessage(p, "derp");
                switch (foundStart)
                {
                    case "theme": foundLevel.theme = message.Substring(message.IndexOf(' ') + 1); foundLevel.ChatLevel("Map theme: &b" + foundLevel.theme); break;
                    case "finite": foundLevel.finite = !foundLevel.finite; foundLevel.ChatLevel("Finite mode: " + FoundCheck(foundLevel, foundLevel.finite)); if (p == null) Player.SendMessage(p, "Finite mode: " + FoundCheck(foundLevel, foundLevel.finite, true)); break;
                    case "ai": foundLevel.ai = !foundLevel.ai; foundLevel.ChatLevel("Animal AI: " + FoundCheck(foundLevel, foundLevel.ai)); if (p == null) Player.SendMessage(p, "Animal AI: " + FoundCheck(foundLevel, foundLevel.ai, true)); break;
                    case "edge": foundLevel.edgeWater = !foundLevel.edgeWater; foundLevel.ChatLevel("Edge water: " + FoundCheck(foundLevel, foundLevel.edgeWater)); if (p == null) Player.SendMessage(p, "Edge water: " + FoundCheck(foundLevel, foundLevel.edgeWater, true)); break;
                    case "grass": foundLevel.GrassGrow = !foundLevel.GrassGrow; foundLevel.ChatLevel("Growing grass: " + FoundCheck(foundLevel, foundLevel.GrassGrow)); if (p == null) Player.SendMessage(p, "Growing grass: " + FoundCheck(foundLevel, foundLevel.GrassGrow, true)); break;
                    case "ps":
                    case "physicspeed":
                        if (int.Parse(message.Split(' ')[1]) < 10) { Player.SendMessage(p, "Cannot go below 10"); return; }
                        foundLevel.speedPhysics = int.Parse(message.Split(' ')[1]);
                        foundLevel.ChatLevel("Physics speed: &b" + foundLevel.speedPhysics);
                        break;
                    case "overload":
                        if (int.Parse(message.Split(' ')[1]) < 500) { Player.SendMessage(p, "Cannot go below 500 (default is 1500)"); return; }
                        if (p != null && p.group.Permission < LevelPermission.Admin && int.Parse(message.Split(' ')[1]) > 2500) { Player.SendMessage(p, "Only SuperOPs may set higher than 2500"); return; }
                        foundLevel.overload = int.Parse(message.Split(' ')[1]);
                        foundLevel.ChatLevel("Physics overload: &b" + foundLevel.overload);
                        break;
                    case "motd":
                        if (message.Split(' ').Length == 1) foundLevel.motd = "ignore";
                        else foundLevel.motd = message.Substring(message.IndexOf(' ') + 1);
                        foundLevel.ChatLevel("Map's MOTD was changed to: &b" + foundLevel.motd);
                        break;
                    case "death": foundLevel.Death = !foundLevel.Death; foundLevel.ChatLevel("Survival death: " + FoundCheck(foundLevel, foundLevel.Death)); if (p == null) Player.SendMessage(p, "Survival death: " + FoundCheck(foundLevel, foundLevel.Death, true)); break;
                    case "killer": foundLevel.Killer = !foundLevel.Killer; foundLevel.ChatLevel("Killer blocks: " + FoundCheck(foundLevel, foundLevel.Killer)); if (p == null) Player.SendMessage(p, "Killer blocks: " + FoundCheck(foundLevel, foundLevel.Killer, true)); break;
                    case "fall": foundLevel.fall = int.Parse(message.Split(' ')[1]); foundLevel.ChatLevel("Fall distance: &b" + foundLevel.fall); break;
                    case "drown": foundLevel.drown = int.Parse(message.Split(' ')[1]) * 10; foundLevel.ChatLevel("Drown time: &b" + (foundLevel.drown / 10)); break;
                    case "unload": foundLevel.unload = !foundLevel.unload; foundLevel.ChatLevel("Auto unload: " + FoundCheck(foundLevel, foundLevel.unload)); if (p == null) Player.SendMessage(p, "Auto unload: " + FoundCheck(foundLevel, foundLevel.unload, true)); break;
                    case "rp":
                    case "restartphysics": foundLevel.rp = !foundLevel.rp; foundLevel.ChatLevel("Auto physics: " + FoundCheck(foundLevel, foundLevel.rp)); if (p == null) Player.SendMessage(p, "Auto physics: " + FoundCheck(foundLevel, foundLevel.rp, true)); break;
                    case "instant":
                        if (p != null && p.group.Permission < LevelPermission.Admin) { Player.SendMessage(p, "This is reserved for Super+"); return; }
                        foundLevel.Instant = !foundLevel.Instant; foundLevel.ChatLevel("Instant building: " + FoundCheck(foundLevel, foundLevel.Instant)); if(p == null) Player.SendMessage(p, "Instant building: " + FoundCheck(foundLevel, foundLevel.Instant, true)); break;
                    case "chat":
                        foundLevel.worldChat = !foundLevel.worldChat; foundLevel.ChatLevel("RP chat: " + FoundCheck(foundLevel, !foundLevel.worldChat)); if(p == null) Player.SendMessage(p, "RP chat: " + FoundCheck(foundLevel, !foundLevel.worldChat, true)); break;
                    case "load":
                    case "autoload":
                    case "loadongoto":
                        foundLevel.loadOnGoto = !foundLevel.loadOnGoto; foundLevel.ChatLevel("Load on /goto: " + FoundCheck(foundLevel, foundLevel.loadOnGoto)); if(p == null) Player.SendMessage(p, "Load on /goto: " + FoundCheck(foundLevel, foundLevel.loadOnGoto, true)); break;
                    case "leaf":
                    case "leafdecay":
                        foundLevel.leafDecay = !foundLevel.leafDecay; foundLevel.ChatLevel("Leaf decay: " + FoundCheck(foundLevel, foundLevel.leafDecay)); if (p == null) Player.SendMessage(p, "Leaf decay: " + FoundCheck(foundLevel, foundLevel.leafDecay, true)); break;
                    case "flow":
                    case "randomflow":
                        foundLevel.randomFlow = !foundLevel.randomFlow; foundLevel.ChatLevel("Random flow: " + FoundCheck(foundLevel, foundLevel.randomFlow)); if (p == null) Player.SendMessage(p, "Random flow: " + FoundCheck(foundLevel, foundLevel.randomFlow, true)); break;
                    case "tree":
                    case "growtrees":
                        foundLevel.growTrees = !foundLevel.growTrees; foundLevel.ChatLevel("Tree growing: " + FoundCheck(foundLevel, foundLevel.growTrees)); if (p == null) Player.SendMessage(p, "Tree growing: " + FoundCheck(foundLevel, foundLevel.growTrees, true)); break;
                    default:
                        Player.SendMessage(p, "Could not find option entered.");
                        return;
                }
                foundLevel.changed = true;
                if (p != null && p.level != foundLevel) Player.SendMessage(p, "/map finished!");
            }
            catch { Player.SendMessage(p, "INVALID INPUT"); }
        }
        public string FoundCheck(Level level, bool check, bool console = false)        
        {
            if (gettinginfo == false) Level.SaveSettings(level);
            return console ? (check ? "ON" : "OFF") : (check ? "&aON" : "&cOFF");
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/map [level] [toggle] - Sets [toggle] on [level]");
            Player.SendMessage(p, "Possible toggles: theme, finite, randomflow, ai, edge, grass, growtrees, leafdecay, ps, overload, motd, death, fall, drown, unload, loadongoto, rp, instant, killer, chat");
            Player.SendMessage(p, "Edge will cause edge water to flow.");
            Player.SendMessage(p, "Grass will make grass not grow without physics.");
            Player.SendMessage(p, "Tree growing will make saplings grow into trees after a while.");
            Player.SendMessage(p, "Leaf decay will make leaves not connected to a log within 4 blocks disappear randomly.");
            Player.SendMessage(p, "Finite will cause all liquids to be finite.");
            Player.SendMessage(p, "Random flow makes mass flooding liquids flow less uniformly.");
            Player.SendMessage(p, "AI will make animals hunt or flee.");
            Player.SendMessage(p, "PS will set the map's physics speed.");
            Player.SendMessage(p, "Overload will change how easy/hard it is to kill physics.");
            Player.SendMessage(p, "MOTD will set a custom motd for the map. (leave blank to reset)");
            Player.SendMessage(p, "Death will allow survival-style dying (falling, drowning)");
            Player.SendMessage(p, "Fall/drown set the distance/time before dying from each.");
            Player.SendMessage(p, "Drowning value is 15 for one second of air.");
            Player.SendMessage(p, "Killer turns killer blocks on and off.");
            Player.SendMessage(p, "Unload sets whether the map unloads when no one's there.");
            Player.SendMessage(p, "Load on /goto sets whether the map can be loaded when some uses /goto. Only works if the load on /goto server option is enabled.");
            Player.SendMessage(p, "RP sets whether the physics auto-start for the map");
            Player.SendMessage(p, "Instant mode works by not updating everyone's screens");
            Player.SendMessage(p, "Chat sets the map to recieve no messages from other maps");
        }
    }
}
