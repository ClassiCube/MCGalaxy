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
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.PropertyGridInternal;

namespace MCGalaxy.Gui {
    /// <summary> Hacky workaround for PropertyGrid to fix crashing with some versions of wine-mono </summary>
    public sealed class HackyPropertyGrid : PropertyGrid {
        
        sealed class HackyPropertiesTab : PropertiesTab {
            
            // With some versions of wine-mono, if you try to change PropertyGrid's selected object,
            //  some Exceptions gets thrown. (see examples below)
            // The root cause of the Exception is a PropertiesTab instance returning 'null' from its
            //  Bitmap property implementation - so workaround this by never returning null.
            public override Bitmap Bitmap {
                get { return base.Bitmap ?? new Bitmap(16, 16); }
            }
        }        
        protected override PropertyTab CreatePropertyTab(Type tabType) { return new HackyPropertiesTab(); }
        
        /*
Type: IndexOutOfRangeException
Source: System.Windows.Forms
Message: Index was outside the bounds of the array.
Target: UpdatePropertiesViewTabVisibility
Trace:   at System.Windows.Forms.PropertyGrid.UpdatePropertiesViewTabVisibility ()
  at System.Windows.Forms.PropertyGrid.ShowEventsButton (System.Boolean value)
  at System.Windows.Forms.PropertyGrid.set_SelectedObjects (System.Object[] value)
  at System.Windows.Forms.PropertyGrid.set_SelectedObject (System.Object value)
  at (wrapper remoting-invoke-with-check) System.Windows.Forms.PropertyGrid.set_SelectedObject(object)
  at MCGalaxy.Gui.Window.pl_listBox_Click (System.Object sender, System.EventArgs e)
  at System.Windows.Forms.Control.OnClick (System.EventArgs e)
  at System.Windows.Forms.ListBox.WndProc (System.Windows.Forms.Messagem)
  at System.Windows.Forms.Control+ControlNativeWindow.OnMessage (System.Windows.Forms.Messagem)
  at System.Windows.Forms.Control+ControlNativeWindow.WndProc (System.Windows.Forms.Messagem)
  at System.Windows.Forms.NativeWindow.Callback (System.IntPtr hWnd, System.Int32 msg, System.IntPtr wparam, System.IntPtr lparam)
         */
        /*
Type: IndexOutOfRangeException
Source: System.Windows.Forms
Message: Index was outside the bounds of the array.
Target: RefreshProperties
Trace:   at System.Windows.Forms.PropertyGrid.RefreshProperties (System.Boolean clearCached)
  at System.Windows.Forms.PropertyGrid.Refresh (System.Boolean clearCached)
  at System.Windows.Forms.PropertyGrid.Refresh ()
  at System.Windows.Forms.PropertyGrid.OnFontChanged (System.EventArgs e)
  at System.Windows.Forms.Control.AssignParent (System.Windows.Forms.Control value)
  at System.Windows.Forms.Control+ControlCollection.Add (System.Windows.Forms.Control value)
  at System.Windows.Forms.TabPage+TabPageControlCollection.Add (System.Windows.Forms.Control value)
  at MCGalaxy.Gui.PropertyWindow.InitializeComponent ()
  at MCGalaxy.Gui.PropertyWindow..ctor ()
  at (wrapper remoting-invoke-with-check) MCGalaxy.Gui.PropertyWindow..ctor()
  at MCGalaxy.Gui.Window.btnProperties_Click (System.Object sender, System.EventArgs e)
  at System.Windows.Forms.Control.OnClick (System.EventArgs e)
  at System.Windows.Forms.Button.OnClick (System.EventArgs e)
  at System.Windows.Forms.Button.OnMouseUp (System.Windows.Forms.MouseEventArgs mevent)
  at System.Windows.Forms.Control.WmMouseUp (System.Windows.Forms.Messagem, System.Windows.Forms.MouseButtons button, System.Int32 clicks)
  at System.Windows.Forms.Control.WndProc (System.Windows.Forms.Messagem)
  at System.Windows.Forms.ButtonBase.WndProc (System.Windows.Forms.Messagem)
  at System.Windows.Forms.Button.WndProc (System.Windows.Forms.Messagem)
  at System.Windows.Forms.Control+ControlNativeWindow.OnMessage (System.Windows.Forms.Messagem)
  at System.Windows.Forms.Control+ControlNativeWindow.WndProc (System.Windows.Forms.Messagem)
  at System.Windows.Forms.NativeWindow.Callback (System.IntPtr hWnd, System.Int32 msg, System.IntPtr wparam, System.IntPtr lparam)
         */
    }
}