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
using System.Threading;
using MCGalaxy.Maths;

namespace MCGalaxy {
    public abstract class Entity {
        
        // Raw orientation/position - access must be threadsafe
        volatile uint _rot;
        long _pos;
        
        // Last sent orientation/position, for delta calculation
        protected internal Orientation lastRot;
        protected internal Position lastPos;
        internal bool hasExtPositions;
        
        public string Model = "humanoid";
        public AABB ModelBB;
        public string SkinName;
        public float ScaleX, ScaleY, ScaleZ;

        public Orientation Rot {
            get { return Orientation.Unpack(_rot); }
            set { _rot = value.Pack(); OnSetRot(); }
        }
        
        public Position Pos {
            get { return Position.Unpack(Interlocked.Read(ref _pos)); }
            set { Interlocked.Exchange(ref _pos, value.Pack()); OnSetPos(); }
        }
        
        public void SetInitialPos(Position pos) {
            Pos = pos; lastPos = pos;
        }
        
        public void SetModel(string model) {
            Model   = model;
            ModelBB = ModelInfo.CalcAABB(this);
        }
        
        public void SetYawPitch(byte yaw, byte pitch) {
            Orientation rot = Rot;
            rot.RotY = yaw; rot.HeadX = pitch;
            Rot = rot;
        }

        public abstract bool CanSeeEntity(Entity other);
        public abstract byte EntityID { get; }
        public abstract Level Level { get; }
        public abstract bool RestrictsScale { get; }
        
        protected virtual void OnSetPos() { }
        
        protected virtual void OnSetRot() { }
        
    }
}
