/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
	
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.IO;
using System.Linq;

namespace MCGalaxy
{
    public sealed class LevelCollection : List<Level>, ITypedList
    {
        ILevelViewBuilder _viewBuilder;

        public LevelCollection(ILevelViewBuilder viewBuilder)
        {
            _viewBuilder = viewBuilder;
        }

        #region ITypedList Members

        protected PropertyDescriptorCollection _props;

        public PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors)
        {
            if (_props == null)
            {
                _props = _viewBuilder.GetView();
            }
            return _props;
        }

        public string GetListName(PropertyDescriptor[] listAccessors)
        {
            return ""; // was used by 1.1 datagrid
        }

        #endregion
    }

    public interface ILevelViewBuilder
    {
        PropertyDescriptorCollection GetView();
    }

    public class LevelListView : ILevelViewBuilder
    {
        public PropertyDescriptorCollection GetView()
        {
            List<PropertyDescriptor> props = new List<PropertyDescriptor>();
            LevelMethodDelegate del = l => l.name;
            props.Add(new LevelMethodDescriptor("Name", del, typeof(string)));

            del = l => l.players.Count;
            props.Add(new LevelMethodDescriptor("Players", del, typeof(int)));

            del = l => l.physics;
            props.Add(new LevelMethodDescriptor("Physics", del, typeof(int)));

            del = delegate(Level l)
            {
                //return l.permissionvisit.ToString();
                Group grp = Group.GroupList.Find(g => g.Permission == l.permissionvisit);
                return grp == null ? l.permissionvisit.ToString() : grp.name;
            };
            props.Add(new LevelMethodDescriptor("PerVisit", del, typeof(string)));

            del = delegate(Level l)
                      {
                          //return l.permissionbuild.ToString();
                Group grp = Group.GroupList.Find(g => g.Permission == l.permissionbuild);
                          return grp == null ? l.permissionbuild.ToString() : grp.name;
                      };
            props.Add(new LevelMethodDescriptor("PerBuild", del, typeof(string)));

            PropertyDescriptor[] propArray = new PropertyDescriptor[props.Count];
            props.CopyTo(propArray);
            return new PropertyDescriptorCollection(propArray);
        }
    }

    public delegate object LevelMethodDelegate(Level l);

    public class LevelMethodDescriptor : PropertyDescriptor
    {
        protected LevelMethodDelegate _method;
        protected Type _methodReturnType;

        public LevelMethodDescriptor(string name, LevelMethodDelegate method,
         Type methodReturnType)
            : base(name, null)
        {
            _method = method;
            _methodReturnType = methodReturnType;
        }

        public override object GetValue(object component)
        {
            Level l = (Level)component;
            return _method(l);
        }

        public override Type ComponentType
        {
            get { return typeof(Level); }
        }

        public override Type PropertyType
        {
            get { return _methodReturnType; }
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override void ResetValue(object component) { }

        public override bool IsReadOnly
        {
            get { return true; }
        }

        public override void SetValue(object component, object value) { }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
