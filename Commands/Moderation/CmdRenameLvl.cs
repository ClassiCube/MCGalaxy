/*
    Copyright 2011 MCGalaxy
        
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
using System.IO;
using MCGalaxy.SQL;
namespace MCGalaxy.Commands
{
    public sealed class CmdRenameLvl : Command
    {
        public override string name { get { return "renamelvl"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Moderation; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        public CmdRenameLvl() { }

        public override void Use(Player p, string message)
        {
            if (message == "" || message.IndexOf(' ') == -1) { Help(p); return; }
            Level foundLevel = LevelInfo.Find(message.Split(' ')[0]);
            if (foundLevel == null)
            {
                Player.SendMessage(p, "Level not found");
                return;
            }

            string newName = message.Split(' ')[1];

            if (LevelInfo.ExistsOffline(newName)) { Player.SendMessage(p, "Level already exists."); return; }
            if (foundLevel == Server.mainLevel) { Player.SendMessage(p, "Cannot rename the main level."); return; }

            foundLevel.Unload();


            File.Move("levels/" + foundLevel.name + ".lvl", "levels/" + newName + ".lvl");
            try
            {
                File.Move("levels/" + foundLevel.name + ".lvl.backup", "levels/" + newName + ".lvl.backup");
            } catch { }
            
            try
            {
                File.Move("levels/level properties/" + foundLevel.name + ".properties", "levels/level properties/" + newName + ".properties");
            }
            catch { }
            try
            {
                File.Move("levels/level properties/" + foundLevel.name, "levels/level properties/" + newName + ".properties");
            }
            catch { }
            
            try {
                if (File.Exists("blockdefs/lvl_" + foundLevel.name + ".json"))
                    File.Move("blockdefs/lvl_" + foundLevel.name + ".json", "blockdefs/lvl_" + newName + ".json");
            } catch {}

            //Move and rename backups
            try
            {
                string foundLevelDir, newNameDir;
                for (int i = 1; ; i++)
                {
                    foundLevelDir = @Server.backupLocation + "/" + foundLevel.name + "/" + i + "/";
                    newNameDir = @Server.backupLocation + "/" + newName + "/" + i + "/";

                    if (File.Exists(foundLevelDir + foundLevel.name + ".lvl"))
                    {
                        Directory.CreateDirectory(newNameDir);
                        File.Move(foundLevelDir + foundLevel.name + ".lvl", newNameDir + newName + ".lvl");
                        if (DirectoryEmpty(foundLevelDir))
                            Directory.Delete(foundLevelDir);
                    }
                    else
                    {
                        if (DirectoryEmpty(@Server.backupLocation + "/" + foundLevel.name + "/"))
                            Directory.Delete(@Server.backupLocation + "/" + foundLevel.name + "/");
                        break;
                    }
                }
            }
            catch { }

            //safe against SQL injections because foundLevel is being checked and,
            //newName is being split and partly checked on illegal characters reserved for Windows.
            if (Server.useMySQL)
                Database.executeQuery(String.Format("RENAME TABLE `Block{0}` TO `Block{1}`, " +
                                                    "`Portals{0}` TO `Portals{1}`, " +
                                                    "`Messages{0}` TO `Messages{1}`, " +
                                                    "`Zone{0}` TO `Zone{1}`", foundLevel.name.ToLower(), newName.ToLower()));
            else {
                using (DatabaseTransactionHelper helper = SQLiteTransactionHelper.Create()) { // ensures that it's either all work, or none work.
                    helper.Execute(String.Format("ALTER TABLE Block{0} RENAME TO Block{1}", foundLevel.name.ToLower(), newName.ToLower()));
                    helper.Execute(String.Format("ALTER TABLE Portals{0} RENAME TO Portals{1}", foundLevel.name.ToLower(), newName.ToLower()));
                    helper.Execute(String.Format("ALTER TABLE Messages{0} RENAME TO Messages{1}", foundLevel.name.ToLower(), newName.ToLower()));
                    helper.Execute(String.Format("ALTER TABLE Zone{0} RENAME TO Zone{1}", foundLevel.name.ToLower(), newName.ToLower()));
                    helper.Commit();
                }
            }
            try { Command.all.Find("load").Use(p, newName); }
            catch { }
            Player.GlobalMessage("Renamed " + foundLevel.name + " to " + newName);
        }
        
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/renamelvl <level> <new name> - Renames <level> to <new name>");
            Player.SendMessage(p, "Portals going to <level> will be lost");
        }

        public static bool DirectoryEmpty(string dir)
        {
            if (!Directory.Exists(dir))
                return true;
            if (Directory.GetDirectories(dir).Length > 0)
                return false;
            if (Directory.GetFiles(dir).Length > 0)
                return false;

            return true;
        }
    }
}
