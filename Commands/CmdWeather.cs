using System;
using System.IO;

namespace MCGalaxy.Commands
{
    public class CmdWeather : Command
    {
        public override string name { get { return "weather"; } }
        public override string shortcut { get { return  ""; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdWeather() { }
        
        public override void Use(Player p, string message)
        {
            if (message == "")
            {
                message = "sunny";
            }
            
            Level targetLevel;
            string forecast;
            
            if (message.Split(' ').Length > 1)
            {
                targetLevel = Level.Find(message.Split(' ')[0].Trim());
                if (targetLevel == null)
                {
                    Player.SendMessage(p, "Level \"" + message.Split(' ')[0].Trim() + "\" was not found...");
                    return;
                }
                forecast = message.Split(' ')[1].Trim().ToLower();
            }
            else
            {
                targetLevel = p.level;
                forecast = message.Trim().ToLower();
            }
            
            byte weatherType;
            
            switch (forecast)
            {
            case "sunny":
                weatherType = 0;
                break;
            case "raining":
                weatherType = 1;
                break;
            case "snowing":
                weatherType = 2;
                break;
            default:
                goto case "sunny";
            }
            
            Player.players.ForEach(delegate(Player pl)
                                   { 
                if (pl.level == targetLevel && pl.HasExtension("EnvWeatherType"))
                {
                    pl.SendSetMapWeather(weatherType);
                }
            });
            
            Player.GlobalMessage("Weather was set to &c" + forecast + Server.DefaultColor + " on &b" + targetLevel.name);
            targetLevel.weather = weatherType;
        }
        
        public override void Help(Player p)
        {
            Player.SendMessage(p, "/weather <level> [forecast] - Changes current levels weather.");
            Player.SendMessage(p, "Available forecasts: Sunny, Raining, Snowing.");
        }
    }
}
