using MCGalaxy.Commands;

namespace MCGalaxy.Commands {
    public sealed class CmdFixTP : Command {
        public override string name { get { return "FixTP"; } }
        public override string type { get { return CommandTypes.Building; } }
        public override LevelPermission defaultRank { get { return LevelPermission.Admin; } }
        
        public override void Use(Player p, string message) {
        	string[] urls = message.SplitSpaces(2);
        	if (urls.Length < 2) { Help(p); return; }
        	int changed = 0;
        	
        	string[] maps = LevelInfo.AllMapNames();
        	Level lvl;
        	foreach (string map in maps) {
        		LevelConfig cfg = LevelInfo.GetConfig(map, out lvl);
        		
        		if (cfg.Terrain.CaselessEq(urls[0])) {
        			p.Message("Changed terrain.png of map " + cfg.Color + map);
        			cfg.Terrain = urls[1];
        			cfg.SaveFor(map);
        			changed++;
        		} else if (cfg.TexturePack.CaselessEq(urls[0])) {
        			p.Message("Changed texture pack of map " + cfg.Color + map);
        			cfg.TexturePack = urls[1];
        			cfg.SaveFor(map);
        			changed++;
        		}
        	}
        	p.Message("Gone through all the maps. {0} were changed. ");
        }
        
        public override void Help(Player p) {
        	p.Message("%T/FixTP [source] [dest]");
        	p.Message("%HReplaces any maps using [source] texture pack with [dest] texture pack");
        }
    }
}
