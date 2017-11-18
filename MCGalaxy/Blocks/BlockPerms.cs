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
        public static bool UsableBy(Player p, byte block) {
            BlockPerms b = List[block];
            LevelPermission perm = p.Rank;
            return (perm >= b.MinRank || b.Allowed.Contains(perm)) && !b.Disallowed.Contains(perm);
        }
        
        /// <summary> Returns whether the given rank can modify the given block. </summary>
        public static bool UsableBy(LevelPermission perm, byte block) {
            BlockPerms b = List[block];
            return (perm >= b.MinRank || b.Allowed.Contains(perm)) && !b.Disallowed.Contains(perm);
        }
        
        public static void ResendAllBlockPermissions() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) { pl.SendCurrentBlockPermissions(); }
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
                w.WriteLine("#   This file list the ranks that can use each block");
                w.WriteLine("#   Disallow and allow can be left empty.");
                w.WriteLine("#   Works entirely on rank permission values, not rank names.");
                w.WriteLine("#");
                w.WriteLine("#   Layout: Block ID : MinRank : Disallow : Allow");
                w.WriteLine("#   lava : 60 : 80,67 : 40,41,55");
                w.WriteLine("");

                foreach (BlockPerms perms in list) {
                    if (Block.Name(perms.BlockID).CaselessEq("unknown")) continue;
                    
                    string line = perms.BlockID + " : " + (int)perms.MinRank + " : "
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
                ProcessLines(lines);
            } else {
                Save();
            }
            
            foreach (Group grp in Group.GroupList) {
                grp.SetUsableBlocks();
            }
        }
        
        static void ProcessLines(string[] lines) {
            string[] args = new string[4];
            foreach (string line in lines) {
                if (line.Length == 0 || line[0] == '#') continue;
                // Format is - Name/ID : Lowest : Disallow : Allow
                line.Replace(" ", "").FixedSplit(args, ':');
                
                byte block;
                if (!byte.TryParse(args[0], out block)) {
                    block = Block.Byte(args[0]);
                }
                if (block == Block.Invalid) continue;
                
                BlockPerms perms = new BlockPerms();
                perms.BlockID = block;
                try {
                    perms.MinRank = (LevelPermission)int.Parse(args[1]);
                    string disallowRaw = args[2], allowRaw = args[3];
                    
                    perms.Allowed = CommandPerms.ExpandPerms(allowRaw);
                    perms.Disallowed = CommandPerms.ExpandPerms(disallowRaw);
                } catch {
                    Logger.Log(LogType.Warning, "Hit an error on the block " + line);
                    continue;
                }
                List[perms.BlockID] = perms;
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
                } else if (props.IsDoor || props.IsTDoor || props.oDoorIndex != Block.Invalid) {
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
                case Block.Bedrock:
                case Block.Air_Flood:
                case Block.Air_FloodDown:
                case Block.Air_FloodLayer:
                case Block.Air_FloodUp:

                case Block.TNT_Big:
                case Block.TNT_Nuke:
                case Block.RocketStart:
                case Block.RocketHead:

                case Block.Creeper:
                case Block.ZombieBody:
                case Block.ZombieHead:

                case Block.Bird_Red:
                case Block.Bird_Killer:
                case Block.Bird_Blue:

                case Block.Fish_Gold:
                case Block.Fish_Sponge:
                case Block.Fish_Shark:
                case Block.Fish_Salmon:
                case Block.Fish_Betta:
                case Block.Fish_LavaShark:

                case Block.Snake:
                case Block.SnakeTail:
                case Block.FlagBase:
                    return LevelPermission.Operator;

                case Block.FloatWood:
                case Block.LavaSponge:
                case Block.Door_Log_air:
                case Block.Door_Green_air:
                case Block.Door_TNT_air:

                case Block.Water:
                case Block.Lava:
                case Block.FastLava:
                case Block.WaterDown:
                case Block.LavaDown:
                case Block.WaterFaucet:
                case Block.LavaFaucet:
                case Block.FiniteWater:
                case Block.FiniteLava:
                case Block.FiniteFaucet:
                case Block.Magma:
                case Block.Geyser:
                case Block.Deadly_Lava:
                case Block.Deadly_Water:
                case Block.Deadly_Air:
                case Block.Deadly_ActiveWater:
                case Block.Deadly_ActiveLava:
                case Block.Deadly_FastLava:
                case Block.LavaFire:

                case Block.C4:
                case Block.C4Detonator:
                case Block.TNT_Small:
                case Block.TNT_Explosion:
                case Block.Fireworks:
                case Block.Checkpoint:
                case Block.Train:

                case Block.Bird_White:
                case Block.Bird_Black:
                case Block.Bird_Water:
                case Block.Bird_Lava:
                    return LevelPermission.AdvBuilder;
            }
            return LevelPermission.Guest;
        }
    }
}
