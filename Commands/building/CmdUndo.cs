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
using System.Collections.Generic;
using MCGalaxy.Drawing;
using MCGalaxy.Drawing.Ops;
using MCGalaxy.Util;

namespace MCGalaxy.Commands
{
	public sealed class CmdUndo : Command
	{
		public override string name { get { return "undo"; } }
		public override string shortcut { get { return "u"; } }
		public override string type { get { return CommandTypes.Building; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
		public override CommandPerm[] AdditionalPerms {
			get { return new[] {
					new CommandPerm(LevelPermission.Operator, "The lowest rank to undo other players actions", 1),
					new CommandPerm(LevelPermission.AdvBuilder, "The lowest rank to be able to undo physics", 2),
				}; }
		}

		public override void Use(Player p, string message) {
			int ignored = 0;
			if (message == "") {
				if (p == null) { Player.SendMessage(null, "Console doesn't have an undo buffer."); return; }
				UndoSelf(p); return;
			} else if (p != null && int.TryParse(message, out ignored)) {
				message = p.name.ToLower() + " " + message;
			}
			
			string[] parts = message.Split(' ');
			bool undoPhysics = parts[0].CaselessEq("physics");
			Player who = undoPhysics ? null : PlayerInfo.Find(parts[0]);
			long seconds = GetSeconds(p, who, parts.Length > 1 ? parts[1] : "30");
			
			if (parts.Length > 1 && parts[1].CaselessEq("update")) {
				UndoFile.UpgradePlayerUndoFiles(parts[0]);
				Player.SendMessage(p, "Updated undo files for " + parts[0] + " to the new binary format.");
				return;
			}

			if (who != null)
				UndoOnlinePlayer(p, seconds, who);
			else if (undoPhysics)
				UndoLevelPhysics(p, seconds);
			else
				UndoOfflinePlayer(p, seconds, parts[0]);
		}

		const int undoMax = -1; // allows everything to be undone.
		internal static long GetSeconds(Player p, Player who, string param) {
			long secs;
			if (param.CaselessEq("all")) {
				secs = (p.group.maxUndo == undoMax || p == who) ? int.MaxValue : p.group.maxUndo;
			} else if (!long.TryParse(param, out secs)) {
				Player.SendMessage(p, "Invalid seconds, using 30 seconds.");
				return 30;
			}

			if (secs == 0) secs = 5400;
			if (p != null && p != who && p.group.maxUndo != undoMax && secs > p.group.maxUndo) {
				Player.SendMessage(p, p.group.name + "s may only undo up to " + p.group.maxUndo + " seconds.");
				return p.group.maxUndo;
			}
			return secs;
		}
		
		void UndoSelf(Player p) {
			UndoDrawOpEntry[] entries = p.DrawOps.Items;
			if (entries.Length == 0) {
				Player.SendMessage(p, "You have no draw operations to undo.");
				Player.SendMessage(p, "Try using %T/undo <seconds> %Sinstead.");
				return;
			}
			
			for (int i = entries.Length - 1; i >= 0; i--) {
				UndoDrawOpEntry entry = entries[i];
				if (entry.DrawOpName == "UndoSelf") continue;
				p.DrawOps.Remove(entry);
				
				UndoSelfDrawOp op = new UndoSelfDrawOp();
				op.who = p;
				op.Start = entry.Start; op.End = entry.End;
				DrawOp.DoDrawOp(op, null, p, new [] { Vec3U16.MaxVal, Vec3U16.MaxVal } );
				Player.SendMessage(p, "Undo performed.");
				return;
			}
			
			Player.SendMessage(p, "Unable to undo any draw operations, as all of the " +
			                   "past 50 draw operations are %T/undo%S or %T/undo <seconds>.");
			Player.SendMessage(p, "Try using %T/undo <seconds> %Sinstead.");
		}
		
		void UndoOnlinePlayer(Player p, long seconds, Player who) {
			if (p != null && p != who) {
				if (who.group.Permission > p.group.Permission) {
					MessageTooHighRank(p, "undo", true); return;
				}
				if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can undo other players."); return; }
			}
			
			UndoOnlineDrawOp op;
			if (p == who) op = new UndoSelfDrawOp();
			else op = new UndoOnlineDrawOp();
			op.Start = DateTime.UtcNow.AddTicks(-seconds * TimeSpan.TicksPerSecond);
			op.who = who;
			DrawOp.DoDrawOp(op, null, p, new [] { Vec3U16.MaxVal, Vec3U16.MaxVal } );
			
			Level saveLevel = op.saveLevel;
			if (p == who) {
				Player.SendMessage(p, "Undid your actions for the past &b" + seconds + " %Sseconds.");
			} else {
				Player.SendChatFrom(who, who.color + who.DisplayName + "%S's actions for the past &b" + seconds + " seconds were undone.", false);
			}
			Server.s.Log(who.name + "'s actions for the past " + seconds + " seconds were undone.");
			if (saveLevel != null) saveLevel.Save(true);
		}
		
		void UndoOfflinePlayer(Player p, long seconds, string whoName) {
			if (!CheckAdditionalPerm(p)) { MessageNeedPerms(p, "can undo other players."); return; }
			UndoOfflineDrawOp op = new UndoOfflineDrawOp();
			op.Start = DateTime.UtcNow.AddTicks(-seconds * TimeSpan.TicksPerSecond);
			op.whoName = whoName;
			DrawOp.DoDrawOp(op, null, p, new [] { Vec3U16.MaxVal, Vec3U16.MaxVal } );

			if (op.foundUser) {
				Player.GlobalMessage(Server.FindColor(whoName) + whoName + "%S's actions for the past &b" + seconds + " %Sseconds were undone.");
				Server.s.Log(whoName + "'s actions for the past " + seconds + " seconds were undone.");
				p.level.Save(true);
			} else {
				Player.SendMessage(p, "Could not find player specified.");
			}
		}
		
		void UndoLevelPhysics(Player p, long seconds) {
			if (!CheckAdditionalPerm(p, 2)) { MessageNeedPerms(p, "can undo physics.", 2); return; }
			if (p != null && !p.group.CanExecute("physics")) {
				Player.SendMessage(p, "You can only undo physics if you can use /physics."); return;
			}
			Command.all.Find("physics").Use(p, "0");
			UndoPhysicsDrawOp op = new UndoPhysicsDrawOp();
			op.seconds = seconds;
			DrawOp.DoDrawOp(op, null, p, new [] { Vec3U16.MaxVal, Vec3U16.MaxVal } );
			
			Player.GlobalMessage("Physics were undone &b" + seconds + " %Sseconds");
			Server.s.Log( "Physics were undone &b" + seconds + " %Sseconds");
			p.level.Save(true);
		}

		public override void Help(Player p) {
			Player.SendMessage(p, "/undo [player] [seconds] - Undoes the blockchanges made by [player] in the previous [seconds].");
			if (p == null || (p.group.maxUndo <= 500000 || p.group.maxUndo == 0))
				Player.SendMessage(p, "/undo [player] all - &cWill undo 68 years, 18 days, 15 hours, 28 minutes, 31 seconds for [player]");
			if (p == null || (p.group.maxUndo <= 1800 || p.group.maxUndo == 0))
				Player.SendMessage(p, "/undo [player] - &cWill undo 30 minutes");
			Player.SendMessage(p, "/undo physics [seconds] - Undoes the physics for the current map");
		}
	}
}
