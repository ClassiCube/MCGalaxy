/*
    Copyright 2011 MCForge
        
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Data;
using System.Data.SQLite;

namespace MCGalaxy.SQL {
    
    public static class SQLite {
        
        static string connStringFormat = "Data Source =" + Server.apppath + "/MCGalaxy.db; Version =3; Pooling ={0}; Max Pool Size =1000;";
        public static string connString { get { return String.Format(connStringFormat, Server.DatabasePooling); } }       
        internal static SQLiteParameterisedQuery query = new SQLiteParameterisedQuery();

        public static void AddParams(string name, object param) { query.AddParam(name, param); }
        
        public static void ClearParams() { query.ClearParams(); }
    }
}