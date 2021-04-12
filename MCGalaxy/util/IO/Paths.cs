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

namespace MCGalaxy {
    
	/// <summary> Provides a centralised list of files and paths used. </summary>
    public static class Paths {
        
        public const string CustomColorsFile = "text/customcolors.txt";
        public const string TempRanksFile = "text/tempranks.txt";
        public const string TempBansFile = "text/tempbans.txt";
        public const string CustomTokensFile = "text/custom$s.txt";
        public const string BadWordsFile = "text/badwords.txt";
        public const string EatMessagesFile = "text/eatmessages.txt";
        public const string RulesFile = "text/rules.txt";
        public const string OprulesFile = "text/oprules.txt";
        public const string FaqFile = "text/faq.txt";
        public const string AnnouncementsFile = "text/messages.txt";
        public const string AliasesFile = "text/aliases.txt";
        public const string NewsFile = "text/news.txt";
        public const string WelcomeFile = "text/welcome.txt";
        public const string JokerFile = "text/joker.txt";        
        public const string EightBallFile = "text/8ball.txt";   
        
        public const string BlockPermsFile = "properties/block.properties";
        public const string CmdPermsFile = "properties/command.properties";
        public const string CmdExtraPermsFile = "properties/ExtraCommandPermissions.properties";
        public const string EconomyPropsFile = "properties/economy.properties";
        public const string ServerPropsFile = "properties/server.properties";
        public const string RankPropsFile = "properties/ranks.properties";        
        
        public const string ImportsDir = "extra/import/";
        public const string WaypointsDir = "extra/Waypoints/";
        
        /// <summary> Relative path of the file containing a map's bots. </summary>
        public static string BotsPath(string map) { return "extra/bots/" + map + ".json"; }
        
        /// <summary> Relative path of the file containing a map's block definitions. </summary>
        public static string MapBlockDefs(string map) { return "blockdefs/lvl_" + map + ".json"; }
        
        /// <summary> Relative path of a deleted level's map file. </summary>
        public static string DeletedMapFile(string map) { return "levels/deleted/" + map + ".lvl"; }       
        
        /// <summary> Relative path of a level's previous save map file. </summary>
        public static string PrevMapFile(string map) { return "levels/prev/" + map.ToLower() + ".lvl.prev"; }

        /// <summary> Relative path of a block properties file. </summary>     
        public static string BlockPropsPath(string group) { return "blockprops/" + group + ".txt"; }
    }
}
