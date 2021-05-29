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
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Config;
using MCGalaxy.Events.GameEvents;
using MCGalaxy.Events.LevelEvents;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Events.ServerEvents;
using MCGalaxy.Network;

namespace MCGalaxy.Games {

    /// <summary> Stores map-specific game configuration state. </summary>
    public abstract class RoundsGameMapConfig {
        
        protected void LoadFrom(ConfigElement[] cfg, string propsDir, string map) {
            string path = propsDir + map + ".properties";
            ConfigElement.ParseFile(cfg, path, this);
        }
        
        protected void SaveTo(ConfigElement[] cfg, string propsDir, string map) {
            string path = propsDir + map + ".properties";
            UnsafeIO.CreateDirectory(propsDir);
            ConfigElement.SerialiseSimple(cfg, path, this);
        }
        
        /// <summary> Saves this configuration to disc. </summary>
        public abstract void Save(string map);
        /// <summary> Loads this configuration from disc. </summary>
        public abstract void Load(string map);
        /// <summary> Applies default values for config fields which differ per map. </summary>
        /// <remarks> e.g. spawn positions, zones </remarks>
        public abstract void SetDefaults(Level lvl);
    }
    
    /// <summary> Stores overall game configuration state. </summary>
    public abstract class RoundsGameConfig {
        [ConfigBool("start-on-server-start", "Game", false)] 
        public bool StartImmediately;
        [ConfigBool("set-main-level", "Game", false)] 
        public bool SetMainLevel;
        [ConfigBool("map-in-heartbeat", "Game", false)]
        public bool MapInHeartbeat;
        [ConfigStringList("maps", "Game")] 
        public List<string> Maps = new List<string>();

        /// <summary> Whether users are allowed to auto-join maps used by this game. </summary>
        /// <remarks> If false, users can only join these maps when manually /load ed. </remarks>
        public abstract bool AllowAutoload { get; }
        protected abstract string PropsPath { get; }
        protected abstract string GameName { get; }
        
        ConfigElement[] cfg;
        public virtual void Save() {
            if (cfg == null) cfg = ConfigElement.GetAll(GetType());
            ConfigElement.SerialiseSimple(cfg, PropsPath, this);
        }
        
        public virtual void Load() {
            if (cfg == null) cfg = ConfigElement.GetAll(GetType());
            ConfigElement.ParseFile(cfg, PropsPath, this);
        }
        
        
        public static void AddMap(Player p, string map, LevelConfig lvlCfg, RoundsGame game) {
            RoundsGameConfig cfg = game.GetConfig();
            string coloredName = lvlCfg.Color + map;
            
            if (cfg.Maps.CaselessContains(map)) {
                p.Message("{0} &Sis already in the list of {1} maps", coloredName, game.GameName);
            } else {
                p.Message("Added {0} &Sto the list of {1} maps", coloredName, game.GameName);
                cfg.Maps.Add(map);
                if (!cfg.AllowAutoload) lvlCfg.LoadOnGoto = false;
                
                cfg.Save();
                lvlCfg.SaveFor(map);
                OnMapsChangedEvent.Call(game);
            }
        }
        
        public static void RemoveMap(Player p, string map, LevelConfig lvlCfg, RoundsGame game) {
            RoundsGameConfig cfg = game.GetConfig();
            string coloredName = lvlCfg.Color + map;
                
            if (!cfg.Maps.CaselessRemove(map)) {
                p.Message("{0} &Swas not in the list of {1} maps", coloredName, game.GameName);
            } else {
                p.Message("Removed {0} &Sfrom the list of {1} maps", coloredName, game.GameName);
                lvlCfg.AutoUnload = true;
                if (!cfg.AllowAutoload) lvlCfg.LoadOnGoto = true;
                
                cfg.Save();
                lvlCfg.SaveFor(map);
                OnMapsChangedEvent.Call(game);
            }
        }
    }
}
