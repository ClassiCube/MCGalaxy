/*
    Copyright 2011 MCForge
        
    Dual-licensed under the Educational Community License, Version 2.0 and
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
using System.Collections.Generic;

namespace MCGalaxy.Commands {
    
    public sealed class CommandKeywords {
        public static List<CommandKeywords> all = new List<CommandKeywords>();
        public Command Cmd;
        public string[] Keywords;
        const StringComparison comp = StringComparison.OrdinalIgnoreCase;
        
        public CommandKeywords(Command cmd, string key) {
            this.Cmd = cmd;
            string keyword = key + " " + cmd.name + " " + cmd.type;
            if (cmd.shortcut.Length > 3) { keyword += " " + cmd.shortcut; }
            this.Keywords = keyword.Split(' ');
            all.Add(this);
        }
        
        public static void SetKeyWords() {
            new CommandKeywords((new CmdAbort()), "command action mode");
            new CommandKeywords((new CmdAbout()), "info block history grief");
            new CommandKeywords((new CmdAdminChat()), "admin chat opchat");
            new CommandKeywords((new CmdAfk()), "away dnd");
            new CommandKeywords((new CmdAgree()), "accept yes rules");
            new CommandKeywords((new CmdAka()), "nick reset user name");
            new CommandKeywords((new CmdAlive()), "life health living");
            new CommandKeywords((new CmdAllowGuns()), "allow gun missile shoot boom terrorism");
            new CommandKeywords((new CmdAscend()), "up level block move player");
            new CommandKeywords((new CmdAward()), "reward trophy price");
            new CommandKeywords((new CmdAwards()), "price list trophy info");
            new CommandKeywords((new CmdAwardMod()), "trophy add del price");
            new CommandKeywords((new CmdBan()), "kick mod punish");
            new CommandKeywords((new CmdBanEdit()), "reason edit ban");
            new CommandKeywords((new CmdBaninfo()), "info ban details");
            new CommandKeywords((new CmdBanip()), "ip ban kick mod punish");
            new CommandKeywords((new CmdBanlist()), "list ban info mod");
            new CommandKeywords((new CmdBind()), "block replace");
            new CommandKeywords((new CmdBlocks()), "block info list");
            new CommandKeywords((new CmdBlockSet()), "rank mod block");
            new CommandKeywords((new CmdBlockSpeed()), "block speed mod setting");
            new CommandKeywords((new CmdBotAdd()), "bot add mod fun");
            new CommandKeywords((new CmdBotAI()), "bot ai add del remove");
            new CommandKeywords((new CmdBotRemove()), "bot remove del");
            new CommandKeywords((new CmdBots()), "bot list ai");
            new CommandKeywords((new CmdBotSet()), "set bot ai");
            new CommandKeywords((new CmdBotSummon()), "summon bot");
            new CommandKeywords((new CmdC4()), "tnt c4 explosion");
            new CommandKeywords((new CmdChain()), "grab block mushroom brown");
            new CommandKeywords((new CmdChangeLog()), "MCGalaxy change log");
            new CommandKeywords((new CmdClearBlockChanges()), "about block change remove del");
            new CommandKeywords((new CmdClick()), "block use");
            new CommandKeywords((new CmdClones()), "clone ip player info");
            new CommandKeywords((new CmdCmdBind()), "bind cmd command shortcut");
            new CommandKeywords((new CmdCmdSet()), "set rank cmd command");
            new CommandKeywords((new CmdColor()), "user name");
            new CommandKeywords((new CmdCopy()), "clipboard blocks save");
            new CommandKeywords((new CmdCopyLVL()), "copy lvl level map");
            new CommandKeywords((new CmdCountdown()), "count down");
            new CommandKeywords((new CmdCrashServer()), "error stop server");
            new CommandKeywords((new CmdCTF()), "capture flag");
            new CommandKeywords((new CmdCuboid()), "box set area block");
            new CommandKeywords((new CmdDelete()), "del mode");
            new CommandKeywords((new CmdDeleteLvl()), "delete remove level lvl");
            new CommandKeywords((new CmdDelTempRank()), "del remove temp rank");
            new CommandKeywords((new CmdDemote()), "rank lower");
            new CommandKeywords((new CmdDevs()), "dev MCGalaxy forgeware forgecraft");
            new CommandKeywords((new CmdDisagree()), "no rules");
            new CommandKeywords((new CmdDescend()), "down below");
            new CommandKeywords((new CmdDisInfect()), "infect player");
            new CommandKeywords((new CmdDraw()), "cone sphere pyramid create");
            new CommandKeywords((new CmdDrill()), "dig distance");
            new CommandKeywords((new CmdEconomy()), "money cash " + Server.moneys);
            new CommandKeywords((new CmdEmote()), "smiley emoticon");
            new CommandKeywords((new CmdEndRound()), "end round game");
            new CommandKeywords((new CmdExplode()), "explosion boom");
            new CommandKeywords((new CmdFakePay()), "fake troll pay " + Server.moneys);
            new CommandKeywords((new CmdFakeRank()), "rank mod fake troll");
            new CommandKeywords((new CmdFaq()), "freq ask question");
            new CommandKeywords((new CmdFill()), "cuboid edit");
            new CommandKeywords((new CmdFixGrass()), "grass fix");
            new CommandKeywords((new CmdFlipHead()), "head fix flip troll");
            new CommandKeywords((new CmdFlipHeads()), "head fix flip troll");
            new CommandKeywords((new CmdFly()), "air ctf glass carpet");
            new CommandKeywords((new CmdFollow()), "posses impersonate");
            new CommandKeywords((new CmdFreeze()), "ice move player");
            new CommandKeywords((new CmdGarbage()), "memory clean unused");
            new CommandKeywords((new CmdGive()), "money pay " + Server.moneys);
            new CommandKeywords((new CmdGlobalCLS()), "clear global chat");
            new CommandKeywords((new CmdGoto()), "level lvl change map");
            new CommandKeywords((new CmdGun()), "shoot boom terrorism missile");
            new CommandKeywords((new CmdHackRank()), "fake rank color set mod");
            new CommandKeywords((new CmdHacks()), "hack fake");
            new CommandKeywords((new CmdHeartbeat()), "heart server list");
            new CommandKeywords((new CmdHelp()), "info commands cmd list");
            new CommandKeywords((new CmdHide()), "hidden show invisible");
            new CommandKeywords((new CmdHigh5()), "high 5 fun");
            new CommandKeywords((new CmdHighlight()), "mod high light block change history");
            new CommandKeywords((new CmdHollow()), "block create");
            new CommandKeywords((new CmdHost()), "owner server " + Server.server_owner + " " + Server.ZallState);
            new CommandKeywords((new CmdIgnore()), "abort chat player");
            new CommandKeywords((new CmdImport()), "dat imp extra");
            new CommandKeywords((new CmdImageprint()), "jpg bmp gif png image print");
            new CommandKeywords((new CmdInfect()), "virus inf");
            new CommandKeywords((new CmdInfected()), "infect player list");
            new CommandKeywords((new CmdInfo()), "server detail");
            new CommandKeywords((new CmdInvincible()), "god life inf");
            new CommandKeywords((new CmdJail()), "prison punish");
            new CommandKeywords((new CmdJoker()), "joke troll fun");
            new CommandKeywords((new CmdKick()), "moderate ban player");
            new CommandKeywords((new CmdKickban()), "kick ban moderate");
            new CommandKeywords((new CmdKill()), "die player murder");
            new CommandKeywords((new CmdKillPhysics()), "kill physics level");
            new CommandKeywords((new CmdLastCmd()), "cmd command last info");
            new CommandKeywords((new CmdLavaSurvival()), "lava surv");
            new CommandKeywords((new CmdLevels()), "level map info list");
            new CommandKeywords((new CmdLimit()), "lim moderate type");
            new CommandKeywords((new CmdLine()), "draw block paint");
            new CommandKeywords((new CmdLoad()), "level map lvl");
            new CommandKeywords((new CmdLockdown()), "lock down level map lvl player");
            new CommandKeywords((new CmdLoginMessage()), "login msg message");
            new CommandKeywords((new CmdLogoutMessage()), "logout quit exit stop msg message");
            new CommandKeywords((new CmdLowlag()), "low less lag");
            new CommandKeywords((new CmdMain()), "default map level lvl");
            new CommandKeywords((new CmdMap()), "level lvl info edit");
            new CommandKeywords((new CmdMapInfo()), "map info");
            new CommandKeywords((new CmdMaze()), "labyrint create");
            new CommandKeywords((new CmdMe()), "do action");
            new CommandKeywords((new CmdMeasure()), "meas block length distance");
            new CommandKeywords((new CmdMessageBlock()), "message block msg");
            new CommandKeywords((new CmdMissile()), "gun missil");
            new CommandKeywords((new CmdMode()), "block place");
            new CommandKeywords((new CmdModerate()), "chat enable disable allow disallow");
            new CommandKeywords((new CmdMoney()), "cash " + Server.moneys);
            new CommandKeywords((new CmdMove()), "player pos");
            new CommandKeywords((new CmdMoveAll()), "move all player pos");
            new CommandKeywords((new CmdMuseum()), "musea map lvl level");
            new CommandKeywords((new CmdMute()), "voice chat player");
            new CommandKeywords((new CmdNewLvl()), "new add lvl level map");
            new CommandKeywords((new CmdNews()), "info latest");
            new CommandKeywords((new CmdOHide()), "hide rank player invisible");
            new CommandKeywords((new CmdOpChat()), "private chat op");
            new CommandKeywords((new CmdOpRules()), "op rules info");
            new CommandKeywords((new CmdOpStats()), "stats op info");
            new CommandKeywords((new CmdOutline()), "out line layer");
            new CommandKeywords((new CmdOverseer()), "over see map level lvl");
            new CommandKeywords((new CmdOZone()), "zone map lvl level entire");
            new CommandKeywords((new CmdP2P()), "tp tele port player move");
            new CommandKeywords((new CmdPaint()), "mode block place");
            new CommandKeywords((new CmdPaste()), "copy clipboard out");
            new CommandKeywords((new CmdPatrol()), "teleport random");
            new CommandKeywords((new CmdPause()), "physics reset");
            new CommandKeywords((new CmdPay()), "money give " + Server.moneys);
            new CommandKeywords((new CmdPlayerEditDB()), "player block limit mod edit");
            new CommandKeywords((new CmdPCount()), "player online total number count");
            new CommandKeywords((new CmdPCreate()), "create add new plugin");
            new CommandKeywords((new CmdPerbuildMax()), "perm build max rank");
            new CommandKeywords((new CmdPermissionBuild()), "perm build rank");
            new CommandKeywords((new CmdPermissionVisit()), "perm visit rank");
            new CommandKeywords((new CmdPervisitMax()), "perm visit max rank");
            new CommandKeywords((new CmdPhysics()), "block move");
            new CommandKeywords((new CmdPlace()), "block pos");
            new CommandKeywords((new CmdPlayerCLS()), "clear del player chat");
            new CommandKeywords((new CmdPlayers()), "player list info");
            new CommandKeywords((new CmdPortal()), "teleport tp move transport");
            new CommandKeywords((new CmdPossess()), "imp control");
            new CommandKeywords((new CmdPromote()), "rank up");
            new CommandKeywords((new CmdPyramid()), "egypt pyram piram sand");
            new CommandKeywords((new CmdQueue()), "zombie");
            new CommandKeywords((new CmdRagequit()), "rage quit exit stop leave");
            new CommandKeywords((new CmdRainbow()), "rain bow dash");
            new CommandKeywords((new CmdRankInfo()), "rank info display");
            new CommandKeywords((new CmdRankMsg()), "rank msg edit set");
            new CommandKeywords((new CmdRedo()), "undo edit block change");
            new CommandKeywords((new CmdReload()), "level lvl map");
            new CommandKeywords((new CmdReferee()), "enable disable ref mode");
            new CommandKeywords((new CmdRenameLvl()), "rename lvl level map");
            new CommandKeywords((new CmdRepeat()), "rep again");
            new CommandKeywords((new CmdReplace()), "block level lvl map");
            new CommandKeywords((new CmdReplaceAll()), "all block replace lvl level map");
            new CommandKeywords((new CmdReplaceNot()), "not replace block level lvl map");
            new CommandKeywords((new CmdReport()), "rep alert");
            new CommandKeywords((new CmdRestart()), "start again over new");
            new CommandKeywords((new CmdRestartPhysics()), "restart physics");
            new CommandKeywords((new CmdRestore()), "old rest copy");
            new CommandKeywords((new CmdRestoreSelection()), "restore select");
            new CommandKeywords((new CmdReveal()), "show");
            new CommandKeywords((new CmdReview()), "view look judge");
            new CommandKeywords((new CmdRide()), "drive train");
            new CommandKeywords((new CmdRoll()), "dice gamble");
            new CommandKeywords((new CmdRules()), "read prot");
            new CommandKeywords((new CmdSave()), "store load");
            new CommandKeywords((new CmdSay()), "speak broad cast");
            new CommandKeywords((new CmdSearch()), "find block command player rank");
            new CommandKeywords((new CmdSeen()), "saw last user");
            new CommandKeywords((new CmdServerReport()), "report server");
            new CommandKeywords((new CmdServer()), "setting option");
            new CommandKeywords((new CmdSetRank()), "rank set user player");
            new CommandKeywords((new CmdSetspawn()), "spawn set map level lvl");
            new CommandKeywords((new CmdShutdown()), "shut down stop exit quit");
            new CommandKeywords((new CmdSlap()), "slam facepalm");
            new CommandKeywords((new CmdSpawn()), "level lvl map player user");
            new CommandKeywords((new CmdSpheroid()), "sphere");
            new CommandKeywords((new CmdSpin()), "rotate");
            new CommandKeywords((new CmdSPlace()), "measure place");
            new CommandKeywords((new CmdStatic()), "toggle mode");
            new CommandKeywords((new CmdSummon()), "move teleport tp player user");
            new CommandKeywords((new CmdTake()), "get money");
            new CommandKeywords((new CmdTColor()), "title color set");
            new CommandKeywords((new CmdTempBan()), "temp ban");
            new CommandKeywords((new CmdTempRank()), "temp rank");
            new CommandKeywords((new CmdTempRankInfo()), "temp rank info");
            new CommandKeywords((new CmdTempRankList()), "temp rank list");
            new CommandKeywords((new CmdText()), "write read view able");
            new CommandKeywords((new CmdTime()), "server");
            new CommandKeywords((new CmdTimer()), "count down");
            new CommandKeywords((new CmdTitle()), "set user");
            new CommandKeywords((new CmdTnt()), "c4 explo");
            new CommandKeywords((new CmdTntWars()), "tnt war c4 explo");
            new CommandKeywords((new CmdTopTen()), "top ten user");
            new CommandKeywords((new CmdTp()), "teleport move player user");
            new CommandKeywords((new CmdTpZone()), "tp zone teleport");
            new CommandKeywords((new CmdTree()), "log");
            new CommandKeywords((new CmdTrust()), "allow agree");
            new CommandKeywords((new CmdUBan()), "ban undo kick mod");
            new CommandKeywords((new CmdUnban()), "undo ban kick mod");
            new CommandKeywords((new CmdUnbanip()), "undo ban ip kick mod");
            new CommandKeywords((new CmdUndo()), "redo action block change");
            new CommandKeywords((new CmdUnflood()), "flood un restore");
            new CommandKeywords((new CmdUnload()), "load un map level lvl");
            new CommandKeywords((new CmdUnloaded()), "map level lvl list");
            new CommandKeywords((new CmdUnlock()), "lock un level lvl map");
            new CommandKeywords((new CmdView()), "file content player user");
            new CommandKeywords((new CmdViewRanks()), "show rank view user player");
            new CommandKeywords((new CmdVIP()), "list add remove del");
            new CommandKeywords((new CmdVoice()), "speak moderate");
            new CommandKeywords((new CmdVote()), "yes no ");
            new CommandKeywords((new CmdVoteKick()), "vote kick");
            new CommandKeywords((new CmdVoteResults()), "vote result");
            new CommandKeywords((new CmdWarn()), "kick user");
            new CommandKeywords((new CmdWarp()), "move teleport tp pos");
            new CommandKeywords((new CmdWaypoint()), "way point");
            new CommandKeywords((new CmdWhisper()), "tell private");
            new CommandKeywords((new CmdWhitelist()), "white list allow acces server");
            new CommandKeywords((new CmdWhoip()), "who ip info");
            new CommandKeywords((new CmdWhois()), "who player info");
            new CommandKeywords((new CmdWhowas()), "who player info");
            new CommandKeywords((new CmdWrite()), "block text");
            new CommandKeywords((new CmdWriteText()), "block text");
            new CommandKeywords((new CmdXban()), "ban undo admin");
            new CommandKeywords((new CmdXhide()), "hide all extra");
            new CommandKeywords((new CmdXJail()), "extra jail undo");
            new CommandKeywords((new CmdXmute()), "mute extra");
            new CommandKeywords((new CmdXspawn()), "extra spawn");
            new CommandKeywords((new CmdXundo()), "undo extra");
            new CommandKeywords((new CmdZombieGame()), "zombie game");
            new CommandKeywords((new CmdZone()), "area");
        }
        
        public void Addcustom(Command cmd, string keywords) {
            new CommandKeywords(cmd, keywords);
        }
        
        public static string[] Find(string word) {
            if (word == "") return null;
            List<string> list = new List<string>();

            foreach (CommandKeywords ckw in CommandKeywords.all)
                foreach (string key in ckw.Keywords)
            {
                if (key == "" || key.IndexOf(word, comp) < 0) continue;
                list.Add(ckw.Cmd.name); break;
            }
            return list.Count == 0 ? null : list.ToArray();
        }
        
        public static string[] Find(string[] words) {
            List<string> list = new List<string>();

            foreach (CommandKeywords ckw in CommandKeywords.all)
                foreach (string key in ckw.Keywords)
            {
                for (int i = 0; i < words.Length; i++) {
                    if (key == "" || key.IndexOf(words[i], comp) < 0) continue;
                    if (!list.Contains(ckw.Cmd.name)) list.Add(ckw.Cmd.name);
                    break;
                }
            }
            return list.Count == 0 ? null : list.ToArray();
        }
    }
}
