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
        protected internal static List<IEvent<IMethod>> handlers = new List<IEvent<IMethod>>();
        protected IMethod method;
        protected Priority priority;
        
        /// <summary> Registers the given handler to this event. </summary>
        /// <param name="priority"> The priority (imporantance) of the given handler. </param>
        /// <param name="bypass"> Whether the given handler is able to be registered multiple times. </param>
        public static void Register(IMethod method, Priority priority,  bool bypass = false) {
            if (Find(method) != null && !bypass)
                throw new ArgumentException("Method already registered as a handler!");
            
            IEvent<IMethod> handler = new IEvent<IMethod>();
            handler.method = method; handler.priority = priority;
            handlers.Add(handler);
            SortHandlers();
        }
        
        /// <summary> Unregisters the given handler from this event. </summary>
        public static void Unregister(IMethod method) {
            if (Find(method) == null)
                throw new ArgumentException("Method was not registered as a handler!");
           
            handlers.Remove(Find(method));
        }
        
        public static IEvent<IMethod> Find(IMethod method) {
            Delegate methodDel = (Delegate)((object)method);
            foreach (var p in handlers) {
                Delegate pMethodDel = (Delegate)((object)p.method);
                if (pMethodDel == methodDel) return p;
            }
            return null;
        }
        
        
        protected static void SortHandlers() {
            handlers.Sort((a, b) => b.priority.CompareTo(a.priority));
        }
        
        protected static void CallImpl(Action<IMethod> action) {
            try {
                foreach (var pl in handlers) {
                    try {
                        action(pl.method);
                    } catch (Exception ex) {
                        Logger.Log(LogType.Warning, "Plugin {0} errored when calling {1} event", 
                                       GetFullMethodName(pl.method), typeof(IMethod).Name);
                        Logger.LogError(ex);
                    }
                }
            } catch (Exception ex) {
                Logger.Log(LogType.Warning, "Error when calling {0} event", typeof(IMethod).Name);
                Logger.LogError(ex);
            }
        }
        
        static string GetFullMethodName(object method) {
            Delegate del = (Delegate)((object)method);
            return del.Method.ReflectedType.FullName + "." + del.Method.Name;
        }
    }
}
