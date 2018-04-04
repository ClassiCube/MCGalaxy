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

namespace MCGalaxy.Commands.CPE {
	public sealed class CmdTexture : Command {
		public override string name { get { return "Texture"; } }
		public override string type { get { return CommandTypes.Other; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }

		public override void Use(Player p, string message) {
			if (message.Length == 0) { Help(p); return; }
			string[] args = message.SplitSpaces();
			string scope = args[0].ToLower();
			if (scope == "local")    scope = "level";
			if (scope == "localzip") scope = "levelzip";
			
			if (args.Length == 1) {
				if (scope == "level")
					Player.Message(p, "Level terrain: "   + GetPath(p.level.Config.Terrain));
				else if (scope == "levelzip")
					Player.Message(p, "Level tex pack: "  + GetPath(p.level.Config.TexturePack));
				else if (scope == "global")
					Player.Message(p, "Global terrain: "  + GetPath(ServerConfig.DefaultTerrain));
				else if (scope == "globalzip")
					Player.Message(p, "Global tex pack: " + GetPath(ServerConfig.DefaultTexture));
				else
					Help(p);
				return;
			}
			
			string url = args[1];
			if (url.CaselessEq("normal") || url.CaselessEq("reset")) {
				url = "";
			} else if (!(url.StartsWith("http://") || url.StartsWith("https://"))) {
				Player.Message(p, "Please use http:// or https:// in front of your URL"); return;
			}
			FilterURL(ref url);
			
			if (url.Length > NetUtils.StringSize) {
				Player.Message(p, "The URL must be " + NetUtils.StringSize + " characters or less."); return;
			}
			if (url.Length > 0 && !(url.EndsWith(".png") || url.EndsWith(".zip"))) {
				Player.Message(p, "URL must end in .png (for terrain) or .zip (for texture pack)"); return;
			}

			if (scope == "global" || scope == "globalzip") {
				if (url.Length == 0) {
					ServerConfig.DefaultTerrain = "";
					ServerConfig.DefaultTexture = "";
					Player.Message(p, "Reset server textures to default");
				} else if (url.CaselessEnds(".png")) {
					ServerConfig.DefaultTerrain = url;
					Player.Message(p, "Set server's default terrain to " + url);
				} else if (url.CaselessEnds(".zip")) {
					ServerConfig.DefaultTexture = url;
					Player.Message(p, "Set server's default texture pack to " + url);
				}
				UpdateGlobal(p);
			} else if (scope == "level" || scope == "levelzip") {
				if (!LevelInfo.ValidateAction(p, p.level.name, "set texture of this level")) return;
				
				if (url.Length == 0) {
					p.level.Config.Terrain = "";
					p.level.Config.TexturePack = "";
					Player.Message(p, "Reset level textures to server default");
				} else if (url.CaselessEnds(".png")) {
					p.level.Config.Terrain = url;
					Player.Message(p, "Set level's default terrain to " + url);
				} else if (url.CaselessEnds(".zip")) {
					p.level.Config.TexturePack = url;
					Player.Message(p, "Set level's default texture pack to " + url);
				}
				UpdateLevel(p);
			} else {
				Help(p);
			}
		}
		
		internal static void FilterURL(ref string url) {
			// a lot of people try linking to the dropbox page instead of directly to file, so we auto correct them
			if (url.StartsWith("http://www.dropbox")) {
				url = "http://dl.dropbox" + url.Substring("http://www.dropbox".Length);
				url = url.Replace("?dl=0", "");
			} else if (url.StartsWith("https://www.dropbox")) {
				url = "https://dl.dropbox" + url.Substring("https://www.dropbox".Length);
				url = url.Replace("?dl=0", "");
			}
		}
		
		static string GetPath(string url) { return url.Length == 0 ? "(none)" : url; }
		
		void UpdateGlobal(Player p) {
			Player[] players = PlayerInfo.Online.Items;
			foreach (Player pl in players) {
				pl.SendCurrentTextures();
			}
			SrvProperties.Save();
		}
		
		void UpdateLevel(Player p) {
			Player[] players = PlayerInfo.Online.Items;
			foreach (Player pl in players) {
				if (pl.level != p.level) continue;
				pl.SendCurrentTextures();
			}
			Level.SaveSettings(p.level);
		}
		
		public override void Help(Player p) {
			Player.Message(p, "%T/Texture global/level [url]");
			Player.Message(p, "%HChanges server default or current level's texture.");
			Player.Message(p, "%H[url] must end with .png (terrain) or .zip (texture pack)");
			Player.Message(p, "%HUsing 'reset' for [url] will reset the texture to default");
			Player.Message(p, "%H  Note: zip texture packs are not supported by all clients");
		}
	}
}
