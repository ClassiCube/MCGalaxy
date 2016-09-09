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
        public abstract string Name { get; }
        
        /// <summary> Performs a tick for this instruction. </summary>
        /// <returns> false if the bot should proceed to execute the 
        /// next instruction in the same tick. </returns>
        public abstract bool Execute(PlayerBot bot, InstructionData data);
        
        /// <summary> Parses the given line which contains the metadata for this instruction. </summary>
        public virtual InstructionData Parse(string[] args) {
            return default(InstructionData);
        }
        
        /// <summary> All instructions that bots can execute. </summary>
        public static List<BotInstruction> Instructions = new List<BotInstruction>() {
            new NodInstruction(), new SpinInstruction(), new HuntInstruction(), new KillInstruction(),      	
        	new TeleportInstruction(), new WalkInstruction(), new JumpInstruction(), new SpeedInstruction(),        	
        	new RemoveInstruction(), new ResetInstruction(), new LinkScriptInstruction(), new WaitInstruction(),
        };
        
        /// <summary> Finds the instruction which has the given identifying name. </summary>
        public static BotInstruction Find(string name) {
            foreach (BotInstruction ins in Instructions) {
                if (ins.Name.CaselessEq(name)) return ins;
            }
            return null;
        }
    }
    
    public struct InstructionData { 
        public string Name;
        public int seconds, rotspeed; 
        public object Metadata;
    }
}
