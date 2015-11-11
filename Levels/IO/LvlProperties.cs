/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.IO;

namespace MCGalaxy.Levels.IO {

	public static class LvlProperties {
		
		public static void Save(Level level, string path) {
			try {
				using (StreamWriter writer = File.CreateText(path + ".properties"))
					WriteLevelProperties(level, writer);
			} catch (Exception ex) {
				Server.s.Log("Failed to save level properties!");
				Logger.WriteError(ex);
				return;
			}
			
			try {
				using( StreamWriter writer = new StreamWriter(File.Create(path + ".env")))
					WriteEnvProperties(level, writer);
			} catch (Exception ex) {
				Server.s.Log("Failed to save environment properties");
				Logger.WriteError(ex);
			}
		}
		
		static void WriteLevelProperties(Level level, StreamWriter writer) {
			writer.WriteLine("#Level properties for " + level.name);
			writer.WriteLine("#Drown-time in seconds is [drown time] * 200 / 3 / 1000");
			writer.WriteLine("Theme = " + level.theme);
			writer.WriteLine("Physics = " + level.physics.ToString());
			writer.WriteLine("Physics speed = " + level.speedPhysics.ToString());
			writer.WriteLine("Physics overload = " + level.overload.ToString());
			writer.WriteLine("Finite mode = " + level.finite.ToString());
			writer.WriteLine("Animal AI = " + level.ai.ToString());
			writer.WriteLine("Edge water = " + level.edgeWater.ToString());
			writer.WriteLine("Survival death = " + level.Death.ToString());
			writer.WriteLine("Fall = " + level.fall.ToString());
			writer.WriteLine("Drown = " + level.drown.ToString());
			writer.WriteLine("MOTD = " + level.motd);
			writer.WriteLine("JailX = " + level.jailx.ToString());
			writer.WriteLine("JailY = " + level.jaily.ToString());
			writer.WriteLine("JailZ = " + level.jailz.ToString());
			writer.WriteLine("Unload = " + level.unload.ToString());
			writer.WriteLine("WorldChat = " + level.worldChat.ToString());
			
			writer.WriteLine("PerBuild = " + GetName(level.permissionbuild));
			writer.WriteLine("PerVisit = " + GetName(level.permissionvisit));
			writer.WriteLine("PerBuildMax = " + GetName(level.perbuildmax));
			writer.WriteLine("PerVisitMax = " + GetName(level.pervisitmax));
			
			writer.WriteLine("Guns = " + level.guns.ToString());
			writer.WriteLine("LoadOnGoto = " + level.loadOnGoto.ToString());
			writer.WriteLine("LeafDecay = " + level.leafDecay.ToString());
			writer.WriteLine("RandomFlow = " + level.randomFlow.ToString());
			writer.WriteLine("GrowTrees = " + level.growTrees.ToString());
			writer.WriteLine("Weather = " + level.weather.ToString());
			writer.WriteLine("Texture = " + level.textureUrl);
		}
		
		static string GetName(LevelPermission perm) {
			string permName = Level.PermissionToName(perm).ToLower();
			return Group.Exists(permName) ? permName :
				Level.PermissionToName(LevelPermission.Nobody);
		}
		
