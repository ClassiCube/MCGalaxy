/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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
using System.IO;
using System.Text;
using MCGalaxy.Commands;
using MCGalaxy.Network;

namespace MCGalaxy.Blocks {

    /// <summary> Represents which ranks are allowed (and which are disallowed) to use a block. </summary>
    public class BlockPerms {
        
        /// <summary> ID of block these permissions are for. </summary>
        public byte BlockID;
        
        /// <summary> Minimum rank normally able to use the block. </summary>
        public LevelPermission MinRank;
        
        /// <summary> Ranks specifically allowed to use the block </summary>
        public List<LevelPermission> Allowed = new List<LevelPermission>();
        
        /// <summary> Ranks specifically prevented from using the block. </summary>
        public List<LevelPermission> Disallowed = new List<LevelPermission>();
        
        /// <summary> Creates a copy of this instance. </summary>
        public BlockPerms Copy() {
            BlockPerms perms = new BlockPerms();
            perms.BlockID = BlockID;
            perms.MinRank = MinRank;
            perms.Allowed = new List<LevelPermission>(Allowed);
            perms.Disallowed = new List<LevelPermission>(Disallowed);
            return perms;
        }
        
        public static BlockPerms[] List = new BlockPerms[Block.Count];


        /// <summary> Returns whether the given rank can modify the given block. </summary>        
        public static bool CanModify(Player p, byte block) {
            BlockPerms b = List[block];
            LevelPermission perm = p.Rank;
            return (perm >= b.MinRank || b.Allowed.Contains(perm)) && !b.Disallowed.Contains(perm);
        }
        
        /// <summary> Returns whether the given rank can modify the given block. </summary>
        public static bool CanModify(LevelPermission perm, byte block) {
            BlockPerms b = List[block];
            return (perm >= b.MinRank || b.Allowed.Contains(perm)) && !b.Disallowed.Contains(perm);
        }
        
        /// <summary> Globally sends appropriate CPE block permission packets for after a block's permission is changed. </summary>
        public static void ResendBlockPermissions(byte block) {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                if (!pl.HasCpeExt(CpeExt.BlockPermissions)) continue;
                
                int count = pl.hasCustomBlocks ? Block.CpeCount : Block.OriginalCount;
                if (block < count) {
                    bool canAffect = CanModify(pl, block);
                    pl.Send(Packet.BlockPermission(block, 
                                                   canAffect && pl.level.CanPlace, 
                                                   canAffect && pl.level.CanDelete));
                }
            }
        }
                        
        public void MessageCannotUse(Player p, string action) {
            StringBuilder builder = new StringBuilder("Only ");
            Formatter.PrintRanks(MinRank, Allowed, Disallowed, builder);
            
            builder.Append( " %Scan ").Append(action).Append(' ');
            builder.Append(Block.Name(BlockID)).Append(".");
            Player.Message(p, builder.ToString());
        }
        
        
        static readonly object saveLock = new object();
        
        /// <summary> Saves the list of all block permissions. </summary>
        public static void Save() {
            try {
                lock (saveLock)
                    SaveCore(List);
            } catch (Exception e) {
                Logger.LogError(e);
            }
        }
        
        static void SaveCore(IEnumerable<BlockPerms> list) {
            using (StreamWriter w = new StreamWriter(Paths.BlockPermsFile)) {
                w.WriteLine("#Version 2");
                w.WriteLine("#   This file list the ranks that can use each command.");
                w.WriteLine("#   Disallow and allow can be left empty.");
                w.WriteLine("#   Works entirely on rank permission values, not rank names.");
                w.WriteLine("#");
                w.WriteLine("#   Layout: CommandName : MinRank : Disallow : Allow");
                w.WriteLine("#   lava : 60 : 80,67 : 40,41,55");
                w.WriteLine("");

                foreach (BlockPerms perms in list) {
                    if (Block.Name(perms.BlockID).CaselessEq("unknown")) continue;
                    
                    string line = Block.Name(perms.BlockID) + " : " + (int)perms.MinRank + " : "
                        + CommandPerms.JoinPerms(perms.Disallowed) + " : " + CommandPerms.JoinPerms(perms.Allowed);
                    w.WriteLine(line);
                }
            }
        }
        

        /// <summary> Loads the list of all block permissions. </summary>        
        public static void Load() {
            SetDefaultPerms();
            
            // Custom permissions set by the user.
            if (File.Exists(Paths.BlockPermsFile)) {
                string[] lines = File.ReadAllLines(Paths.BlockPermsFile);
                if (lines.Length > 0 && lines[0] == "#Version 2") {
                    LoadVersion2(lines);
                } else {
                    LoadVersion1(lines);
                }
            } else {
                Save();
            }
            
            foreach (Group grp in Group.GroupList) {
                grp.FillBlocks();
            }
        }
        
