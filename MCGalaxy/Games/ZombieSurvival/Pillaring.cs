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
using MCGalaxy.Blocks;
using MCGalaxy.Events;

namespace MCGalaxy.Games.ZS {
    
    internal static class Pillaring {
        
        internal static bool Handles(Player p, ushort x, ushort y, ushort z,
                                     bool placing, ExtBlock block, ExtBlock old, ZSGame game) {
            
            if (placing && !game.Map.Config.Pillaring && !p.Game.Referee) {
                if (NotPillaring(game.Map, block, old)) {
                    p.Game.BlocksStacked = 0;
                } else if (CheckCoords(p, x, y, z)) {
                    p.Game.BlocksStacked++;
                } else {
                    p.Game.BlocksStacked = 0;
                }
                
                if (MessagePillaring(p, x, y, z)) return true;
            }
            p.Game.LastX = x; p.Game.LastY = y; p.Game.LastZ = z;
            return false;
        }

        static bool NotPillaring(Level lvl, ExtBlock b, ExtBlock old) {
            byte collide = lvl.CollideType(b);
            if (collide == CollideType.WalkThrough) return true;
            
            collide = lvl.CollideType(old);
            return collide == CollideType.SwimThrough || collide == CollideType.LiquidWater
                || collide == CollideType.LiquidLava;
        }
        
        static bool CheckCoords(Player p, ushort x, ushort y, ushort z) {
            if (p.Game.LastY != y - 1 || p.Game.LastX != x || p.Game.LastZ != z) return false;
            int minX = (p.Pos.X - 8) / 32, minZ = (p.Pos.Z - 8) / 32;
            int maxX = (p.Pos.X + 8) / 32, maxZ = (p.Pos.Z + 8) / 32;
            
            // Check the four possible coords/blocks the player could be pillaring up on
            return (minX == x && minZ == z) || (minX == x && maxZ == z)
                || (maxX == x && minZ == z) || (maxX == x && maxZ == z);
        }
        
        static bool MessagePillaring(Player p, ushort x, ushort y, ushort z) {
            if (p.Game.BlocksStacked == 2) {
                TimeSpan delta = DateTime.UtcNow - p.Game.LastPillarWarn;
                if (delta.TotalSeconds >= 5) {
                    Chat.MessageOps("  &cWarning: " + p.ColoredName + " %Sis pillaring!");
                    p.Game.LastPillarWarn = DateTime.UtcNow;
                }
                
                string action = p.Game.PillarFined ? "kicked" : "fined 10 " + ServerConfig.Currency;
                Player.Message(p, "You are pillaring! &cStop before you are " + action + "!");
            } else if (p.Game.BlocksStacked == 4) {
                if (!p.Game.PillarFined) {
                    Chat.MessageOps("  &cWarning: " + p.ColoredName + " %Sis pillaring!");
                    Command.all.FindByName("Take").Use(null, p.name + " 10 Auto fine for pillaring");
                    Player.Message(p, "  &cThe next time you pillar, you will be &4kicked&c.");
                } else {                   
                    ModAction action = new ModAction(p.name, null, ModActionType.Kicked, "Auto kick for pillaring");
                    OnModActionEvent.Call(action);
                    p.Kick("No pillaring allowed!");
                }
                
                p.RevertBlock(x, y, z);
                p.Game.PillarFined = true;
                p.Game.BlocksStacked = 0;
                return true;
            }
            return false;
        }
    }
}
