/* 
	Copyright 2011 MCDerp Team Based in Canada
	MCDerp has been Licensed with the below license:
	
	http://www.binpress.com/license/view/l/62e7c4034ccb45cd39d8dcbe9ed87bd8
	Or, you can read the below summary;

	Can be used on 1 site, unlimited servers
	Personal use only (cannot be resold or distributed)
	Non-commercial use only
	Cannot modify source-code for any purpose (cannot create derivative works)
	Software trademarks are included in the license
	Software patents are included in the license
*/
		

using System;
using System.Timers;

namespace MCForge
{
    public class CmdInterval : Command
    {
        public override string name { get { return "interval"; } }
        public override string shortcut { get { return ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return false; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        Command nowCmd;
        Timer checker;
        public override void Use(Player p, string message)
        {
            if (checker==null){
                checker = new Timer(60000);
                checker.Elapsed += new ElapsedEventHandler(checker_Elapsed);
                checker.Start();
            }
            String[] incoming = message.Split(' ');
            if(incoming.Length == 2)
            {
                nowCmd = Command.all.Find(incoming[0]);
                if (nowCmd != null)
                {
                    //command exists
                    nowCmd.isIntervalized = true;
                    nowCmd.intervalInMinutes = int.Parse(incoming[1]);
                    nowCmd.intervalUsingPlayer = p;
                    try
                    {
                        nowCmd.Use(nowCmd.intervalUsingPlayer, "");
                    }
                    catch (Exception e)
                    {
                        Player.SendMessage(p, "An error occoured. please report this to the devs, cause this cmd seems to be not Interval-Compatible atm");
                        nowCmd.isIntervalized = false;
                        return;
                    }
                    nowCmd.nextExecution = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, DateTime.Today.Hour, DateTime.Today.Minute + nowCmd.intervalInMinutes, 0);
                }else{
                    Player.SendMessage(p, "This command doesn't exist.");
                }
            }else if(incoming.Length == 1){
                nowCmd = Command.all.Find(incoming[0]);
                if (nowCmd != null)
                {
                    //command exists
                    if (nowCmd.isIntervalized)
                    {
                        nowCmd.isIntervalized = false;
                        Player.SendMessage(p, "Command will not be executed anymore in its Interval");
                    }
                }
                else
                {
                    Player.SendMessage(p, "This command doesn't exist.");
                }
            }else{
                this.Help(p); return;
            }
            nowCmd = null;
        }

        private void  checker_Elapsed(object sender, ElapsedEventArgs e)
        {
            foreach (Command cmd in Command.all.commands)
            {
                try
                {
                    if (cmd.nextExecution.CompareTo(DateTime.Today) > 0 && cmd.isIntervalized)
                    {
                        cmd.Use(cmd.intervalUsingPlayer, "");
                        cmd.nextExecution = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, DateTime.Today.Hour, DateTime.Today.Minute + nowCmd.intervalInMinutes, 0);
                    }
                }catch(Exception ex){
                }
            }
        }

        public override void Help(Player p)
        {
            Player.SendMessage(p, "/Interval <Command> <Interval in minutes> - Executes a command every given minutes. The command will be executed if you send this command.");
        }
    }
}