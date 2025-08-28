﻿/*
    Copyright 2010 MCLawl Team - Written by Valek (Modified by MCGalaxy)

    Edited for use with MCGalaxy
 
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MCGalaxy.Platform;

namespace MCGalaxy.Scripting
{
    /// <summary> Exception raised when attempting to load a new command/plugin 
    /// that has the same name as an already loaded command/plugin </summary>
    public sealed class AlreadyLoadedException : Exception
    {
        public AlreadyLoadedException(string msg) : base(msg) { }
    }
    
    /// <summary> Utility methods for loading assemblies, commands, and plugins </summary>
    public static class IScripting
    {
        public const string COMMANDS_DLL_DIR = "extra/commands/dll/";
        public const string PLUGINS_DLL_DIR  = "plugins/";
        
        /// <summary> Returns the default .dll path for the custom command with the given name </summary>
        public static string CommandPath(string name) { return COMMANDS_DLL_DIR + "Cmd" + name + ".dll"; }
        /// <summary> Returns the default .dll path for the plugin with the given name </summary>
        public static string PluginPath(string name)  { return PLUGINS_DLL_DIR + name + ".dll"; }
        
        
        public static void Init() {
            Directory.CreateDirectory(COMMANDS_DLL_DIR);
            Directory.CreateDirectory(PLUGINS_DLL_DIR);
            AppDomain.CurrentDomain.AssemblyResolve += ResolvePluginAssembly;
        }

        // only used for resolving plugin DLLs depending on other plugin DLLs
        static Assembly ResolvePluginAssembly(object sender, ResolveEventArgs args) {
#if !NET_20
            // This property only exists in .NET framework 4.0 and later
            Assembly requestingAssembly = args.RequestingAssembly;
            
            if (requestingAssembly == null)       return null;
            if (!IsPluginDLL(requestingAssembly)) return null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assem in assemblies)
            {
                if (!IsPluginDLL(assem)) continue;

                if (args.Name == assem.FullName) return assem;
            }

            Assembly coreRef = DotNetBackend.ResolvePluginReference(args.Name);
            if (coreRef != null) return coreRef;

            Logger.Log(LogType.Warning, "Custom command/plugin [{0}] tried to load [{1}], but it could not be found",
                       requestingAssembly.FullName, args.Name);
#endif
            return null;
        }

        static bool IsPluginDLL(Assembly a) { return String.IsNullOrEmpty(a.Location); }
        
        
        /// <summary> Constructs instances of all types which derive from T in the given assembly. </summary>
        /// <returns> The list of constructed instances. </returns>
        public static List<T> LoadTypes<T>(Assembly lib) {
            List<T> instances = new List<T>();
            
            foreach (Type t in lib.GetTypes())
            {
                if (t.IsAbstract || t.IsInterface || !t.IsSubclassOf(typeof(T))) continue;
                object instance = Activator.CreateInstance(t);
                
                if (instance == null) {
                    Logger.Log(LogType.Warning, "{0} \"{1}\" could not be loaded", typeof(T).Name, t.Name);
                    throw new BadImageFormatException();
                }
                instances.Add((T)instance);
            }
            return instances;
        }
        
        /// <summary> Loads the given assembly from disc (and associated .pdb debug data) </summary>
        public static Assembly LoadAssembly(string path) {
            byte[] data  = File.ReadAllBytes(path);
            byte[] debug = GetDebugData(path);
            return Assembly.Load(data, debug);
        }
        
        static byte[] GetDebugData(string path) {
            // Cmdtest.dll -> Cmdtest.pdb
			string pdb_path = Path.ChangeExtension(path, ".pdb");
            try {
                return File.ReadAllBytes(pdb_path);
            } catch (FileNotFoundException) {
            } catch (Exception ex) {
                Logger.LogError("Error loading .pdb " + pdb_path, ex);
                return null;
            }

            if (!Server.RunningOnMono()) return null;

            // Cmdtest.dll -> Cmdtest.dll.mdb
            string mdb_path = path + ".mdb";
            try {
                return File.ReadAllBytes(mdb_path);
            } catch (FileNotFoundException) {
            } catch (Exception ex) {
                Logger.LogError("Error loading .mdb " + mdb_path, ex);
            }
            return null;
        }
        
        
        public static void AutoloadCommands() {
            string[] files = FileIO.TryGetFiles(COMMANDS_DLL_DIR, "*.dll");
            if (files == null) return;
            
            foreach (string path in files) { AutoloadCommands(path); }
        }
        
        static void AutoloadCommands(string path) {
            List<Command> cmds;
            
            try {
                cmds = LoadCommands(path);
            } catch (Exception ex) {
                Logger.LogError("Error loading commands from " + path, ex);
                return;
            }
            
            Logger.Log(LogType.SystemActivity, "AUTOLOAD: Loaded {0} from {1}",
                       cmds.Join(c => "/" + c.name), Path.GetFileName(path));
        }
        
        /// <summary> Loads and registers all the commands from the given .dll path </summary>
        public static List<Command> LoadCommands(string path) {
            Assembly lib = LoadAssembly(path);
            List<Command> commands = LoadTypes<Command>(lib);
            
            if (commands.Count == 0)
                throw new InvalidOperationException("No commands in " + path);
            
            foreach (Command cmd in commands)
            {
                if (Command.Find(cmd.name) != null)
                    throw new AlreadyLoadedException("/" + cmd.name + " is already loaded");
                
                Command.Register(cmd);
            }
            return commands;
        }
        
        public static string DescribeLoadError(string path, Exception ex) {
            string file = Path.GetFileName(path);
            
            if (ex is BadImageFormatException) {
                return "&W" + file + " is not a valid assembly, or has an invalid dependency. Details in the error log.";
            } else if (ex is FileLoadException) {
                return "&W" + file + " or one of its dependencies could not be loaded. Details in the error log.";
            }
            
            return "&WAn unknown error occured. Details in the error log.";
            // p.Message("&WError loading plugin. See error logs for more information.");
        }
        
        
        public static void AutoloadPlugins() {
            string[] files = FileIO.TryGetFiles(PLUGINS_DLL_DIR, "*.dll");
            if (files == null) return;
            
            // Ensure that plugin files are loaded in a consistent order,
            //  in case plugins have a dependency on other plugins
            Array.Sort<string>(files);
            
            foreach (string path in files)
            {
                try {
                    LoadPlugin(path, true);
                } catch (Exception ex) {
                    Logger.LogError("Error loading plugins from " + path, ex);
                }
            }
        }
        
        /// <summary> Loads all plugins from the given .dll path. </summary>
        public static List<Plugin> LoadPlugin(string path, bool auto) {
            Assembly lib = LoadAssembly(path);
            List<Plugin> plugins = LoadTypes<Plugin>(lib);
            
            foreach (Plugin pl in plugins)
            {
                if (Plugin.FindCustom(pl.name) != null)
                    throw new AlreadyLoadedException("Plugin " + pl.name + " is already loaded");
                
                Plugin.Load(pl, auto);
            }
            return plugins;
        }
    }
}
