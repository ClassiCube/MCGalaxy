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
using MCGalaxy.Maths;
using BlockID = System.UInt16;

namespace MCGalaxy.Commands {
    
    /// <summary> Provides helper methods for parsing arguments for commands. </summary>
    public static class CommandParser {
        
        /// <summary> Attempts to parse the given argument as a boolean. </summary>
        public static bool GetBool(Player p, string input, ref bool result) {
            if (input.CaselessEq("1") || input.CaselessEq("true")
                || input.CaselessEq("yes") || input.CaselessEq("on")) {
                result = true; return true;
            }
            
            if (input.CaselessEq("0") || input.CaselessEq("false")
                || input.CaselessEq("no") || input.CaselessEq("off")) {
                result = false; return true;
            }
            
            p.Message("%W\"{0}\" is not a valid boolean.", input);
            p.Message("%WValue must be either 1/yes/on or 0/no/off");
            return false;
        }
        
        /// <summary> Attempts to parse the given argument as an enumeration member. </summary>
        public static bool GetEnum<TEnum>(Player p, string input, string argName,
                                          ref TEnum result) where TEnum : struct {
            try {
                result = (TEnum)Enum.Parse(typeof(TEnum), input, true);
                if (Enum.IsDefined(typeof(TEnum), result)) return true;
            } catch {
            }
            
            string[] names = Enum.GetNames(typeof(TEnum));
            p.Message(argName + " must be one of the following: &f" + names.Join());
            return false;
        }
        
        /// <summary> Attempts to parse the given argument as an timespan in short form. </summary>
        public static bool GetTimespan(Player p, string input, ref TimeSpan span,
                                       string action, string defUnit) {
            try {
                span = input.ParseShort(defUnit);
                // Typically span is added to current time, so ensure span isn't too big
                DateTime.UtcNow.Add(span).AddYears(1);
                return true;
            } catch (OverflowException) {
                p.Message("%WTimespan given is too big");
            } catch (ArgumentOutOfRangeException) {
                p.Message("%WTimespan given is too big");
            } catch (FormatException ex) {
                p.Message("%W{0} is not a valid quantifier.", ex.Message);
                p.Message(TimespanHelp, action);
            }
            return false;
        }
        public const string TimespanHelp = "For example, to {0} 25 and a half hours, use \"1d1h30m\".";
        
        
        /// <summary> Attempts to parse the given argument as an integer. </summary>
        public static bool GetInt(Player p, string input, string argName, ref int result,
                                  int min = int.MinValue, int max = int.MaxValue) {
            int value;
            if (!int.TryParse(input, out value)) {
                p.Message("%W\"{0}\" is not a valid integer.", input); return false;
            }
            
            if (value < min || value > max) {
                // Try to provide more helpful range messages
                if (max == int.MaxValue) {
                    p.Message("%W{0} must be {1} or greater", argName, min);
                } else if (min == int.MinValue) {
                    p.Message("%W{0} must be {1} or less", argName, max);
                } else {
                    p.Message("%W{0} must be between {1} and {2}", argName, min, max);
                }
                return false;
            }
            
            result = value; return true;
        }
        
        /// <summary> Attempts to parse the given argument as a real number. </summary>
        public static bool GetReal(Player p, string input, string argName, ref float result,
                                   float min = float.NegativeInfinity, float max = float.MaxValue) {
            float value;
            if (!Utils.TryParseSingle(input, out value)) {
                p.Message("%W\"{0}\" is not a valid number.", input); return false;
            }
            
            if (value < min || value > max) {
                p.Message("%W{0} must be between {1} and {2}", argName, 
                               min.ToString("F4"), max.ToString("F4"));
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
        public static bool GetHex(Player p, string input, ref ColorDesc col) {
            ColorDesc tmp;
            if (!Colors.TryParseHex(input, out tmp)) {
                p.Message("%W\"#{0}\" is not a valid HEX color.", input); return false;
            }
            col = tmp; return true;
        }
        
        internal static bool GetCoords(Player p, string[] args, int argsOffset, ref Vec3S32 P) {
            return
                GetCoord(p, args[argsOffset + 0], P.X, "X coordinate", out P.X) &&
                GetCoord(p, args[argsOffset + 1], P.Y, "Y coordinate", out P.Y) &&
                GetCoord(p, args[argsOffset + 2], P.Z, "Z coordinate", out P.Z);
        }
        
        static bool GetCoord(Player p, string arg, int cur, string axis, out int value) {
            bool relative = arg[0] == '~';
            if (relative) arg = arg.Substring(1);
            value = 0;
            // ~ should work as ~0
            if (relative && arg.Length == 0) { value += cur; return true; }
            
            if (!GetInt(p, arg, axis, ref value)) return false;
            if (relative) value += cur;
            return true;
        }
        
        
        /// <summary> Attempts to parse the given argument as either a block name or a block ID. </summary>
        public static bool GetBlock(Player p, string input, out BlockID block, bool allowSkip = false) {
            block = Block.Air;
            // Skip/None block for draw operations
            if (allowSkip && (input.CaselessEq("skip") || input.CaselessEq("none"))) {
                block = Block.Invalid; return true;
            }
            
            block = Block.Parse(p, input);
            if (block == Block.Invalid) p.Message("%WThere is no block \"{0}\".", input);
            return block != Block.Invalid;
        }

        /// <summary> Attempts to parse the given argument as either a block name or a block ID. </summary>
        /// <remarks> Also ensures the player is allowed to place the given block. </remarks>
        public static bool GetBlockIfAllowed(Player p, string input, out BlockID block, bool allowSkip = false) {
            if (!GetBlock(p, input, out block, allowSkip)) return false;
            if (allowSkip && block == Block.Invalid) return true;
            return IsBlockAllowed(p, "draw with", block);
        }
        
        /// <summary> Returns whether the player is allowed to place/modify/delete the given block. </summary>
        /// <remarks> Outputs information of which ranks can modify the block if not. </remarks>
        public static bool IsBlockAllowed(Player p, string action, BlockID block) {
            if (p.group.Blocks[block]) return true;
            BlockPerms.Find(block).MessageCannotUse(p, action);
            return false;
        }
    }
}
