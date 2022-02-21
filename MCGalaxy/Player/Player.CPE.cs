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
using MCGalaxy.Network;
using BlockID = System.UInt16;

namespace MCGalaxy 
{
    public partial class Player 
    {
        public bool hasCpe;
        public string appName;
        CpeExt[] extensions = CpeExtension.Empty;
        
        internal CpeExt FindExtension(string extName) {
            foreach (CpeExt ext in extensions) 
            {
                if (ext.Name.CaselessEq(extName)) return ext;
            }
            return null;
        }
        
        // these are checked very frequently, so avoid overhead of .Supports(
        public bool hasCustomBlocks, hasBlockDefs, hasTextColors, hasExtBlocks,
        hasChangeModel, hasExtList, hasCP437, hasBulkBlockUpdate;

        /// <summary> Whether this player's client supports the given CPE extension at the given version </summary>
        public bool Supports(string extName, int version = 1) {
            if (!hasCpe) return false;
            CpeExt ext = FindExtension(extName);
            return ext != null && ext.ClientVersion == version;
        }
        
        public string GetTextureUrl() {
            string url = level.Config.TexturePack.Length == 0 ? level.Config.Terrain : level.Config.TexturePack;
            if (url.Length == 0) {
                url = Server.Config.DefaultTexture.Length == 0 ? Server.Config.DefaultTerrain : Server.Config.DefaultTexture;
            }
            return url;
        }
        
        
        string lastUrl = "";
        public void SendCurrentTextures() {
            Zone zone = ZoneIn;
            int cloudsHeight = CurrentEnvProp(EnvProp.CloudsLevel, zone);
            int edgeHeight   = CurrentEnvProp(EnvProp.EdgeLevel,   zone);
            int maxFogDist   = CurrentEnvProp(EnvProp.MaxFog,      zone);
            
            byte side = (byte)CurrentEnvProp(EnvProp.SidesBlock, zone);
            byte edge = (byte)CurrentEnvProp(EnvProp.EdgeBlock,  zone);

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

        [Obsolete("Use Session.SendSetEnvColor instead", true)]
        public void SendEnvColor(byte type, string hex) {
            Session.SendSetEnvColor(type, hex);
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