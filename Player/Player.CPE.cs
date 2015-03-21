
using System;
namespace MCGalaxy
{
    public partial class Player
    {
        public int ClickDistance = 0;
        public int CustomBlocks = 0;
        public int HeldBlock = 0;
        public int TextHotKey = 0;
        public int ExtPlayerList = 0;
        public int EnvColors = 0;
        public int SelectionCuboid = 0;
        public int BlockPermissions = 0;
        public int ChangeModel = 0;
        public int EnvMapAppearance = 0;
        public int EnvWeatherType = 0;
        public int HackControl = 0;
        public int EmoteFix = 0;
        public int MessageTypes = 0;

        public void AddExtension(string Extension, int version)
        {
            lock (this)
            {
                switch (Extension.Trim())
                {
                    case "ClickDistance":
                        ClickDistance = version;
                        break;
                    case "CustomBlocks":
                        CustomBlocks = version;
                        if (version == 1)
                            SendCustomBlockSupportLevel(1);
                        break;
                    case "HeldBlock":
                        HeldBlock = version;
                        break;
                    case "TextHotKey":
                        TextHotKey = version;
                        break;
                    case "ExtPlayerList":
                        ExtPlayerList = version;
                        break;
                    case "EnvColors":
                        EnvColors = version;
                        break;
                    case "SelectionCuboid":
                        SelectionCuboid = version;
                        break;
                    case "BlockPermissions":
                        BlockPermissions = version;
                        break;
                    case "ChangeModel":
                        ChangeModel = version;
                        break;
                    case "EnvMapAppearance":
                        EnvMapAppearance = version;
                        break;
                    case "EnvWeatherType":
                        EnvWeatherType = version;
                        break;
                    case "HackControl":
                        HackControl = version;
                        break;
                    case "EmoteFix":
                        EmoteFix = version;
                        break;
                    case "MessageTypes":
                        MessageTypes = version;
                        break;
                }
            }
        }

        public bool HasExtension(string Extension, int version = 1)
        {
            if (!extension) return false;
            switch (Extension)
            {
                case "ClickDistance": return ClickDistance == version;
                case "CustomBlocks": return CustomBlocks == version;
                case "HeldBlock": return HeldBlock == version;
                case "TextHotKey": return TextHotKey == version;
                case "ExtPlayerList": return ExtPlayerList == version;
                case "EnvColors": return EnvColors == version;
                case "SelectionCuboid": return SelectionCuboid == version;
                case "BlockPermissions": return BlockPermissions == version;
                case "ChangeModel": return ChangeModel == version;
                case "EnvMapAppearance": return EnvMapAppearance == version;
                case "EnvWeatherType": return EnvWeatherType == version;
                case "HackControl": return HackControl == version;
                case "EmoteFix": return EmoteFix == version;
                case "MessageTypes": return MessageTypes == version;
                default: return false;
            }
        }
    }
}