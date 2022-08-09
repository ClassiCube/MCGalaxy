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
#if !MCG_STANDALONE
using System;
using System.IO;

namespace MCGalaxy.Modules.Compiling 
{    
    public static class CompilerOperations 
    {   
        public static ICompiler GetCompiler(Player p, string name) {
            if (name.Length == 0) return ICompiler.Compilers[0];
            
            foreach (ICompiler comp in ICompiler.Compilers) 
            {
                if (comp.ShortName.CaselessEq(name)) return comp;
            }
            
            p.Message("&WUnknown language \"{0}\"", name);
            p.Message("&HAvailable languages: &f{0}",
                      ICompiler.Compilers.Join(c => c.ShortName + " (" + c.FullName + ")"));
            return null;
        }
        
        
        public static bool CreateCommand(Player p, string name, ICompiler compiler) {
            string path   = compiler.CommandPath(name);
            string source = compiler.GenExampleCommand(name);
            
            return CreateFile(p, name, path, "command &fCmd", source);
        }
    	
    	public static bool CreatePlugin(Player p, string name, ICompiler compiler) {
            string path    = compiler.PluginPath(name);
            string creator = p.IsSuper ? Server.Config.Name : p.truename;
            string source  = compiler.GenExamplePlugin(name, creator);
            
            return CreateFile(p, name, path, "plugin &f", source);
        }
    	
    	static bool CreateFile(Player p, string name, string path, string type, string source) {
            if (File.Exists(path)) {
                p.Message("File {0} already exists. Choose another name.", path); 
                return false;
            }
    		
            File.WriteAllText(path, source);
            p.Message("Successfully saved example {2}{0} &Sto {1}", name, path, type);
            return true;
        }


        const int MAX_LOG = 2;
        
        /// <summary> Attempts to compile the given source code files into a .dll </summary>
        /// <param name="p"> Player to send messages to </param>
        /// <param name="type"> Type of files being compiled (e.g. Plugin, Command) </param>
        /// <param name="srcs"> Path of the source code files </param>
        /// <param name="dst"> Path to the destination .dll </param>
        /// <returns> Whether compilation succeeded </returns>
        public static bool Compile(Player p, ICompiler compiler, string type, string[] srcs, string dst) {
            foreach (string path in srcs) 
            {
                if (File.Exists(path)) continue;
                
                p.Message("File &9{0} &Snot found.", path);
                return false;
            }
            
            ICompilerErrors errors = compiler.Compile(srcs, dst, true);
            if (!errors.HasErrors) {
                p.Message("{0} compiled successfully from {1}", 
                        type, srcs.Join(file => Path.GetFileName(file)));
                return true;
            }
            
            SummariseErrors(errors, srcs, p);
            return false;
        }
        
        static void SummariseErrors(ICompilerErrors errors, string[] srcs, Player p) {
            int logged = 0;
            foreach (ICompilerError err in errors) 
            {
                p.Message("&W{1} - {0}", err.ErrorText,
                          ICompiler.DescribeError(err, srcs, " #" + err.ErrorNumber));
                logged++;
                if (logged >= MAX_LOG) break;
            }
            
            if (errors.Count > MAX_LOG) {
                p.Message(" &W.. and {0} more", errors.Count - MAX_LOG);
            }
            p.Message("&WCompilation error. See " + ICompiler.ERROR_LOG_PATH + " for more information.");
        }
    }
}
#endif
