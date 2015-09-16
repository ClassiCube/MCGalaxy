/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.Data;
using System.Text;
namespace MCGalaxy.Commands
{
    public sealed class CmdEnvironment : Command
    {
        public override string name { get { return "environment"; } }
        public override string shortcut { get { return "env"; } }
        public override string type { get { return "other"; } }
        public override bool museumUsable { get { return true; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Operator; } }
        public CmdEnvironment() { }

        public override void Use(Player p, string message)
        {
            if (message == "") { Help(p); return; }
            string[] pars = message.Split(' ');
            string target = "";
            if(pars[0].ToLower() == "player" || pars[0].ToLower() == "p")
            {
                target = "p";
            }
            if(pars[0].ToLower() == "level" || pars[0].ToLower() == "l")
            {
                target = "l";
            }
            if (target == "")
                return;
            Parse(p, target, pars);
        }
        public override void Help(Player p)
        {
            p.SendMessage("/env [target] [variable] [value]");
            p.SendMessage("Valid targets: player, level [Abbreviated as p and l]");
            p.SendMessage("Valid variables: fog, cloud, sky, sun, shadow, level, horizon, border, weather");
            p.SendMessage("Using 'normal' as a value will reset the variable");

        }
        public void Parse(Player p, string target, string[] pars)
        {
            string valueText = "";
            try
            {
                valueText = pars[2];
            }
            catch { }
            bool isValid = true;
            if (target == "p")
            {
                switch (pars[1].ToLower())
                {
                    case "fog":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset fog color for {0}&S to normal", "you"));
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.SendMessage(string.Format("Set fog color for {0}&S to #{1}", "you", valueText));
                            }
                        }
                            if (p.HasExtension("EnvSetColor"))
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + valueText.ToUpper());
                                    p.SendEnvSetColor(2, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    p.SendEnvSetColor(2, -1, -1, -1);
                                }
                            }
                        break;
                    case "cloud":
                    case "clouds":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset cloud color for {0}&S to normal", "you"));
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.SendMessage(string.Format("Set cloud color for {0}&S to #{1}", "you", valueText));

                            }
                        }
                            if (p.HasExtension("EnvSetColor"))
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + valueText.ToUpper());
                                    p.SendEnvSetColor(1, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    p.SendEnvSetColor(1, -1, -1, -1);
                                }
                            }
                        break;

                    case "sky":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset sky color for {0}&S to normal", "you"));
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.SendMessage(string.Format("Set sky color for {0}&S to #{1}", "you", valueText));
                            }
                        }

                        if (p.HasExtension("EnvSetColor"))
                        {
                            try
                            {
                                System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + valueText.ToUpper());
                                p.SendEnvSetColor(0, col.R, col.G, col.B);
                            }
                            catch
                            {
                                p.SendEnvSetColor(0, -1, -1, -1);
                            }
                        }
                        break;
                    case "dark":
                    case "shadow":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset shadow color for {0}&S to normal", "you"));
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.SendMessage(string.Format("Set shadow color for {0}&S to #{1}", "you", valueText));
                            }
                        }
                            if (p.HasExtension("EnvSetColor"))
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + valueText.ToUpper());
                                    p.SendEnvSetColor(3, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    p.SendEnvSetColor(3, -1, -1, -1);
                                }
                            }
                        break;

                    case "sun":
                    case "light":
                    case "sunlight":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset sunlight color for {0}&S to normal", "you"));
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.SendMessage(string.Format("Set sunlight color for {0}&S to #{1}", "you", valueText));
                            }
                        }
                        if (p.HasExtension("EnvSetColor"))
                        {
                            try
                            {
                                System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + valueText);
                                p.SendEnvSetColor(4, col.R, col.G, col.B);
                            }
                            catch
                            {
                                p.SendEnvSetColor(4, -1, -1, -1);
                            }
                        }
                        break;
                    case "level":
                    case "horizon":
                    case "edge":
                    case "water":
                    case "side":
                    case "border":
                    case "bedrock":
                        p.SendMessage("This feature is not available for 'player' target");
                        break;
                    case "weather":
                        byte weather = 0;
                        if (valueText.Equals("normal", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset weather for {0}&S to normal(0) ", "you"));
                        }
                        else
                        {
                            if (!byte.TryParse(valueText, out weather))
                            {
                                if (valueText.Equals("sun", StringComparison.OrdinalIgnoreCase))
                                {
                                    weather = 0;
                                }
                                else if (valueText.Equals("rain", StringComparison.OrdinalIgnoreCase))
                                {
                                    weather = 1;
                                }
                                else if (valueText.Equals("snow", StringComparison.OrdinalIgnoreCase))
                                {
                                    weather = 2;
                                }
                            }
                            if (weather < 0 || weather > 2)
                            {
                                p.SendMessage("Please use a valid integer(0,1,2) or string(sun,rain,snow)");
                                return;
                            }
                            p.SendMessage(string.Format("&aSet weather for {0}&a to {1} ({2}&a)", "you", weather, weather == 0 ? "&sSun" : (weather == 1 ? "&1Rain" : "&fSnow")));
                                p.SendSetMapWeather(byte.Parse(valueText));
                        }
                        break;
                    default:
                        Help(p);
                        break;
                }
            }
            if (target == "l")
            {
                switch (pars[1].ToLower())
                {
                    case "fog":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset fog color for {0}&S to normal", p.level.name));
                            p.level.FogColor = null;
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.level.FogColor = valueText;
                                p.SendMessage(string.Format("Set fog color for {0}&S to #{1}", p.level.name, valueText));
                            }
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvSetColor") && p.level == pl.level)
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + p.level.FogColor.ToUpper());
                                    pl.SendEnvSetColor(2, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    pl.SendEnvSetColor(2, -1, -1, -1);
                                }
                            }
                        }
                        break;
                    case "cloud":
                    case "clouds":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset cloud color for {0}&S to normal", p.level.name));
                            p.level.CloudColor = null;
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.level.CloudColor = valueText;
                                p.SendMessage(string.Format("Set cloud color for {0}&S to #{1}", p.level.name, valueText));

                            }
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvSetColor") && p.level == pl.level)
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + p.level.CloudColor.ToUpper());
                                    pl.SendEnvSetColor(1, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    pl.SendEnvSetColor(1, -1, -1, -1);
                                }
                            }
                        }
                        break;

                    case "sky":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset sky color for {0}&S to normal", p.level.name));
                            p.level.SkyColor = null;
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.level.SkyColor = valueText;
                                p.SendMessage(string.Format("Set sky color for {0}&S to #{1}", p.level.name, valueText));
                            }
                        }

                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvSetColor") && p.level == pl.level)
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + p.level.SkyColor.ToUpper());
                                    pl.SendEnvSetColor(0, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    pl.SendEnvSetColor(0, -1, -1, -1);
                                }
                            }
                        }
                        break;
                    case "dark":
                    case "shadow":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset shadow color for {0}&S to normal", p.level.name));
                            p.level.ShadowColor = null;
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.level.ShadowColor = valueText;
                                p.SendMessage(string.Format("Set shadow color for {0}&S to #{1}", p.level.name, valueText));
                            }
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvSetColor") && p.level == pl.level)
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + p.level.ShadowColor.ToUpper());
                                    pl.SendEnvSetColor(3, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    pl.SendEnvSetColor(3, -1, -1, -1);
                                }
                            }
                        }
                        break;

                    case "sun":
                    case "light":
                    case "sunlight":
                        if (valueText.Equals("-1") || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset sunlight color for {0}&S to normal", p.level.name));
                            p.level.LightColor = null;
                        }
                        else
                        {
                            isValid = IsValidHex(valueText);
                            if (!isValid)
                            {
                                p.SendMessage(string.Format("Env: \"#{0}\" is not a valid HEX color code.", valueText));
                                return;
                            }
                            else
                            {
                                p.level.LightColor = valueText;
                                p.SendMessage(string.Format("Set sunlight color for {0}&S to #{1}", p.level.name, valueText));
                            }
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvSetColor") && p.level == pl.level)
                            {
                                try
                                {
                                    System.Drawing.Color col = System.Drawing.ColorTranslator.FromHtml("#" + p.level.LightColor.ToUpper());
                                    pl.SendEnvSetColor(4, col.R, col.G, col.B);
                                }
                                catch
                                {
                                    pl.SendEnvSetColor(4, -1, -1, -1);
                                }
                            }
                        }
                        break;
                    case "level":
                        short level;
                        if (valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("reset", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase) || valueText.Equals("middle", StringComparison.OrdinalIgnoreCase) || valueText.Equals("center", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset water level for {0}&S to normal", p.level.name));
                            p.level.EdgeLevel = (short)(p.level.height / 2);
                        }
                        else
                        {
                            if (!short.TryParse(valueText, out level))
                            {
                                p.SendMessage(string.Format("Env: \"{0}\" is not a valid integer.", valueText));
                                return;
                            }
                            else
                            {
                                p.level.EdgeLevel = level;
                                p.SendMessage(string.Format("Set water level for {0}&S to {1}", p.level.name, level));
                            }
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvMapAppearance") && p.level == pl.level)
                            {
                                pl.SendSetMapAppearance(p.level.textureUrl, p.level.EdgeBlock, p.level.HorizonBlock, p.level.EdgeLevel);
                            }
                        }
                        break;

                    case "horizon":
                    case "edge":
                    case "water":
                        byte block = Block.Byte(valueText);
                        if (block == Block.Zero && !(valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase)))
                        {
                            Help(p);
                            return;
                        }
                        if (block == Block.water || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset water block for {0}&S to normal (Water)", p.level.name));
                            p.level.HorizonBlock = Block.water;
                        }
                        else
                        {
                            if (block == Block.air || block == Block.shrub || block == Block.glass || block == Block.yellowflower || block == Block.redflower || block == Block.mushroom || block == Block.redmushroom || block == Block.rope || block == Block.fire)
                            {
                                p.SendMessage(string.Format("Env: Cannot use {0} for water textures.", block));
                                return;
                            }
                            else
                            {
                                p.level.HorizonBlock = block;
                                p.SendMessage(string.Format("Set water block for {0}&S to {1}", p.level.name, block));
                            }
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvMapAppearance") && p.level == pl.level)
                            {
                                pl.SendSetMapAppearance(p.level.textureUrl, p.level.EdgeBlock, p.level.HorizonBlock, p.level.EdgeLevel);
                            }
                        }
                        break;

                    case "side":
                    case "border":
                    case "bedrock":
                        byte blockhorizon = Block.Byte(valueText);
                        if (blockhorizon == Block.Zero && !(valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase)))
                        {
                            Help(p);
                            return;
                        }
                        if (blockhorizon == Block.blackrock || valueText.Equals("normal", StringComparison.OrdinalIgnoreCase) || valueText.Equals("default", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset bedrock block for {0}&S to normal (Bedrock)", p.level.name));
                            p.level.EdgeBlock = Block.blackrock;
                        }
                        else
                        {
                            if (blockhorizon == Block.air || blockhorizon == Block.shrub || blockhorizon == Block.glass || blockhorizon == Block.yellowflower || blockhorizon == Block.redflower || blockhorizon == Block.mushroom || blockhorizon == Block.redmushroom || blockhorizon == Block.rope || blockhorizon == Block.fire)
                            {
                                p.SendMessage(string.Format("Env: Cannot use {0} for bedrock textures.", blockhorizon));
                                return;
                            }
                            else
                            {
                                p.level.EdgeBlock = blockhorizon;
                                p.SendMessage(string.Format("Set bedrock block for {0}&S to {1}", p.level.name, blockhorizon));
                            }
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvMapAppearance") && p.level == pl.level)
                            {
                                pl.SendSetMapAppearance(p.level.textureUrl, p.level.EdgeBlock, p.level.HorizonBlock, p.level.EdgeLevel);
                            }
                        }
                        break;
                    case "weather":
                        byte weather = 0;
                        if (valueText.Equals("normal", StringComparison.OrdinalIgnoreCase))
                        {
                            p.SendMessage(string.Format("Reset weather for {0}&S to normal(0) ", p.level.name));
                            p.level.weather = 0;
                        }
                        else
                        {
                            if (!byte.TryParse(valueText, out weather))
                            {
                                if (valueText.Equals("sun", StringComparison.OrdinalIgnoreCase))
                                {
                                    weather = 0;
                                }
                                else if (valueText.Equals("rain", StringComparison.OrdinalIgnoreCase))
                                {
                                    weather = 1;
                                }
                                else if (valueText.Equals("snow", StringComparison.OrdinalIgnoreCase))
                                {
                                    weather = 2;
                                }
                            }
                            if (weather < 0 || weather > 2)
                            {
                                p.SendMessage("Please use a valid integer(0,1,2) or string(sun,rain,snow)");
                                return;
                            }
                            p.level.weather = weather;
                            p.SendMessage(string.Format("&aSet weather for {0}&a to {1} ({2}&a)", p.level.name, weather, weather == 0 ? "&sSun" : (weather == 1 ? "&1Rain" : "&fSnow")));
                        }
                        foreach (Player pl in Player.players)
                        {
                            if (pl.HasExtension("EnvWeatherType") && p.level == pl.level)
                            {
                                pl.SendSetMapWeather(p.level.weather);
                            }
                        }
                        break;
                    default:
                        Help(p);
                        break;
                }
            }
            p.level.Save(true);
        }
        /// <summary> Ensures that the hex color has the correct length (1-6 characters)
        /// and character set (alphanumeric chars allowed). </summary>
        public static bool IsValidHex(string hex)
        {
            if (hex == null) throw new ArgumentNullException("hex");
            if (hex.StartsWith("#")) hex = hex.Remove(0, 1);
            if (hex.Length < 1 || hex.Length > 6) return false;
            for (int i = 0; i < hex.Length; i++)
            {
                char ch = hex[i];
                if (ch < '0' || ch > '9' &&
                    ch < 'A' || ch > 'Z' &&
                    ch < 'a' || ch > 'z')
                {
                    return false;
                }
            }
            return true;
        }
    }
}