        static void LoadVersion2(string[] lines) {
            char[] colon = new char[] { ':' };
            foreach (string line in lines) {
                if (line == "" || line[0] == '#') continue;
                //Name : Lowest : Disallow : Allow
                string[] args = line.Replace(" ", "").Split(colon);
                
                BlockPerms perms = new BlockPerms();
                if (Block.Byte(args[0]) == Block.Invalid) continue;
                perms.BlockID = Block.Byte(args[0]);

                try {
                    perms.MinRank = (LevelPermission)int.Parse(args[1]);
                    string disallowRaw = args.Length > 2 ? args[2] : null;
                    string allowRaw = args.Length > 3 ? args[3] : null;
                    
                    perms.Allowed = CommandPerms.ExpandPerms(allowRaw);
                    perms.Disallowed = CommandPerms.ExpandPerms(disallowRaw);
                } catch {
                    Logger.Log(LogType.Warning, "Hit an error on the block " + line);
                    continue;
                }
                List[perms.BlockID] = perms;
            }
        }
        
        static void LoadVersion1(string[] lines) {
            foreach (string line in lines) {
                if (line == "" || line[0] == '#') continue;
                
                try {
                    byte block = Block.Byte(line.SplitSpaces()[0]);
                    Group group = Group.Find(line.SplitSpaces()[2]);
                    
                    if (group != null)
                        List[block].MinRank = group.Permission;
                    else
                        throw new InvalidDataException("Line " + line + " is invalid.");
                } catch { 
                    Logger.Log(LogType.Warning, "Could not find the rank given on {0}. Using default", line);
                }
            }
        }
        
        
        static void SetDefaultPerms() {
            for (int i = 0; i < Block.Count; i++) {
                BlockPerms perms = new BlockPerms();
                perms.BlockID = (byte)i;
                BlockProps props = Block.Props[i];
                
                if (i == Block.Invalid) {
                    perms.MinRank = LevelPermission.Admin;
                } else if (props.OPBlock) {
                    perms.MinRank = LevelPermission.Operator;
                } else if (props.IsDoor || props.IsTDoor || props.ODoorId != Block.Invalid) {
                    perms.MinRank = LevelPermission.Builder;
                } else if (props.IsPortal || props.IsMessageBlock) {
                    perms.MinRank = LevelPermission.AdvBuilder;
                } else {
                    perms.MinRank = DefaultPerm(i);
                }
                List[i] = perms;
            }
        }
        
        static LevelPermission DefaultPerm(int i) {
            switch (i)
            {
                case Block.blackrock:
                case Block.air_flood:
                case Block.air_flood_down:
                case Block.air_flood_layer:
                case Block.air_flood_up:

                case Block.bigtnt:
                case Block.nuketnt:
                case Block.rocketstart:
                case Block.rockethead:

                case Block.creeper:
                case Block.zombiebody:
                case Block.zombiehead:

                case Block.birdred:
                case Block.birdkill:
                case Block.birdblue:

                case Block.fishgold:
                case Block.fishsponge:
                case Block.fishshark:
                case Block.fishsalmon:
                case Block.fishbetta:
                case Block.fishlavashark:

                case Block.snake:
                case Block.snaketail:
                case Block.flagbase:
                    return LevelPermission.Operator;

                case Block.wood_float:
                case Block.lava_sponge:
                case Block.door_tree_air:
                case Block.door_green_air:
                case Block.door_tnt_air:

                case Block.water:
                case Block.lava:
                case Block.lava_fast:
                case Block.WaterDown:
                case Block.LavaDown:
                case Block.WaterFaucet:
                case Block.LavaFaucet:
                case Block.finiteWater:
                case Block.finiteLava:
                case Block.finiteFaucet:
                case Block.magma:
                case Block.geyser:
                case Block.deathlava:
                case Block.deathwater:
                case Block.deathair:
                case Block.activedeathwater:
                case Block.activedeathlava:
                case Block.fastdeathlava:
                case Block.lava_fire:

                case Block.c4:
                case Block.c4det:
                case Block.smalltnt:
                case Block.tntexplosion:
                case Block.firework:
                case Block.checkpoint:
                case Block.train:

                case Block.birdwhite:
                case Block.birdblack:
                case Block.birdwater:
                case Block.birdlava:
                    return LevelPermission.AdvBuilder;
            }
            return LevelPermission.Banned;
        }
    }
}
