using System;
using System.IO;
using MCGalaxy;

namespace MCGalaxy.Commands
{
    public class CmdXModel : Command
    {
        public override string name { get { return "xmodel"; } }
        public override string shortcut { get { return "xm"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdXModel() { }

        public override void Use(Player p, string message)
        {
            if (p == null)
            {
                Player.SendMessage(p, "You cannot use this command from the console!");
                return;
            }
            if (message == "")
            {
                Command.all.Find("model").Use(p, p.name + " normal");
            }
            else
            {
                Command.all.Find("model").Use(p, p.name + " " + message);
            }

        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/xm [model] - Changes your player model.");
            Player.SendMessage(p, "Available models: Chicken, Creeper, Croc, Humanoid, Pig, Printer, Sheep, Spider, Skeleton, Zombie.");
            Player.SendMessage(p, "You can also place a block ID instead of a model name, to change your model into a block!");
        }
    }
    
}
