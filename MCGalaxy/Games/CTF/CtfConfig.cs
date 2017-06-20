/*
    Copyright 2011 MCForge
    
    Written by fenderrock87
    
    Dual-licensed under the    Educational Community License, Version 2.0 and
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
using System.Collections.Generic;
using System.IO;
using MCGalaxy.SQL;

namespace MCGalaxy.Games {
    public sealed partial class CTFGame {
 
        void LineProcessor(string key, string value) {
            switch (key.ToLower()) {
                case "base.red.x":
                    redbase.x = ushort.Parse(value); break;
                case "base.red.y":
                    redbase.y = ushort.Parse(value); break;
                case "game.maxpoints":
                    maxpoints = int.Parse(value); break;
                case "game.tag.points-gain":
                    tagpoint = int.Parse(value); break;
                case "game.tag.points-lose":
                    taglose = int.Parse(value); break;
                case "game.capture.points-gain":
                    cappoint = int.Parse(value); break;
                case "game.capture.points-lose":
                    caplose = int.Parse(value); break;
                case "auto.setup":
                    needSetup = bool.Parse(value); break;
                case "base.red.z":
                    redbase.z = ushort.Parse(value); break;
                case "base.red.block":
                    redbase.block = ExtBlock.FromRaw(byte.Parse(value)); break;
                case "base.blue.block":
                    bluebase.block = ExtBlock.FromRaw(byte.Parse(value)); break;
                case "base.blue.spawnx":
                    bluebase.spawnx = ushort.Parse(value); break;
                case "base.blue.spawny":
                    bluebase.spawny = ushort.Parse(value); break;
                case "base.blue.spawnz":
                    bluebase.spawnz = ushort.Parse(value); break;
                case "base.red.spawnx":
                    redbase.spawnx = ushort.Parse(value); break;
                case "base.red.spawny":
                    redbase.spawny = ushort.Parse(value); break;
                case "base.red.spawnz":
                    redbase.spawnz = ushort.Parse(value); break;
                case "base.blue.x":
                    bluebase.x = ushort.Parse(value); break;
                case "base.blue.y":
                    bluebase.y = ushort.Parse(value); break;
                case "base.blue.z":
                    bluebase.z = ushort.Parse(value); break;
                case "map.line.z":
                    zline = ushort.Parse(value); break;
            }
        }
        
        bool LoadConfig() {
            //Load some configs
            if (!Directory.Exists("CTF")) Directory.CreateDirectory("CTF");
            if (!File.Exists("CTF/maps.config")) return false;
            
            string[] lines = File.ReadAllLines("CTF/maps.config");
            maps = new List<string>(lines);
            return maps.Count > 0;
        }
    }
}
