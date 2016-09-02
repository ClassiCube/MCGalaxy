/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
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

namespace MCGalaxy.SQL {
    
    public abstract class IDatabaseBackend {
        
        /// <summary> Describes the arguments for a database connection 
        /// (such as database name or file location) </summary>
        public abstract string ConnectionString { get; }
        
        /// <summary> Whether this backend enforces the character length in VARCHAR columns. </summary>
        public abstract bool EnforcesTextLength { get; }
        
        /// <summary> Returns a new BulkTransaction instance, which can be used to execute
        /// many sql statements as one single transaction. </summary>
        public abstract BulkTransaction CreateBulk();
        
        /// <summary> Returns a new ParameterisedQuery instance, which executes sql statements 
        /// and manages binding of parameteries for sql queries. </summary>
        public abstract ParameterisedQuery CreateParameterised();
        
        /// <summary> Returns the shared static ParamterisedQuery instance, that is only used 
        /// for sql queries with no parameters. </summary>
        internal abstract ParameterisedQuery GetStaticParameterised();
        
        
        // == Higher level functions ==
        
        /// <summary> Returns whether a table (case sensitive) exists by that name. </summary>
        public abstract bool TableExists(string table);
        
        /// <summary> Renames the source table to the given name. </summary>
        public abstract void RenameTable(string srcTable, string dstTable);
        
        /// <summary> Removes all entries from the given table. </summary>
        public abstract void ClearTable(string table);
    }
}
