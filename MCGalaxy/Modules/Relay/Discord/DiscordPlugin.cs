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
using MCGalaxy.Events.ServerEvents;

namespace MCGalaxy.Modules.Relay.Discord 
{
    public sealed class DiscordConfig 
    {
        [ConfigBool("enabled", "General", false)]
        public bool Enabled;
        [ConfigString("bot-token", "General", "", true)]
        public string BotToken = "";
        [ConfigBool("use-nicknames", "General", true)]
        public bool UseNicks = true;
        
        [ConfigString("channel-ids", "General", "", true)]
        public string Channels = "";
        [ConfigString("op-channel-ids", "General", "", true)]
        public string OpChannels = "";
        [ConfigString("ignored-user-ids", "General", "", true)]
        public string IgnoredUsers = "";
        
        [ConfigBool("presence-enabled", "Presence (Status)", true)]
        public bool PresenceEnabled = true;
        [ConfigEnum("presence-status", "Presence (Status)", PresenceStatus.online, typeof(PresenceStatus))]
        public PresenceStatus Status = PresenceStatus.online;        
        [ConfigEnum("presence-activity", "Presence (Status)", PresenceActivity.Playing, typeof(PresenceActivity))]
        public PresenceActivity Activity = PresenceActivity.Playing;
        [ConfigString("status-message", "Presence (Status)", "with {PLAYERS} players")]
        public string StatusMessage = "with {PLAYERS} players";
        
        [ConfigBool("can-mention-users", "Mentions", true)]
        public bool CanMentionUsers = true;
        [ConfigBool("can-mention-roles", "Mentions", true)]
        public bool CanMentionRoles = true;
        [ConfigBool("can-mention-everyone", "Mentions", false)]
        public bool CanMentionHere;
        
        [ConfigInt("embed-color", "Embeds", 9758051)]
        public int EmbedColor = 9758051;
        [ConfigBool("embed-show-game-statuses", "Embeds", true)]
        public bool EmbedGameStatuses = true;
        
        public const string PROPS_PATH = "properties/discordbot.properties";
        static ConfigElement[] cfg;
        
        public void Load() {
            // create default config file
            if (!File.Exists(PROPS_PATH)) Save();

            if (cfg == null) cfg = ConfigElement.GetAll(typeof(DiscordConfig));
            ConfigElement.ParseFile(cfg, PROPS_PATH, this);
        }
        
        public void Save() {
            if (cfg == null) cfg = ConfigElement.GetAll(typeof(DiscordConfig));
            
            using (StreamWriter w = new StreamWriter(PROPS_PATH)) {
                w.WriteLine("# Discord relay bot configuration");
                w.WriteLine("# See " + Updater.SourceURL + "/wiki/Discord-relay-bot/");
                w.WriteLine();
                ConfigElement.Serialise(cfg, w, this);
            }
        }
    }
    
    public enum PresenceStatus { online, dnd, idle, invisible }
    public enum PresenceActivity { Playing = 0, Listening = 2, Watching = 3, Competing = 5 }
    
    public sealed class DiscordPlugin : Plugin 
    {
        public override string name { get { return "DiscordRelay"; } }
        
        public static DiscordConfig Config = new DiscordConfig();
        public static DiscordBot Bot = new DiscordBot();
        
        public override void Load(bool startup) {
            Bot.Config = Config;
            Bot.ReloadConfig();
            Bot.Connect();
            OnConfigUpdatedEvent.Register(OnConfigUpdated, Priority.Low);
        }
        
        public override void Unload(bool shutdown) {
            OnConfigUpdatedEvent.Unregister(OnConfigUpdated);
            Bot.Disconnect("Disconnecting Discord bot");
        }
        
        void OnConfigUpdated() { Bot.ReloadConfig(); }
    }
    
    public sealed class CmdDiscordBot : RelayBotCmd 
    {
        public override string name { get { return "DiscordBot"; } }
        protected override RelayBot Bot { get { return DiscordPlugin.Bot; } }
    }
    
    public sealed class CmdDiscordControllers : BotControllersCmd 
    {
        public override string name { get { return "DiscordControllers"; } }
        protected override RelayBot Bot { get { return DiscordPlugin.Bot; } }
    }
}
