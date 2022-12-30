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
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using MCGalaxy.Scripting;

namespace MCGalaxy.Modules.Compiling 
{
    /// <summary> Compiles source code files for a particular programming language into a .dll </summary>
    public abstract class ICompiler 
    {   
        public const string COMMANDS_SOURCE_DIR = "extra/commands/source/";
        public const string PLUGINS_SOURCE_DIR  = "plugins/";
        public const string ERROR_LOG_PATH      = "logs/errors/compiler.log";
        
        /// <summary> Default file extension used for source code files </summary>
        /// <example> .cs, .vb </example>
        public abstract string FileExtension { get; }
        /// <summary> The short name of this programming language </summary>
        /// <example> C#, VB </example>
        public abstract string ShortName { get; }
        /// <summary> The full name of this programming language </summary>
        /// <example> CSharp, Visual Basic </example>
        public abstract string FullName { get; }
        /// <summary> Returns source code for an example Command </summary>
        public abstract string CommandSkeleton { get; }
        /// <summary> Returns source code for an example Plugin </summary>
        public abstract string PluginSkeleton { get; }
        
        public string CommandPath(string name) { return COMMANDS_SOURCE_DIR + "Cmd" + name + FileExtension; }
        public string PluginPath(string name)  { return PLUGINS_SOURCE_DIR  + name + FileExtension; }
        
        public static List<ICompiler> Compilers = new List<ICompiler>() { 
            new CSCompiler()
        };


        static string FormatSource(string source, params string[] args) {
            // Always use \r\n line endings so it looks correct in Notepad
            source = source.Replace(@"\t", "\t");
            source = source.Replace("\n", "\r\n");
            return string.Format(source, args);
        }
        
        /// <summary> Generates source code for an example command, 
        /// preformatted with the given command name </summary>
        public string GenExampleCommand(string cmdName) {
            cmdName = cmdName.ToLower().Capitalize();
            return FormatSource(CommandSkeleton, cmdName);
        }
        
        /// <summary> Generates source code for an example plugin, 
        /// preformatted with the given name and creator </summary>
        public string GenExamplePlugin(string plugin, string creator) {
            return FormatSource(PluginSkeleton, plugin, creator, Server.Version);
        }


        /// <summary> Attempts to compile the given source code files to a .dll file. </summary>
        /// <param name="logErrors"> Whether to log compile errors to ERROR_LOG_PATH </param>
        public ICompilerErrors Compile(string[] srcPaths, string dstPath, bool logErrors) {
            ICompilerErrors errors = DoCompile(srcPaths, dstPath);
            if (!errors.HasErrors || !logErrors) return errors;
            
            SourceMap sources = new SourceMap(srcPaths);
            StringBuilder sb  = new StringBuilder();
            sb.AppendLine("############################################################");
            sb.AppendLine("Errors when compiling " + srcPaths.Join());
            sb.AppendLine("############################################################");
            sb.AppendLine();
            
            foreach (ICompilerError err in errors) 
            {
                string type = err.IsWarning ? "Warning" : "Error";
                sb.AppendLine(DescribeError(err, srcPaths, "") + ":");
                
                if (err.Line > 0) sb.AppendLine(sources.Get(err.FileName, err.Line - 1));
                if (err.Column > 0) sb.Append(' ', err.Column - 1);
                sb.AppendLine("^-- " + type + " #" + err.ErrorNumber + " - " + err.ErrorText);
                
                sb.AppendLine();
                sb.AppendLine("-------------------------");
                sb.AppendLine();
            }
            
            using (StreamWriter w = new StreamWriter(ERROR_LOG_PATH, true)) {
                w.Write(sb.ToString());
            }
            return errors;
        }

        public static string DescribeError(ICompilerError err, string[] srcs, string text) {
            string type = err.IsWarning ? "Warning" : "Error";
            string file = Path.GetFileName(err.FileName);
            
            // Include filename if compiling multiple source code files
            return string.Format("{0}{1}{2}{3}", type, text,
                                 err.Line    > 0 ? " on line " + err.Line : "",
                                 srcs.Length > 1 ? " in " + file          : "");
        }

        
        /// <summary> Compiles the given source code. </summary>
        protected abstract ICompilerErrors DoCompile(string[] srcPaths, string dstPath);


        /// <summary> Converts source file paths to full paths, 
        /// then returns list of parsed referenced assemblies </summary>
        public static List<string> ProcessInput(string[] srcPaths, string commentPrefix) {
            List<string> referenced = new List<string>();
            
            for (int i = 0; i < srcPaths.Length; i++) 
            {
                // CodeDomProvider doesn't work properly with relative paths
                string path = Path.GetFullPath(srcPaths[i]);
                
                AddReferences(path, commentPrefix, referenced);
                srcPaths[i] = path;
            }

            referenced.Add(Server.GetServerDLLPath());
            return referenced;
        }

        static void AddReferences(string path, string commentPrefix, List<string> referenced) {
            // Allow referencing other assemblies using '//reference [assembly name]' at top of the file
            using (StreamReader r = new StreamReader(path)) {               
                string refPrefix = commentPrefix + "reference ";
                string plgPrefix = commentPrefix + "pluginref ";
                string line;
                
                while ((line = r.ReadLine()) != null) 
                {
                    if (line.CaselessStarts(refPrefix)) {
                        referenced.Add(GetDLL(line));
                    } else if (line.CaselessStarts(plgPrefix)) {
                        path = Path.Combine(IScripting.PLUGINS_DLL_DIR, GetDLL(line));
                        referenced.Add(Path.GetFullPath(path));
                    } else {
                        continue;
                    }
                }
            }
        }

        static string GetDLL(string line) {
            int index = line.IndexOf(' ') + 1;
            // For consistency with C#, treat '//reference X.dll;' as '//reference X.dll'
            return line.Substring(index).Replace(";", "");
        }
    }

    public class ICompilerErrors : List<ICompilerError>
    {
        public bool HasErrors {
            get { return FindIndex(ce => !ce.IsWarning) >= 0; }
        }
    }

    public class ICompilerError
    {
        public int Line, Column;
        public string ErrorNumber, ErrorText;
        public bool IsWarning;
        public string FileName;
    }

    
    class SourceMap 
    {
        readonly string[] files;
        readonly List<string>[] sources;
        
        public SourceMap(string[] paths) {
            files   = paths;
            sources = new List<string>[paths.Length];
        }
        
        int FindFile(string file) {
            for (int i = 0; i < files.Length; i++) 
            {
                if (file.CaselessEq(files[i])) return i;
            }
            return -1;
        }
        
        /// <summary> Returns the given line in the given source code file </summary>
        public string Get(string file, int line) {
            int i = FindFile(file);
            if (i == -1) return "";
            
            List<string> source = sources[i];
            if (source == null) {
                try {
                    source = Utils.ReadAllLinesList(file);
                } catch {
                    source = new List<string>();
                }
                sources[i] = source;
            }            
            return line < source.Count ? source[line] : "";
        }
    }
}
#endif
