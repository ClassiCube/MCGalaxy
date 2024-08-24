using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCGalaxy {
    public class CinematicGui {
        public void HideHand(Player p, bool hidden) { hideHand = hidden; p.Session.SendCinematicGui(this); }
        public bool hideHand;

        public void HideHotbar(Player p, bool hidden) { hideHotbar = hidden; p.Session.SendCinematicGui(this); }
        public bool hideHotbar;

        public void SetBarColor(Player p, ColorDesc color) { barColor = color; p.Session.SendCinematicGui(this); }
        public ColorDesc barColor;

        /// <summary>
        /// From 0 to 1 where 0 is not visible and 1 is screen fully covered
        /// </summary>
        public void SetBarSize(Player p, float size) { barSize = size; p.Session.SendCinematicGui(this); }
        public float barSize;
    }
}
