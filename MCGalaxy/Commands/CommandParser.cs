/*
    Copyright 2015 MCGalaxy team

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
using MCGalaxy.Blocks;

namespace MCGalaxy {
    
    /// <summary> Provides helper methods for parsing arguments for commands. </summary>
    public static class CommandParser {
        
        /// <summary> Attempts to parse the given argument as a boolean. </summary>
        public static bool GetBool(Player p, string input, ref bool result) {
            if (input == "1" || input.CaselessEq("true") || input.CaselessEq("yes") || input.CaselessEq("on")) {
                result = true; return true;
            }
            
            if (input == "0" || input.CaselessEq("false") || input.CaselessEq("no") || input.CaselessEq("off")) {
                result = false; return true;
            }
            
            Player.Message(p, "\"{0}\" is not a valid boolean.", input);
            Player.Message(p, "Value must be either 1/yes/on or 0/no/off");
            return false;
        }
        
        /// <summary> Attempts to parse the given argument as an enumeration member. </summary>
        public static bool GetEnum<TEnum>(Player p, string input, string argName, 
                                          ref TEnum result) where TEnum : struct {
            try {
                result = (TEnum)Enum.Parse(typeof(TEnum), input, true);
                return true;
            } catch (Exception) {
                string[] names = Enum.GetNames(typeof(TEnum));
                Player.Message(p, argName + " must be one of the following: " + names.Join());
                return false;
            }
        }
        
        
        /// <summary> Attempts to parse the given argument as an integer. </summary>
        public static bool GetInt(Player p, string input, string argName, ref int result,
                                  int min = int.MinValue, int max = int.MaxValue) {
            int value;
            if (!int.TryParse(input, out value)) {
                Player.Message(p, "\"{0}\" is not a valid integer.", input); return false;
            }
            
            if (value < min || value > max) {
                // Try to provide more helpful range messages
                if (max == int.MaxValue) {
                    Player.Message(p, "{0} must be {1} or greater", argName, min);
                } else if (min == int.MinValue) {
                    Player.Message(p, "{0} must be {1} or less", argName, max);
                } else {
                    Player.Message(p, "{0} must be between {1} and {2}", argName, min, max);
                }
                return false;
            }
            
            result = value; return true;
        }
        
        
        /// <summary> Attempts to parse the given argument as an byte. </summary>
        public static bool GetByte(Player p, string input, string argName, ref byte result,
                                   byte min = byte.MinValue, byte max = byte.MaxValue) {
            int temp = 0;
            if (!GetInt(p, input, argName, ref temp, min, max)) return false;
            
            result = (byte)temp; return true;
        }
        
        /// <summary> Attempts to parse the given argument as an byte. </summary>
        public static bool GetUShort(Player p, string input, string argName, ref ushort result,
                                     ushort min = ushort.MinValue, ushort max = ushort.MaxValue) {
            int temp = 0;
            if (!GetInt(p, input, argName, ref temp, min, max)) return false;
            
            result = (ushort)temp; return true;
        }
        
        /// <summary> Attempts to parse the given argument as a hex color. </summary>
        public static bool GetHex(Player p, string input, ref CustomColor col) {
            if (input.Length > 0 && input[0] == '#')
                input = input.Substring(1);
            
            if (!Utils.IsValidHex(input)) {
                Player.Message(p, "\"#{0}\" is not a valid HEX color.", input); return false;
            }
            
            col = Colors.ParseHex(input); return true;
        }
        
        
        /// <summary> Attempts to parse the given argument as either a block name or a block ID. </summary>
        public static bool GetBlock(Player p, string input, out byte block,
                                    out byte extBlock, bool allowSkip = false) {
            block = 0; extBlock = 0;
            // Skip/None block for draw operations
            if (allowSkip && (input.CaselessEq("skip") || input.CaselessEq("none"))) {
                block = Block.Invalid; return true;
            }
            
            byte match = Block.Byte(input);
            if (match != Block.Invalid) { block = match; return true; }
            
            // find custom block
            match = BlockDefinition.GetBlock(input, p);
            if (match == Block.Invalid) {
                Player.Message(p, "&cThere is no block \"{0}\".", input);
                return false;
            }
            
            // custom block overriding a core block
            if (match < Block.CpeCount) { block = match; return true; }
            
            // Normal custom block
            block = Block.custom_block; extBlock = match;
            return true;
        }

        /// <summary> Attempts to parse the given argument as either a block name or a block ID. </summary>
        /// <remarks> Also ensures the player is allowed to place the given block. </remarks>
        public static bool GetBlockIfAllowed(Player p, string input, out byte block,
                                             out byte extBlock, bool allowSkip = false) {
            if (!GetBlock(p, input, out block, out extBlock, allowSkip)) return false;
            if (allowSkip && block == Block.Invalid) return true;
            return IsBlockAllowed(p, "draw with ", block);
        }
        
        /// <summary> Returns whether the player is allowed to place/modify/delete the given block. </summary>
        /// <remarks> Outputs information of which ranks can modify the block if not. </remarks>
        public static bool IsBlockAllowed(Player p, string action, byte block) {
            if (p == null || BlockPerms.CanModify(p, block)) return true;
            Formatter.MessageBlock(p, action, block);
            return false;
        }
    }
}
