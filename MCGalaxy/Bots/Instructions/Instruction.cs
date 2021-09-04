/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Bots {
    
    /// <summary> Represents an action that a bot will perform </summary>
    public abstract class BotInstruction {
        
        /// <summary> Gets the identifying name for this instruction. </summary>
        public string Name;
        
        /// <summary> Performs a tick for this instruction. </summary>
        /// <returns> false if the bot should proceed to execute the 
        /// next instruction in the same tick. </returns>
        public abstract bool Execute(PlayerBot bot);
        
        /// <summary> Parses the given arguments which contains the data for this instruction. </summary>
        public virtual void Parse(string[] args) { }
        
        /// <summary> Writes the data for this instruction to the given AI file. </summary>
        public virtual void Output(Player p, string[] args, TextWriter w) {
            w.WriteLine(Name);
        }
        
        /// <summary> Returns the help for this instruction. n</summary>
        public abstract string[] Help { get; }
        
        /// <summary> All instructions that bots can execute. </summary>
        public static List<BotInstruction> Instructions = new List<BotInstruction>() {
            new NodInstruction(), new SpinInstruction(), 
            new HuntInstruction(), new KillInstruction(), new StareInstruction(),
            new TeleportInstruction(), new WalkInstruction(), new JumpInstruction(), new SpeedInstruction(),            
            new RemoveInstruction(), new ResetInstruction(), new LinkScriptInstruction(), new WaitInstruction(),
        };
        
        /// <summary> Creates the instruction which has the given identifying name. </summary>
        /// <remarks> Returns null if there is no instruction with the given name </remarks>
        public static BotInstruction Create(string name) {
            foreach (BotInstruction ins in Instructions) {
                if (ins.Name.CaselessEq(name)) return ins;
            }
            return null;
        }
    }
}
