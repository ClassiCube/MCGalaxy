/*
	Copyright 2011 MCGalaxy
	
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
namespace MCGalaxy.Commands
{
    public sealed class CmdCalculate : Command
    {
        public override string name { get { return "calculate"; } }
        public override string shortcut { get { return "calc"; } }
        public override string type { get { return "build"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Banned; } }
        public override void Use(Player p, string message)
        {
            if(message == "")
            {
                Help(p);
                return;
            }

            var split = message.Split(' ');
			if(split.Length < 2)
			{
				Help(p);
				return;
			}
			
			if(!ValidChar(split[0]))
			{
				Player.SendMessage(p, "Invalid number given");
				return;
			}

            double result = 0;
			float num1 = float.Parse(split[0]);
			String operation = split[1];
			
			// All 2-parameter operations go here
            
			if(split.Length == 2)
			{
				switch(operation)
				{
					case "square":
						result = num1 * num1;
						Player.SendMessage(p, "The answer: %aThe square of " + split[0] + Server.DefaultColor + " = %c" + result);
						return;
					case "root":
						result = Math.Sqrt(num1);
						Player.SendMessage(p, "The answer: %aThe root of " + split[0] + Server.DefaultColor + " = %c" + result);
						return;
					case "cube":
						result = num1 * num1 * num1;
						Player.SendMessage(p, "The answer: %aThe cube of " + split[0] + Server.DefaultColor + " = %c" + result);
						return;
					case "pi":
						result = num1 * Math.PI;
						Player.SendMessage(p, "The answer: %a" + split[0] + " x PI" + Server.DefaultColor + " = %c" + result);
						return;
					default:
						Player.SendMessage(p, "There is no such method");
						return;
				}
			}
			
			// Now we try 3-parameter methods
			
			if(split.Length == 3)
			{
				if(!ValidChar(split[2]))
				{
					Player.SendMessage(p, "Invalid number given");
					return;
				}
				
				float num2 = float.Parse(split[2]);
				
				switch(operation)
				{
					case "x":
					case "*":
						result = num1 * num2;
						Player.SendMessage(p, "The answer: %a" + split[0] + " x " + split[2] + Server.DefaultColor + " = %c" + result);
						return;
					case "+":
						result = num1 + num2;
						Player.SendMessage(p, "The answer: %a" + split[0] + " + " + split[2] + Server.DefaultColor + " = %c" + result);
						return;
					case "-":
						result = num1 - num2;
						Player.SendMessage(p, "The answer: %a" + split[0] + " - " + split[2] + Server.DefaultColor + " = %c" + result);
						return;
					case "/":
						if(num2 == 0)
						{
							Player.SendMessage(p, "Cannot divide by 0");
							return;
						}
						
						result = num1 / num2;
						Player.SendMessage(p, "The answer: %a" + split[0] + " / " + split[2] + Server.DefaultColor + " = %c" + result);
						return;
					default:
						Player.SendMessage(p, "There is no such method");
						return;
				}
			}

            // If we get here, the player did something wrong

			Help(p);

        }
        public override void Help(Player p)
        {
            //Help message
            Player.SendMessage(p, "/calculate <num1> <method> <num2> - Calculates <num1> <method> <num2>");
            Player.SendMessage(p, "methods with 3 fillins: /, x, -, +");
            Player.SendMessage(p, "/calculate <num1> <method> - Calculates <num1> <method>");
            Player.SendMessage(p, "methods with 2 fillins: square, root, pi, cube");
        }
        public static bool ValidChar(string chr)
        {
            string allowedchars = "01234567890.,";
            foreach (char ch in chr) { if (allowedchars.IndexOf(ch) == -1) { return false; } } return true;
        }
    }
}