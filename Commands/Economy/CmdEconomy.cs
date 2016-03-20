/*
    Copyright 2011 MCForge
    
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
using System;
using System.Globalization;
using System.Threading;
using MCGalaxy.Eco;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands {
    
    /// <summary> Economy Beta v1.0 QuantumHive  </summary>
    public sealed class CmdEconomy : Command {
        public override string name { get { return "economy"; } }
        public override string shortcut { get { return "eco"; } }
        public override string type { get { return CommandTypes.Economy; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public override CommandPerm[] AdditionalPerms {
            get { return new[] { new CommandPerm(LevelPermission.Operator, "The lowest rank that can setup the economy") }; }
        }
        
        public override void Use(Player p, string message) {
            string[] raw = message.Split(' ');
            string[] args = { "", "", "", "", "", "", "", "", "" };
            for (int i = 0; i < raw.Length; i++)
                args[i] = i < 2 ? raw[i].ToLower() : raw[i];

            if (args[0] == "setup")
                HandleSetup(p, message, args);
            else
                Help(p);
        }
        
        void HandleSetup(Player p, string message, string[] args) {
            if (p != null && (int)p.group.Permission < CommandOtherPerms.GetPerm(this)) {
                Player.SendMessage(p, "%cYou are not allowed to use %f/eco setup"); return;
            }
            
            switch (args[1]) {
                case "apply":
                    Economy.Load();
                    Player.SendMessage(p, "%aApplied changes");
                    return;                  

                case "enable":
                    Player.SendMessage(p, "%aThe economy system is now enabled"); 
                    Economy.Enabled = true; return;

                case "disable":
                    Player.SendMessage(p, "%aThe economy system is now disabled"); 
                    Economy.Enabled = false; return;
                    
                case "help":
                    SetupHelp(p); return;

                default:
                    foreach (Item item in Economy.Items)
                        foreach (string alias in item.Aliases) 
                    {
                        if (args[1].CaselessEq(alias)) {
                            item.OnSetupCommand(p, args); 
                            Economy.Save(); return;
                        }
                    }
                                        
                    if (args[1] == "") { SetupHelp(p); return; }
                    Player.SendMessage(p, "%cThat wasn't a valid command addition!");
                    return;
            }
            Economy.Save();
        }
        
        public override void Help(Player p) {
            Player.SendMessage(p, "%cMost commands have been removed from /economy, " +
                               "use the appropriate command from %T/help economy %cinstead.");
            if ((int)p.group.Permission >= CommandOtherPerms.GetPerm(this)) {
                Player.SendMessage(p, "%f/eco setup <type> %e- to setup economy");
                Player.SendMessage(p, "%f/eco setup help %e- get more specific help for setting up the economy");
            }
        }

        void SetupHelp(Player p) {
            Player.SendMessage(p, "%4/eco setup apply %e- reloads changes made to 'economy.properties'");
            Player.SendMessage(p, "%4/eco setup [%aenable%4/%cdisable%4] %e- to enable/disable the economy system");
            Player.SendMessage(p, "%4/eco setup [title/color/tcolor/rank/map] [%aenable%4/%cdisable%4] %e- to enable/disable that feature");
            Player.SendMessage(p, "%4/eco setup [title/color/tcolor] [%3price%4] %e- to setup the prices for these features");
            Player.SendMessage(p, "%4/eco setup rank price [%frank%4] [%3price%4] %e- to set the price for that rank");
            Player.SendMessage(p, "%4/eco setup rank maxrank [%frank%4] %e- to set the max buyable rank");
            Player.SendMessage(p, "%4/eco setup map new [%fname%4] [%fx%4] [%fy%4] [%fz%4] [%ftype%4] [%3price%4] %e- to setup a map preset");
            Player.SendMessage(p, "%4/eco setup map delete [%fname%4] %e- to delete a map");
            Player.SendMessage(p, "%4/eco setup map edit [%fname%4] [name/x/y/z/type/price] [%fvalue%4] %e- to edit a map preset");
        }
    }
}
