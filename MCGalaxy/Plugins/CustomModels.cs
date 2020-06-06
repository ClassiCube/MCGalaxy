//reference Newtonsoft.Json.dll

using MCGalaxy.Commands;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MCGalaxy {
    public sealed class CustomModelsPlugin : Plugin {
        public override string name => "CustomModels";
        public override string MCGalaxy_Version => "1.9.2.0";
        public override string creator => "SpiralP & Goodly";

        //------------------------------------------------------------------bbmodel/ccmodel file loading

        public static Dictionary<string, CustomModel> CustomModels = new Dictionary<string, CustomModel>();
        const string blockBenchExt = ".bbmodel";
        const string storedModelExt = ".ccmodel";
        const string BBdirectory = "plugins/models/bbmodels/";
        const string CCdirectory = "plugins/models/";

        // don't serialize "name" because we will use filename for model name
        // don't serialize "parts" because we store those in the full .bbmodel file
        // don't serialize "u/vScale" because we take it from bbmodel's resolution.width
        struct StoredCustomModel {
            public float nameY;
            public float eyeY;
            public Vec3F32 collisionBounds;
            public AABBF32 pickingBoundsAABB;
            public bool bobbing;
            public bool pushes;
            public bool usesHumanSkin;
            public bool calcHumanAnims;
            public bool hideFirstPersonArm;
        }

        static void WriteCCModelFile(string modelName, CustomModel model) {
            // convert to pixel units
            var storedCustomModel = new StoredCustomModel {
                nameY = model.nameY * 16.0f,
                eyeY = model.eyeY * 16.0f,
                collisionBounds = {
                    X = model.collisionBounds.X * 16.0f,
                    Y = model.collisionBounds.Y * 16.0f,
                    Z = model.collisionBounds.Z * 16.0f,
                },
                pickingBoundsAABB = {
                    Min = new Vec3F32 {
                        X = model.pickingBoundsAABB.Min.X * 16.0f,
                        Y = model.pickingBoundsAABB.Min.Y * 16.0f,
                        Z = model.pickingBoundsAABB.Min.Z * 16.0f,
                    },
                    Max = new Vec3F32 {
                        X = model.pickingBoundsAABB.Max.X * 16.0f,
                        Y = model.pickingBoundsAABB.Max.Y * 16.0f,
                        Z = model.pickingBoundsAABB.Max.Z * 16.0f,
                    },
                },
                bobbing = model.bobbing,
                pushes = model.pushes,
                usesHumanSkin = model.usesHumanSkin,
                calcHumanAnims = model.calcHumanAnims,
                hideFirstPersonArm = model.hideFirstPersonArm,
            };

            var storedJsonModel = JsonConvert.SerializeObject(storedCustomModel, Formatting.Indented, jsonSettings);
            File.WriteAllText(CCdirectory + modelName + storedModelExt, storedJsonModel);
        }

        static void CreateCCmodelFromBBmodel() {
            if (!Directory.Exists(CCdirectory)) Directory.CreateDirectory(BBdirectory);
            DirectoryInfo info = new DirectoryInfo(BBdirectory);
            FileInfo[] filesInBBdir = info.GetFiles();

            for (int i = 0; i < filesInBBdir.Length; i++) {
                string fileName = filesInBBdir[i].Name;
                string modelName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);

                if (!extension.CaselessEq(blockBenchExt)) { continue; }
                if (File.Exists(CCdirectory + modelName + storedModelExt)) { continue; }

                WriteCCModelFile(modelName, new CustomModel { });

                Logger.Log(
                    LogType.SystemActivity,
                    "CustomModels: Created a new default template \"{0}\" in {1}.",
                    modelName + storedModelExt,
                    CCdirectory
                );
            }
        }

        static CustomModel ReadCCModelFile(string name) {
            string contentsCC = File.ReadAllText(CCdirectory + name + storedModelExt);
            string contentsBB = File.ReadAllText(BBdirectory + name + blockBenchExt);

            StoredCustomModel storedCustomModel = JsonConvert.DeserializeObject<StoredCustomModel>(contentsCC);
            var blockBench = BlockBench.Parse(contentsBB);
            var parts = blockBench.ToCustomModelParts();

            // convert to block units
            var model = new CustomModel {
                name = name,
                parts = parts,
                uScale = blockBench.resolution.width,
                vScale = blockBench.resolution.height,

                nameY = storedCustomModel.nameY / 16.0f,
                eyeY = storedCustomModel.eyeY / 16.0f,
                collisionBounds = new Vec3F32 {
                    X = storedCustomModel.collisionBounds.X / 16.0f,
                    Y = storedCustomModel.collisionBounds.Y / 16.0f,
                    Z = storedCustomModel.collisionBounds.Z / 16.0f,
                },
                pickingBoundsAABB = new AABBF32 {
                    Min = new Vec3F32 {
                        X = storedCustomModel.pickingBoundsAABB.Min.X / 16.0f,
                        Y = storedCustomModel.pickingBoundsAABB.Min.Y / 16.0f,
                        Z = storedCustomModel.pickingBoundsAABB.Min.Z / 16.0f,
                    },
                    Max = new Vec3F32 {
                        X = storedCustomModel.pickingBoundsAABB.Max.X / 16.0f,
                        Y = storedCustomModel.pickingBoundsAABB.Max.Y / 16.0f,
                        Z = storedCustomModel.pickingBoundsAABB.Max.Z / 16.0f,
                    },
                },
                bobbing = storedCustomModel.bobbing,
                pushes = storedCustomModel.pushes,
                usesHumanSkin = storedCustomModel.usesHumanSkin,
                calcHumanAnims = storedCustomModel.calcHumanAnims,
                hideFirstPersonArm = storedCustomModel.hideFirstPersonArm,
            };

            return model;
        }

        static void LoadModels() {
            if (!Directory.Exists(CCdirectory)) Directory.CreateDirectory(CCdirectory);
            DirectoryInfo info = new DirectoryInfo(CCdirectory);
            FileInfo[] filesInModels = info.GetFiles();

            for (int i = 0; i < filesInModels.Length; i++) {
                string fileName = filesInModels[i].Name;
                string modelName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);

                if (!extension.CaselessEq(storedModelExt)) { continue; }
                if (!File.Exists(BBdirectory + modelName + blockBenchExt)) { continue; }

                CustomModel model = ReadCCModelFile(modelName);
                CustomModels[modelName] = model;

                Logger.Log(
                    LogType.SystemActivity,
                    "CustomModels: Loaded model {0}.",
                    modelName
                );
            }
        }

        static void DefineModel(Player pl, CustomModel model) {
            if (!pl.Supports(CpeExt.CustomModels)) { return; }
            byte[] modelPacket = Packet.DefineModel(model);
            pl.Send(modelPacket, true);

            // tell the client to update these entities who are currently
            // using the same model
            foreach (Player e in PlayerInfo.Online.Items) {
                if (e.Model == model.name) {
                    Entities.UpdateModel(pl, e, model.name);
                }
            }

            foreach (PlayerBot e in pl.level.Bots.Items) {
                if (e.Model == model.name) {
                    Entities.UpdateModel(pl, e, model.name);
                }
            }
        }

        static void DefineModelForAllPlayers(CustomModel model) {
            foreach (Player p in PlayerInfo.Online.Items) {
                DefineModel(p, model);
            }
        }

        static void DefineModels(Player pl) {
            if (!pl.Supports(CpeExt.CustomModels)) { return; }
            foreach (KeyValuePair<string, CustomModel> entry in CustomModels) {
                var model = entry.Value;
                DefineModel(pl, model);
                pl.Message("Defined model %b{0}%S!", entry.Key);
            }
        }
        static void DefineModelsForAllPlayers() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player pl in players) {
                DefineModels(pl);
            }
        }

        //------------------------------------------------------------------plugin interface

        CmdCustomModel command = null;
        public override void Load(bool startup) {
            command = new CmdCustomModel();
            Command.Register(command);
            //Logger.Log(LogType.Warning, "loading god");
            OnJoinedLevelEvent.Register(OnJoinedLevel, Priority.Low);

            CreateCCmodelFromBBmodel();
            LoadModels();

            DefineModelsForAllPlayers();
        }

        class ChatType {
            public Func<CustomModel, string> get;
            // (model, pl, input) => bool
            public Func<CustomModel, Player, string[], bool> set;
            public string[] types;

            public ChatType(string type, Func<CustomModel, string> get, Func<CustomModel, Player, string, bool> set) {
                this.types = new string[] { type };
                this.get = get;
                this.set = (model, pl, inputs) => {
                    return set(model, pl, inputs[0]);
                };
            }

            public ChatType(
                string[] types,
                Func<CustomModel, string> get,
                Func<CustomModel, Player, string[], bool> set
            ) {
                this.types = types;
                this.get = get;
                this.set = set;
            }
        }

        public override void Unload(bool shutdown) {
            OnJoinedLevelEvent.Unregister(OnJoinedLevel);
            if (command != null) {
                Command.Unregister(command);
                command = null;
            }
        }

        static void OnJoinedLevel(Player pl, Level prevLevel, Level level, ref bool announce) {
            DefineModels(pl);
        }

        //------------------------------------------------------------------commands

        static bool GetRealPixels(Player pl, string input, string argName, ref float output) {
            float tmp = 0.0f;
            if (CommandParser.GetReal(pl, input, "nameY", ref tmp)) {
                output = tmp * 16.0f;
                return true;
            } else {
                return false;
            }
        }

        static Dictionary<string, ChatType> modifiableFields = new Dictionary<string, ChatType> {
            {
                "nameY",
                new ChatType(
                    "nameY",
                    (model) => "" + model.nameY * 16.0f,
                    (model, pl, input) => GetRealPixels(pl, input, "nameY", ref model.nameY)
                )
            },
            {
                "eyeY",
                new ChatType(
                    "eyeY",
                    (model) => "" + model.eyeY * 16.0f,
                    (model, pl, input) => GetRealPixels(pl, input, "eyeY", ref model.eyeY)
                )
            },
            {
                "collisionBounds",
                new ChatType(
                    new string[] {"x", "y", "z"},
                    (model) => {
                        return string.Format(
                            "({0}, {1}, {2})",
                            model.collisionBounds.X * 16.0f,
                            model.collisionBounds.Y * 16.0f,
                            model.collisionBounds.Z * 16.0f
                        );
                    },
                    (model, pl, input) => {
                        if (!GetRealPixels(pl, input[0], "x", ref model.collisionBounds.X)) return false;
                        if (!GetRealPixels(pl, input[1], "y", ref model.collisionBounds.Y)) return false;
                        if (!GetRealPixels(pl, input[2], "z", ref model.collisionBounds.Z)) return false;
                        return true;
                    }
                )
            },
            {
                "pickingBoundsAABB",
                new ChatType(
                    new string[] {"minX", "minY", "minZ", "maxX", "maxY", "maxZ"},
                    (model) => {
                        return string.Format(
                            "from ({0}, {1}, {2}) to ({3}, {4}, {5})",
                            model.pickingBoundsAABB.Min.X * 16.0f,
                            model.pickingBoundsAABB.Min.Y * 16.0f,
                            model.pickingBoundsAABB.Min.Z * 16.0f,
                            model.pickingBoundsAABB.Max.X * 16.0f,
                            model.pickingBoundsAABB.Max.Y * 16.0f,
                            model.pickingBoundsAABB.Max.Z * 16.0f
                        );
                    },
                    (model, pl, input) => {
                        if (!GetRealPixels(pl, input[0], "minX", ref model.pickingBoundsAABB.Min.X)) return false;
                        if (!GetRealPixels(pl, input[1], "minY", ref model.pickingBoundsAABB.Min.Y)) return false;
                        if (!GetRealPixels(pl, input[2], "minZ", ref model.pickingBoundsAABB.Min.Z)) return false;
                        if (!GetRealPixels(pl, input[0], "maxX", ref model.pickingBoundsAABB.Max.X)) return false;
                        if (!GetRealPixels(pl, input[1], "maxY", ref model.pickingBoundsAABB.Max.Y)) return false;
                        if (!GetRealPixels(pl, input[2], "maxZ", ref model.pickingBoundsAABB.Max.Z)) return false;
                        return true;
                    }
                )
            },
            {
                "bobbing",
                new ChatType(
                    "bobbing",
                    (model) => model.bobbing.ToString(),
                    (model, pl, input) => CommandParser.GetBool(pl, input, ref model.bobbing)
                )
            },
            {
                "pushes",
                new ChatType(
                    "pushes",
                    (model) => model.pushes.ToString(),
                    (model, pl, input) => CommandParser.GetBool(pl, input, ref model.pushes)
                )
            },
            {
                "usesHumanSkin",
                new ChatType(
                    "usesHumanSkin",
                    (model) => model.usesHumanSkin.ToString(),
                    (model, pl, input) => CommandParser.GetBool(pl, input, ref model.usesHumanSkin)
                )
            },
            {
                "calcHumanAnims",
                new ChatType(
                    "calcHumanAnims",
                    (model) => model.calcHumanAnims.ToString(),
                    (model, pl, input) => CommandParser.GetBool(pl, input, ref model.calcHumanAnims)
                )
            },
            {
                "hideFirstPersonArm",
                new ChatType(
                    "hideFirstPersonArm",
                    (model) => model.hideFirstPersonArm.ToString(),
                    (model, pl, input) => CommandParser.GetBool(pl, input, ref model.hideFirstPersonArm)
                )
            },
        };

        class CmdCustomModel : Command {
            public override string name => "CustomModel";
            public override string type => "model";
            public override string shortcut => "cm";

            public override void Help(Player p) {
                p.Message("%T/" + this.name);
                p.Message("%HKisses u.");
            }

            public override void Use(Player p, string message) {
                var words = message.SplitSpaces();
                if (words.Length >= 2) {
                    // /CustomModel config [model name]
                    if (words[0] == "config") {
                        var modelName = words[1];
                        if (!CustomModels.ContainsKey(modelName)) {
                            p.Message("%cmodel '%f{0}%c' not found", modelName);
                            return;
                        }

                        var model = CustomModels[modelName];

                        if (words.Length == 2 || words.Length == 3) {
                            // /CustomModel config [model name]
                            // or
                            // /CustomModel config [model name] [field]

                            foreach (var entry in modifiableFields) {
                                var fieldName = entry.Key;
                                var chatType = entry.Value;
                                p.Message(
                                    fieldName + " = " + chatType.get.Invoke(model)
                                );
                            }
                            return;
                        } else if (words.Length >= 4) {
                            // /CustomModel config [model name] [field] [value]
                            var fieldName = words[2];
                            if (modifiableFields.ContainsKey(fieldName)) {
                                var chatType = modifiableFields[fieldName];
                                var inputs = words.Skip(3).ToArray();
                                if (inputs.Length == chatType.types.Length) {
                                    if (chatType.set.Invoke(model, p, inputs)) {
                                        // field was set, update file!
                                        DefineModelForAllPlayers(model);
                                        WriteCCModelFile(modelName, model);
                                        return;
                                    } else {
                                        p.Message("%cUMMMM");
                                        return;
                                    }
                                } else {
                                    p.Message("%cnot enough inputs");
                                    return;
                                }
                            } else {
                                p.Message("%cno such field '{0}'", fieldName);
                                return;
                            }
                        }
                    }
                }

                this.Help(p);
            }
        }

        //------------------------------------------------------------------bbmodel json parsing

        class WritablePropertiesOnlyResolver : DefaultContractResolver {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                // Writable because Vec3F32 has some "getter-only" fields we don't want to serialize
                return props.Where(p => p.Writable).ToList();
            }
        }

        static JsonSerializerSettings jsonSettings = new JsonSerializerSettings {
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        class BlockBench {
            public class JsonRoot {
                public Meta meta;
                public string name;
                public Element[] elements;
                public Resolution resolution;

                public string ToJson() {
                    return JsonConvert.SerializeObject(this);
                }

                public CustomModelPart[] ToCustomModelParts() {
                    var list = new List<CustomModelPart>();

                    if (!this.meta.box_uv) {
                        throw new Exception("unimplemented: not using box_uv");
                    }

                    bool notifiedTexture = false;
                    foreach (Element e in this.elements) {
                        if (e.visibility.HasValue && e.visibility.Value == false) {
                            continue;
                        }

                        if (!notifiedTexture &&
                            (!e.faces.north.texture.HasValue ||
                                !e.faces.east.texture.HasValue ||
                                !e.faces.south.texture.HasValue ||
                                !e.faces.west.texture.HasValue ||
                                !e.faces.up.texture.HasValue ||
                                !e.faces.down.texture.HasValue
                            )
                        ) {
                            Logger.Log(
                                LogType.Warning,
                                "Warning: Custom Model '" +
                                this.name +
                                "' has one or more faces with no texture!"
                            );
                            notifiedTexture = true;
                        }

                        UInt16 texX = 0;
                        UInt16 texY = 0;
                        if (e.uv_offset != null) {
                            texX = e.uv_offset[0];
                            texY = e.uv_offset[1];
                        }

                        Vec3F32 rotation = new Vec3F32 { X = 0, Y = 0, Z = 0 };
                        if (e.rotation != null) {
                            rotation = new Vec3F32 {
                                X = e.rotation[0],
                                Y = e.rotation[1],
                                Z = e.rotation[2],
                            };
                        }

                        Vec3F32 v1 = new Vec3F32 {
                            X = e.from[0] - e.inflate,
                            Y = e.from[1] - e.inflate,
                            Z = e.from[2] - e.inflate,
                        };
                        Vec3F32 v2 = new Vec3F32 {
                            X = e.to[0] + e.inflate,
                            Y = e.to[1] + e.inflate,
                            Z = e.to[2] + e.inflate,
                        };

                        if (e.shade.HasValue && e.shade.Value == false) {
                            // mirroring enabled, flip X's
                            float tmp = v1.X;
                            v1.X = v2.X;
                            v2.X = tmp;
                        }

                        var part = new CustomModelPart {
                            boxDesc = new BoxDesc {
                                texX = texX,
                                texY = texY,
                                sizeX = (byte)Math.Abs(e.faces.up.uv[2] - e.faces.up.uv[0]),
                                sizeY = (byte)Math.Abs(e.faces.east.uv[3] - e.faces.east.uv[1]),
                                sizeZ = (byte)Math.Abs(e.faces.east.uv[2] - e.faces.east.uv[0]),
                                x1 = v1.X / 16.0f,
                                y1 = v1.Y / 16.0f,
                                z1 = v1.Z / 16.0f,
                                x2 = v2.X / 16.0f,
                                y2 = v2.Y / 16.0f,
                                z2 = v2.Z / 16.0f,
                                rotX = e.origin[0] / 16.0f,
                                rotY = e.origin[1] / 16.0f,
                                rotZ = e.origin[2] / 16.0f,
                            },
                            rotation = rotation,
                            anim = CustomModelAnim.None,
                            fullbright = false,
                        };

                        if (e.name.CaselessContains("head")) {
                            part.anim = CustomModelAnim.Head;
                        } else if (e.name.CaselessContains("leftleg")) {
                            part.anim = CustomModelAnim.LeftLeg;
                        } else if (e.name.CaselessContains("rightleg")) {
                            part.anim = CustomModelAnim.RightLeg;
                        } else if (e.name.CaselessContains("leftarm")) {
                            part.anim = CustomModelAnim.LeftArm;
                        } else if (e.name.CaselessContains("rightarm")) {
                            part.anim = CustomModelAnim.RightArm;
                        }

                        if (e.name.CaselessContains("fullbright")) {
                            part.fullbright = true;
                        }

                        list.Add(part);

                    }

                    return list.ToArray();
                }

                public class Resolution {
                    public UInt16 width;
                    public UInt16 height;
                }
                public class Meta {
                    public bool box_uv;
                }
                public class Element {
                    public string name;
                    // 3 numbers
                    public float[] from;
                    // 3 numbers
                    public float[] to;

                    public bool? visibility;

                    // if set to 1, uses a default png with some colors on it,
                    // we will only support skin pngs, so maybe notify user?
                    public UInt16 autouv;

                    // optional
                    public float inflate;

                    // if false, mirroring is enabled
                    // if null, mirroring is disabled
                    public bool? shade;

                    // so far only false?
                    // public locked: bool;
                    // optional, 3 numbers
                    public float[] rotation;

                    /// "Pivot Point"
                    // 3 numbers
                    public float[] origin;

                    // optional, 2 numbers
                    public UInt16[] uv_offset;
                    public Faces faces;
                }
                public class Faces {
                    public Face north;
                    public Face east;
                    public Face south;
                    public Face west;
                    public Face up;
                    public Face down;
                }
                public class Face {
                    // 4 numbers
                    public UInt16[] uv;
                    public UInt16? texture;
                }
            }

            public static JsonRoot Parse(string json) {
                JsonRoot m = JsonConvert.DeserializeObject<JsonRoot>(json);
                return m;
            }
        }

    }

}
