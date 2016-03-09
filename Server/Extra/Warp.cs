/*
	Copyright 2011 MCForge
		
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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy
{
    public static class Warp
    {
        public static List<Wrp> Warps = new List<Wrp>();
        private static List<Wrp> TempDeletedWarpsList = new List<Wrp>();
        private static List<Wrp> FailedLoadingWarpsList = new List<Wrp>();

        public class Wrp
        {
            public string name;
            public string lvlname;
            public ushort x;
            public ushort y;
            public ushort z;
            public byte rotx;
            public byte roty;
        }

        public static Wrp GetWarp(string name)
        {
            foreach (Wrp w in Warps)
            {
                if (w.name.ToLower().Trim() == name.ToLower().Trim())
                {
                    return w;
                }
            }
            return null;
        }

        public static void AddWarp(string name, Player p)
        {
            Wrp w = new Wrp();
            try
            {
                w.name = name;
                w.lvlname = p.level.name;
                w.x = p.pos[0];
                w.y = p.pos[1];
                w.z = p.pos[2];
                w.rotx = p.rot[0];
                w.roty = p.rot[1];
                Warps.Add(w);
                SAVE();
            }
            catch { }
        }

        public static void DeleteWarp(string name)
        {
            Wrp wa = new Wrp();
            foreach (Wrp w in Warps)
            {
                if (w.name.ToLower().Trim() == name.ToLower().Trim())
                {
                    wa = w;
                    break;
                }
                
            }
            TempDeletedWarpsList.Add(wa);
            Warps.Remove(wa); 
            SAVE();
        }

        public static void MoveWarp(string Wrp, Player p)
        {
            Wrp w = new Wrp();
            w = GetWarp(Wrp);
            Wrp wa = new Wrp();
            try
            {
                Warps.Remove(w);
                wa.name = w.name;
                wa.lvlname = p.level.name;
                wa.x = p.pos[0];
                wa.y = p.pos[1];
                wa.z = p.pos[2];
                wa.rotx = p.rot[0];
                wa.roty = p.rot[1];
                Warps.Add(wa);
                SAVE();
            }
            catch { }
        }

        public static bool WarpExists(string name)
        {
            foreach (Wrp w in Warps)
            {
                if (w.name.ToLower().Trim() == name.ToLower().Trim())
                {
                    return true;
                }
            }
            return false;
        }

        public static void SAVE()
        {
            using (StreamWriter SW = new StreamWriter("extra/warps.save"))
            {
                foreach (Wrp warp in Warps)
                {
                    SW.WriteLine(warp.name + ":" + warp.lvlname + ":" + warp.x.ToString() + ":" + warp.y.ToString() + ":" + warp.z.ToString() + ":" + warp.rotx.ToString() + ":" + warp.roty.ToString());
                }

                try
                {
                    if (TempDeletedWarpsList.Count >= 1)
                    {
                        SW.WriteLine("");
                        SW.WriteLine("#Deleted Warps:");
                        foreach (Wrp BAKwarp in TempDeletedWarpsList)
                        {
                            SW.WriteLine("#" + BAKwarp.name + ":" + BAKwarp.lvlname + ":" + BAKwarp.x.ToString() + ":" + BAKwarp.y.ToString() + ":" + BAKwarp.z.ToString() + ":" + BAKwarp.rotx.ToString() + ":" + BAKwarp.roty.ToString());
                        }
                    }
                    TempDeletedWarpsList.Clear();
                }
                catch { Server.s.Log("Saving backups of deleted warps failed!"); }

                try
                {
                    if (FailedLoadingWarpsList.Count >= 1)
                    {
                        SW.WriteLine("#");
                        SW.WriteLine("#FAILED LOADING:");
                        foreach (Wrp FAILwarp in FailedLoadingWarpsList)
                        {
                            SW.WriteLine("#" + FAILwarp.name + ":" + FAILwarp.lvlname + ":" + FAILwarp.x.ToString() + ":" + FAILwarp.y.ToString() + ":" + FAILwarp.z.ToString() + ":" + FAILwarp.rotx.ToString() + ":" + FAILwarp.roty.ToString());
                        }
                    }
                    FailedLoadingWarpsList.Clear();
                }
                catch { Server.s.Log("Saving failed loading warps failed!"); }
                SW.Dispose();
            }
        }

        public static void LOAD()
        {
            if (File.Exists("extra/warps.save"))
            {
                using (StreamReader SR = new StreamReader("extra/warps.save"))
                {
                    bool failed = false;
                    bool anyfailed = false;
                    string line;
                    while (SR.EndOfStream == false)
                    {
                        line = SR.ReadLine().ToLower().Trim();
                        if (!line.StartsWith("#") && line.Contains(":"))
                        {
                            string[] LINE = line.ToLower().Split(':');
                            Wrp warp = new Wrp();
                            failed = false;
                            try
                            {
                                warp.name = LINE[0];
                                warp.lvlname = LINE[1];
                                warp.x = ushort.Parse(LINE[2]);
                                warp.y = ushort.Parse(LINE[3]);
                                warp.z = ushort.Parse(LINE[4]);
                                warp.rotx = byte.Parse(LINE[5]);
                                warp.roty = byte.Parse(LINE[6]);
                            }
                            catch
                            {
                                Server.s.Log("Couldn't load a Warp! Look in the 'extra/warps.save' file to see the unloaded warp");
                                FailedLoadingWarpsList.Add(warp);
                                failed = true;
                                anyfailed = true;
                            }
                            if (failed == false)
                            {
                                Warps.Add(warp);
                            }
                        }
                    }
                    if (anyfailed)
                    {
                        SAVE();
                    }
                    SR.Dispose();
                }
            }
        }
    }
}
