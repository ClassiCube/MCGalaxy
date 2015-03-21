using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MCGalaxy
{
	public class PlayerDB
	{
		public static bool Load( Player p ) {
			if ( File.Exists( "players/" + p.name + "DB.txt" ) ) {
				foreach ( string line in File.ReadAllLines( "players/" + p.name + "DB.txt" ) ) {
					if ( !string.IsNullOrEmpty( line ) && !line.StartsWith( "#" ) ) {
						string key = line.Split( '=' )[0].Trim();
						string value = line.Split( '=' )[1].Trim();
						string section = "nowhere yet...";

						try {
							switch ( key.ToLower() ) {
								case "nick":
								p.DisplayName = value;
								section = key;
								break;
							}
						} catch(Exception e) {
							Server.s.Log( "Loading " + p.name + "'s EXP database failed at section: " + section );
							Server.ErrorLog( e );
						}

						p.timeLogged = DateTime.Now;
					}
				}

				p.SetPrefix();
				return true;
			} else {
				Save( p );
				return false;
			}
		}

		public static void Save( Player p ) {
			StreamWriter sw = new StreamWriter( File.Create( "players/" + p.name + "DB.txt" ) );
			sw.WriteLine ("Nick = " + p.DisplayName );
			sw.Flush();
			sw.Close();
			sw.Dispose();
		}
	}
}