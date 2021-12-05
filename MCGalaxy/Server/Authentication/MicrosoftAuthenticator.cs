using System;
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Network;

namespace MCGalaxy.Authentication
{
    public class MicrosoftAuthenticator
    {
        public bool Authenticate(Player p) {
            String serverId = Server.Config.ListenIP + ":" + Server.Config.Port;
            var hash = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(serverId));
            serverId = string.Concat(hash.Select(b => b.ToString("x2")));

            if (!hasJoined(p.truename, serverId)) return false;

            AuthServiceConfig cfg = Config;
            
            p.verifiedName = true;
            p.SkinName     = cfg.SkinPrefix + p.SkinName;
            
            p.name        += cfg.NameSuffix;
            p.truename    += cfg.NameSuffix;
            p.DisplayName += cfg.NameSuffix;
            return true;
        }

        private void hasJoined(String username, String serverId) {
            HttpURLConnection connection;

            URL url = new URL("https://sessionserver.mojang.com/session/minecraft/hasJoined?username=" + username + "&serverId=" + serverId );
            connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod("GET");
            connection.setDoInput(true);
            connection.setDoOutput(false);

            connection.connect();

            return connection.getResponseCode() == 200 || connection.getResponseCode() == 204;
        }
    }
}