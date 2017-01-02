/*
    Copyright 2011 MCForge
        
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
using MCGalaxy.Commands.Building;

namespace MCGalaxy.Commands {
    
    public sealed class CmdUndoArea : CmdUndo {
        public override string name { get { return "undoarea"; } }
        public override string shortcut { get { return "ua"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms { get { return null; } }
        public override CommandAlias[] Aliases { get { return null; } }
        
        public override void Use(Player p, string message) {
            UndoAreaArgs args = default(UndoAreaArgs);
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") { Player.Message(p, "You need to provide a player name."); return; }
            
            string[] parts = message.Split(' ');
            parts[0] = PlayerInfo.FindOfflineNameMatches(p, parts[0]);
            if (parts[0] == null) return;
            
            args.message = parts[0];
            args.delta = GetDelta(p, parts[0], parts.Length > 1 ? parts[1] : "30m");
            if (args.delta == TimeSpan.MinValue) return;
            
            Player.Message(p, "Place two blocks to determine the edges.");
            p.MakeSelection(2, args, DoUndo);
        }
        
        bool DoUndo(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            UndoAreaArgs args = (UndoAreaArgs)state;       
            UndoPlayer(p, args.delta, args.message, marks);
            return false;
        }
        
        protected override bool CheckUndoPerms(Player p, Group grp) {
             if (grp.Permission >= p.Rank) { MessageTooHighRank(p, "undo", false); return false; }
             return true;
        }

        struct UndoAreaArgs { public string message; public TimeSpan delta; }

        public override void Help(Player p) {
            Player.Message(p, "%T/undoarea [player] <timespan>");
            Player.Message(p, "%HUndoes the blockchanges made by [player] in the previous <timespan> in a specific area.");
            Player.Message(p, "%H If <timespan> is not given, undoes 30 minutes.");
            Player.Message(p, "%H e.g. to undo the past 90 minutes, <timespan> would be %S1h30m");
            if (p == null || p.group.maxUndo == -1 || p.group.maxUndo == int.MaxValue)
                Player.Message(p, "%T/undoarea [player] all &c- Undoes 68 years for [player]");
        }
    }
}
