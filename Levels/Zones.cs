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
using MCGalaxy.SQL;

namespace MCGalaxy {

	public static class Zones {
		
		public static void Delete(string level, Level.Zone zn) {
			ParameterisedQuery query = ParameterisedQuery.Create();
			query.AddParam("@Owner", zn.Owner);
			Database.executeQuery(query, "DELETE FROM `Zone" + level + "` WHERE Owner=@Owner" +
			                      " AND SmallX='" + zn.smallX + "' AND SMALLY='" +
			                      zn.smallY + "' AND SMALLZ='" + zn.smallZ + "' AND BIGX='" +
			                      zn.bigX + "' AND BIGY='" + zn.bigY + "' AND BIGZ='" + zn.bigZ + "'");
		}
		
		public static void Create(string level, Level.Zone zn) {
			ParameterisedQuery query = ParameterisedQuery.Create();
			query.AddParam("@Owner", zn.Owner);
			Database.executeQuery(query, "INSERT INTO `Zone" + level + 
			                      "` (SmallX, SmallY, SmallZ, BigX, BigY, BigZ, Owner) VALUES ("
			                      + zn.smallX + ", " + zn.smallY + ", " + zn.smallZ + ", " 
			                      + zn.bigX + ", " + zn.bigY + ", " + zn.bigZ + ", @Owner)");
		}
	}
}