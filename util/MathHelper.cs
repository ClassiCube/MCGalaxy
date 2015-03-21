/*
	Copyright 2011 MCGalaxy
	
	Author: fenderrock87
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
namespace MCGalaxy
{
    public static class MathHelper
    {
        public static decimal Clamp(decimal value, decimal low, decimal high)
        {
            return Math.Max(Math.Min(value, high), low);
        }

        public static double Clamp(double value, double low, double high)
        {
            return Math.Max(Math.Min(value, high), low);
        }
    }
}
