/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace MCGalaxy.SQL {
    
    public static class MySQL { //: Database //Extending for future improvement (Making it object oriented later)
        
        static string connStringFormat = "Data Source={0};Port={1};User ID={2};Password={3};Pooling={4}";
        public static string connString { get { return String.Format(connStringFormat, Server.MySQLHost, Server.MySQLPort, Server.MySQLUsername, Server.MySQLPassword, Server.DatabasePooling); } }
        internal static MySQLParameterisedQuery query = new MySQLParameterisedQuery();

        public static void AddParams(string name, object param) { query.AddParam(name, param); }
        
        public static void ClearParams() { query.ClearParams(); }
    }
}

