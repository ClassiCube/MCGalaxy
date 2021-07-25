/*
    Copyright 2015 MCGalaxy
    
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
using MCGalaxy.Commands;
 
namespace MCGalaxy.Modules.Relay 
{
    public abstract class RelayBotCmd : Command2 
    {
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        protected abstract RelayBot Bot { get; }
        
        public override void Use(Player p, string message, CommandData data) {
            RelayBot bot = Bot;
            
            if (message.CaselessEq("reset") || message.CaselessEq("reconnect")) {
                if (!bot.Enabled) { p.Message("{0} is not enabled", bot.RelayName); }
                bot.Reset();
            } else if (message.CaselessEq("connect")) {
                string err = bot.Connect();
                if (err != null) p.Message("{0} bot {1}", bot.RelayName, err);
            } else if (message.CaselessEq("disconnect")) {
                bot.Disconnect("Disconnecting IRC bot");
            } else {
                Help(p);
            }
        }
        
        public override void Help(Player p) {
            string cmd   = name;
            string relay = Bot.RelayName;
            
            p.Message("&T/{0} connect", cmd);
            p.Message("&HCauses the {0} bot to connect to {0}.", relay);
            p.Message("&T/{0} disconnect", cmd);
            p.Message("&HCauses the {0} bot to disconnect from {0}.", relay);
            p.Message("&T/{0} reset", cmd);
            p.Message("&HCauses the {0} bot to disconnect then reconnect.", relay);
        }
    }
}
