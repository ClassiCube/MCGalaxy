/*
    Copyright 2015 MCGalaxy team
    
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
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands {
    
    public abstract class ReplaceCmd : Command {
        
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return false; } }

        protected ExtBlock[] toAffect;
        protected ExtBlock target;
        
        public override void Use(Player p, string message) {
            string[] parts = message.Split(' ');
            for (int i = 0; i < parts.Length; i++)
                parts[i] = parts[i].ToLower();
            if (parts.Length != 2) {
                p.SendMessage("You need to provide exactly two arguments.");
                Help(p); return;
            }

            string[] names = parts[0].Split(',');
            toAffect = new ExtBlock[names.Length];
            for (int i = 0; i < names.Length; i++) {
                ExtBlock block;
                block.Type = DrawCmd.GetBlock(p, names[i], out block.ExtType);
                
                if (block.Type == Block.Zero) return;
                toAffect[i] = block;
            }
            
            target.Type = DrawCmd.GetBlock(p, parts[1], out target.ExtType);
            if (target.Type == Block.Zero) return;           
            BeginReplace(p);
        }
        
        protected abstract void BeginReplace(Player p);
    }
}
