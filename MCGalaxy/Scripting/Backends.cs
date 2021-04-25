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
using System.CodeDom.Compiler;
using System;
#if NETSTANDARD
using Microsoft.CodeDom.Providers.DotNetCompilerPlatform;
#endif

namespace MCGalaxy.Scripting {
    public sealed class CSCompiler : ICodeDomCompiler {        
        public override string FileExtension { get { return ".cs"; } }
        public override string LanguageName  { get { return "CSharp"; } }        

        protected override CodeDomProvider CreateProvider() {
#if NETSTANDARD
            return new CSharpCodeProvider();
#else
            return CodeDomProvider.CreateProvider("CSharp");
#endif
        }
        
        protected override void PrepareArgs(CompilerParameters args) {
            args.CompilerOptions += " /unsafe";
        }
        
        public override string CommandSkeleton {
            get {
                return @"//\tAuto-generated command skeleton class.
//\tUse this as a basis for custom MCGalaxy commands.
//\tFile and class should be named a specific way. For example, /update is named 'CmdUpdate.cs' for the file, and 'CmdUpdate' for the class.
// As a note, MCGalaxy is designed for .NET 4.0

// To reference other assemblies, put a ""//reference [assembly filename]"" at the top of the file
//   e.g. to reference the System.Data assembly, put ""//reference System.Data.dll""

// Add any other using statements you need after this
using System;

namespace MCGalaxy 
{{
\tpublic class Cmd{0} : Command
\t{{
\t\t// The command's name (what you put after a slash to use this command)
\t\tpublic override string name {{ get {{ return ""{0}""; }} }}

\t\t// Command's shortcut, can be left blank (e.g. ""/Copy"" has a shortcut of ""c"")
\t\tpublic override string shortcut {{ get {{ return """"; }} }}

\t\t// Which submenu this command displays in under /Help
\t\tpublic override string type {{ get {{ return ""other""; }} }}

\t\t// Whether or not this command can be used in a museum. Block/map altering commands should return false to avoid errors.
\t\tpublic override bool museumUsable {{ get {{ return true; }} }}

\t\t// The default rank required to use this command. Valid values are:
\t\t//   LevelPermission.Guest, LevelPermission.Builder, LevelPermission.AdvBuilder,
\t\t//   LevelPermission.Operator, LevelPermission.Admin, LevelPermission.Nobody
\t\tpublic override LevelPermission defaultRank {{ get {{ return LevelPermission.Guest; }} }}

\t\t// This is for when a player executes this command by doing /{0}
\t\t//   p is the player object for the player executing the command. 
\t\t//   message is the arguments given to the command. (e.g. for '/update this', message is ""this"")
\t\tpublic override void Use(Player p, string message)
\t\t{{
\t\t\tp.Message(""Hello World!"");
\t\t}}

\t\t// This is for when a player does /Help {0}
\t\tpublic override void Help(Player p)
\t\t{{
\t\t\tp.Message(""/{0} - Does stuff. Example command."");
\t\t}}
\t}}
}}";
            }
        }
        
        public override string PluginSkeleton {
            get {
                return @"//This is an example plugin source!
using System;
namespace MCGalaxy
{{
\tpublic class {0} : Plugin
\t{{
\t\tpublic override string name {{ get {{ return ""{0}""; }} }}
\t\tpublic override string MCGalaxy_Version {{ get {{ return ""{2}""; }} }}
\t\tpublic override string welcome {{ get {{ return ""Loaded Message!""; }} }}
\t\tpublic override string creator {{ get {{ return ""{1}""; }} }}

\t\tpublic override void Load(bool startup)
\t\t{{
\t\t\t//LOAD YOUR PLUGIN WITH EVENTS OR OTHER THINGS!
\t\t}}
                        
\t\tpublic override void Unload(bool shutdown)
\t\t{{
\t\t\t//UNLOAD YOUR PLUGIN BY SAVING FILES OR DISPOSING OBJECTS!
\t\t}}
                        
\t\tpublic override void Help(Player p)
\t\t{{
\t\t\t//HELP INFO!
\t\t}}
\t}}
}}";
            }
        }
    }
    
    public sealed class VBCompiler : ICodeDomCompiler {       
        public override string FileExtension { get { return ".vb"; } }
        public override string LanguageName  { get { return "Visual Basic"; } }
        
        protected override CodeDomProvider CreateProvider() {
#if NETSTANDARD
            return new VBCodeProvider();
#else
            return CodeDomProvider.CreateProvider("VisualBasic");
#endif
        }
        
        protected override void PrepareArgs(CompilerParameters args) { }
        
        public override string CommandSkeleton {
            get {
                return @"'\tAuto-generated command skeleton class.
'\tUse this as a basis for custom MCGalaxy commands.
'\tFile and class should be named a specific way. For example, /update is named 'CmdUpdate.vb' for the file, and 'CmdUpdate' for the class.
' As a note, MCGalaxy is designed for .NET 4.0.

' To reference other assemblies, put a ""//reference [assembly filename]"" at the top of the file
'   e.g. to reference the System.Data assembly, put ""//reference System.Data.dll""

' Add any other Imports statements you need after this
Imports System

Namespace MCGalaxy
\tPublic Class Cmd{0}
\t\tInherits Command

\t\t' The command's name (what you put after a slash to use this command)
\t\tPublic Overrides ReadOnly Property name() As String
\t\t\tGet
\t\t\t\tReturn ""{0}""
\t\t\tEnd Get
\t\tEnd Property

\t\t' Command's shortcut, can be left blank (e.g. ""/Copy"" has a shortcut of ""c"")
\t\tPublic Overrides ReadOnly Property shortcut() As String
\t\t\tGet
\t\t\t\tReturn """"
\t\t\tEnd Get
\t\tEnd Property

\t\t' Which submenu this command displays in under /Help   
\t\tPublic Overrides ReadOnly Property type() As String
\t\t\tGet
\t\t\t\tReturn ""other""
\t\t\tEnd Get
\t\t End Property

\t\t' Whether or not this command can be used in a museum. Block/map altering commands should return False to avoid errors.
\t\tPublic Overrides ReadOnly Property museumUsable() As Boolean
\t\t\tGet
\t\t\t\tReturn True
\t\t\tEnd Get
\t\tEnd Property

\t\t' The default rank required to use this command. Valid values are:
\t\t'   LevelPermission.Guest, LevelPermission.Builder, LevelPermission.AdvBuilder,
\t\t'   LevelPermission.Operator, LevelPermission.Admin, LevelPermission.Nobody
\t\tPublic Overrides ReadOnly Property defaultRank() As LevelPermission
\t\t\tGet
\t\t\t\tReturn LevelPermission.Guest
\t\t\tEnd Get
\t\tEnd Property

\t\t' This is for when a player executes this command by doing /{0}
\t\t'   p is the player object for the player executing the command.
\t\t'   message is the arguments given to the command. (e.g. for '/update this', message is ""this"")
\t\tPublic Overrides Sub Use(p As Player, message As String)
\t\t\tp.Message(""Hello World!"")
\t\tEnd Sub

\t\t' This is for when a player does /Help {0}
\t\tPublic Overrides Sub Help(p As Player)
\t\t\tp.Message(""/{0} - Does stuff. Example command."")
\t\tEnd Sub
\tEnd Class
End Namespace";
            }
        }
        
        public override string PluginSkeleton {
            get {
                return @"' This is an example plugin source!
Imports System

Namespace MCGalaxy
\tPublic Class {0}
\t\tInherits Plugin

\t\tPublic Overrides ReadOnly Property name() As String
\t\t\tGet
\t\t\t\tReturn ""{0}""
\t\t\tEnd Get
\t\t End Property
\t\tPublic Overrides ReadOnly Property MCGalaxy_Version() As String
\t\t\tGet
\t\t\t\tReturn ""{2}""
\t\t\tEnd Get
\t\t End Property
\t\tPublic Overrides ReadOnly Property welcome() As String
\t\t\tGet
\t\t\t\tReturn ""Loaded Message!""
\t\t\tEnd Get
\t\t End Property
\t\tPublic Overrides ReadOnly Property creator() As String
\t\t\tGet
\t\t\t\tReturn ""{1}""
\t\t\tEnd Get
\t\t End Property

\t\tPublic Overrides Sub Load(startup As Boolean)
\t\t\t' LOAD YOUR PLUGIN WITH EVENTS OR OTHER THINGS!
\t\tEnd Sub
                        
\t\tPublic Overrides Sub Unload(shutdown As Boolean)
\t\t\t' UNLOAD YOUR PLUGIN BY SAVING FILES OR DISPOSING OBJECTS!
\t\tEnd Sub
                        
\t\tPublic Overrides Sub Help(p As Player)
\t\t\t' HELP INFO!
\t\tEnd Sub
\tEnd Class
End Namespace";
            }
        }
    }    
}