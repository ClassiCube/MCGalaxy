/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified by MCGalaxy)

    Edited for use with MCGalaxy
 
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

namespace MCGalaxy.Scripting
{
    public static class ScriptingOperations
    {
        public static bool LoadCommands(Player p, string path) {
            if (!File.Exists(path)) {
                p.Message("File &9{0} &Snot found.", path);
                return false;
            }
            
            try {
                List<Command> cmds = IScripting.LoadCommands(path);
                
                p.Message("Successfully loaded &T{0}",
                          cmds.Join(c => "/" + c.name));
                return true;
            } catch (AlreadyLoadedException ex) {
                p.Message(ex.Message);
                return false;
            } catch (Exception ex) {
                p.Message(IScripting.DescribeLoadError(path, ex));
                Logger.LogError("Error loading commands from " + path, ex);
                return false;
            }
        }

        public static bool LoadPlugins(Player p, string path) {
            if (!File.Exists(path)) {
                p.Message("File &9{0} &Snot found.", path);
                return false;
            }
            
            try {
                List<Plugin> plugins = IScripting.LoadPlugin(path, false);
                
                p.Message("Plugin {0} loaded successfully",
                          plugins.Join(pl => pl.name));
                return true;
            } catch (AlreadyLoadedException ex) {
                p.Message(ex.Message);
                return false;
            } catch (Exception ex) {
                p.Message(IScripting.DescribeLoadError(path, ex));
                Logger.LogError("Error loading plugins from " + path, ex);
                return false;
            }
        }
    }
}
