using System;
using MCGalaxy;

namespace MCGalaxy.Commands
{
    public class CmdXNick : Command
    {
        public override string name { get { return "xnick"; } }
        public override string shortcut { get { return "xnickname"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdXNick() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                p.DisplayName = p.name;
                Player.GlobalChat(p, p.color + p.prefix + p.DisplayName + "&g has reverted their nick to their original name.", false);
                Player.GlobalDie(p, false);
                Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
                return;
            }

            string newName = "";
            newName = message;

            if (newName.Length > 60) { Player.SendMessage(p, "Nick must be under 60 letters."); return; }
            
            if (newName != "") Player.GlobalChat(p, p.color + p.DisplayName + "&g has changed their nick to " + newName + "&g.", false);
            p.DisplayName = newName;
            Player.GlobalDie(p, false);
            Player.GlobalSpawn(p, p.pos[0], p.pos[1], p.pos[2], p.rot[0], p.rot[1], false);
            PlayerDB.Save(p);
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/xnick [newName] - Gives you the nick of [newName].");
            Player.SendMessage(p, "If no [newName] is given, your nick is reverted to your original name.");
        }
    }
}

