/*
    Copyright 2011 MCForge
    
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
using MCGalaxy.Games;

namespace MCGalaxy.Commands.Fun {
    public sealed class CmdGun : Command2 {
        public override string name { get { return "Gun"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public override bool SuperUseable { get { return false; } }
        
        public override void Use(Player p, string message, CommandData data) {
            if (!p.level.Config.Guns) {
                p.Message("Guns cannot be used on this map!"); return;
            }
            if (p.weapon != null && message.Length == 0) {
                p.weapon.Disable(); return;
            }
                    
            WeaponType type = Weapon.ParseType(message);
            if (type == WeaponType.Invalid) { Help(p); return; }
            GetGun(type).Enable(p);
        }
        
        static Gun GetGun(WeaponType type) {
            if (type == WeaponType.Destroy)  return new PenetrativeGun();
            if (type == WeaponType.Teleport) return new TeleportGun();
            if (type == WeaponType.Explode)  return new ExplosiveGun();
            if (type == WeaponType.Laser)    return new LaserGun();
            return new Gun();
        }
        
        public override void Help(Player p) {
            p.Message("&T/Gun [at end]");
            p.Message("&HAllows you to fire bullets at people");
            p.Message("&HAvailable [at end] types: &Sexplode, destroy, laser, tp");
        }
    }
}
