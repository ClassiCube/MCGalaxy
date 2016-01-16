/*
    Copyright 2011 MCGalaxy
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
       public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdMap() { }

        public override void Use(Player p, string message)
        {
            if (message == "") message = p.level.name;
            Level lvl;

            if (message.IndexOf(' ') == -1)
            {
                lvl = Level.Find(message);
                if (lvl == null)
                {
                    if (p != null)
                        lvl = p.level;
                }
                else
                {
                    Player.SendMessage(p, "MOTD: &b" + lvl.motd);
                    Player.SendMessage(p, "Finite mode: " + GetBool(lvl.finite));
                    Player.SendMessage(p, "Random flow: " + GetBool(lvl.randomFlow));
                    Player.SendMessage(p, "Animal AI: " + GetBool(lvl.ai));
                    Player.SendMessage(p, "Edge water: " + GetBool(lvl.edgeWater));
                    Player.SendMessage(p, "Grass growing: " + GetBool(lvl.GrassGrow));
                    Player.SendMessage(p, "Tree growing: " + GetBool(lvl.growTrees));
                    Player.SendMessage(p, "Leaf decay: " + GetBool(lvl.leafDecay));
                    Player.SendMessage(p, "Physics speed: &b" + lvl.speedPhysics);
                    Player.SendMessage(p, "Physics overload: &b" + lvl.overload);
                    Player.SendMessage(p, "Survival death: " + GetBool(lvl.Death) + "(Fall: " + lvl.fall + ", Drown: " + lvl.drown + ")");
                    Player.SendMessage(p, "Killer blocks: " + GetBool(lvl.Killer));
                    Player.SendMessage(p, "Unload: " + GetBool(lvl.unload));
                    Player.SendMessage(p, "Load on /goto: " + GetBool(lvl.loadOnGoto));
                    Player.SendMessage(p, "Auto physics: " + GetBool(lvl.rp));
                    Player.SendMessage(p, "Instant building: " + GetBool(lvl.Instant));
                    Player.SendMessage(p, "RP chat: " + GetBool(!lvl.worldChat));
                    Player.SendMessage(p, "Guns: " + GetBool(lvl.guns));
                    Player.SendMessage(p, "Buildable: " + GetBool(lvl.Buildable));
                    Player.SendMessage(p, "Deletable: " + GetBool(lvl.Deletable));
                    return;
                }
            }
            else
            {
                lvl = Level.Find(message.Split(' ')[0]);

                if (lvl == null || message.Split(' ')[0].ToLower() == "ps" || message.Split(' ')[0].ToLower() == "rp") lvl = p.level;
                else message = message.Substring(message.IndexOf(' ') + 1);
            }

            if (p != null)
                if ((int)p.group.Permission < CommandOtherPerms.GetPerm(this)) { Player.SendMessage(p, "Setting map options is reserved to " + Group.findPermInt(CommandOtherPerms.GetPerm(this)).name + "+"); return; }

            string foundStart;
            if (message.IndexOf(' ') == -1) foundStart = message.ToLower();
            else foundStart = message.Split(' ')[0].ToLower();

            try
            {
                if (lvl == null) Player.SendMessage(p, "derp");
                switch (foundStart)
                {
                    case "theme": lvl.theme = message.Substring(message.IndexOf(' ') + 1); lvl.ChatLevel("Map theme: &b" + lvl.theme); break;
                    case "finite":
                        SetBool(p, lvl, ref lvl.finite, "Finite mode: "); break;
                    case "ai":
                        SetBool(p, lvl, ref lvl.ai, "Animal AI: "); break;
                    case "edge":
                        SetBool(p, lvl, ref lvl.edgeWater, "Edge water: "); break;
                    case "grass":
                        SetBool(p, lvl, ref lvl.GrassGrow, "Growing grass: "); break;
                    case "ps":
                    case "physicspeed":
                        if (int.Parse(message.Split(' ')[1]) < 10) { Player.SendMessage(p, "Cannot go below 10"); return; }
                        lvl.speedPhysics = int.Parse(message.Split(' ')[1]);
                        lvl.ChatLevel("Physics speed: &b" + lvl.speedPhysics);
                        break;
                    case "overload":
                        if (int.Parse(message.Split(' ')[1]) < 500) { Player.SendMessage(p, "Cannot go below 500 (default is 1500)"); return; }
                        if (p != null && p.group.Permission < LevelPermission.Admin && int.Parse(message.Split(' ')[1]) > 2500) { Player.SendMessage(p, "Only SuperOPs may set higher than 2500"); return; }
                        lvl.overload = int.Parse(message.Split(' ')[1]);
                        lvl.ChatLevel("Physics overload: &b" + lvl.overload);
                        break;
                    case "motd":
                        if (message.Split(' ').Length == 1) lvl.motd = "ignore";
                        else lvl.motd = message.Substring(message.IndexOf(' ') + 1);
                        lvl.ChatLevel("Map's MOTD was changed to: &b" + lvl.motd);
                        break;
                    case "death":
                        SetBool(p, lvl, ref lvl.Death, "Survival death: "); break;
                    case "killer":
                        SetBool(p, lvl, ref lvl.Killer, "Killer blocks: "); break;
                    case "fall": lvl.fall = int.Parse(message.Split(' ')[1]); lvl.ChatLevel("Fall distance: &b" + lvl.fall); break;
                    case "drown": lvl.drown = int.Parse(message.Split(' ')[1]); lvl.ChatLevel("Drown time: &b" + ((float)lvl.drown / 10)); break;
                    case "unload":
                        SetBool(p, lvl, ref lvl.unload, "Auto unload: "); break;
                    case "rp":
                    case "restartphysics":
                        SetBool(p, lvl, ref lvl.rp, "Auto physics: ");break;
                    case "instant":
                        if (p != null && p.group.Permission < LevelPermission.Admin) { Player.SendMessage(p, "This is reserved for Super+"); return; }
                        SetBool(p, lvl, ref lvl.Instant, "Instant building: "); break;
                    case "chat":
                        SetBool(p, lvl, ref lvl.worldChat, "RP chat: "); break;
                    case "load":
                    case "autoload":
                    case "loadongoto":
                        SetBool(p, lvl, ref lvl.loadOnGoto, "Load on goto: "); break;
                    case "leaf":
                    case "leafdecay":
                        SetBool(p, lvl, ref lvl.leafDecay, "Leaf deacy: "); break;
                    case "flow":
                    case "randomflow":
                        SetBool(p, lvl, ref lvl.randomFlow, "Ranbow flow: "); break;
                    case "tree":
                    case "growtrees":
                        SetBool(p, lvl, ref lvl.growTrees, "Tree growing: "); break;
                    case "buildable":
                        SetBool(p, lvl, ref lvl.Buildable, "Buildable: "); 
                        lvl.UpdateBlockPermissions(); break;
                    case "deletable":
                        SetBool(p, lvl, ref lvl.Deletable, "Deleteable: "); 
                        lvl.UpdateBlockPermissions(); break;
                    
                    default:
                        Player.SendMessage(p, "Could not find option entered."); return;
                }
                lvl.changed = true;
                if (p != null && p.level != lvl) Player.SendMessage(p, "/map finished!");
            }
            catch { Player.SendMessage(p, "INVALID INPUT"); }
        }
        
        void SetBool(Player p, Level lvl, ref bool target, string message) {
            target = !target;
            Level.SaveSettings(lvl);
            lvl.ChatLevel(message + GetBool(target));
            
            if (p == null) 
                Player.SendMessage(p, message + GetBool(target, true));
        }
        
        string GetBool(bool value, bool console = false) {
            return console ? (value ? "ON" : "OFF") : (value ? "&aON" : "&cOFF");
        }

        public override void Help(Player p) {
            Player.SendMessage(p, "/map [level] [toggle] - Sets [toggle] on [level]");
            Player.SendMessage(p, "Possible toggles: theme, finite, randomflow, ai, edge, grass, growtrees, leafdecay, ps, overload, motd, " +
                               "death, fall, drown, unload, loadongoto, rp, instant, killer, chat, buildable, deletable");
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
            Player.SendMessage(p, "Drowning value is 10 for one second of air.");
            Player.SendMessage(p, "Killer turns killer blocks on and off.");
            Player.SendMessage(p, "Unload sets whether the map unloads when no one's there.");
            Player.SendMessage(p, "Load on /goto sets whether the map can be loaded when some uses /goto. Only works if the load on /goto server option is enabled.");
            Player.SendMessage(p, "RP sets whether the physics auto-start for the map");
            Player.SendMessage(p, "Instant mode works by not updating everyone's screens");
            Player.SendMessage(p, "Buildable sets whether any blocks can be placed by any player");
            Player.SendMessage(p, "Deleteable sets whether any blocks can be deleted by any player");
        }
    }
}
