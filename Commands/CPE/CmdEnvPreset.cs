/*
    Copyright 2016 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using MCGalaxy.Commands.Building;
using MCGalaxy.Levels.IO;
using System.Linq;
using System.Text;

namespace MCGalaxy.Commands
{
    public sealed class CmdEnvPreset : Command
    {
        public override string name { get { return "envpreset"; } }
        public override string shortcut { get { return "ep"; } }
        public override string type { get { return CommandTypes.World; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }

        public override void Use(Player p, string message)
        {
            if (p == null) { MessageInGameOnly(p); return; }
            string[] args = message.SplitSpaces(2);
            if (args.Length <= 1) { Help(p); return; }
            string name = args[1].ToLower();
            if (!Command.ValidName(p, name, "preset")) return;
            string path = "presets/" + name + ".env";
            if (args[0].ToLower() == "add")
            {
                if (File.Exists(path))
                {
                    Player.SendMessage(p, "A preset with the name \"" + name + "\" already exists, please choose another name.");
                    return;
                }
                Create(p, name, path);
                Player.SendMessage(p, "%HSuccessfully saved preset named: " + name);
            }
            else if (args[0].ToLower() == "delete")
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Player.SendMessage(p, "%HSuccessfully deleted preset named: " + name);
                }
                else
                {
                    Player.SendMessage(p, "%HError: Preset does not exist.");
                }
            }
            else if (args[0].ToLower() == "replace")
            {
                Create(p, name, path);
                Player.SendMessage(p, "%HSuccessfully replaced preset named: " + name);
            }
        }


        internal static void Create(Player p, string name, string path)
        { 
            if (!Directory.Exists("presets")) { Directory.CreateDirectory("presets"); }
            Level lvl = p.level;
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2} {3} {4}", lvl.FogColor, lvl.SkyColor, lvl.CloudColor, lvl.LightColor, lvl.ShadowColor);
            string value = sb.ToString();
            File.WriteAllText(path, value);
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "%H/ep add <presetname> - Saves the current level's Env as a new preset.");
            Player.SendMessage(p, "%H/ep delete <presetname> - Deletes the custom preset from the server.");
            Player.SendMessage(p, "%H/ep replace <presetname> - Replaces the custom preset with the current level's Env.");
        }
    }
}
