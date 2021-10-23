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
using System.IO;
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy 
{
    public sealed class CpeExtension 
    {
        public string Name;
        public bool Enabled;
        public byte ClientVersion, ServerVersion = 1;
        
        public CpeExtension(string name, bool enabled) { Name = name; Enabled = enabled; }
        public CpeExtension(string name, byte version, bool enabled) 
        {
            Name = name; ServerVersion = version; Enabled = enabled;
        }
    }
    
    public partial class Player 
    {
        public bool hasCpe, finishedCpeLogin;
        public string appName;
        int extensionCount;
        
        public static CpeExtension[] Extensions = new CpeExtension[] 
        {
            new CpeExtension(CpeExt.ClickDistance, true),    new CpeExtension(CpeExt.CustomBlocks, true),
            new CpeExtension(CpeExt.HeldBlock, true),        new CpeExtension(CpeExt.TextHotkey, true),
            new CpeExtension(CpeExt.ExtPlayerList, 2, true), new CpeExtension(CpeExt.EnvColors, true),
            new CpeExtension(CpeExt.SelectionCuboid, true),  new CpeExtension(CpeExt.BlockPermissions, true),
            new CpeExtension(CpeExt.ChangeModel, true),      new CpeExtension(CpeExt.EnvMapAppearance, 2, true),
            new CpeExtension(CpeExt.EnvWeatherType, true),   new CpeExtension(CpeExt.HackControl, true),
            new CpeExtension(CpeExt.EmoteFix, true),         new CpeExtension(CpeExt.MessageTypes, true),
            new CpeExtension(CpeExt.LongerMessages, true),   new CpeExtension(CpeExt.FullCP437, true),
            new CpeExtension(CpeExt.BlockDefinitions, true), new CpeExtension(CpeExt.BlockDefinitionsExt, 2, true),
            new CpeExtension(CpeExt.TextColors, true),       new CpeExtension(CpeExt.BulkBlockUpdate, true),
            new CpeExtension(CpeExt.EnvMapAspect, true),     new CpeExtension(CpeExt.PlayerClick, true),
            new CpeExtension(CpeExt.EntityProperty, true),   new CpeExtension(CpeExt.ExtEntityPositions, true),
            new CpeExtension(CpeExt.TwoWayPing, true),       new CpeExtension(CpeExt.InventoryOrder, true),
            new CpeExtension(CpeExt.InstantMOTD, true),      new CpeExtension(CpeExt.FastMap, true),
            new CpeExtension(CpeExt.ExtTextures, true),      new CpeExtension(CpeExt.SetHotbar, true),
            new CpeExtension(CpeExt.SetSpawnpoint, true),    new CpeExtension(CpeExt.VelocityControl, true),
            new CpeExtension(CpeExt.CustomParticles, true),  new CpeExtension(CpeExt.CustomModels, 2, true),
            #if TEN_BIT_BLOCKS
            new CpeExtension(CpeExt.ExtBlocks, true),
            #endif
        };
        
        CpeExtension FindExtension(string extName) 
        {
            foreach (CpeExtension ext in Extensions) 
            {
                Logger.Log(LogType.Debug, ext.Name + " " + ext.Enabled);
                if (ext.Name.CaselessEq(extName)) if (ext.Enabled) return ext;
            }

            return null;
        }
        
        // these are checked very frequently, so avoid overhead of .Supports(
        public bool hasCustomBlocks, hasBlockDefs, hasTextColors, hasExtBlocks, hasEmoteFix,
        hasChangeModel, hasExtList, hasCP437, hasTwoWayPing, hasBulkBlockUpdate, hasExtTexs;

        /// <summary> Whether this player's client supports the given CPE extension at the given version </summary>
        public bool Supports(string extName, int version = 1) 
        {
            if (!hasCpe) return false;
            CpeExtension ext = FindExtension(extName);
            return ext != null && ext.ClientVersion == version;
        }
        
        public string GetTextureUrl() 
        {
            string url = level.Config.TexturePack.Length == 0 ? level.Config.Terrain : level.Config.TexturePack;
            if (url.Length == 0) {
                url = Server.Config.DefaultTexture.Length == 0 ? Server.Config.DefaultTerrain : Server.Config.DefaultTexture;
            }
            return url;
        }
        
        
        int HandleExtInfo(byte[] buffer, int offset, int left) 
        {
            const int size = 1 + 64 + 2;
            if (left < size) return 0;
            
            appName        = NetUtils.ReadString(buffer, offset + 1);
            extensionCount = buffer[offset + 66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
            return size;
        }

        int HandleExtEntry(byte[] buffer, int offset, int left) 
        {
            const int size = 1 + 64 + 4;
            if (left < size) return 0;
            
            string extName = NetUtils.ReadString(buffer, offset + 1);
            int extVersion = NetUtils.ReadI32(buffer,    offset + 65);
            AddExtension(extName, extVersion);
            extensionCount--;
            CheckReadAllExtensions();
            return size;
        }
        
        void AddExtension(string extName, int version) 
        {
            CpeExtension ext = FindExtension(extName.Trim());
            if (ext == null) return;
            ext.ClientVersion = (byte)version;
            
            if (ext.Name == CpeExt.CustomBlocks) 
            {
                if (version == 1) Send(Packet.CustomBlockSupportLevel(1));
                hasCustomBlocks = true;
                
                UpdateFallbackTable();
                if (MaxRawBlock < Block.CPE_MAX_BLOCK) MaxRawBlock = Block.CPE_MAX_BLOCK;
            } 
            else if (ext.Name == CpeExt.ChangeModel) 
            {
                hasChangeModel = true;
            } 
            else if (ext.Name == CpeExt.EmoteFix) 
            {
                hasEmoteFix = true;
            } 
            else if (ext.Name == CpeExt.FullCP437) 
            {
                hasCP437 = true;
            } 
            else if (ext.Name == CpeExt.ExtPlayerList) 
            {
                hasExtList = true;
            } 
            else if (ext.Name == CpeExt.BlockDefinitions) 
            {
                hasBlockDefs = true;
                if (MaxRawBlock < 255) MaxRawBlock = 255;
            } 
            else if (ext.Name == CpeExt.TextColors) 
            {
                hasTextColors = true;
                for (int i = 0; i < Colors.List.Length; i++) 
                {
                    if (!Colors.List[i].IsModified()) continue;
                    Send(Packet.SetTextColor(Colors.List[i]));
                }
            } 
            else if (ext.Name == CpeExt.ExtEntityPositions) 
            {
                hasExtPositions = true;
            } 
            else if (ext.Name == CpeExt.TwoWayPing) 
            {
                hasTwoWayPing = true;
            } 
            else if (ext.Name == CpeExt.BulkBlockUpdate) 
            {
                hasBulkBlockUpdate = true;
            } 
            else if (ext.Name == CpeExt.ExtTextures) 
            {
                hasExtTexs = true;
            }
            #if TEN_BIT_BLOCKS
            else if (ext.Name == CpeExt.ExtBlocks) 
            {
                hasExtBlocks = true;
                if (MaxRawBlock < 767) MaxRawBlock = 767;
            }
            #endif
        }
        
        void CheckReadAllExtensions() 
        {
            if (extensionCount <= 0 && !finishedCpeLogin) 
            {
                CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }
        
        
        string lastUrl = "";
        public void SendCurrentTextures() 
        {
            Zone zone = ZoneIn;
            int cloudsHeight = CurrentEnvProp(EnvProp.CloudsLevel, zone);
            int edgeHeight   = CurrentEnvProp(EnvProp.EdgeLevel,   zone);
            int maxFogDist   = CurrentEnvProp(EnvProp.MaxFog,      zone);
            
            int sideRaw  = CurrentEnvProp(EnvProp.SidesBlock, zone);
            int edgeRaw  = CurrentEnvProp(EnvProp.EdgeBlock,  zone);
            byte side    = (byte)ConvertBlock((BlockID)sideRaw);
            byte edge    = (byte)ConvertBlock((BlockID)edgeRaw);

            string url = GetTextureUrl();
            if (Supports(CpeExt.EnvMapAspect)) 
            {
                // reset all other textures back to client default.
                if (url != lastUrl) Send(Packet.EnvMapUrl("", hasCP437));
                Send(Packet.EnvMapUrl(url, hasCP437));
            } 
            else if (Supports(CpeExt.EnvMapAppearance, 2)) 
            {
                // reset all other textures back to client default.
                if (url != lastUrl) 
                {
                    Send(Packet.MapAppearanceV2("", side, edge, edgeHeight, cloudsHeight, maxFogDist, hasCP437));
                }
                Send(Packet.MapAppearanceV2(url, side, edge, edgeHeight, cloudsHeight, maxFogDist, hasCP437));
                lastUrl = url;
            } 
            else if (Supports(CpeExt.EnvMapAppearance)) 
            {
                url = level.Config.Terrain.Length == 0 ? Server.Config.DefaultTerrain : level.Config.Terrain;
                Send(Packet.MapAppearance(url, side, edge, edgeHeight, hasCP437));
            }
        }

        public void SendEnvColor(byte type, string hex) 
        {
            ColorDesc c;
            if (Colors.TryParseHex(hex, out c)) 
            {
                Send(Packet.EnvColor(type, c.R, c.G, c.B));
            } 
            else 
            {
                Send(Packet.EnvColor(type, -1, -1, -1));
            }
        }
        
        public void SendCurrentBlockPermissions() 
        {
            if (!Supports(CpeExt.BlockPermissions)) return;
            // Write the block permissions as one bulk TCP packet
            SendAllBlockPermissions();
        }
        
        void SendAllBlockPermissions() 
        {
            int count = MaxRawBlock + 1;
            int size = hasExtBlocks ? 5 : 4;
            byte[] bulk = new byte[count * size];
            
            for (int i = 0; i < count; i++) 
            {
                BlockID block = Block.FromRaw((BlockID)i);
                bool place  = group.Blocks[block] && level.CanPlace;
                // NOTE: If you can't delete air, then you're no longer able to place blocks
                // (see ClassiCube client #815)
                // TODO: Maybe better solution than this?
                bool delete = group.Blocks[block] && (level.CanDelete || i == Block.Air);
                
                // Placing air is the same as deleting existing block at that position in the world
                if (block == Block.Air) place &= delete;
                Packet.WriteBlockPermission((BlockID)i, place, delete, hasExtBlocks, bulk, i * size);
            }
            Send(bulk);
        }
    }
    
    public static class CpeExt 
    {
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
    }
    
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
    
    public sealed class CPEConfig 
    {
        public static void Load() {
            if (!File.Exists(Paths.CPEFile)) {
                SaveConfig();
            }
            LoadConfig();
        }

        static void LoadConfig() {
            CpeExtension cur = null;
            PropertiesFile.Read(Paths.CPEFile, ref cur, ParseProperty, '=', true);
        }

        static void ParseProperty(string key, string value, ref CpeExtension cur) {
            int i = 0;
            foreach (CpeExtension c in Player.Extensions) 
            {
                if (key.CaselessEq(c.Name)) {
                    Player.Extensions[i] = new CpeExtension(c.Name, bool.Parse(value));
                }
                i++;
            }
        }

        static void SaveConfig() {
            using (StreamWriter w = new StreamWriter(Paths.CPEFile)) {
                Logger.Log(LogType.Debug, "test");
                w.WriteLine("# CPE configuration");
                w.WriteLine("#   Here you can configure settings for CPE (Classic Protocol Extension).");
                w.WriteLine("#   You shouldn't touch any settings here unless you know what you are doing.");
                w.WriteLine();
                
                foreach (CpeExtension c in Player.Extensions) 
                {
                    w.WriteLine(c.Name + " = " + c.Enabled);
                }
            }
        }
    }
}