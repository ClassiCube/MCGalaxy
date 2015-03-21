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
namespace MCGalaxy
{
    public sealed class PlayerCollection : List<Player>, ITypedList
    {
        protected IPlayerViewBuilder _viewBuilder;

        public PlayerCollection(IPlayerViewBuilder viewBuilder)
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

    public interface IPlayerViewBuilder
    {
        PropertyDescriptorCollection GetView();
    }

    public class PlayerListView : IPlayerViewBuilder
    {
        public PropertyDescriptorCollection GetView()
        {
            List<PropertyDescriptor> props = new List<PropertyDescriptor>();
            /*PlayerMethodDelegate del = delegate(Player p)
            {
                return p.name;
            };*/
            props.Add(new PlayerMethodDescriptor("Name", p => p.name, typeof(string)));

            props.Add(new PlayerMethodDescriptor("Map", p => p.level.name, typeof(string)));

            props.Add(new PlayerMethodDescriptor("Rank", p => p.group.name, typeof(string)));

            props.Add(new PlayerMethodDescriptor("Status", p =>
                      {
                          if (p.hidden)
                              return "hidden";
                          if (Server.afkset.Contains(p.name))
                              return "afk";
                          return "active";
                      }, typeof(string)));

            PropertyDescriptor[] propArray = new PropertyDescriptor[props.Count];
            props.CopyTo(propArray);
            return new PropertyDescriptorCollection(propArray);
        }
    }


    public delegate object PlayerMethodDelegate(Player player);

    public class PlayerMethodDescriptor : PropertyDescriptor
    {
        protected PlayerMethodDelegate _method;
        protected Type _methodReturnType;

        public PlayerMethodDescriptor(string name, PlayerMethodDelegate method,
         Type methodReturnType)
            : base(name, null)
        {
            _method = method;
            _methodReturnType = methodReturnType;
        }

        public override object GetValue(object component)
        {
            return _method((Player)component);
        }

        public override Type ComponentType
        {
            get { return typeof(Player); }
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
