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
using System;
using MCGalaxy.Commands;
using MCGalaxy.Events.ServerEvents;

namespace MCGalaxy.Modules.Relay.IRC {
    
    public sealed class IRCPlugin : Plugin {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.Version; } }
        public override string name { get { return "IRCRelay"; } }

        public static IRCBot Bot = new IRCBot();
        
        public override void Load(bool startup) {
            Bot.ReloadConfig();
            Bot.Connect();
            OnConfigUpdatedEvent.Register(OnConfigUpdated, Priority.Low);
        }
        
        public override void Unload(bool shutdown) {
            OnConfigUpdatedEvent.Unregister(OnConfigUpdated);
            Bot.Disconnect("Disconnecting IRC bot");
        }
        
        void OnConfigUpdated() { Bot.ReloadConfig(); }
    }
    
    public sealed class CmdIRCBot : RelayBotCmd {
        public override string name { get { return "IRCBot"; } }
        public override CommandAlias[] Aliases {
            get { return new[] { new CommandAlias("ResetBot", "reset"), new CommandAlias("ResetIRC", "reset") }; }
        }
        protected override RelayBot Bot { get { return IRCPlugin.Bot; } }
    }
    
    public sealed class CmdIrcControllers : BotControllersCmd {
        public override string name { get { return "IRCControllers"; } }
        public override string shortcut { get { return "IRCCtrl"; } }
        protected override RelayBot Bot { get { return IRCPlugin.Bot; } }
    }
}