		static void WriteEnvProperties(Level level, StreamWriter writer) {
			if(level.CloudColor != null)
				writer.WriteLine("CloudColor = " + level.CloudColor.ToString());
			if (level.SkyColor != null)
				writer.WriteLine("SkyColor = " + level.SkyColor.ToString());
			if (level.LightColor != null)
				writer.WriteLine("LightColor = " + level.LightColor.ToString());
			if (level.ShadowColor != null)
				writer.WriteLine("ShadowColor = " + level.ShadowColor.ToString());
			if (level.FogColor != null)
				writer.WriteLine("FogColor = " + level.FogColor.ToString());
			
			writer.WriteLine("EdgeLevel = " + level.EdgeLevel.ToString());
			writer.WriteLine("EdgeBlock = " + level.EdgeBlock.ToString());
			writer.WriteLine("HorizonBlock = " + level.HorizonBlock.ToString());
		}
		
		
		public static void Load(Level level, string path) {
			string propsFile = path + ".properties";
			if (!File.Exists(propsFile))
				propsFile = path;
			
			foreach (string line in File.ReadAllLines(propsFile)) {
				try {
					if (line[0] == '#') continue;
					int sepIndex = line.IndexOf(" = ");
					if (sepIndex < 0) continue;
					
					string key = line.Substring(0, sepIndex).ToLower();
					string value = line.Substring(sepIndex + 3);
					ParseProperty(level, key, value);
				} catch (Exception e) {
					Server.ErrorLog(e);
				}
			}
			
			if (!File.Exists(path + ".env")) return;
			foreach (string line in File.ReadAllLines(path + ".env")) {
				try {
					if (line[0] == '#') continue;
					int sepIndex = line.IndexOf(" = ");
					if (sepIndex < 0) continue;
					string value = line.Substring(sepIndex + 3);

					switch (line.Substring(0, sepIndex).ToLower())
					{
							case "cloudcolor": level.CloudColor = value; break;
							case "fogcolor": level.FogColor = value; break;
							case "skycolor": level.SkyColor = value; break;
							case "shadowcolor": level.ShadowColor = value; break;
							case "lightcolor": level.LightColor = value; break;
							case "edgeblock": level.EdgeBlock = byte.Parse(value); break;
							case "edgelevel": level.EdgeLevel = byte.Parse(value); break;
							case "horizonblock": level.HorizonBlock = byte.Parse(value); break;
					}
				} catch {
				}
			}
		}
		
		static void ParseProperty(Level level, string key, string value) {
			switch (value)
			{
				case "theme":
					level.theme = value;
					break;
				case "physics":
					level.setPhysics(int.Parse(value));
					break;
				case "physics speed":
					level.speedPhysics = int.Parse(value);
					break;
				case "physics overload":
					level.overload = int.Parse(value);
					break;
				case "finite mode":
					level.finite = bool.Parse(value);
					break;
				case "animal ai":
					level.ai = bool.Parse(value);
					break;
				case "edge water":
					level.edgeWater = bool.Parse(value);
					break;
				case "survival death":
					level.Death = bool.Parse(value);
					break;
				case "fall":
					level.fall = int.Parse(value);
					break;
				case "drown":
					level.drown = int.Parse(value);
					break;
				case "motd":
					level.motd = value;
					break;
				case "jailx":
					level.jailx = ushort.Parse(value);
					break;
				case "jaily":
					level.jaily = ushort.Parse(value);
					break;
				case "jailz":
					level.jailz = ushort.Parse(value);
					break;
				case "unload":
					level.unload = bool.Parse(value);
					break;
				case "worldchat":
					level.worldChat = bool.Parse(value);
					break;
				case "perbuild":
					level.permissionbuild = GetPerm(value);
					break;
				case "pervisit":
					level.permissionvisit = GetPerm(value);
					break;
				case "perbuildmax":
					level.perbuildmax = GetPerm(value);
					break;
				case "pervisitmax":
					level.pervisitmax = GetPerm(value);
					break;
				case "guns":
					level.guns = bool.Parse(value);
					break;
				case "loadongoto":
					level.loadOnGoto = bool.Parse(value);
					break;
				case "leafdecay":
					level.leafDecay = bool.Parse(value);
					break;
				case "randomflow":
					level.randomFlow = bool.Parse(value);
					break;
				case "growtrees":
					level.growTrees = bool.Parse(value);
					break;
				case "weather":
					level.weather = byte.Parse(value);
					break;
				case "texture":
					level.textureUrl = value;
					break;
			}
		}
		
		static LevelPermission GetPerm(string value) {
			LevelPermission perm = Level.PermissionFromName(value);
			return perm != LevelPermission.Null ? perm : LevelPermission.Guest;
		}
	}
}