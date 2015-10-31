using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MCGalaxy
{
    public class API
    {
        public string[] players { get; set; }
        public int max_players { get; set; }
        public ChatMessage[] chat { get; set; }
    }
}
