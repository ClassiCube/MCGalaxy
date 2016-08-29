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
using MCGalaxy.Drawing.Ops;

namespace MCGalaxy.Commands {
    
    public sealed class CmdUndoArea : Command {
        public override string name { get { return "undoarea"; } }
        public override string shortcut { get { return "ua"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        
        public override void Use(Player p, string message) {
            UndoArgs args = default(UndoArgs);
            if (Player.IsSuper(p)) { MessageInGameOnly(p); return; }
            if (message == "") { Player.Message(p, "You need to provide a player name."); return; }
            
            string[] parts = message.Split(' ');
            args.message = parts[0];
            args.delta = CmdUndo.GetDelta(p, null, parts.Length > 1 ? parts[1] : "30");
            if (args.delta == TimeSpan.MinValue) return;
            
            Player.Message(p, "Place two blocks to determine the edges.");           
            p.MakeSelection(2, args, DoUndo);
        }
        
        bool DoUndo(Player p, Vec3S32[] marks, object state, byte type, byte extType) {
            UndoArgs args = (UndoArgs)state;       
            Player who = PlayerInfo.Find(args.message);

            if (who != null) UndoOnlinePlayer(p, who, args, marks);
            else UndoOfflinePlayer(p, args.message, args, marks);
            return false;
        }
        
        void UndoOnlinePlayer(Player p, Player who, UndoArgs args, Vec3S32[] marks) {
            if (p != who && who.Rank >= p.Rank) {
                MessageTooHighRank(p, "undo", false); return;
            }
            
            UndoOnlineDrawOp op = new UndoOnlineDrawOp();
            op.Start = DateTime.UtcNow.Subtract(args.delta);
            op.who = who;
            DrawOp.DoDrawOp(op, null, p, marks);
            
            Player.SendChatFrom(who, who.ColoredName + 
                                "%S's actions for the past &b" + args.delta.Shorten() + " %Swere undone.", false);
            Server.s.Log(who.name + "'s actions for the past " + args.delta.Shorten() + " were undone.");
        }
        
        void UndoOfflinePlayer(Player p, string whoName, UndoArgs args, Vec3S32[] marks) {
            Group group = Group.findPlayerGroup(whoName);
            if (group.Permission >= p.Rank) {
                MessageTooHighRank(p, "undo", false); return;
            }
            
            UndoOfflineDrawOp op = new UndoOfflineDrawOp();
            op.Start = DateTime.UtcNow.Subtract(args.delta);
            op.whoName = whoName;
            DrawOp.DoDrawOp(op, null, p, marks);

            if (op.found) {
                Chat.MessageAll("{0}{1}%S's actions for the past &b{2} %Swere undone.", 
            	                group.color, whoName, args.delta.Shorten());
            	Server.s.Log(whoName + "'s actions for the past " + args.delta.Shorten() + " were undone.");
                p.level.Save(true);
            } else {
                Player.Message(p, "Could not find player specified.");
            }
        }
        
        struct UndoArgs { public string message; public TimeSpan delta; }

        public override void Help(Player p) {
            Player.Message(p, "%T/undoarea [player] [seconds]");
            Player.Message(p, "%HUndoes the blockchanges made by [player] in the previous [seconds] in a specific area.");
            if (p == null || (p.group.maxUndo <= 500000 || p.group.maxUndo == 0))
                Player.Message(p, "%T/undoarea [player] all %H- Undoes 68 years for [player]");
        }
    }
}
