/*
    Copyright 2015 MCGalaxy
    
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
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace MCGalaxy.Platform
{
#if !NETSTANDARD
    public static class DotNetBackend
    {
        public static void Init() { }
        
        public static string GetExePath(string path) {
            return path;
        }
        
        public static Assembly ResolvePluginReference(string name) {
            return null;
        }
    }
#else
    public static class DotNetBackend
    {
        public static void Init() {
            NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), ImportResolver);
        }

        static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchPath) {
            return IntPtr.Zero;
        }

        
        public static string GetExePath(string path) {
            // NET core/5/6 executables tend to use the following structure:
            //   MCGalaxyCLI_core --> MCGalaxyCLI_core.dll
            // in this case, 'RestartPath' will include '.dll' since this file
            //  is actually the managed assembly, but we need to remove '.dll'
            //   as the actual executable which must be started is the non .dll file
            if (path.CaselessEnds(".dll")) path = path.Substring(0, path.Length - 4);
            return path;
        }
        
        public static Assembly ResolvePluginReference(string name) {
            // When there is a .deps.json, dotnet won't automatically always try looking in application's directory to resolve references
            // https://learn.microsoft.com/en-us/dotnet/core/dependency-loading/default-probing?source=recommendations#how-are-the-properties-populated

            try {
                AssemblyName name = new AssemblyName(name);
                string path = name.Name + ".dll";
                if (File.Exists(path)) return Assembly.LoadFrom(path);
            } catch (Exception ex) {
                Logger.LogError("Resolving plugin DLL reference", ex);
            }
            return null;
        }
    }
#endif
}