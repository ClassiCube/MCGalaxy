/*
    Copyright 2010 MCLawl Team -
    Created by Snowl (David D.) and Cazzar (Cayde D.)

    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.osedu.org/licenses/ECL-2.0
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;

namespace MCGalaxy.Games.ZS {
    
    internal static class Pillaring {
        
        internal static bool Handles(Player p, ushort x, ushort y, ushort z,
                                     byte action, byte block, byte old, ZombieGame game) {
            
            if (action == 1 && !game.CurLevel.Pillaring && !p.Game.Referee) {
                if (NotPillaring(block, old)) {
                    p.Game.BlocksStacked = 0;
                } else if (p.Game.LastY == y - 1 && p.Game.LastX == x && p.Game.LastZ == z ) {
                    p.Game.BlocksStacked++;
                } else {
                    p.Game.BlocksStacked = 0;
                }
                
                if (MessagePillaring(p, x, y, z)) return true;
            }
            p.Game.LastX = x; p.Game.LastY = y; p.Game.LastZ = z;
            return false;
        }
        
        static bool MessagePillaring(Player p, ushort x, ushort y, ushort z) {
            if (p.Game.BlocksStacked == 2) {
                TimeSpan delta = DateTime.UtcNow - p.Game.LastPillarWarn;
                if (delta.TotalSeconds >= 5) {
                    Chat.MessageOps("  &cWarning: " + p.ColoredName + " %Sis pillaring!");
                    p.Game.LastPillarWarn = DateTime.UtcNow;
                }
                
                string action = p.Game.PillarFined ? "kicked" : "fined 10 " + Server.moneys;
                p.SendMessage("You are pillaring! &cStop before you are " + action + "!");
            } else if (p.Game.BlocksStacked == 4) {
                if (!p.Game.PillarFined) {
                    Chat.MessageOps("  &cWarning: " + p.ColoredName + " %Sis pillaring!");
                    Command.all.Find("take").Use(null, p.name + " 10 Auto fine for pillaring");
                    p.SendMessage("  &cThe next time you pillar, you will be &4kicked&c.");
                } else {
                    p.Kick("No pillaring allowed!");
                    Player.AddNote(p.name, null, "K", "Auto kick for pillaring");
                }
                
				p.RevertBlock(x, y, z);
                p.Game.PillarFined = true;
                p.Game.BlocksStacked = 0;
                return true;
            }
            return false;
        }
        
        static bool NotPillaring(byte block, byte old) {
            if (block == Block.shrub) return true;
            if (block >= Block.yellowflower && block <= Block.redmushroom) return true;
            
            old = Block.Convert(old);
            return old >= Block.water && block <= Block.lavastill;
        }
    }
}
