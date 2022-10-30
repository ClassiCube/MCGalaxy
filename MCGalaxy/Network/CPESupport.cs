/*
    Copyright 2015 MCGalaxy
        
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
using MCGalaxy.Network;

namespace MCGalaxy 
{
    public enum CpeMessageType : byte 
    {
        Normal = 0, Status1 = 1, Status2 = 2, Status3 = 3,
        BottomRight1 = 11, BottomRight2 = 12, BottomRight3 = 13,
        Announcement = 100, BigAnnouncement = 101, SmallAnnouncement = 102 
    }
    
    public enum EnvProp : byte 
    {
        SidesBlock = 0, EdgeBlock = 1, EdgeLevel = 2,
        CloudsLevel = 3, MaxFog = 4, CloudsSpeed = 5,
        WeatherSpeed = 6, WeatherFade = 7, ExpFog = 8,
        SidesOffset = 9, SkyboxHorSpeed = 10, SkyboxVerSpeed = 11,
        
        Max,
        Weather = 255, // this is internal, not an official env prop
    }
    
    public enum EntityProp : byte 
    {
        RotX = 0, RotY = 1, RotZ = 2, ScaleX = 3, ScaleY = 4, ScaleZ = 5,
    }
    
    
    public class CpeExt 
    {
        /// <summary> Name of the CPE extension (e.g. ExtPlayerList) </summary>
        public string Name;
        /// <summary> Highest version of this CPE extension supported by the server </summary>
        public byte ServerVersion;
        /// <summary> Highest version of this CPE extension supported by the client </summary>
        public byte ClientVersion;
        
        public const string ClickDistance = "ClickDistance";
        public const string CustomBlocks = "CustomBlocks";
        public const string HeldBlock = "HeldBlock";
        public const string TextHotkey = "TextHotKey";
        public const string ExtPlayerList = "ExtPlayerList";
        public const string EnvColors = "EnvColors";
        public const string SelectionCuboid = "SelectionCuboid";
        public const string BlockPermissions = "BlockPermissions";
        public const string ChangeModel = "ChangeModel";
        public const string EnvMapAppearance = "EnvMapAppearance";
        public const string EnvWeatherType = "EnvWeatherType";
        public const string HackControl = "HackControl";
        public const string EmoteFix = "EmoteFix";
        public const string MessageTypes = "MessageTypes";
        public const string LongerMessages = "LongerMessages";
        public const string FullCP437 = "FullCP437";
        public const string BlockDefinitions = "BlockDefinitions";
        public const string BlockDefinitionsExt = "BlockDefinitionsExt";
        public const string TextColors = "TextColors";
        public const string BulkBlockUpdate = "BulkBlockUpdate";
        public const string EnvMapAspect = "EnvMapAspect";
        public const string PlayerClick = "PlayerClick";
        public const string EntityProperty = "EntityProperty";
        public const string ExtEntityPositions = "ExtEntityPositions";
        public const string TwoWayPing = "TwoWayPing";
        public const string InventoryOrder = "InventoryOrder";
        public const string InstantMOTD = "InstantMOTD";
        public const string FastMap = "FastMap";
        public const string ExtBlocks = "ExtendedBlocks";
        public const string ExtTextures = "ExtendedTextures";
        public const string SetHotbar = "SetHotbar";
        public const string SetSpawnpoint = "SetSpawnpoint";
        public const string VelocityControl = "VelocityControl";
        public const string CustomParticles = "CustomParticles";
        public const string CustomModels = "CustomModels";
        public const string PluginMessages = "PluginMessages";
        public const string ExtEntityTeleport = "ExtEntityTeleport";
    }
    
    public sealed class CpeExtension 
    {
        /// <summary> Name of the CPE extension (e.g. ExtPlayerList) </summary>
        public string Name;
        /// <summary> Short description of this CPE extension </summary>
        public string Desc;
        /// <summary> Highest version of this CPE extension supported by the server </summary>
        public byte Version;
        /// <summary> Whether this CPE extension is currently enabled by the server </summary>
        public bool Enabled = true;
        
        public CpeExtension(string name, string desc) {
            Name = name; Desc = desc; Version = 1;
        }
        public CpeExtension(string name, string desc, byte version) {
            Name = name; Desc = desc; Version = version;
        }
        
        
        /// <summary> Array of all supported CPE extensions </summary>
        public static CpeExtension[] All = new CpeExtension[] {
            new CpeExtension(CpeExt.ClickDistance,       "Allows controlling how far away blocks can be placed/deleted (/Reach)"),    
            new CpeExtension(CpeExt.CustomBlocks,        "Allows showing blocks 50 - 65 (Cobblestone Slab - Stone Brick)"),
            new CpeExtension(CpeExt.HeldBlock,           "Allows setting currently held block/block in hand"),
            new CpeExtension(CpeExt.TextHotkey,          "Allows defining custom hotkeys"),
            new CpeExtension(CpeExt.ExtPlayerList,       "Allows separating tab list from entities in current map", 2), 
            new CpeExtension(CpeExt.EnvColors,           "Allows customing environment colors such as sky color (/Env)"),
            new CpeExtension(CpeExt.SelectionCuboid,     "Allows showing colored boxes in the map (/Zone set"),
            new CpeExtension(CpeExt.BlockPermissions,    "Allows controlling which individual blocks can be placed/deleted"),
            new CpeExtension(CpeExt.ChangeModel,         "Allows changing models of entities (/Model)"),      
            new CpeExtension(CpeExt.EnvMapAppearance,    "Allows customising map appearance (/Texture)", 2),
            new CpeExtension(CpeExt.EnvWeatherType,      "Allows customising weather (/Env)"),
            new CpeExtension(CpeExt.HackControl,         "Allows changing hacks permissions (e.g. speed) on the fly"), 
            new CpeExtension(CpeExt.EmoteFix,            "Allows clients to properly render emotes"),         
            new CpeExtension(CpeExt.MessageTypes,        "Allows messages to appear in special locations (e.g. top right)"),
            new CpeExtension(CpeExt.LongerMessages,      "Allows typing multiline chat in clients"),   
            new CpeExtension(CpeExt.FullCP437,           "Allows using all code page 437 characters in chat"),
            new CpeExtension(CpeExt.BlockDefinitions,    "Allows defining custom blocks (/LB and /GB)"),
            new CpeExtension(CpeExt.BlockDefinitionsExt, "Allows defining custom blocks with more flexible shapes (/LB and /GB)", 2),
            new CpeExtension(CpeExt.TextColors,          "Allows customising chat color codes (/CustomColors)"),   
            new CpeExtension(CpeExt.BulkBlockUpdate,     "Allows sending block updates in a faster way"),
            new CpeExtension(CpeExt.EnvMapAspect,        "Allows customising map appearance (/Env and /Texture)"),
            new CpeExtension(CpeExt.PlayerClick,         "Allows retrieving precise details on mouse clicks"),
            new CpeExtension(CpeExt.EntityProperty,      "Allows customising properties of entities (/EntityRot)"),   
            new CpeExtension(CpeExt.ExtEntityPositions,  "Allows entities to appear at positions past 1023"),
            new CpeExtension(CpeExt.TwoWayPing,          "Allows measuring ping (/Ping)"),       
            new CpeExtension(CpeExt.InventoryOrder,      "Allows configuring order of blocks in Inventory menu"),
            new CpeExtension(CpeExt.InstantMOTD,         "Allows sending MOTD packets without also needing to resend map"),      
            new CpeExtension(CpeExt.FastMap,             "Allows sending maps in a faster way"),
            new CpeExtension(CpeExt.ExtTextures,         "Allows using texture IDs over 255 in block definitions"),
            new CpeExtension(CpeExt.SetHotbar,           "Allows setting blocks in hotbar (the bar with 9 blocks)"),
            new CpeExtension(CpeExt.SetSpawnpoint,       "Allows changing spawn point of players without teleporting them"),    
            new CpeExtension(CpeExt.VelocityControl,     "Allows adjusting velocity of players"),
            new CpeExtension(CpeExt.CustomParticles,     "Allows defining and spawning custom particles"),
            new CpeExtension(CpeExt.CustomModels,        "Allows defining custom models for entities", 2),
            new CpeExtension(CpeExt.PluginMessages,      "Allows sending and receiving plugin messages from clients"),
            new CpeExtension(CpeExt.ExtEntityTeleport,   "Allows sending more precisely controlled teleports"),
            #if TEN_BIT_BLOCKS
            new CpeExtension(CpeExt.ExtBlocks,           "Allows using block IDs over 255 in block definitions"),
            #endif
        };
        internal static CpeExt[] Empty = new CpeExt[0];
        
        /// <summary> Retrieves a list of all supported and enabled CPE extensions </summary>
        public static CpeExt[] GetAllEnabled() {
            if (!Server.Config.EnableCPE) return Empty;
            CpeExtension[] all = All;
            List<CpeExt> exts  = new List<CpeExt>(all.Length);
            
            for (int i = 0; i < all.Length; i++)
            {
                CpeExtension e = all[i];
                if (!e.Enabled) continue;
                
                exts.Add(new CpeExt() { Name = e.Name, ServerVersion = e.Version });
            }
            return exts.ToArray();
        }
        
        
        static int supportedCount;
        public static void LoadDisabledList() {
            supportedCount = 0;

            foreach (CpeExtension e in All) { e.Enabled = true; }
            PropertiesFile.Read(Paths.CPEDisabledFile, ParseLine, '=');
            
            // file is out of sync with actual list
            if (supportedCount == All.Length) return;
            try {
                SaveDisabledList();
            } catch (Exception ex) {
                Logger.LogError("Error saving CPE disabled list", ex);
            }
        }

        static void ParseLine(string name, string value) {
            foreach (CpeExtension c in All) 
            {
                if (!name.CaselessEq(c.Name)) continue;
                
                c.Enabled = bool.Parse(value);
                supportedCount++;
                return;
            }
        }

        static void SaveDisabledList() {
            using (StreamWriter w = new StreamWriter(Paths.CPEDisabledFile)) {
                w.WriteLine("# CPE configuration");
                w.WriteLine("#   This file allows disabling non-classic (CPE) extensions ");
                w.WriteLine("#   To disable an extension, just change '= True' to '= False'");
                w.WriteLine("#   You do not normally need to edit this file - all non-classic functionality can be simply disabled by setting 'enable-cpe' in server.properties to 'False'");
                w.WriteLine();

                foreach (CpeExtension c in All) 
                {
                    w.WriteLine("#  " + c.Desc);
                    w.WriteLine(c.Name + " = " + c.Enabled);
                }
            }
        }
    }
}