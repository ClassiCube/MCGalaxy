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
using System.IO;
using MCGalaxy.Config;

namespace MCGalaxy.Modules.Relay.Discord {

    public sealed class DiscordConfig {
        [ConfigString("bot-token", null, "", true)]
        public string BotToken = "";
        [ConfigString("read-channel-ids", null, "", true)]
        public string ReadChannels = "";
        [ConfigString("send-channel-ids", null, "", true)]
        public string SendChannels = "";
        [ConfigString("operator-user-ids", null, "", true)]
        public string OperatorUsers = "";
        
        [ConfigBool("enabled", null, false)]
        public bool Enabled;
        [ConfigString("status-message", null, "with {PLAYERS} players")]
        public string Status = "with {PLAYERS} players";
        
        const string file = "properties/discordbot.properties";
        static ConfigElement[] cfg;
        
        public void Load() {
            // create default config file
            if (!File.Exists(file)) Save();

            if (cfg == null) cfg = ConfigElement.GetAll(typeof(DiscordConfig));
            ConfigElement.ParseFile(cfg, file, this);
        }
        
        public void Save() {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(DiscordConfig));
            ConfigElement.SerialiseSimple(cfg, file, this);
        }
    }
    
    public sealed class DiscordPlugin : Plugin {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.Version; } }
        public override string name { get { return "DiscordRelayPlugin"; } }
        
        public static DiscordConfig Config = new DiscordConfig();
        public static DiscordBot Bot;
        
        public override void Load(bool startup) {
            Config.Load();
            if (Config.Enabled) Connect();
        }
        
        public override void Unload(bool shutdown) {
            Disconnect();
        }
        
        public static void Connect() {
            if (Bot != null && !Bot.Disconnected) return;
            Bot = new DiscordBot();
            Bot.RunAsync(Config);
        }
        
        public static void Disconnect() {
            if (Bot == null) return;
            Bot.Stop();
            Bot = null;
        }
    }
}
