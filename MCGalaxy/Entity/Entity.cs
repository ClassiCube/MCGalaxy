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

namespace MCGalaxy {
    
    /// <summary> Represents a player or an NPC. </summary>
    public abstract class Entity {
        
        // Raw orientation/position - NOT threadsafe
        protected Orientation _rot;
        protected Position _pos;
        
        // Last sent orientation/position, for delta calculation
        protected internal Orientation lastRot;
        protected internal Position lastPos;
        // TODO: struct assignment needs to be THREADSAFE
        
        
        /// <summary> Model name of this entity. </summary>
        public string Model = "humanoid";
        
        /// <summary> AABB of the model of this entity. </summary>
        public AABB ModelBB;
        
        /// <summary> Skin name of this entity. </summary>
        public string SkinName;
        
        
        /// <summary> Gets or sets the orientation of this entity. </summary>
        public Orientation Rot {
            get { return _rot; }
            set { _rot = value; OnSetRot(); }
        }
        
        /// <summary> Gets or sets the position of this entity. </summary>
        public Position Pos {
            get { return _pos; }
            set { _pos = value; OnSetPos(); }
        }
        
        /// <summary> Sets only the yaw and pitch of the orientation of this entity. </summary>
        public void SetYawPitch(byte yaw, byte pitch) {
            Orientation rot = Rot;
            rot.RotY = yaw; rot.HeadX = pitch;
            Rot = rot;
        }
        
        
        /// <summary> Returns whether this entity can see the given entity in the world/level. </summary>
        public abstract bool CanSeeEntity(Entity other);
        
        /// <summary> Gets the entity/player ID of this entity. </summary>
        public abstract byte EntityID { get; }
        
        /// <summary> Gets the world/level this entity is on. </summary>
        public abstract Level Level { get; }
        
        protected virtual void OnSetPos() { }
        
        protected virtual void OnSetRot() { }
        
    }
}
