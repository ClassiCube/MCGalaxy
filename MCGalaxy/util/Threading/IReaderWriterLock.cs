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
using System.Threading;

#if !NET_20
namespace MCGalaxy.Util {

    public sealed class IReaderWriterLock {
        
        ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        public IDisposable AccquireRead() { return AccquireRead(-1); }
        public IDisposable AccquireWrite() { return AccquireWrite(-1); }
        
        public IDisposable AccquireRead(int msTimeout) {
            if (!locker.TryEnterReadLock(msTimeout)) return null;
            return new SlimLock(locker, false);
        }

        public IDisposable AccquireWrite(int msTimeout) {
            if (!locker.TryEnterWriteLock(msTimeout)) return null;
            return new SlimLock(locker, true);
        }
        
        
        class SlimLock : IDisposable {
            ReaderWriterLockSlim locker;
            bool writeMode;
            
            public SlimLock(ReaderWriterLockSlim locker, bool writeMode) {
                this.locker = locker;
                this.writeMode = writeMode;
            }
            
            public void Dispose() {
                if (writeMode) {
                    locker.ExitWriteLock();
                } else {
                    locker.ExitReadLock();
                }
                locker = null;
            }
        }
    }
}
#endif