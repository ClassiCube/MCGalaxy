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
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy 
{
    public partial class Player 
    {
        public bool hasCpe, finishedCpeLogin;
        public string appName;
        int extensionCount;
        
        public CpeExtension[] Extensions = new CpeExtension[] {
            new CpeExtension(CpeExt.ClickDistance),    new CpeExtension(CpeExt.CustomBlocks),
            new CpeExtension(CpeExt.HeldBlock),        new CpeExtension(CpeExt.TextHotkey),
            new CpeExtension(CpeExt.ExtPlayerList, 2), new CpeExtension(CpeExt.EnvColors),
            new CpeExtension(CpeExt.SelectionCuboid),  new CpeExtension(CpeExt.BlockPermissions),
            new CpeExtension(CpeExt.ChangeModel),      new CpeExtension(CpeExt.EnvMapAppearance, 2),
            new CpeExtension(CpeExt.EnvWeatherType),   new CpeExtension(CpeExt.HackControl),
            new CpeExtension(CpeExt.EmoteFix),         new CpeExtension(CpeExt.MessageTypes),
            new CpeExtension(CpeExt.LongerMessages),   new CpeExtension(CpeExt.FullCP437),
            new CpeExtension(CpeExt.BlockDefinitions), new CpeExtension(CpeExt.BlockDefinitionsExt, 2),
            new CpeExtension(CpeExt.TextColors),       new CpeExtension(CpeExt.BulkBlockUpdate),
            new CpeExtension(CpeExt.EnvMapAspect),     new CpeExtension(CpeExt.PlayerClick),
            new CpeExtension(CpeExt.EntityProperty),   new CpeExtension(CpeExt.ExtEntityPositions),
            new CpeExtension(CpeExt.TwoWayPing),       new CpeExtension(CpeExt.InventoryOrder),
            new CpeExtension(CpeExt.InstantMOTD),      new CpeExtension(CpeExt.FastMap),
            new CpeExtension(CpeExt.ExtTextures),      new CpeExtension(CpeExt.SetHotbar),
            new CpeExtension(CpeExt.SetSpawnpoint),    new CpeExtension(CpeExt.VelocityControl),
            new CpeExtension(CpeExt.CustomParticles),  new CpeExtension(CpeExt.CustomModels, 2),
            #if TEN_BIT_BLOCKS
            new CpeExtension(CpeExt.ExtBlocks),
            #endif
        };
        
        CpeExtension FindExtension(string extName) {
            foreach (CpeExtension ext in Extensions) {
                if (ext.Name.CaselessEq(extName)) return ext;
            }
            return null;
        }
        
        // these are checked very frequently, so avoid overhead of .Supports(
        public bool hasCustomBlocks, hasBlockDefs, hasTextColors, hasExtBlocks, hasEmoteFix,
        hasChangeModel, hasExtList, hasCP437, hasTwoWayPing, hasBulkBlockUpdate, hasExtTexs;

        /// <summary> Whether this player's client supports the given CPE extension at the given version </summary>
        public bool Supports(string extName, int version = 1) {
            if (!hasCpe) return false;
            CpeExtension ext = FindExtension(extName);
            return ext != null && ext.ClientVersion == version;
        }
        
        public string GetTextureUrl() {
            string url = level.Config.TexturePack.Length == 0 ? level.Config.Terrain : level.Config.TexturePack;
            if (url.Length == 0) {
                url = Server.Config.DefaultTexture.Length == 0 ? Server.Config.DefaultTerrain : Server.Config.DefaultTexture;
            }
            return url;
        }
        
        
        int HandleExtInfo(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 2;
            if (left < size) return 0;
            
            appName        = NetUtils.ReadString(buffer, offset + 1);
            extensionCount = buffer[offset + 66];
            CheckReadAllExtensions(); // in case client supports 0 CPE packets
            return size;
        }

        int HandleExtEntry(byte[] buffer, int offset, int left) {
            const int size = 1 + 64 + 4;
            if (left < size) return 0;
            
            string extName = NetUtils.ReadString(buffer, offset + 1);
            int extVersion = NetUtils.ReadI32(buffer,    offset + 65);
            AddExtension(extName, extVersion);
            extensionCount--;
            CheckReadAllExtensions();
            return size;
        }
        
        void AddExtension(string extName, int version) {
            CpeExtension ext = FindExtension(extName.Trim());
            if (ext == null) return;
            ext.ClientVersion = (byte)version;
            
            if (ext.Name == CpeExt.CustomBlocks) {
                if (version == 1) Send(Packet.CustomBlockSupportLevel(1));
                hasCustomBlocks = true;
                
                UpdateFallbackTable();
                if (MaxRawBlock < Block.CPE_MAX_BLOCK) MaxRawBlock = Block.CPE_MAX_BLOCK;
            } else if (ext.Name == CpeExt.ChangeModel) {
                hasChangeModel = true;
            } else if (ext.Name == CpeExt.EmoteFix) {
                hasEmoteFix = true;
            } else if (ext.Name == CpeExt.FullCP437) {
                hasCP437 = true;
            } else if (ext.Name == CpeExt.ExtPlayerList) {
                hasExtList = true;
            } else if (ext.Name == CpeExt.BlockDefinitions) {
                hasBlockDefs = true;
                if (MaxRawBlock < 255) MaxRawBlock = 255;
            } else if (ext.Name == CpeExt.TextColors) {
                hasTextColors = true;
                for (int i = 0; i < Colors.List.Length; i++) {
                    if (!Colors.List[i].IsModified()) continue;
                    Send(Packet.SetTextColor(Colors.List[i]));
                }
            } else if (ext.Name == CpeExt.ExtEntityPositions) {
                hasExtPositions = true;
            } else if (ext.Name == CpeExt.TwoWayPing) {
                hasTwoWayPing = true;
            } else if (ext.Name == CpeExt.BulkBlockUpdate) {
                hasBulkBlockUpdate = true;
            } else if (ext.Name == CpeExt.ExtTextures) {
                hasExtTexs = true;
            }
            #if TEN_BIT_BLOCKS
            else if (ext.Name == CpeExt.ExtBlocks) {
                hasExtBlocks = true;
                if (MaxRawBlock < 767) MaxRawBlock = 767;
            }
            #endif
        }
        
        void CheckReadAllExtensions() {
            if (extensionCount <= 0 && !finishedCpeLogin) {
                CompleteLoginProcess();
                finishedCpeLogin = true;
            }
        }
        
        
        string lastUrl = "";
        public void SendCurrentTextures() {
            Zone zone = ZoneIn;
            int cloudsHeight = CurrentEnvProp(EnvProp.CloudsLevel, zone);
            int edgeHeight   = CurrentEnvProp(EnvProp.EdgeLevel,   zone);
            int maxFogDist   = CurrentEnvProp(EnvProp.MaxFog,      zone);
            
            int sideRaw  = CurrentEnvProp(EnvProp.SidesBlock, zone);
            int edgeRaw  = CurrentEnvProp(EnvProp.EdgeBlock,  zone);
            byte side    = (byte)ConvertBlock((BlockID)sideRaw);
            byte edge    = (byte)ConvertBlock((BlockID)edgeRaw);

            string url = GetTextureUrl();
            if (Supports(CpeExt.EnvMapAspect)) {
                // reset all other textures back to client default.
                if (url != lastUrl) Send(Packet.EnvMapUrl("", hasCP437));
                Send(Packet.EnvMapUrl(url, hasCP437));
            } else if (Supports(CpeExt.EnvMapAppearance, 2)) {
                // reset all other textures back to client default.
                if (url != lastUrl) {
                    Send(Packet.MapAppearanceV2("", side, edge, edgeHeight, cloudsHeight, maxFogDist, hasCP437));
                }
                Send(Packet.MapAppearanceV2(url, side, edge, edgeHeight, cloudsHeight, maxFogDist, hasCP437));
                lastUrl = url;
            } else if (Supports(CpeExt.EnvMapAppearance)) {
                url = level.Config.Terrain.Length == 0 ? Server.Config.DefaultTerrain : level.Config.Terrain;
                Send(Packet.MapAppearance(url, side, edge, edgeHeight, hasCP437));
            }
        }

        public void SendEnvColor(byte type, string hex) {
            ColorDesc c;
            if (Colors.TryParseHex(hex, out c)) {
                Send(Packet.EnvColor(type, c.R, c.G, c.B));
            } else {
                Send(Packet.EnvColor(type, -1, -1, -1));
            }
        }
        
        public void SendCurrentBlockPermissions() {
            if (!Supports(CpeExt.BlockPermissions)) return;
            // Write the block permissions as one bulk TCP packet
            SendAllBlockPermissions();
        }
        
        void SendAllBlockPermissions() {
            int count = MaxRawBlock + 1;
            int size = hasExtBlocks ? 5 : 4;
            byte[] bulk = new byte[count * size];
            
            for (int i = 0; i < count; i++) {
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
}