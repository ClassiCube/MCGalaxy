/*
    Copyright 2013 - Headdetect, And his almightyness

    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MCGalaxy.Util {
    public sealed class MCGalaxyScripter {

        /// <summary> Compiles the specified source code. </summary>
        /// <param name="text">The text.</param>
        /// <param name="language">The language.</param>
        /// <returns>A result from the compilation</returns>
        public static CompileResult Compile(string text, ScriptLanguage language) {
            Scripting engine = language == ScriptLanguage.VB ? Scripting.VB : Scripting.CS;           
            CompilerParameters args = new CompilerParameters();
            args.GenerateInMemory = true;
            
            CompilerResults results = engine.CompileSource(text, args);
            if (results.Errors.HasErrors)
                return new CompileResult(null, results.Errors);
            List<Command> list = Scripting.LoadFrom(results.CompiledAssembly);
            return new CompileResult(list.ToArray(), null);
        }

        public static Command[] FromAssemblyFile(string file) {
            Assembly lib = Assembly.LoadFile(file);
            return Scripting.LoadFrom(lib).ToArray();
        }
    }

    public sealed class CompileResult {
        /// <summary> Array of errors, if any. </summary>
        public CompilerErrorCollection CompilerErrors { get; internal set; }

        /// <summary> Gets the command object. </summary>
        public Command[] Commands { get; internal set; }

        /// <summary> Initializes a new instance of the <see cref="CompileResult"/> class. </summary>
        /// <param name="commands">The command object.</param>
        /// <param name="errors">The errors.</param>
        public CompileResult ( Command[] commands, CompilerErrorCollection errors ) {
            Commands = commands;
            CompilerErrors = errors;
        }

        /// <summary> Initializes a new instance of the <see cref="CompileResult"/> class. </summary>
        public CompileResult () { }
    }

    public enum ScriptLanguage {
        /// <summary> C#.net Scripting Interface Language </summary>
        CSharp,
        /// <summary> VB.net Scripting Interface Language </summary>
        VB,
        /// <summary> JavaScript Scripting Interface Language. NOTE: Not yet implemented </summary>
        JavaScript
    }
}
