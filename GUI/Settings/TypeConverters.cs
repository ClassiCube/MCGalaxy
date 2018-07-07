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
using System.ComponentModel;

namespace MCGalaxy.Gui {
    
    internal class ColorConverter : StringConverter {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
        
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            List<string> colors = new List<string>();
            for (int i = 0; i < Colors.List.Length; i++) {
                if (Colors.List[i].Undefined) continue;
                colors.Add(Colors.List[i].Name);
            }
            return new StandardValuesCollection(colors);
        }
    }
    
    internal class RankConverter : StringConverter {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
        
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            List<string> ranks = new List<string>();
            foreach (Group g in Group.GroupList) {
                if (g.Permission <= LevelPermission.Banned || g.Permission >= LevelPermission.Nobody) continue;
                ranks.Add(g.Name);
            }
            return new StandardValuesCollection(ranks);
        }
    }
    
    internal class LevelConverter : StringConverter {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) { return true; }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) { return true; }
        
        public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            List<string> levels = new List<string>();
            Level[] loaded = LevelInfo.Loaded.Items;
            foreach (Level lvl in loaded)
                levels.Add(lvl.name);
            return new StandardValuesCollection(levels);
        }
    }
}
