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
using System.Data;
using MCGalaxy.SQL;

namespace MCGalaxy.DB {
    
    /// <summary> Optimised in-memory BlockDB cache. </summary>
    public sealed class BlockDBCache {
        
        public BlockDBCacheNode Head, Tail;
        
        int nextSize = 10000;
    }
    
    // TODO: track start time so we can use int16 instead of int32 time delta
    public sealed class BlockDBCacheNode {
        
        public BlockDBCacheNode Prev, Next;
        
        public int Count;
        
        public BlockDBEntry[] Entries;
    }
}
