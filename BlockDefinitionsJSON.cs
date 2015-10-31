using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace MCGalaxy
{
    public class BlockDefinitionsJSON
    {
        public static List<BlockDefinitions> Load(string path)
        {
            List<BlockDefinitions> bd = new List<BlockDefinitions>();
            var json = "";
            try {
                if (File.Exists(path))
                    json = File.ReadAllText(path);
                bd = JsonConvert.DeserializeObject<List<BlockDefinitions>>(json);
            }
            catch { }
            return bd;
        }
        public static void Write(List<BlockDefinitions> bd, string path)
        {
            string json = "";
            json = JsonConvert.SerializeObject(bd);
            File.WriteAllText(path, json);
        }
    }
}
