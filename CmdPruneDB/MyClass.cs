using System;
using System.Collections.Generic;
using System.IO;
using MCGalaxy.DB;
using MCGalaxy.Maths;
using MCGalaxy.Util;

namespace MCGalaxy.Commands.Moderation {
	public class CmdPruneDB : Command2 {
		public override string name { get { return "PruneDB"; } }
		public override string type { get { return CommandTypes.Moderation; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
		public override bool SuperUseable { get { return false; } }

		public unsafe override void Use(Player p, string message, CommandData data) {
			if (message.Length == 0) { Player.Message(p, "You need to provide a player name."); return; }
			
			string[] parts = message.SplitSpaces(), names = null;
			int[] ids = GetIds(p, data, parts, out names);
			if (ids == null) return;
			
			TimeSpan delta = GetDelta(p, parts[0], parts, 1);
			if (delta == TimeSpan.MinValue) return;

			BlockDB db = p.level.BlockDB;
			DateTime startTime = DateTime.UtcNow - delta;
			
			Vec3U16 dims;
			FastList<BlockDBEntry> entries = new FastList<BlockDBEntry>(4096);
			byte[] bulk = new byte[BlockDBFile.BulkEntries * BlockDBFile.EntrySize];
			int start = (int)((startTime - BlockDB.Epoch).TotalSeconds);
			long total;
			int changed = 0;
			
			using (IDisposable locker = db.Locker.AccquireWrite()) {
				if (!File.Exists(db.FilePath)) {
					Player.Message(p, "BlockDB file for this map doesn't exist.");
					return;
				}

				using (Stream src = OpenRead(db.FilePath), dst = OpenWrite(db.FilePath + ".tmp")) {
					BlockDBFile format = BlockDBFile.ReadHeader(src, out dims);
					BlockDBFile.WriteHeader(dst, dims);
					total = format.CountEntries(src);

					src.Position = src.Length;
					fixed (byte* ptr = bulk) {
						while (true) {
							BlockDBEntry* entry = (BlockDBEntry*)ptr;
							int count = format.ReadBackward(src, bulk, entry);
							if (count == 0) break;
							entry += (count - 1);
							
							for (int i = count - 1; i >= 0; i--, entry--) {
								if (entry->TimeDelta < start) goto finished;
								for (int j = 0; j < ids.Length; j++) {
									
									if (entry->PlayerID != ids[j]) continue;
									changed++;
									entries.Add(*entry);
									
									if (entries.Count == 4096) {
										format.WriteEntries(dst, entries);
										entries.Count = 0;
									}
								}
							}
						}
					}

				finished:
					// flush remaining few entries
					if (entries.Count > 0) format.WriteEntries(dst, entries);
				}
				
				string namesStr = names.Join(name => PlayerInfo.GetColoredName(p, name));
				if (changed > 0) {
					File.Delete(db.FilePath);
					File.Move(db.FilePath + ".tmp", db.FilePath);
					p.Message("Pruned {2} changes by {1}%S's in the past &b{0} %S({3} entries left)",
					          delta.Shorten(true), namesStr, changed, total - changed);
				} else {
					File.Delete(db.FilePath + ".tmp");
					p.Message("No changes found by {1} %Sin the past &b{0}",
					          delta.Shorten(true), namesStr);
				}
			}
		}
		
		// all this copy paste makes me sad
		
		static FileStream OpenWrite(string path) {
			return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
		}
		
		static FileStream OpenRead(string path) {
			return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
		}
		
		static int[] GetIds(Player p, CommandData data, string[] parts, out string[] names) {
			int count = Math.Max(1, parts.Length - 1);
			List<int> ids = new List<int>();
			names = new string[count];
			
			for (int i = 0; i < names.Length; i++) {
				p.Message("Searching PlayerDB for \"{0}\"..", parts[i]);
				names[i] = PlayerDB.MatchNames(p, parts[i]);
				if (names[i] == null) return null;
				
				if (!p.name.CaselessEq(names[i])) {
					Group grp = Group.GroupIn(names[i]);
					if (!CheckRank(p, data, grp.Permission, "undo", false)) return null;
				}
				ids.AddRange(NameConverter.FindIds(names[i]));
			}
			return ids.ToArray();
		}
		
		static TimeSpan GetDelta(Player p, string name, string[] parts, int offset) {
			TimeSpan delta = TimeSpan.Zero;
			string timespan = parts.Length > offset ? parts[parts.Length - 1] : "30m";
			bool self = p.name.CaselessEq(name);
			
			if (timespan.CaselessEq("all")) {
				return self ? TimeSpan.FromSeconds(int.MaxValue) : p.group.MaxUndo;
			} else if (!CommandParser.GetTimespan(p, timespan, ref delta, "undo the past", "s")) {
				return TimeSpan.MinValue;
			}

			if (delta.TotalSeconds == 0)
				delta = TimeSpan.FromMinutes(90);
			if (!self && delta > p.group.MaxUndo) {
				p.Message("{0}%Ss may only undo up to {1}",
				          p.group.ColoredName, p.group.MaxUndo.Shorten(true, true));
				return p.group.MaxUndo;
			}
			return delta;
		}


		public override void Help(Player p) {
			p.Message("%T/PruneDB [player1] <player2..> <timespan>");
			p.Message("%HDeletes the block changes of [players] in the past <timespan> from BlockDB.");
			p.Message("&cSlow and dangerous. Use with care.");
		}
	}
}