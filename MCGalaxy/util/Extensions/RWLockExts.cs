/*
    Copyright 2015 MCGalaxy
    
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
using System.Threading;

namespace MCGalaxy {    
    public static class RWLockExts {

        /// <summary> Accquires a read lock which can be implictly disposed in a using statement. </summary>
        public static IDisposable AccquireReadLock(this ReaderWriterLockSlim locker) {
            return new ReadLock(locker);
        }

        /// <summary> Accquires a write lock which can be implictly disposed in a using statement. </summary>        
        public static IDisposable AccquireWriteLock(this ReaderWriterLockSlim locker) {
            return new WriteLock(locker);
        }
               
        struct ReadLock : IDisposable {
            ReaderWriterLockSlim locker;
            
            public ReadLock(ReaderWriterLockSlim locker) {
                locker.EnterReadLock();
                this.locker = locker;
            }
            
            public void Dispose() {
                locker.ExitReadLock();
                locker = null;
            }
        }
        
        struct WriteLock : IDisposable {
            ReaderWriterLockSlim locker;
            
            public WriteLock(ReaderWriterLockSlim locker) {
                locker.EnterWriteLock();
                this.locker = locker;
            }
            
            public void Dispose() {
                locker.ExitWriteLock();
                locker = null;
            }
        }
    }
}
