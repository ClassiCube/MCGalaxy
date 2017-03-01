/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.ComponentModel;

namespace MCGalaxy {
    public sealed class PlayerCollection : List<Player>, ITypedList {
        PropertyDescriptorCollection _props;

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
            if (_props == null)
                _props = new PropertyDescriptorCollection(MakeProperties());
            return _props;
        }

        public string GetListName(PropertyDescriptor[] listAccessors) {
            return ""; // was used by 1.1 datagrid
        }
        
        PropertyDescriptor[] MakeProperties() {
            return new PropertyDescriptor[] {
                new MethodDescriptor<Player>("Name", p => p.name, typeof(string)),
                new MethodDescriptor<Player>("Map", p => p.level.name, typeof(string)),
                new MethodDescriptor<Player>("Rank", p => p.group.name, typeof(string)),
            };
        }
    }
    
    public sealed class LevelCollection : List<Level>, ITypedList {
        PropertyDescriptorCollection _props;

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
            if (_props == null)
                _props = new PropertyDescriptorCollection(MakeProperties());
            return _props;
        }

        public string GetListName(PropertyDescriptor[] listAccessors) {
            return ""; // was used by 1.1 datagrid
        }
        
        PropertyDescriptor[] MakeProperties() {
            return new PropertyDescriptor[] {
                new MethodDescriptor<Level>("Name", l => l.name, typeof(string)),
                new MethodDescriptor<Level>("Players", l => l.players.Count, typeof(int)),
                new MethodDescriptor<Level>("Physics", l => l.physics, typeof(int)),
            };
        }
    }
    
    public class MethodDescriptor<T> : PropertyDescriptor {
        Func<T, object> _func;
        Type _returnType;

        public MethodDescriptor(string name, Func<T, object> func, Type returnType) : base(name, null) {
            _func = func;
            _returnType = returnType;
        }

        public override object GetValue(object component) { return _func((T)component); }

        public override Type ComponentType { get { return typeof(T); } }

        public override Type PropertyType { get { return _returnType; } }

        public override bool CanResetValue(object component) { return false; }

        public override void ResetValue(object component) { }

        public override bool IsReadOnly { get { return true; } }

        public override void SetValue(object component, object value) { }

        public override bool ShouldSerializeValue(object component) { return false; }
    }
}
