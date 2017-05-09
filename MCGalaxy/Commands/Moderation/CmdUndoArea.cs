﻿/*
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

namespace MCGalaxy.Commands.Moderation {  
    public sealed class CmdUndoArea : CmdUndoPlayer {
        public override string name { get { return "undoarea"; } }
        public override string shortcut { get { return "ua"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public override CommandPerm[] ExtraPerms { get { return null; } }
        public override CommandAlias[] Aliases { get { return null; } }
        
        public override void Use(Player p, string message)
        {
            UndoAreaArgs args = default(UndoAreaArgs);
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") { Player.Message(p, "You need to provide a player name."); return; }
            
            string[] parts = message.SplitSpaces(), names = null;
            int[] ids = GetIds(p, parts, out names);
            if (ids == null) return;
            
            args.names = names; args.ids = ids;
            args.delta = CmdUndo.GetDelta(p, parts[0], parts, 1);
            if (args.delta == TimeSpan.MinValue) return;
            
            Player.Message(p, "Place or break two blocks to determine the edges.");
            p.MakeSelection(2, args, DoUndo);
        }
        
        bool DoUndo(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            UndoAreaArgs args = (UndoAreaArgs)state;       
            UndoPlayer(p, args.delta, args.names, args.ids, marks);
            return false;
        }

        struct UndoAreaArgs { public string[] names; public int[] ids; public TimeSpan delta; }

        public override void Help(Player p) {
            Player.Message(p, "%T/undoarea [player1] <player2..> <timespan>");
            Player.Message(p, "%HUndoes the blockchanges made by [players] in the past <timespan> in a specific area");
            Player.Message(p, "%H  If <timespan> is not given, undoes 30 minutes.");
            if (p == null || p.group.maxUndo == -1 || p.group.maxUndo == int.MaxValue)
                Player.Message(p, "%H  if <timespan> is all, &cundoes for 68 years");
            Player.Message(p, "%H  e.g. to undo 90 minutes, <timespan> would be %S1h30m");
        }
    }
}
