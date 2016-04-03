using System;

namespace MCGalaxy {
    public class WhoWas {
        public string rank { get; set; }
        public int modified_blocks { get; set; }
        public string time { get; set; }
        public string first_login { get; set; }
        public string last_login { get; set; }
        public int total_logins { get; set; }
        public bool banned { get; set; }
        public string ban_reason { get; set; }
        public string banned_by { get; set; }
        public string banned_time { get; set; }
        public int kicks { get; set; }

        public WhoWas(string p) {
            rank = Group.findPlayer(p);
            OfflinePlayer pl = PlayerInfo.FindOffline(p, true);
            if (pl == null) return;
            
            modified_blocks = int.Parse(pl.blocks);
            time = pl.totalTime;
            first_login = pl.firstLogin;
            last_login = pl.lastLogin;
            total_logins = int.Parse(pl.logins);
            kicks = int.Parse(pl.kicks);
        }
    }
}