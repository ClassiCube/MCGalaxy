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
#if !DISABLE_COMPILING
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

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
        /// <summary> Returns the starting characters for a comment </summary>
        /// <example> For C# this is "//" </example>
        public virtual string CommentPrefix { get { return "//"; } }
        
        public string CommandPath(string name) { return COMMANDS_SOURCE_DIR + "Cmd" + name + FileExtension; }
        public string PluginPath(string name)  { return PLUGINS_SOURCE_DIR  + name + FileExtension; }
        
        public static List<ICompiler> Compilers = new List<ICompiler>() { 
            new CSCompiler(), new VBCompiler() 
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
        
        
        /// <summary> Attempts to compile the given source code file to a .dll file. </summary>
        /// <remarks> Logs errors to IScripting.ErrorPath. </remarks>
        public CompilerResults Compile(string srcPath, string dstPath) {
            return Compile(new [] { srcPath }, dstPath);
        }
        
        /// <summary> Attempts to compile the given source code files to a .dll file. </summary>
        /// <remarks> Logs errors to IScripting.ErrorPath. </remarks>
        public CompilerResults Compile(string[] srcPaths, string dstPath) {
            CompilerResults results = DoCompile(srcPaths, dstPath);
            if (!results.Errors.HasErrors) return results;
            
            SourceMap sources = new SourceMap(srcPaths);
            StringBuilder sb  = new StringBuilder();
            sb.AppendLine("############################################################");
            sb.AppendLine("Errors when compiling " + srcPaths.Join());
            sb.AppendLine("############################################################");
            sb.AppendLine();
            
            foreach (CompilerError err in results.Errors) 
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
            return results;
        }
        
        /// <summary> Compiles the given source code. </summary>
        protected abstract CompilerResults DoCompile(string[] srcPaths, string dstPath);
        
        public static string DescribeError(CompilerError err, string[] srcs, string text) {
            string type = err.IsWarning ? "Warning" : "Error";
            string file = Path.GetFileName(err.FileName);
            // TODO line 0 shouldn't appear
            
            // Include filename if compiling multiple source code files
            return string.Format("{0}{1} on line {2}{3}", type, text, err.Line,
                                 srcs.Length > 1 ? " in " + file : "");
        }
    }
    
    /// <summary> Compiles source code files from a particular language into a .dll file, using a CodeDomProvider for the compiler. </summary>
    public abstract class ICodeDomCompiler : ICompiler 
    {
        readonly object compilerLock = new object();
        protected CodeDomProvider compiler;
        
        // Lazy init compiler when it's actually needed
        protected void InitCompiler(string language) {
            lock (compilerLock) {
                if (compiler != null) return;
                compiler = CodeDomProvider.CreateProvider(language);
                if (compiler != null) return;
                
                Logger.Log(LogType.Warning,
                           "WARNING: {0} compiler is missing, you will be unable to compile {1} files.",
                           FullName, FileExtension);
                // TODO: Should we log "You must have .net developer tools. (You need a visual studio)" ?
            }
        }
        
        protected override CompilerResults DoCompile(string[] srcPaths, string dstPath) {
            CompilerParameters args = new CompilerParameters();
            args.GenerateExecutable      = false;
            args.IncludeDebugInformation = true;
            args.OutputAssembly          = dstPath;
            
            for (int i = 0; i < srcPaths.Length; i++) 
            {
                // CodeDomProvider doesn't work properly with relative paths
                string path = Path.GetFullPath(srcPaths[i]);
                
                AddReferences(path, args);
                srcPaths[i] = path;
            }

            // TODO use absolute path?
            string serverDLL = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            args.ReferencedAssemblies.Add(serverDLL);
            return DoCompile(srcPaths, args);
        }
        
        protected abstract CompilerResults DoCompile(string[] srcPaths, CompilerParameters args);
        
        void AddReferences(string path, CompilerParameters args) {
            // Allow referencing other assemblies using '//reference [assembly name]' at top of the file
            using (StreamReader r = new StreamReader(path)) {               
                string refPrefix = CommentPrefix + "reference ";
                string line;
                
                while ((line = r.ReadLine()) != null) {
                    if (!line.CaselessStarts(refPrefix)) break;
                    
                    int index = line.IndexOf(' ') + 1;
                    // For consistency with C#, treat '//reference X.dll;' as '//reference X.dll'
                    string assem = line.Substring(index).Replace(";", "");
                    
                    args.ReferencedAssemblies.Add(assem);
                }
            }
        }
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
            for (int i = 0; i < files.Length; i++) {
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
