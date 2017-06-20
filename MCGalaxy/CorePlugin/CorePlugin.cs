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
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Eco;
using MCGalaxy.Events;
using MCGalaxy.Tasks;

namespace MCGalaxy.Core {

    public sealed class CorePlugin : Plugin_Simple {
        public override string creator { get { return Server.SoftwareName + " team"; } }
        public override string MCGalaxy_Version { get { return Server.VersionString; } }
        public override string name { get { return "CorePlugin"; } }
        SchedulerTask clearTask;

        public override void Load(bool startup) {
            OnPlayerConnectEvent.Register(ConnectHandler.HandleConnect,
                                          Priority.Critical, this);
            OnPlayerCommandEvent.Register(ChatHandler.HandleCommand,
                                          Priority.Critical, this);
            OnPlayerConnectingEvent.Register(ConnectingHandler.HandleConnecting,
                                          Priority.Critical, this);
            
            OnJoinedLevelEvent.Register(MiscHandlers.HandleOnJoinedLevel,
                                        Priority.Critical, this);
            OnPlayerMoveEvent.Register(MiscHandlers.HandlePlayerMove,
                                       Priority.Critical, this);
            OnPlayerClickEvent.Register(MiscHandlers.HandlePlayerClick,
                                        Priority.Critical, this);
            OnEcoTransactionEvent.Register(EcoHandlers.HandleEcoTransaction,
                                           Priority.Critical, this);
            OnModActionEvent.Register(ModActionHandler.HandleModAction, 
                                      Priority.Critical, this);
            
            clearTask = Server.Background.QueueRepeat(IPThrottler.CleanupTask, null, 
                                                      TimeSpan.FromMinutes(10));
        }
        
        public override void Unload(bool shutdown) {
            OnPlayerConnectEvent.UnRegister(this);
            OnPlayerCommandEvent.UnRegister(this);
            OnPlayerConnectingEvent.UnRegister(this);
            
            OnJoinedLevelEvent.UnRegister(this);
            OnPlayerMoveEvent.UnRegister(this);
            OnPlayerClickEvent.UnRegister(this);
            OnEcoTransactionEvent.UnRegister(this);
            OnModActionEvent.UnRegister(this);
            
            Server.Background.Cancel(clearTask);
        }
    }
}
