/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Events;

namespace MCGalaxy.Core {

    public sealed class NotesPlugin : Plugin {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.Version; } }
        public override string name { get { return "Core_NotesPlugin"; } }

        public override void Load(bool startup) {
            OnModActionEvent.Register(HandleModerationAction, Priority.Low);
        }
        
        public override void Unload(bool shutdown) {
            OnModActionEvent.Unregister(HandleModerationAction);
        }
        
        static void HandleModerationAction(ModAction action) {
            switch (action.Type) {
                case ModActionType.Frozen:
                    AddNote(action, "F"); break;
                case ModActionType.Kicked:
                    AddNote(action, "K"); break;
                case ModActionType.Muted:
                    AddNote(action, "M"); break; 
                case ModActionType.Warned:
                    AddNote(action, "W"); break;
                case ModActionType.Ban:
                    string banType = action.Duration.Ticks != 0 ? "T" : "B";
                    AddNote(action, banType); break;
            }
        }
        
        static void AddNote(ModAction e, string type) {
             if (!Server.Config.LogNotes) return;
             string src = e.Actor.name;
             
             string time = DateTime.UtcNow.ToString("dd/MM/yyyy");
             string data = e.Target + " " + type + " " + src + " " + time + " " + 
                           e.Reason.Replace(" ", "%20") + " " + e.Duration.Ticks;
             Server.Notes.Append(data);
        }
    }
}
