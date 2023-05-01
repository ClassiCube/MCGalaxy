/*
Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCForge)
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
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using MCGalaxy.Scripting;
using MCGalaxy.Modules.Compiling;

namespace MCGalaxy.Gui.Popups {
    public partial class CustomCommands : Form {
        
        public CustomCommands() {
            InitializeComponent();
            LoadCompilers();

            //Sigh. I wish there were SOME event to help me.
            foreach (Command cmd in Command.allCmds) {
                if (!Command.IsCore(cmd)) lstCommands.Items.Add(cmd.name);
            }
        }
        
        void CustomCommands_Load(object sender, EventArgs e) {
            GuiUtils.SetIcon(this);
        }
        
        void LoadCompilers() {
            Button[] buttons = { btnCreate1, btnCreate2, btnCreate3, btnCreate4, btnCreate5 };
            List<ICompiler> compilers = ICompiler.Compilers;
            int i;
            
            for (i = 0; i < Math.Min(compilers.Count, buttons.Length); i++)
            {
                // must be copied to local variable because of the way C# for loop closures work,
                //  as otherwise the delegate { ... compilers[i] ... } uses compiler 
                //   from LAST iteration instead of the current iteration
                ICompiler compiler = compilers[i];
                buttons[i].Visible = true;
                buttons[i].Text    = "Create " + compiler.ShortName;
                buttons[i].Click  += delegate { CreateCommand(compiler); };
            }
            
            for (; i < buttons.Length; i++) buttons[i].Visible = false;
        }
        
        void CreateCommand(ICompiler compiler) {
            string cmdName = txtCmdName.Text.Trim();
            if (cmdName.Length == 0) {
                Popup.Warning("Command must have a name"); return;
            }
            
            string path = compiler.CommandPath(cmdName);
            if (File.Exists(path)) {
                Popup.Warning("Command already exists"); return;
            }
            
            try {
                string source = compiler.GenExampleCommand(cmdName);
                File.WriteAllText(path, source);
            } catch (Exception ex) {
                Logger.LogError(ex);
                Popup.Error("Failed to generate command. Check error logs for more details.");
                return;
            }
            Popup.Message("Command Cmd" + cmdName + compiler.FileExtension + " created.");
        }
        
        void btnLoad_Click(object sender, EventArgs e) {
            string path;
            
            using (FileDialog dialog = new OpenFileDialog()) {
                dialog.RestoreDirectory = true;
                dialog.Filter = GetFilterText();
                
                if (dialog.ShowDialog() != DialogResult.OK) return;
                path = dialog.FileName;
            }
            if (!File.Exists(path)) return;
            
            if (path.CaselessEnds(".dll")) {
                LoadCommands(path); return;
            }
                          
            // compile to temp .dll and load that
            string tmp = CompileCommands(path);
            if (tmp == null) return; 
            LoadCommands(tmp);
            DeleteAssembly(tmp);
        }

        void btnUnload_Click(object sender, EventArgs e) {
            string cmdName = lstCommands.SelectedItem.ToString();
            Command cmd = Command.Find(cmdName);
            if (cmd == null) {
                Popup.Warning("Command " + cmdName + " is not loaded."); return;
            }

            lstCommands.Items.Remove(cmd.name);
            Command.Unregister(cmd);
            Popup.Message("Command successfully unloaded.");
        }
        
        void lstCommands_SelectedIndexChanged(object sender, EventArgs e) {
            btnUnload.Enabled = lstCommands.SelectedIndex != -1;
        }
        
        
        void LoadCommands(string path) {
            Assembly lib = IScripting.LoadAssembly(path);
            if (lib == null) return;
            List<Command> commands = IScripting.LoadTypes<Command>(lib);
            
            for (int i = 0; i < commands.Count; i++) 
            {
                Command cmd = commands[i];

                if (lstCommands.Items.Contains(cmd.name)) {
                    Popup.Warning("Command " + cmd.name + " already exists, so was not loaded");
                    continue;
                }

                lstCommands.Items.Add(cmd.name);
                Command.Register(cmd);
                Logger.Log(LogType.SystemActivity, "Added /" + cmd.name + " to commands");
            }
        }
        
        string CompileCommands(string path) {
            ICompiler compiler = GetCompiler(path);
            if (compiler == null) {
                Popup.Warning("Unsupported file '" + path + "'");
                return null;
            }
            
            string tmp = "extra/commands/" + Path.GetRandomFileName() + ".dll";
            ConsoleHelpPlayer p = new ConsoleHelpPlayer();
            if (CompilerOperations.Compile(p, compiler, "Command", new[] { path }, tmp))
                return tmp;
            
            Popup.Error(Colors.StripUsed(p.Messages));
            DeleteAssembly(tmp);
            return null;
        }
        
        static ICompiler GetCompiler(string path) {
            foreach (ICompiler c in ICompiler.Compilers)
            {
                if (path.CaselessEnds(c.FileExtension)) return c;
            }
            return null;
        }
        
        static void DeleteAssembly(string path) {
            try { File.Delete(path); } catch { }
            try { File.Delete(path.Replace(".dll", ".pdb")); } catch { }
            try { File.Delete(path + ".mdb"); } catch { }
        }
        
        
        static string ListCompilers(StringFormatter<ICompiler> formatter) {
            return ICompiler.Compilers.Join(formatter, "");
        }
               
        static string GetFilterText() {
            StringBuilder sb = new StringBuilder();
            // Returns e.g. "Accepted File Types (*.cs, *.dll)|*.cs;*.dll|C# Source (*.cs)|*.cs|.NET Assemblies (*.dll)|*.dll";
            
            sb.AppendFormat("Accepted File Types ({0}*.dll)|",
                            ListCompilers(c => string.Format("*{0}, ", c.FileExtension)));
            
            sb.AppendFormat("{0}*.dll|",
                            ListCompilers(c => string.Format("*{0};", c.FileExtension)));
            
            sb.AppendFormat("{0}.NET Assemblies (*.dll)|*.dll",
                            ListCompilers(c => string.Format("{0} Source (*{1})|*{1}|", c.FullName, c.FileExtension)));
            return sb.ToString();
        }
    }
}
