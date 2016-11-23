using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCGalaxy
{
    public enum MouseButton
    {
        Left = 0,
        Right = 1,
        Middle = 2
    }
    
    public enum MouseAction
    {
        Pressed = 0,
        Released = 1
    }

    public enum TargetBlockFace
    {
        AwayX = 0,
        TowardsX = 1,
        AwayY = 2,
        TowardsY = 3,
        AwayZ = 4,
        TowardsZ = 5,
        None = 6
    }
}
