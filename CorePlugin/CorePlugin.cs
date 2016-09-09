/*
    Copyright 2015 MCGalaxy team
        
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
using System.Collections.Generic;
using System.IO;

namespace MCGalaxy.Core {

    public sealed class CorePlugin : Plugin_Simple {
        public override string creator { get { return "MCGalaxy team"; } }
        public override string MCGalaxy_Version { get { return Server.VersionString; } }
        public override string name { get { return "CorePlugin"; } }

        public override void Load(bool startup) {
            OnPlayerConnectEvent.Register(ConnectHandler.HandleConnect,
                                          Priority.System_Level, this, false);
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerConnectEvent.UnRegister(this);
        }
    }
}
