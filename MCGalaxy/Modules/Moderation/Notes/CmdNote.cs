/*
    Copyright 2015-2024 MCGalaxy
        
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
*/
using MCGalaxy.Commands.Moderation;
using MCGalaxy.Events;

namespace MCGalaxy.Modules.Moderation.Notes 
{
    public class CmdNote : Command2 
    {
        public override string name { get { return "Note"; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        protected virtual bool announce { get { return true; } }
        protected virtual ModActionType modActionType { get { return ModActionType.Noted; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (!Server.Config.LogNotes) {
                p.Message("Notes logging must be enabled to note players."); return;
            }
            if (message.Length == 0) { Help(p); return; }
            string[] args = message.SplitSpaces(2);
            if (args.Length == 1) { p.Message("&WYou must provide text for the note."); return; }

            string note = args[1];
            string target = ModActionCmd.FindName(p, "note", "/"+name, "", args[0], ref note);
            if (target == null) return;

            note = ModActionCmd.ExpandReason(p, note);
            if (note == null) return;

            Group group = ModActionCmd.CheckTarget(p, data, "note", target);
            if (group == null) return;

            ModAction action = new ModAction(target, p, modActionType, note);
            action.Announce  = announce;
            OnModActionEvent.Call(action);
        }

        public override void Help(Player p) {
            p.Message("&T/Note [player] [text]");
            p.Message("&HAdds a note to [player]'s /notes");
            p.Message("&HFor [text], @number can be used as a shortcut for that rule.");
        }
    }
    
    public class CmdOpNote : CmdNote 
    {
        public override string name { get { return "OpNote"; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        protected override bool announce { get { return false; } }
        protected override ModActionType modActionType { get { return ModActionType.OpNoted; } }
        
        public override void Help(Player p) {
            p.Message("&T/OpNote [player] [text]");
            p.Message("&HAdds a note to [player]'s /notes that only ops may see.");
            p.Message("&HFor [text], @number can be used as a shortcut for that rule.");
        }
    }
}
