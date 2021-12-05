using System;
using System.Collections.Generic;
using System.IO;
using MCGalaxy.Network;

namespace MCGalaxy.Authentication
{
    public class MicrosoftAuthService
    {
        public override bool Authenticate(Player p, string mppass) {
            String serverId = Server.Config.ListenIP + ":" + Server.Config.Port;
            MessageDigest md = null;
            try {
                md = MessageDigest.getInstance("SHA-1");
            }
            catch(NoSuchAlgorithmException e) {
                Logger.LogError(e)
                return false;
            }
            serverId = new String(md.digest(String.getBytes(serverId)));

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