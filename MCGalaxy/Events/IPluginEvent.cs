/*
    Copyright 2011 MCForge
        
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

namespace MCGalaxy.Events {
    /// <summary> Represents an abstract event. </summary>
    /// <remarks> *** You MUST use a DIFFERENT delegate type for each subclass *** <br/><br/>
    /// This is because the static event lists are unique to each new generic type instantiation, not each new subclass. </remarks>
    public class IEvent<IMethod> {
        protected internal static VolatileArray<IEvent<IMethod>> handlers = new VolatileArray<IEvent<IMethod>>();
        protected internal IMethod method;
        protected Priority priority;
        
        /// <summary> Registers the given handler to this event. </summary>
        /// <param name="priority"> The priority (imporantance) of the given handler. </param>
        /// <param name="bypass"> Whether the given handler is able to be registered multiple times. </param>
        public static void Register(IMethod method, Priority priority,  bool bypass = false) {
            IEvent<IMethod> handler = Find(method);
            if (handler != null && !bypass) {
                string msg = MethodFormat("Method {0} already registered as a {1} event handler", method);
                throw new ArgumentException(msg);
            }
            
            handler = new IEvent<IMethod>();
            handler.method = method; handler.priority = priority;
            AddHandler(handler);
        }
        
        /// <summary> Unregisters the given handler from this event. </summary>
        public static void Unregister(IMethod method) {
            IEvent<IMethod> handler = Find(method);
            handlers.Remove(handler);
        }
        
        public static IEvent<IMethod> Find(IMethod method) {
            Delegate methodDel = (Delegate)((object)method);
            IEvent<IMethod>[] items = handlers.Items;
            
            foreach (var p in items) {
                Delegate pMethodDel = (Delegate)((object)p.method);
                if (pMethodDel == methodDel) return p;
            }
            return null;
        }
        
        
        static void AddHandler(IEvent<IMethod> handler) {
            // We want both the add and sorting is in one step
            lock (handlers.locker) {
                IEvent<IMethod>[] old = handlers.Items;
                IEvent<IMethod>[] items = new IEvent<IMethod>[old.Length + 1];
                for (int i = 0; i < old.Length; i++) {
                    items[i] = old[i];
                }
                
                items[old.Length] = handler;
                Array.Sort(items, (a, b) => b.priority.CompareTo(a.priority));
                handlers.Items = items;
            }
        }
        
        protected static void CallCommon(Action<IMethod> action) {
            IEvent<IMethod>[] items = handlers.Items;
            for (int i = 0; i < items.Length; i++) {
                IEvent<IMethod> handler = items[i];
                
                try { action(handler.method); } 
                catch (Exception ex) { LogHandlerException(ex, handler); }
            }
        }
        
        protected static void LogHandlerException(Exception ex, IEvent<IMethod> handler) {
            string msg = MethodFormat("Method {0} errored when calling {1} event", handler.method);
            Logger.LogError(msg, ex);
        }
        
        static string MethodFormat(string format, IMethod method) {
            Delegate del = (Delegate)((object)method);
            string fullName = del.Method.ReflectedType.FullName + "." + del.Method.Name;
            return string.Format(format, fullName, typeof(IMethod).Name);
        }
    }
}
