/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    https://opensource.org/license/ecl-2-0/
    https://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Globalization;

namespace MCGalaxy 
{
    public static class NumberUtils 
    {
        const NumberStyles DECIMAL_STYLE = NumberStyles.Integer | NumberStyles.AllowDecimalPoint;
        const NumberStyles INTEGER_STYLE = NumberStyles.Integer;
        
        
        // Not all languages use . as their decimal point separator
        public static bool TryParseSingle(string s, out float result) {
            if (s != null && s.IndexOf(',') >= 0) s = s.Replace(',', '.');
            result = 0; float temp;

            if (!Single.TryParse(s, DECIMAL_STYLE, NumberFormatInfo.InvariantInfo, out temp)) return false;
            if (Single.IsInfinity(temp) || Single.IsNaN(temp)) return false;
            result = temp;
            return true;
        }
        
        public static bool TryParseDouble(string s, out double result) {
            if (s != null && s.IndexOf(',') >= 0) s = s.Replace(',', '.');
            result = 0; double temp;
            
            if (!Double.TryParse(s, DECIMAL_STYLE, NumberFormatInfo.InvariantInfo, out temp)) return false;
            if (Double.IsInfinity(temp) || Double.IsNaN(temp)) return false;
            result = temp;
            return true;
        }
        
        // in JSON we must use . instead of ,
        public static string StringifyDouble(double value) {
            return value.ToString(CultureInfo.InvariantCulture);
        }
        
        
        // Some languages don't have - as the negative sign symbol
        public static bool TryParseInt32(string s, out int result) {
            return int.TryParse(s, INTEGER_STYLE, NumberFormatInfo.InvariantInfo, out result);
        }
        
        public static int ParseInt32(string s) {
            return int.Parse(s, INTEGER_STYLE, NumberFormatInfo.InvariantInfo);
        }
    }
}
