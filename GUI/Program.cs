/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
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
using System.IO;
using System.Net;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Reflection;

namespace Starter
{
    class Program
    {
        static int tries = 0;
        static bool needsToRestart = false;
        static string parent = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
        //Console.ReadLine() is ignored while Starter is set as Windows Application in properties. (At least on Windows)

        [STAThread]
        static void Main(string[] args)
        {
            Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (File.Exists("MCGalaxy_.dll"))
            {

                //Crash issue fixed by re-executing the exe to properly load MCGalaxy_.dll.
                if (!needsToRestart)
                    openServer(args);
                else
                    Process.Start(parent);
            }
            else
            {
                needsToRestart = true;
                tries++;
                Console.WriteLine("This is try number " + tries);
                Console.WriteLine("You do not have the required DLL!");
                Console.WriteLine("Go to https://github.com/Hetal728/MCGalaxy yourself and download it, please");
                Console.WriteLine("Place it inside my folder, near me, and restart me.");
                Console.WriteLine("If you have any issues, get the files from the https://github.com/Hetal728/MCGalaxy download page and try again.");
                Console.WriteLine("Press any key to close me...");
                Console.ReadLine();
                Console.WriteLine("Bye!");
            }
        }
        static void openServer(string[] args)
        {
            MCGalaxy.Gui.Program.Main(args);
        }
    }
}