//reference Newtonsoft.Json.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace MCGalaxy {
    public sealed class CustomModelsPlugin : Plugin {
        public override string name { get { return "CustomModels"; } }
        public override string MCGalaxy_Version { get { return "1.9.2.0"; } }
        public override string creator { get { return "god"; } }

        //------------------------------------------------------------------bbmodel/ccmodel file loading

        public static Dictionary<string, CustomModel> CustomModels = new Dictionary<string, CustomModel>();
        const string blockBenchExt = ".bbmodel";
        const string storedModelExt = ".ccmodel";
        const string BBdirectory = "plugins/models/bbmodels/";
        const string CCdirectory = "plugins/models/";

        class StoredCustomModel {
            public CustomModel customModel;
            public string bbmodelName;
        }

        static void CreateCCmodelFromBBmodel() {

            if (!Directory.Exists(CCdirectory)) Directory.CreateDirectory(BBdirectory);
            DirectoryInfo info = new DirectoryInfo(BBdirectory);
            FileInfo[] filesInBBdir = info.GetFiles();

            for (int i = 0; i < filesInBBdir.Length; i++) {
                string fileName = filesInBBdir[i].Name;
                string realName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);

                if (!extension.CaselessEq(blockBenchExt)) { continue; }
                if (File.Exists(CCdirectory + realName + storedModelExt)) { continue; }

                var storedCustomModel = new StoredCustomModel {
                    customModel = new CustomModel { name = realName },
                    bbmodelName = realName
                };

                var storedJsonModel = JsonConvert.SerializeObject(storedCustomModel, Formatting.Indented, jsonSettings);
                File.WriteAllText(CCdirectory + realName + storedModelExt, storedJsonModel);

                Logger.Log(LogType.SystemActivity, "CustomModels: Created a new default template \"{0}\" in {1}.", realName + storedModelExt, CCdirectory);
            }
        }

        static void LoadModels() {

            if (!Directory.Exists(CCdirectory)) Directory.CreateDirectory(CCdirectory);
            DirectoryInfo info = new DirectoryInfo(CCdirectory);
            FileInfo[] filesInModels = info.GetFiles();

            for (int i = 0; i < filesInModels.Length; i++) {
                string fileName = filesInModels[i].Name;
                string realName = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);

                if (!extension.CaselessEq(storedModelExt)) { continue; }
                if (!File.Exists(BBdirectory + realName + blockBenchExt)) { continue; }

                string contentsCC = File.ReadAllText(CCdirectory + fileName);
                string contentsBB = File.ReadAllText(BBdirectory + realName + blockBenchExt);

                StoredCustomModel storedCustomModel = JsonConvert.DeserializeObject<StoredCustomModel>(contentsCC);
                var blockBench = BlockBench.Parse(contentsBB);
                var parts = blockBench.ToCustomModelParts();

                storedCustomModel.customModel.parts = parts;
                storedCustomModel.customModel.uScale = blockBench.resolution.width;
                storedCustomModel.customModel.vScale = blockBench.resolution.height;

                CustomModels[realName] = storedCustomModel.customModel;
                Logger.Log(LogType.SystemActivity, "CustomModels: Loaded model {0}.", storedCustomModel.customModel.name);
            }
        }

        static void DefineModel(Player p, CustomModel model) {
            if (!p.Supports(CpeExt.CustomModels)) { return; }
            byte[] modelPacket = Packet.DefineModel(model);
            p.Send(modelPacket);
        }
        static void DefineModels(Player p) {
            if (!p.Supports(CpeExt.CustomModels)) { return; }
            foreach (KeyValuePair<string, CustomModel> entry in CustomModels) {
                DefineModel(p, entry.Value);
                p.Message("Defined model %b{0}%S!", entry.Key);
            }
        }
        static void DefineModelsForAllPlayers() {
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                DefineModels(p);
            }
        }

        //------------------------------------------------------------------plugin interface

        public override void Load(bool startup) {
            //Logger.Log(LogType.Warning, "loading god");
            OnPlayerFinishConnectingEvent.Register(OnPlayerFinishConnecting, Priority.Low);

            CreateCCmodelFromBBmodel();
            LoadModels();

            DefineModelsForAllPlayers();
        }

        public override void Unload(bool shutdown) {
            OnPlayerFinishConnectingEvent.Unregister(OnPlayerFinishConnecting);
        }

        static void OnPlayerFinishConnecting(Player p) {
            DefineModels(p);
        }

        //------------------------------------------------------------------bbmodel json parsing

        class WritablePropertiesOnlyResolver : DefaultContractResolver {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                // don't serialize "parts" because we store those in the full .bbmodel file
                // Writable because Vec3F32 has some "getter-only" fields we don't want to serialize
                return props.Where(p =>
                    p.PropertyName != "parts" &&
                    p.PropertyName != "uScale" &&
                    p.PropertyName != "vScale" &&
                    p.Writable
                ).ToList();
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
                            sizeX = (byte) Math.Abs(e.faces.up.uv[2] - e.faces.up.uv[0]),
                            sizeY = (byte) Math.Abs(e.faces.east.uv[3] - e.faces.east.uv[1]),
                            sizeZ = (byte) Math.Abs(e.faces.east.uv[2] - e.faces.east.uv[0]),
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
