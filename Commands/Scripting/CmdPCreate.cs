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
using System.Collections.Generic;
using System.IO;
namespace MCGalaxy.Commands
{
    public sealed class CmdPCreate : Command
    {
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override bool museumUsable { get { return true; } }
        public override string name { get { return "pcreate"; } }
        public override string shortcut { get { return ""; } }
       public override string type { get { return CommandTypes.Moderation; } }
        public override void Use(Player p, string message)
        {
            if (p != null) { Player.Message(p, "Creating a plugin example source"); }
            else { Server.s.Log("Creating a plugin example source"); }

            string name;
            if (p != null) name = p.name;
            else name = Server.name;

            if (!Directory.Exists("plugin_source")) Directory.CreateDirectory("plugin_source");
            List<string> lines = new List<string>();
            lines.Add("//This is an example plugin source!");
            lines.Add("using System;");
            lines.Add("namespace MCGalaxy");
            lines.Add("{");
            lines.Add("    public class " + message + " : Plugin");
            lines.Add("    {");
            lines.Add("        public override string name { get { return \"" + message + "\"; } }");
            lines.Add("        public override string website { get { return \"www.example.com\"; } }");
            lines.Add("        public override string MCGalaxy_Version { get { return \"" + Server.Version + "\"; } }");
            lines.Add("        public override int build { get { return 100; } }");
            lines.Add("        public override string welcome { get { return \"Loaded Message!\"; } }");
            lines.Add("        public override string creator { get { return \"" + name + "\"; } }");
            lines.Add("        public override bool LoadAtStartup { get { return true; } }");
            lines.Add("        public override void Load(bool startup)");
            lines.Add("        {");
            lines.Add("            //LOAD YOUR PLUGIN WITH EVENTS OR OTHER THINGS!");
            lines.Add("        }");
            lines.Add("        public override void Unload(bool shutdown)");
            lines.Add("        {");
            lines.Add("            //UNLOAD YOUR PLUGIN BY SAVING FILES OR DISPOSING OBJECTS!");
            lines.Add("        }");
            lines.Add("        public override void Help(Player p) { //HELP INFO! }");
            lines.Add("    }}");
            lines.Add("}");
            File.WriteAllLines("plugin_source/" + message + ".cs", ListToArray(lines));
        }
        public override void Help(Player p)
        {
            if (p != null) Player.Message(p, "/pcreate <Plugin name> - Create a example .cs file!");
            else Server.s.Log("/pcreate <Plugin name> - Create a example .cs file!");
        }
        public CmdPCreate() { }
        public string[] ListToArray(List<string> list)
        {
            string[] temp = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
                temp[i] = list[i];
            return temp;
        }
    }
}
