/*
	Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)

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
using System.Collections.Generic;
using MCGalaxy.Commands;
namespace MCGalaxy
{
	public abstract class Command
	{
		public abstract string name { get; }
		public abstract string shortcut { get; }
		public abstract string type { get; }
		public abstract bool museumUsable { get; }
		public abstract LevelPermission defaultRank { get; }
		public abstract void Use(Player p, string message);
		public abstract void Help(Player p);
		public bool isIntervalized;
		public int intervalInMinutes;
		public DateTime nextExecution;
		public Player intervalUsingPlayer;

		public static CommandList all = new CommandList();
		public static CommandList core = new CommandList();
		public static void InitAll()
		{
			all.Add(new CmdAbort());
			all.Add(new CmdAbout());
            // all.Add(new CmdAddGlobalBlock());
            all.Add(new CmdAdminChat());
			all.Add(new CmdAllowGuns());
			all.Add(new CmdAfk());
			all.Add(new CmdAka());
			all.Add(new CmdAlive());
			all.Add(new CmdAgree());
			all.Add(new CmdAscend());
			all.Add(new CmdAward());
			all.Add(new CmdAwards());
			all.Add(new CmdAwardMod());
            all.Add(new CmdBack());
			all.Add(new CmdBan());
			all.Add(new CmdBanEdit());
			all.Add(new CmdBaninfo());
			all.Add(new CmdBanip());
			all.Add(new CmdBanlist());
			all.Add(new CmdBind());
			all.Add(new CmdBlocks());
			all.Add(new CmdBlockSet());
			all.Add(new CmdBlockSpeed());
			all.Add(new CmdBotAdd());
			all.Add(new CmdBotAI());
			all.Add(new CmdBotRemove());
			all.Add(new CmdBots());
			all.Add(new CmdBotSet());
			all.Add(new CmdBotSummon());
			all.Add(new CmdC4());
			all.Add(new CmdCenter());
			all.Add(new CmdChain());
			all.Add(new CmdChangeLog());
			all.Add(new CmdChatRoom());
			all.Add(new CmdClearBlockChanges());
			all.Add(new CmdClick());
			all.Add(new CmdClones());
			all.Add(new CmdCmdBind());
			all.Add(new CmdCmdCreate());
			all.Add(new CmdCmdLoad());
			all.Add(new CmdCmdSet());
			all.Add(new CmdCmdUnload());
			all.Add(new CmdCompile());
			all.Add(new CmdCompLoad());
			all.Add(new CmdColor());
			all.Add(new CmdCopy());
			all.Add(new CmdCopyLVL());
			all.Add(new CmdCountdown());
			all.Add(new CmdCrashServer());
			all.Add(new CmdCTF());
			all.Add(new CmdCuboid());
			all.Add(new CmdDelete());
			all.Add(new CmdDeleteLvl());
			all.Add(new CmdDelTempRank());
			all.Add(new CmdDemote());
			all.Add(new CmdDevs());
			all.Add(new CmdDisagree());
			all.Add(new CmdDescend());
			all.Add(new CmdDisInfect());
			all.Add(new CmdDraw());
			all.Add(new CmdDrill());
			all.Add(new CmdEconomy());
			all.Add(new CmdEmote());
			all.Add(new CmdEndRound());
            all.Add(new CmdEnvironment());
			all.Add(new CmdExplode());
			all.Add(new CmdFakePay());
			all.Add(new CmdFakeRank());
			all.Add(new CmdFaq());
			all.Add(new CmdFetch());
			all.Add(new CmdFill());
			all.Add(new CmdFixGrass());
			all.Add(new CmdFlipHead());
			all.Add(new CmdFlipHeads());
			all.Add(new CmdFly());
			all.Add(new CmdFollow());
			all.Add(new CmdFreeze());
			all.Add(new CmdGarbage());
			all.Add(new CmdGcaccept());
            all.Add(new CmdGcmods());
			all.Add(new CmdGcrules());
			//all.Add(new CmdGcbanlistupdate());
			all.Add(new CmdGive());
			all.Add(new CmdGlobal());
			all.Add(new CmdGlobalCLS());
			all.Add(new CmdGoto());
			all.Add(new CmdGun());
			all.Add(new CmdHackRank());
			all.Add(new CmdHacks());
			all.Add(new CmdHasirc());
			//all.Add(new CmdHeartbeat()); // DEBUG COMMAND DO NOT USE
			all.Add(new CmdHelp());
			all.Add(new CmdHide());
			all.Add(new CmdHigh5());
			all.Add(new CmdHighlight());
			all.Add(new CmdHollow());
			all.Add(new CmdHost());
			all.Add(new CmdHug());
			all.Add(new CmdIgnore());
			all.Add(new CmdImpersonate());
			all.Add(new CmdImport());
			all.Add(new CmdImageprint());
			all.Add(new CmdInbox());
			all.Add(new CmdInfect());
			all.Add(new CmdInfected());
			all.Add(new CmdInfo());
			all.Add(new CmdInvincible());
			all.Add(new CmdJail());
			all.Add(new CmdJoker());
			all.Add(new CmdKick());
			all.Add(new CmdKickban());
			all.Add(new CmdKill());
			all.Add(new CmdKillPhysics());
			all.Add(new CmdLastCmd());
			all.Add(new CmdLavaSurvival());
			all.Add(new CmdLevels());
			all.Add(new CmdLimit());
			all.Add(new CmdLine());
			all.Add(new CmdLoad());
			all.Add(new CmdLocation());
			all.Add(new CmdLockdown());
			all.Add(new CmdLoginMessage());
			all.Add(new CmdLogoutMessage());
			all.Add(new CmdLowlag());
			all.Add(new CmdMain());
			all.Add(new CmdMap());
			all.Add(new CmdMapInfo());
			all.Add(new CmdMark());
			all.Add(new CmdMaze());
			all.Add(new CmdMe());
			all.Add(new CmdMeasure());
			all.Add(new CmdMegaboid());
			all.Add(new CmdMessageBlock());
			all.Add(new CmdMissile());
			all.Add(new CmdMode());
            all.Add(new CmdModel());
            all.Add(new CmdMods());
			all.Add(new CmdModerate());
			all.Add(new CmdMoney());
			all.Add(new CmdMove());
			all.Add(new CmdMoveAll());
			all.Add(new CmdMuseum());
			all.Add(new CmdMute());
			all.Add(new CmdNewLvl());
			all.Add(new CmdNews());
			all.Add (new CmdNick ());
			all.Add(new CmdOHide());
			all.Add(new CmdOpChat());
			all.Add(new CmdOpRules());
			all.Add(new CmdOpStats());
			all.Add(new CmdOutline());
			all.Add(new CmdOverseer());
			all.Add(new CmdOZone());
			all.Add(new CmdP2P());
			all.Add(new CmdPaint());
			all.Add(new CmdPass());
			all.Add(new CmdPaste());
			all.Add(new CmdPatrol());
			all.Add(new CmdPause());
			all.Add(new CmdPay());
			all.Add(new CmdPlayerBlock());
			all.Add(new CmdpCinema());
			all.Add(new CmdpCinema2());
			all.Add(new CmdPCount());
			all.Add(new CmdPCreate());
			all.Add(new CmdPerbuildMax());
			all.Add(new CmdPermissionBuild());
			all.Add(new CmdPermissionVisit());
			all.Add(new CmdPervisitMax());
			all.Add(new CmdPhysics());
			all.Add(new CmdPlace());
			all.Add(new CmdPlayerCLS());
			all.Add(new CmdPlayerEditDB());
			all.Add(new CmdPlayers());
			all.Add(new CmdPLoad());
			all.Add(new CmdPortal());
			all.Add(new CmdPossess());
			all.Add(new CmdPromote());
			all.Add(new CmdPUnload());
			all.Add(new CmdPyramid());
			all.Add(new CmdQueue());
			all.Add(new CmdQuick());
			all.Add(new CmdRagequit());
			all.Add(new CmdRainbow());
			all.Add(new CmdRankInfo());
			all.Add(new CmdRankMsg()); 
			all.Add(new CmdRanks());
			all.Add(new CmdRedo());
			all.Add(new CmdReload());
			all.Add(new CmdReferee());
			all.Add(new CmdRenameLvl());
			all.Add(new CmdRepeat());
			all.Add(new CmdReplace());
			all.Add(new CmdReplaceAll());
			all.Add(new CmdReplaceNot());
			all.Add(new CmdReport());
			all.Add(new CmdResetBot());
			all.Add(new CmdResetPass());
			all.Add(new CmdRestart());
			all.Add(new CmdRestartPhysics());
			all.Add(new CmdRestore());
			all.Add(new CmdRestoreSelection());
			all.Add(new CmdReveal());
			all.Add(new CmdReview());
			all.Add(new CmdRide());
			all.Add(new CmdRoll());
			all.Add(new CmdRules());
			all.Add(new CmdSave());
			all.Add(new CmdSay());
			all.Add(new CmdSCinema());
			all.Add(new CmdSearch());
			all.Add(new CmdSeen());
			all.Add(new CmdSend());
			all.Add(new CmdSendCmd());
			all.Add(new CmdServerReport());
			all.Add(new CmdServer());
			all.Add(new CmdSetPass());
			all.Add(new CmdSetRank());
			all.Add(new CmdSetspawn());
			all.Add(new CmdShutdown());
			all.Add(new CmdSlap());
			all.Add(new CmdSpawn());
			all.Add(new CmdSpheroid());
			all.Add(new CmdSpin());
			all.Add(new CmdSPlace());
            all.Add(new CmdStaff());
			all.Add(new CmdStairs());
			all.Add(new CmdStatic());
			all.Add(new CmdSummon());
			all.Add(new CmdTake());
			all.Add(new CmdTColor());
			all.Add(new CmdTempBan());
			all.Add(new CmdTempRank());
			all.Add(new CmdTempRankInfo());
			all.Add(new CmdTempRankList());
			all.Add(new CmdText());
            all.Add(new CmdTexture());
			all.Add(new CmdTime());
			all.Add(new CmdTimer());
			all.Add(new CmdTitle());
			all.Add(new CmdTnt());
			all.Add(new CmdTntWars());
			all.Add(new CmdTop());
            all.Add(new CmdTopFive());
			all.Add(new CmdTopTen());
			all.Add(new CmdTp());
			all.Add(new CmdTpA());
			all.Add(new CmdTpAccept());
			all.Add(new CmdTpDeny());
			all.Add(new CmdTpZone());
			all.Add(new CmdTranslate());
			all.Add(new CmdTree());
			all.Add(new CmdTrust());
			all.Add(new CmdUBan());
			all.Add(new CmdUnban());
			all.Add(new CmdUnbanip());
			all.Add(new CmdUndo());
			all.Add(new CmdUnflood());
			all.Add(new CmdUnload());
			all.Add(new CmdUnloaded());
			all.Add(new CmdUnlock());
			all.Add(new CmdUpdate());
			all.Add(new CmdView());
			all.Add(new CmdViewRanks());
			all.Add(new CmdVIP());
			all.Add(new CmdVoice());
			all.Add(new CmdVote());
			all.Add(new CmdVoteKick());
			all.Add(new CmdVoteResults());
			all.Add(new CmdWarn());
			all.Add(new CmdWarp());
			all.Add(new CmdWaypoint());
			all.Add(new CmdWhisper());
			all.Add(new CmdWhitelist());
			all.Add(new CmdWhoip());
			all.Add(new CmdWhois());
            		all.Add(new CmdWhoNick());
			all.Add(new CmdWhowas());
			all.Add(new CmdWrite());
			all.Add(new CmdXban());
			all.Add(new CmdXColor());
			all.Add(new CmdXhide());
			all.Add(new CmdXJail());
			all.Add(new CmdXModel());
			all.Add(new CmdXmute());
			all.Add(new CmdXNick());
			all.Add(new CmdXspawn());
			all.Add(new CmdXTColor());
			all.Add(new CmdXTitle());
			all.Add(new CmdXundo());
			all.Add(new CmdZombieGame());
			all.Add(new CmdZone());
			all.Add(new CmdZz());
            		all.Add(new CmdQuit());
			core.commands = new List<Command>(all.commands);
			Scripting.Autoload();
		}
		/// <summary>
		/// Add a command to the server
		/// </summary>
		/// <param name="command">The command to add</param>
		public void AddCommand(Command command)
		{
			all.Add(command);
		}
		
		protected static void RevertBlockState(Player p, ushort x, ushort y, ushort z) {
			p.ClearBlockchange();
			byte b = p.level.GetTile(x, y, z);
			p.SendBlockchange(x, y, z, b);
		}
	}
	
	public sealed class CommandTypes {
		public const string Building = "build";
		public const string Chat = "chat";
		public const string Economy = "economy";
		public const string Games = "game";
		public const string Information = "information";
		public const string Moderation = "mod";
		public const string Other = "other";
		public const string World = "world";
	}
}
