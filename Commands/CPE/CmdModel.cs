using System;
using System.IO;
using MCGalaxy;

namespace MCGalaxy.Commands
{
    public class CmdModel : Command
    {
        public override string name { get { return "model"; } }
        public override string shortcut { get { return "setmodel"; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.AdvBuilder; } }
        public CmdModel() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                message = "normal";
            }

            Player targetPlayer;
            string model;

            if (message.Split(' ').Length > 1)
            {
                targetPlayer = Player.Find(message.Split(' ')[0].Trim());
                if (targetPlayer == null)
                {
                    Player.SendMessage(p, "Player \"" + message.Split(' ')[0].Trim() + "\" does not exist");
                    return;
                }
                model = message.Split(' ')[1].Trim();
                targetPlayer.model = model;
            }
            else
            {
                if (p == null)
                {
                    Player.SendMessage(null, "Console can't use this command on itself.");
                    return;
                }
                targetPlayer = p;
                model = message;
                p.model = model;
            }

            foreach (Player pl in Player.players)
            {
                if (pl.level == targetPlayer.level && pl.HasCpeExt(CpeExt.ChangeModel))
                {
                    pl.SendChangeModel(targetPlayer.id, message);
                }
            }

            if (p == null)
            {
                targetPlayer.SendMessage("You're now a &c" + model);
            }
            else
            {
                Command.all.Find("aka").Use(targetPlayer, "");
                Player.GlobalMessage(targetPlayer.color + targetPlayer.DisplayName + "'s" + Server.DefaultColor + " model was changed to a &c" + model);
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/model <player> [model] - Changes your player model.");
            Player.SendMessage(p, "Available models: Chicken, Creeper, Croc, Humanoid, Pig, Printer, Sheep, Spider, Skeleton, Zombie.");
            Player.SendMessage(p, "You can also place a block ID instead of a model name, to change your model into a block!");
        }
    }
}
