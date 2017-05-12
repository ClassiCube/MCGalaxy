/*
    Copyright 2014 MCGalaxy
        
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

namespace MCGalaxy.Eco {
    
    /// <summary> Describes an economic transaction. </summary>
    public sealed class EcoTransaction {
        
        /// <summary> Name of player who caused the transaction. Can be null. (e.g. person who used /give /pay /take) </summary>
        public string SourceName;
        
        /// <summary> Formatted name of source player. Can be null. </summary>
        public string SourceFormatted;

        /// <summary> Name of player who was target of the transaction. (e.g. person who received money from /pay) </summary>        
        public string TargetName;

        /// <summary> Formatted name of target player. </summary>        
        public string TargetFormatted;
        
        
        
        /// <summary> Name of the item involved in the transaction. Can be null. </summary>
        public string ItemName;
        
        /// <summary> Reason provided for the transaction. Can be null. </summary>
        public string Reason;
        
        
        /// <summary> Amount involved in the transaction. Zero or positive. </summary>
        public int Amount;
        
        /// <summary> Type of this transaction. </summary>
        public EcoTransactionType Type;
    }
    
    public enum EcoTransactionType {
        Give, Payment, Take, Purchase,
    }
}