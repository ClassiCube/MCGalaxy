/*
    Copyright 2015 MCGalaxy
        
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
using System;
using MCGalaxy.Events;

namespace MCGalaxy.Modules.Moderation.Notes 
{
    public sealed class NotesPlugin : Plugin 
    {
        public override string name { get { return "Notes"; } }


        static Command[] cmds = new Command[] { new CmdNotes(), new CmdMyNotes(), new CmdNote(), new CmdOpNote(), };

        public override void Load(bool startup) {
            OnModActionEvent.Register(HandleModerationAction, Priority.Low);
            foreach (Command cmd in cmds) { Command.Register(cmd); }
            NoteAcronym.Init();
        }
        
        public override void Unload(bool shutdown) {
            OnModActionEvent.Unregister(HandleModerationAction);
            Command.Unregister(cmds);
        }


        static void HandleModerationAction(ModAction action) {
            string acronym = NoteAcronym.GetAcronym(action);
            if (acronym == null) return;

            AddNote(action, acronym);
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

    /// <summary>
    /// Moderation note actions are logged to disk using single-letter acronyms. This class handles translating these to and from human-readable actions.
    /// </summary>
    public class NoteAcronym
    {
        private readonly static NoteAcronym Warned     = new NoteAcronym("W", "Warned");
        private readonly static NoteAcronym Kicked     = new NoteAcronym("K", "Kicked");
        private readonly static NoteAcronym Muted      = new NoteAcronym("M", "Muted");
        private readonly static NoteAcronym Banned     = new NoteAcronym("B", "Banned");
        private readonly static NoteAcronym Jailed     = new NoteAcronym("J", "Jailed"); // Jailing was removed, but still appears in notes for historical reasons
        private readonly static NoteAcronym Frozen     = new NoteAcronym("F", "Frozen");
        private readonly static NoteAcronym TempBanned = new NoteAcronym("T", "Temp-Banned");
        private readonly static NoteAcronym Noted      = new NoteAcronym("N", "Noted");
        public  readonly static NoteAcronym OpNoted    = new NoteAcronym("O", "OpNoted");

        static NoteAcronym[] All;

        internal static void Init() {
            All = new NoteAcronym[] { Warned, Kicked, Muted, Banned, Jailed, Frozen, TempBanned, Noted, OpNoted };
        }

        /// <summary>
        /// Returns the appropriate Acronym to log when a mod action occurs.
        /// </summary>
        public static string GetAcronym(ModAction action) {
            if (action.Type == ModActionType.Ban) {
                return action.Duration.Ticks != 0 ? TempBanned.Acronym : Banned.Acronym;
            }

            string modActionString = action.Type.ToString();
            foreach (var na in All) {
                if (na.Action == modActionString) { return na.Acronym; }
            }
            return null;
        }
        /// <summary>
        /// Returns the appropriate Action from a mod note acronym. If none are found, returns the original argument.
        /// </summary>
        public static string GetAction(string acronym) {
            foreach (var na in All) {
                if (na.Acronym == acronym) { return na.Action; }
            }
            return acronym;
        }

        public readonly string Acronym;
        public readonly string Action;

        private NoteAcronym(string acronym, string action) {
            Acronym = acronym;
            Action = action;
        }
    }
}
