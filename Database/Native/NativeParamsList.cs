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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MCGalaxy.SQL.Native {
    
    sealed class NativeParamsList : IDataParameterCollection {        

        List<NativeParameter> args = new List<NativeParameter>();
        
        public void Clear() {
            foreach (var param in args)
                param.Dispose();
            args.Clear();
        }
        
        // Boilerplate code for all the interfaces
        public object this[string parameterName] {
            get { return args[IndexOf(parameterName)]; }
            set { args[IndexOf(parameterName)] = (NativeParameter)value; }
        }
        
        public bool Contains(string parameterName) {
            return IndexOf(parameterName) >= 0;
        }
        
        public int IndexOf(string parameterName) {
            for (int i = 0; i < Count; i++) {
                if (args[i].ParameterName == parameterName) return i;
            }
            return -1;
        }

        public void RemoveAt(string parameterName) {
            int index = IndexOf(parameterName);
            if (index >= 0) args.RemoveAt(index);
        }

        
        public bool IsSynchronized { get { return false; } }
        
        public object SyncRoot { get { return this; } }
        
        public int Count { get { return args.Count; } }
        
        public void CopyTo(Array array, int index) { }
        
        
        public object this[int index] {
            get { return args[index]; }
            set { args[index] = (NativeParameter)value; }
        }

        public bool IsReadOnly { get { return false; } }
        
        public bool IsFixedSize { get { return false; } }

        public int Add(object value) { return ((IList)args).Add(value); }

        public bool Contains(object value) { return ((IList)args).Contains(value); }        

        public int IndexOf(object value) { return ((IList)args).IndexOf(value); }

        public void Insert(int index, object value) { ((IList)args).Insert(index, value); }

        public void Remove(object value) { ((IList)args).Remove(value); }

        public void RemoveAt(int index) { args.RemoveAt(index); }
        
        public IEnumerator GetEnumerator() { return args.GetEnumerator(); }
    }
}