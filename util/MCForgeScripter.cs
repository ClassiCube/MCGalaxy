/*
	Copyright 2013 - Headdetect, And his almightyness

    Dual-licensed under the	Educational Community License, Version 2.0 and
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

        private static readonly CompilerParameters _settings = new CompilerParameters(new [] {"mscorlib.dll", "MCGalaxy_.dll", "MCGalaxy.exe"}) {
            GenerateInMemory = true
        };

        /// <summary>
        /// Compiles the specified source code.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="language">The language.</param>
        /// <returns>A result from the compilation</returns>
        public static CompileResult Compile ( string text, ScriptLanguage language ) {
            CodeDomProvider provider = null;

            switch ( language ) {
                case ScriptLanguage.CSharp:
                    provider = CodeDomProvider.CreateProvider( "CSharp" );
                    break;
                case ScriptLanguage.VB:
                    provider = CodeDomProvider.CreateProvider( "VisualBasic" );
                    break;
                case ScriptLanguage.JavaScript:
                    throw new NotImplementedException( "This language interface has not been implemented yet." );

            }

            if ( provider == null ) {
                throw new NotImplementedException( "You must have .net developer tools. (You need a visual studio)" );
            }

            var compile = provider.CompileAssemblyFromSource( _settings, text );

            if ( compile.Errors.Count > 0 ) {
                return new CompileResult( null, compile.Errors );
            }

            var assembly = compile.CompiledAssembly;
            var list = new List<Command>();

            foreach ( Command command in from type in assembly.GetTypes()
                                         where type.BaseType == typeof( Command )
                                         select Activator.CreateInstance( type ) as Command ) {
                list.Add( command );
            }

            return new CompileResult( list.ToArray(), null );
        }

        public static Command[] FromAssemblyFile ( string file ) {
            Assembly lib = Assembly.LoadFile ( file );
            var list = new List<Command>();

            foreach ( var instance in lib.GetTypes().Where( t => t.BaseType == typeof( Command ) ).Select( Activator.CreateInstance ) ) {
                list.Add( (Command) instance );
            }

            return list.ToArray ();
        }

    }

    public sealed class CompileResult {

        /// <summary>
        /// Array of errors, if any.
        /// </summary>
        public CompilerErrorCollection CompilerErrors { get; internal set; }

        /// <summary>
        /// Gets the command object.
        /// </summary>
        public Command[] Commands { get; internal set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileResult"/> class.
        /// </summary>
        /// <param name="commands">The command object.</param>
        /// <param name="errors">The errors.</param>
        public CompileResult ( Command[] commands, CompilerErrorCollection errors ) {
            Commands = commands;
            CompilerErrors = errors;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompileResult"/> class.
        /// </summary>
        public CompileResult () { }
    }

    public enum ScriptLanguage {

        /// <summary>
        /// C#.net Scripting Interface Language
        /// </summary>
        CSharp,

        /// <summary>
        /// VB.net Scripting Interface Language
        /// </summary>
        VB,

        /// <summary>
        /// JavaScript Scripting Interface Language.
        /// NOTE: Not yet implemented
        /// </summary>
        JavaScript

    }
}
