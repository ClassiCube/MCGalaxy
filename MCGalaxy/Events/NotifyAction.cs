using System;
using BlockID = System.UInt16;

namespace MCGalaxy.Events.PlayerEvents {
    public enum NotifyActionType {
        BlockListSelected, BlockListToggled,
        LevelSaved,
        Respawned, SpawnUpdated,
        TexturePackChanged, TexturePackPromptResponse,
        ThirdPersonChanged,
        HotbarIndexChanged
    }

    public abstract class NotifyAction {
        public readonly NotifyActionType type;
        public readonly object value;
        protected NotifyAction(NotifyActionType type, object value) {
            this.type = type;
            this.value = value;
        }

        internal static NotifyAction Create(NotifyActionType type, object value) {

            NotifyAction action;
            switch (type) {
                case NotifyActionType.BlockListSelected:
                    action = new NotifyActionBlockListSelected(type, Block.FromRaw((BlockID)value));
                    break;
                case NotifyActionType.BlockListToggled:
                    action = new NotifyActionBlockListToggled(type, value);
                    break;
                case NotifyActionType.LevelSaved:
                    action = new NotifyActionLevelSaved(type, value);
                    break;
                case NotifyActionType.Respawned:
                    action = new NotifyActionRespawned(type, value);
                    break;
                case NotifyActionType.SpawnUpdated:
                    action = new NotifyActionSpawnUpdated(type, value);
                    break;
                case NotifyActionType.TexturePackChanged:
                    action = new NotifyActionTexturePackChanged(type, value);
                    break;
                case NotifyActionType.TexturePackPromptResponse:
                    action = new NotifyActionTexturePackPromptResponse(type, value);
                    break;
                case NotifyActionType.ThirdPersonChanged:
                    action = new NotifyActionThirdPersonChanged(type, value);
                    break;
                case NotifyActionType.HotbarIndexChanged:
                    action = new NotifyActionHotbarIndexChanged(type, value);
                    break;
                default:
                    throw new System.ArgumentException("Unhandled NotifyAction type \""+ type + "\"");
            }
            return action;
        }
    }
    public class NotifyActionBlockListSelected : NotifyAction {
        public BlockID selectedBlock { get { return (BlockID)value; } }
        public NotifyActionBlockListSelected(NotifyActionType type, object value) : base(type, value) { }
    }
    public class NotifyActionBlockListToggled : NotifyAction {
        public bool OpenedBlockMenu { get { return (ushort)value > 0 ? true : false; } }
        public NotifyActionBlockListToggled(NotifyActionType type, object value) : base(type, value) { }
    }
    public class NotifyActionLevelSaved : NotifyAction {
        public NotifyActionLevelSaved(NotifyActionType type, object value) : base(type, value) { }
    }


    public class NotifyActionRespawned : NotifyAction {
        public Position pos { get { return (Position)value; } }
        public NotifyActionRespawned(NotifyActionType type, object value) : base(type, value) { }
    }
    public class NotifyActionSpawnUpdated : NotifyAction {
        public Position pos { get { return (Position)value; } }
        public NotifyActionSpawnUpdated(NotifyActionType type, object value) : base(type, value) { }
    }
    public class NotifyActionTexturePackChanged : NotifyAction {
        public enum ChangedValue : ushort {
            Failure = 0,
            FromServer = 1,
            FromUser = 2,
        };
        public ChangedValue changedValue { get { return (ChangedValue)value; } }
        public NotifyActionTexturePackChanged(NotifyActionType type, object value) : base(type, value) { }
    }
    public class NotifyActionTexturePackPromptResponse : NotifyAction {
        public enum ResponseValue : ushort {
            Opened = 0,
            Accepted = 1,
            Denied = 2
        };
        public ResponseValue responseValue { get { return (ResponseValue)value; } }
        public NotifyActionTexturePackPromptResponse(NotifyActionType type, object value) : base(type, value) { }
    }
    public class NotifyActionThirdPersonChanged : NotifyAction {
        public enum PointOfView : ushort {
            FirstPerson = 0,
            ThirdPerson = 1,
            FrontFacingThirdPerson = 2,
        };
        public PointOfView pointOfView { get { return (PointOfView)value; } }
        public NotifyActionThirdPersonChanged(NotifyActionType type, object value) : base(type, value) { }
    }
    public class NotifyActionHotbarIndexChanged : NotifyAction {
        public int hotbarNumber { get { return (ushort)value >> 8; } } //hotbar number is stored in the most significant byte
        public int hotbarSlot { get { return (ushort)value & 0xFF; } } //hotbar slot (index) is stored in the least significant byte
        public NotifyActionHotbarIndexChanged(NotifyActionType type, object value) : base(type, value) { }
    }
}
