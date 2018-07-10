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

namespace MCGalaxy.Scripting {
    public sealed class ScriptingCS : IScripting {    
        
        public override string Ext { get { return ".cs"; } }        
        public override string ProviderName { get { return "CSharp"; } }
        
        public override string CommandSkeleton {
            get {
                return @"//\tAuto-generated command skeleton class.
//\tUse this as a basis for custom MCGalaxy commands.
//\tFile and class should be named a specific way. For example, /update is named 'CmdUpdate.cs' for the file, and 'CmdUpdate' for the class.
// As a note, MCGalaxy is designed for .NET 4.0

// To reference other assemblies, put a ""Reference [assembly filename]"" at the top of the file
//   e.g. to reference the System.Data assembly, put ""Reference System.Data.dll""

// Add any other using statements you need after this
using System;

namespace MCGalaxy 
{{
\tpublic class Cmd{0} : Command
\t{{
\t\t// The command's name (what you put after a slash to use this command)
\t\tpublic override string name {{ get {{ return ""{1}""; }} }}

\t\t// Command's shortcut (please take care not to use an existing one, or you may have issues.)
\t\tpublic override string shortcut {{ get {{ return """"; }} }}

\t\t// Determines which submenu this command displays in under /Help.
\t\tpublic override string type {{ get {{ return ""other""; }} }}

\t\t// Determines whether or not this command can be used in a museum. Block/map altering commands should be made false to avoid errors.
\t\tpublic override bool museumUsable {{ get {{ return false; }} }}

\t\t// Determines the default rank required to use this command. Valid values are:
\t\t// LevelPermission.Nobody, LevelPermission.Banned, LevelPermission.Guest
\t\t// LevelPermission.Builder, LevelPermission.AdvBuilder, LevelPermission.Operator, LevelPermission.Admin
\t\tpublic override LevelPermission defaultRank {{ get {{ return LevelPermission.Banned; }} }}

\t\t// This is where the magic happens, naturally.
\t\t// p is the player object for the player executing the command. message is everything after the command invocation itself.
\t\tpublic override void Use(Player p, string message)
\t\t{{
\t\t\tp.Message(""Hello World!"");
\t\t}}

\t\t// This one controls what happens when you use /Help [commandname].
\t\tpublic override void Help(Player p)
\t\t{{
\t\t\tp.Message(""/{1} - Does stuff. Example command."");
\t\t}}
\t}}
}}";
            }
        }
    }
    
    public sealed class ScriptingVB : IScripting {
        
        public override string Ext { get { return ".vb"; } }
        public override string ProviderName { get { return "VisualBasic"; } }
        
        public override string CommandSkeleton {
            get {
                return @"'\tAuto-generated command skeleton class.
'\tUse this as a basis for custom MCGalaxy commands.
'\tFile and class should be named a specific way.  For example, /update is named 'CmdUpdate.vb' for the file, and 'CmdUpdate' for the class.
' As a note, MCGalaxy is designed for .NET 4.0.

' To reference other assemblies, put a ""Reference [assembly filename]"" at the top of the file
'   e.g. to reference the System.Data assembly, put ""Reference System.Data.dll""

' Add any other Imports statements you need after this
Imports System

Namespace MCGalaxy
\tPublic Class Cmd{0}
\t\tInherits Command

' The command's name (what you put after a slash to use this command)
\t\tPublic Overrides ReadOnly Property name() As String
\t\t\tGet
\t\t\t\tReturn ""{1}""
\t\t\tEnd Get
\t\tEnd Property

' Command's shortcut (please take care not to use an existing one, or you may have issues.)
\t\tPublic Overrides ReadOnly Property shortcut() As String
\t\t\tGet
\t\t\t\tReturn """"
\t\t\tEnd Get
\t\tEnd Property

' Determines which submenu this command displays in under /Help.   
\t\tPublic Overrides ReadOnly Property type() As String
\t\t\tGet
\t\t\t\tReturn ""other""
\t\t\tEnd Get
\t\t End Property

' Determines whether or not this command can be used in a museum. Block/map altering commands should be made false to avoid errors.
\t\tPublic Overrides ReadOnly Property museumUsable() As Boolean
\t\t\tGet
\t\t\t\tReturn False
\t\t\tEnd Get
\t\tEnd Property

' Determines the default rank required to use this command. Valid values are:
' LevelPermission.Nobody, LevelPermission.Banned, LevelPermission.Guest
' LevelPermission.Builder, LevelPermission.AdvBuilder, LevelPermission.Operator, LevelPermission.Admin
\t\tPublic Overrides ReadOnly Property defaultRank() As LevelPermission
\t\t\tGet
\t\t\t\tReturn LevelPermission.Banned
\t\t\tEnd Get
\t\tEnd Property

' This is where the magic happens, naturally.
' p is the player object for the player executing the command.  message is everything after the command invocation itself. 
\t\tPublic Overrides Sub Use(p As Player, message As String)
\t\t\tp.Message(""Hello World!"")
\t\tEnd Sub

' This one controls what happens when you use /Help [commandname].
\t\tPublic Overrides Sub Help(p As Player)
\t\t\tp.Message(""/{1} - Does stuff. Example command."")

\t\tEnd Sub
\tEnd Class
End Namespace";
            }
        }
    }    
}