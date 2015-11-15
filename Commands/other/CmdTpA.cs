//Copyright 2015 MCGalaxy
using System;
using System.Threading;

namespace MCGalaxy.Commands
{
    public sealed class CmdTpA : Command
    {
        public override string name { get { return "tpa"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTpA() { }

        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                Help(p); return;
            }
            int number = message.Split(' ').Length;
            if (number > 1) { Help(p); return; }

            Player who = Player.Find(message);
            if (who == p) { Player.SendMessage(p, "&cError:" + Server.DefaultColor + " You cannot send yourself a request!"); return; }
            if (who == null || (who.hidden && p.group.Permission < LevelPermission.Admin)) { Player.SendMessage(p, "There is no player \"" + message + "\"!"); return; }
            if (who.listignored.Contains(p.name)) 
            {
                //Lies
                Player.SendMessage(p, "---------------------------------------------------------");
                Player.SendMessage(p, "Your teleport request has been sent to " + who.color + who.DisplayName);
                Player.SendMessage(p, "This request will timeout after " + c.aqua + "90" + Server.DefaultColor + " seconds.");
                Player.SendMessage(p, "---------------------------------------------------------");
                return; 
            }
            if (who.name == p.currentTpa) { Player.SendMessage(p, "&cError:" + Server.DefaultColor + " You already have a pending request with this player."); return; }
            if (p.level != who.level && who.level.name.Contains("cMuseum"))
            {
                Player.SendMessage(p, "Player \"" + who.color + who.DisplayName + "\" is in a museum!");
                return;
            }

            if (who.Loading)
            {
                Player.SendMessage(p, "Waiting for " + who.color + who.DisplayName + Server.DefaultColor + " to spawn...");
                while (who.Loading) { }
            }

            Player.SendMessage(p, "---------------------------------------------------------");
            Player.SendMessage(p, "Your teleport request has been sent to " + who.color + who.DisplayName);
            Player.SendMessage(p, "This request will timeout after " + c.aqua + "90" + Server.DefaultColor + " seconds.");
            Player.SendMessage(p, "---------------------------------------------------------");
            Player.SendMessage(who, "---------------------------------------------------------");
            Player.SendMessage(who, p.color + p.DisplayName + Server.DefaultColor + " would like to teleport to you!");
            Player.SendMessage(who, "Type " + c.green + "/tpaccept " + Server.DefaultColor + "or " + c.maroon + "/tpdeny" + Server.DefaultColor + "!");
            Player.SendMessage(who, "This request will timeout after " + c.aqua + "90" + Server.DefaultColor + " seconds.");
            Player.SendMessage(who, "---------------------------------------------------------");
            who.senderName = p.DisplayName;
            who.Request = true;
            p.currentTpa = who.name;
            Thread.Sleep(90000);
            if (who.Request != false)
            {
                Player.SendMessage(p, "Your teleport request has timed out.");
                Player.SendMessage(who, "Pending teleport request has timed out.");
                who.Request = false;
                who.senderName = "";
                p.currentTpa = "";
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tpa <player> - Sends a teleport request to the given player");
            Player.SendMessage(p, "/tpaccept - Accepts a teleport request");
            Player.SendMessage(p, "/tpdeny - Denies a teleport request");
        }
    }

    public sealed class CmdTpAccept : Command
    {
        public override string name { get { return "tpaccept"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTpAccept() { }

        public override void Use(Player p, string message)
        {
            if (p.Request == false)
            {
                Player.SendMessage(p, "&cError:" + Server.DefaultColor + " You do not have any pending teleport requests!"); return;
            }
            else
            {
                Player who = Player.Find(p.senderName);
                Player.SendMessage(p, "You have accepted " + who.color + who.DisplayName + Server.DefaultColor + "'s teleportation request.");
                Player.SendMessage(who, p.color + p.DisplayName + Server.DefaultColor + " has accepted your request. Teleporting now...");
                p.Request = false;
                who.currentTpa = "";
                Thread.Sleep(1000);
                if (p.level != who.level)
                {
                    Level where = p.level;
                    Command.all.Find("goto").Use(who, where.name);
                    Thread.Sleep(1000);
                    while (who.Loading) { Thread.Sleep(250); }
                }

                unchecked { who.SendPos((byte)-1, p.pos[0], p.pos[1], p.pos[2], p.rot[0], 0); }
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tpaccept - Accepts a teleport request");
            Player.SendMessage(p, "For use with /tpa");
        }
    }

    public sealed class CmdTpDeny : Command
    {
        public override string name { get { return "tpdeny"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return CommandTypes.Other; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Guest; } }
        public CmdTpDeny() { }

        public override void Use(Player p, string message)
        {
            if (p.Request == false)
            {
                Player.SendMessage(p, "&cError:" + Server.DefaultColor + " You do not have any pending teleport requests!"); return;
            }
            else
            {
                Player who = Player.Find(p.senderName);
                Player.SendMessage(p, "You have denied " + who.color + who.DisplayName + Server.DefaultColor + "'s teleportation request.");
                Player.SendMessage(who, p.color + p.DisplayName + Server.DefaultColor + " has denied your request.");
                p.senderName = "";
                p.Request = false;
                who.currentTpa = "";
            }
        }
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/tpdeny - Denies a teleport request");
            Player.SendMessage(p, "For use with /tpa");
        }
    }
}
