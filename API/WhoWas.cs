using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using MCGalaxy.SQL;

namespace MCGalaxy
{
    public class WhoWas
    {
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

        public WhoWas(string p)
        {
            Server.s.Log(p);
            rank = Group.findPlayer(p);
            ParameterisedQuery query = ParameterisedQuery.Create();
            query.AddParam("@Name", p.ToLower());
            DataTable playerDb = Database.fillData(query, "SELECT * FROM Players WHERE Name=@Name COLLATE NOCASE");
            if (playerDb.Rows.Count == 0)
                return;
            modified_blocks = int.Parse(playerDb.Rows[0]["totalBlocks"].ToString());
            time = playerDb.Rows[0]["TimeSpent"].ToString();
            first_login = playerDb.Rows[0]["FirstLogin"].ToString();
            last_login = playerDb.Rows[0]["LastLogin"].ToString();
            total_logins = int.Parse(playerDb.Rows[0]["totalLogin"].ToString());
            kicks = int.Parse(playerDb.Rows[0]["totalKicked"].ToString());
        }
    }
}