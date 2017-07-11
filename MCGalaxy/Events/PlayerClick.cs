/*
    Copyright 2015 MCGalaxy
    
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

namespace MCGalaxy.Events {
    public enum MouseButton {
        Left = 0,
        Right = 1,
        Middle = 2
    }
    
    public enum MouseAction {
        Pressed = 0,
        Released = 1
    }

    public enum TargetBlockFace {
        AwayX = 0,
        TowardsX = 1,
        AwayY = 2,
        TowardsY = 3,
        AwayZ = 4,
        TowardsZ = 5,
        None = 6
    }
}
