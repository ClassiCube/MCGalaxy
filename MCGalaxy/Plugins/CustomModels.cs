//reference Newtonsoft.Json.dll

using System;
using System.Collections.Generic;
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

        public override void Load(bool startup) {
            Logger.Log(LogType.Warning, "loading god");
            OnPlayerFinishConnectingEvent.Register(OnPlayerFinishConnecting, Priority.Low);
            OnPlayerChatEvent.Register(OnPlayerChat, Priority.Low);
        }

        public override void Unload(bool shutdown) {
            OnPlayerFinishConnectingEvent.Unregister(OnPlayerFinishConnecting);
        }

        static void OnPlayerChat(Player p, string message) {
            if (!p.Supports(CpeExt.DefineModel)) {
                return;
            }

            Logger.Log(LogType.Warning, "Sending DefineModel");

            // var ag = new List<CustomModelPart>();

            // var r = new Random();
            // for (int i = 0; i < 32; i++) {

            //     var x = (((float) r.NextDouble()) * 2) - 1.0f;
            //     var y = (((float) r.NextDouble()) * 2) - 1.0f;
            //     var z = (((float) r.NextDouble()) * 2) - 1.0f;

            //     ag.Add(new CustomModelPart() {
            //         boxDesc = new BoxDesc() {
            //                 texX = 0,
            //                     texY = 0,
            //                     sizeX = 8,
            //                     sizeY = 8,
            //                     sizeZ = 8,
            //                     x1 = x - 0.1f,
            //                     y1 = y - 0.1f,
            //                     z1 = z - 0.1f,
            //                     x2 = x + 0.1f,
            //                     y2 = y + 0.1f,
            //                     z2 = z + 0.1f,
            //                     rotX = 0,
            //                     rotY = 0,
            //                     rotZ = 0,
            //             },
            //             rotation = new Vec3F32() {
            //                 X = 45.0f,
            //                     Y = 0.0f,
            //                     Z = 0.0f
            //             }
            //     });
            // }

            var bb = BlockBench.Parse("{\"meta\":{\"format_version\":\"3.2\",\"model_format\":\"free\",\"box_uv\":true},\"name\":\"rotated\",\"geo_name\":\"\",\"resolution\":{\"width\":64,\"height\":64},\"elements\":[{\"name\":\"cube\",\"from\":[-4,17,-2.025],\"to\":[4,23,3.975],\"autouv\":0,\"color\":1,\"locked\":false,\"origin\":[0,19.25,0.9749999999999999],\"uv_offset\":[0,21],\"faces\":{\"north\":{\"uv\":[6,27,14,33],\"texture\":0},\"east\":{\"uv\":[0,27,6,33],\"texture\":0},\"south\":{\"uv\":[20,27,28,33],\"texture\":0},\"west\":{\"uv\":[14,27,20,33],\"texture\":0},\"up\":{\"uv\":[14,27,6,21],\"texture\":0},\"down\":{\"uv\":[22,21,14,27],\"texture\":0}},\"uuid\":\"391f6a2d-8c36-423e-bf5e-2e88c38c00ef\"},{\"name\":\"rightarm\",\"from\":[4,10.75,-1],\"to\":[7,22.75,3],\"autouv\":0,\"color\":4,\"locked\":false,\"origin\":[5,21.75,1],\"uv_offset\":[28,17],\"faces\":{\"north\":{\"uv\":[32,21,35,33],\"texture\":0},\"east\":{\"uv\":[28,21,32,33],\"texture\":0},\"south\":{\"uv\":[39,21,42,33],\"texture\":0},\"west\":{\"uv\":[35,21,39,33],\"texture\":0},\"up\":{\"uv\":[35,21,32,17],\"texture\":0},\"down\":{\"uv\":[38,17,35,21],\"texture\":0}},\"uuid\":\"a3dff51d-4454-7180-0b88-963958f0193a\"},{\"name\":\"leftarm\",\"from\":[-7,10.75,-1],\"to\":[-4,22.75,3],\"autouv\":0,\"color\":4,\"locked\":false,\"origin\":[-5,21.75,1],\"uv_offset\":[43,17],\"faces\":{\"north\":{\"uv\":[47,21,50,33],\"texture\":0},\"east\":{\"uv\":[43,21,47,33],\"texture\":0},\"south\":{\"uv\":[54,21,57,33],\"texture\":0},\"west\":{\"uv\":[50,21,54,33],\"texture\":0},\"up\":{\"uv\":[50,21,47,17],\"texture\":0},\"down\":{\"uv\":[53,17,50,21],\"texture\":0}},\"uuid\":\"970eb595-7065-461f-c466-0640dae0275d\"},{\"name\":\"cube\",\"from\":[-3.5,15,-1.5],\"to\":[3.5,17,3.5],\"autouv\":0,\"color\":5,\"locked\":false,\"origin\":[0,15.75,1],\"uv_offset\":[0,42],\"faces\":{\"north\":{\"uv\":[5,47,12,49],\"texture\":0},\"east\":{\"uv\":[0,47,5,49],\"texture\":0},\"south\":{\"uv\":[17,47,24,49],\"texture\":0},\"west\":{\"uv\":[12,47,17,49],\"texture\":0},\"up\":{\"uv\":[12,47,5,42],\"texture\":0},\"down\":{\"uv\":[19,42,12,47],\"texture\":0}},\"uuid\":\"d234c90e-207d-1db2-bb5c-1e320c3496e9\"},{\"name\":\"cube\",\"from\":[-4,12,-2],\"to\":[4,15,4],\"autouv\":0,\"color\":4,\"locked\":false,\"origin\":[0,12.75,1.25],\"uv_offset\":[0,51],\"faces\":{\"north\":{\"uv\":[6,57,14,60],\"texture\":0},\"east\":{\"uv\":[0,57,6,60],\"texture\":0},\"south\":{\"uv\":[20,57,28,60],\"texture\":0},\"west\":{\"uv\":[14,57,20,60],\"texture\":0},\"up\":{\"uv\":[14,57,6,51],\"texture\":0},\"down\":{\"uv\":[22,51,14,57],\"texture\":0}},\"uuid\":\"239d3bfe-42c1-38da-27b5-99742ef2caa3\"},{\"name\":\"cube\",\"from\":[-1.5,23,-0.5],\"to\":[1.5,24,2.5],\"autouv\":0,\"color\":2,\"locked\":false,\"origin\":[0,23,1],\"uv_offset\":[0,16],\"faces\":{\"north\":{\"uv\":[3,19,6,20],\"texture\":0},\"east\":{\"uv\":[0,19,3,20],\"texture\":0},\"south\":{\"uv\":[9,19,12,20],\"texture\":0},\"west\":{\"uv\":[6,19,9,20],\"texture\":0},\"up\":{\"uv\":[6,19,3,16],\"texture\":0},\"down\":{\"uv\":[9,16,6,19],\"texture\":0}},\"uuid\":\"32d01d0b-bf2f-04f5-608f-1bf0fbddd09c\"},{\"name\":\"leftleg\",\"from\":[-3.5,0,-1],\"to\":[-0.5,8,3],\"autouv\":0,\"color\":4,\"locked\":false,\"origin\":[-2,11.25,1],\"uv_offset\":[43,36],\"faces\":{\"north\":{\"uv\":[47,40,50,48],\"texture\":0},\"east\":{\"uv\":[43,40,47,48],\"texture\":0},\"south\":{\"uv\":[54,40,57,48],\"texture\":0},\"west\":{\"uv\":[50,40,54,48],\"texture\":0},\"up\":{\"uv\":[50,40,47,36],\"texture\":0},\"down\":{\"uv\":[53,36,50,40],\"texture\":0}},\"uuid\":\"f6b3b187-338f-14fd-15c8-396b381b24e8\"},{\"name\":\"rightleg\",\"from\":[0.5,0,-1],\"to\":[3.5,8,3],\"autouv\":0,\"color\":4,\"locked\":false,\"origin\":[2,11.25,1],\"uv_offset\":[28,36],\"faces\":{\"north\":{\"uv\":[32,40,35,48],\"texture\":0},\"east\":{\"uv\":[28,40,32,48],\"texture\":0},\"south\":{\"uv\":[39,40,42,48],\"texture\":0},\"west\":{\"uv\":[35,40,39,48],\"texture\":0},\"up\":{\"uv\":[35,40,32,36],\"texture\":0},\"down\":{\"uv\":[38,36,35,40],\"texture\":0}},\"uuid\":\"d33e80e1-6dec-5b53-7243-b30e3c0f82cd\"},{\"name\":\"head\",\"from\":[-4,24,-3],\"to\":[4,32,5],\"autouv\":0,\"color\":6,\"locked\":false,\"origin\":[0,28,1],\"faces\":{\"north\":{\"uv\":[8,8,16,16],\"texture\":0},\"east\":{\"uv\":[0,8,8,16],\"texture\":0},\"south\":{\"uv\":[24,8,32,16],\"texture\":0},\"west\":{\"uv\":[16,8,24,16],\"texture\":0},\"up\":{\"uv\":[16,8,8,0],\"texture\":0},\"down\":{\"uv\":[24,0,16,8],\"texture\":0}},\"uuid\":\"f2f36209-a89d-9971-9fe7-b26e5bf8b9e1\"},{\"name\":\"rightleg\",\"from\":[0.25,8,-1.5],\"to\":[4,12,3.5],\"autouv\":0,\"color\":6,\"locked\":false,\"origin\":[2.25,11.25,1],\"uv_offset\":[0,33],\"faces\":{\"north\":{\"uv\":[5,38,8,42],\"texture\":0},\"east\":{\"uv\":[0,38,5,42],\"texture\":0},\"south\":{\"uv\":[13,38,16,42],\"texture\":0},\"west\":{\"uv\":[8,38,13,42],\"texture\":0},\"up\":{\"uv\":[8,38,5,33],\"texture\":0},\"down\":{\"uv\":[11,33,8,38],\"texture\":0}},\"uuid\":\"d32159d8-e1f3-f56f-6861-564d3e31baa4\"},{\"name\":\"leftleg\",\"from\":[-4,8,-1.5],\"to\":[-0.25,12,3.5],\"autouv\":0,\"color\":6,\"locked\":false,\"origin\":[-2.25,11.25,1],\"uv_offset\":[29,51],\"faces\":{\"north\":{\"uv\":[34,56,37,60],\"texture\":0},\"east\":{\"uv\":[29,56,34,60],\"texture\":0},\"south\":{\"uv\":[42,56,45,60],\"texture\":0},\"west\":{\"uv\":[37,56,42,60],\"texture\":0},\"up\":{\"uv\":[37,56,34,51],\"texture\":0},\"down\":{\"uv\":[40,51,37,56],\"texture\":0}},\"uuid\":\"dc73ddff-71b3-14aa-20d4-019698e69361\"},{\"name\":\"head,fullbright\",\"from\":[-4,24,-3],\"to\":[4,32,5],\"autouv\":0,\"color\":0,\"locked\":false,\"inflate\":0.15,\"origin\":[0,28,1],\"uv_offset\":[32,0],\"faces\":{\"north\":{\"uv\":[40,8,48,16],\"texture\":0},\"east\":{\"uv\":[32,8,40,16],\"texture\":0},\"south\":{\"uv\":[56,8,64,16],\"texture\":0},\"west\":{\"uv\":[48,8,56,16],\"texture\":0},\"up\":{\"uv\":[48,8,40,0],\"texture\":0},\"down\":{\"uv\":[56,0,48,8],\"texture\":0}},\"uuid\":\"a8582274-197f-02bc-b883-9b43ef06224b\"}],\"outliner\":[{\"name\":\"Head\",\"uuid\":\"5d9372c6-24e7-066a-02dd-505793a1dd62\",\"export\":true,\"isOpen\":true,\"locked\":false,\"visibility\":true,\"autouv\":0,\"origin\":[3,12,3],\"children\":[\"f2f36209-a89d-9971-9fe7-b26e5bf8b9e1\",\"a8582274-197f-02bc-b883-9b43ef06224b\"]},{\"name\":\"Right_Arm\",\"uuid\":\"4d1f7654-5656-9d1a-4f7e-55a341381f7a\",\"export\":true,\"isOpen\":true,\"locked\":false,\"visibility\":true,\"autouv\":0,\"origin\":[0,0,0],\"children\":[\"a3dff51d-4454-7180-0b88-963958f0193a\"]},{\"name\":\"Left_Arm\",\"uuid\":\"179c156b-2f59-1f85-eab3-2f9fedc736eb\",\"export\":true,\"isOpen\":true,\"locked\":false,\"visibility\":true,\"autouv\":0,\"origin\":[-5,19.75,1],\"children\":[\"970eb595-7065-461f-c466-0640dae0275d\"]},{\"name\":\"Torso\",\"uuid\":\"1cc9cb5f-a8e6-1ee8-3453-3fc2f106a888\",\"export\":true,\"isOpen\":true,\"locked\":false,\"visibility\":true,\"autouv\":0,\"origin\":[2,15,-1],\"children\":[\"32d01d0b-bf2f-04f5-608f-1bf0fbddd09c\",\"239d3bfe-42c1-38da-27b5-99742ef2caa3\",\"391f6a2d-8c36-423e-bf5e-2e88c38c00ef\",\"d234c90e-207d-1db2-bb5c-1e320c3496e9\"]},{\"name\":\"Right_Leg\",\"uuid\":\"4fbe8ef8-a48f-5ea2-0a5a-910ca0c26a2b\",\"export\":true,\"isOpen\":true,\"locked\":false,\"visibility\":true,\"autouv\":0,\"origin\":[2.5,0,-1],\"children\":[\"d32159d8-e1f3-f56f-6861-564d3e31baa4\",\"d33e80e1-6dec-5b53-7243-b30e3c0f82cd\"]},{\"name\":\"Left_Leg\",\"uuid\":\"4b202e36-d29a-e05c-0015-1bcef232736f\",\"export\":true,\"isOpen\":true,\"locked\":false,\"visibility\":true,\"autouv\":0,\"origin\":[2.5,0,-1],\"children\":[\"f6b3b187-338f-14fd-15c8-396b381b24e8\",\"dc73ddff-71b3-14aa-20d4-019698e69361\"]}],\"textures\":[{\"path\":\"C:\\\\Users\\\\SpiralP\\\\Desktop\\\\ag.png\",\"name\":\"ag.png\",\"folder\":\"Desktop\",\"namespace\":\"\",\"id\":\"0\",\"particle\":false,\"mode\":\"bitmap\",\"saved\":true,\"uuid\":\"58722a6d-be58-4525-7f50-898d863662a7\",\"source\":\"data:image/png;base64,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\"}]}");

            Logger.Log(LogType.Warning, "" + bb.name);
            Logger.Log(LogType.Warning, "" + bb.elements.Length + " cubes");

            var parts = bb.ToCustomModelParts();
            Logger.Log(LogType.Warning, "" + parts.Length + " parts");

            var data = Packet.DefineModel(
                new CustomModel {
                    name = "cat",
                        parts = parts
                }
            );
            p.Send(data);

            Logger.Log(LogType.Warning, bb.ToJson());
        }

        class WritablePropertiesOnlyResolver : DefaultContractResolver {
            protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization) {
                IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
                return props.Where(p => p.PropertyName != "parts" && p.Writable).ToList();
            }
        }

        static JsonSerializerSettings jsonSettings = new JsonSerializerSettings {
            ContractResolver = new WritablePropertiesOnlyResolver()
        };

        class StoredCustomModel {
            public CustomModel customModel;
            public string bbmodelName;
        }

        static void OnPlayerFinishConnecting(Player p) {
            if (!p.Supports(CpeExt.DefineModel)) {
                return;
            }

            Logger.Log(LogType.Warning, "Sending DefineModel");

            var customModel = new CustomModel {
                name = "cat",
                nameY = 0.0f,
                eyeY = 0.0f,
                bobbing = false,
                parts = new CustomModelPart[] {
                new CustomModelPart() {
                boxDesc = new BoxDesc() {
                texX = 0,
                texY = 0,
                sizeX = 8,
                sizeY = 8,
                sizeZ = 8,
                x1 = -0.1f,
                y1 = -0.1f,
                z1 = -0.1f,
                x2 = 0.1f,
                y2 = 0.1f,
                z2 = 0.1f,
                rotX = 0,
                rotY = 0,
                rotZ = 0,
                }
                },
                new CustomModelPart() {
                boxDesc = new BoxDesc() {
                texX = 0,
                texY = 0,
                sizeX = 8,
                sizeY = 8,
                sizeZ = 8,
                x1 = 1.0f,
                y1 = 1.0f,
                z1 = 1.0f,
                x2 = 1.1f,
                y2 = 1.1f,
                z2 = 1.1f,
                rotX = 0,
                rotY = 0,
                rotZ = 0,
                },
                rotation = new Vec3F32() {
                X = 45.0f,
                Y = 0.0f,
                Z = 0.0f
                }
                }
                }
            };

            var storedCustomModel = new StoredCustomModel {
                customModel = customModel,
                bbmodelName = "big mommy gf"
            };

            var json = JsonConvert.SerializeObject(storedCustomModel, Formatting.Indented, jsonSettings);
            Logger.Log(LogType.Warning, json);

            var ag = JsonConvert.DeserializeObject<StoredCustomModel>(json);
            Logger.Log(LogType.Warning, "" + ag.customModel.collisionBounds.Y);

            var data = Packet.DefineModel(
                new CustomModel {
                    name = "cat",
                        nameY = 0.0f,
                        eyeY = 0.0f,
                        bobbing = false,
                        parts = new CustomModelPart[] {
                            new CustomModelPart() {
                                    boxDesc = new BoxDesc() {
                                        texX = 0,
                                            texY = 0,
                                            sizeX = 8,
                                            sizeY = 8,
                                            sizeZ = 8,
                                            x1 = -0.1f,
                                            y1 = -0.1f,
                                            z1 = -0.1f,
                                            x2 = 0.1f,
                                            y2 = 0.1f,
                                            z2 = 0.1f,
                                            rotX = 0,
                                            rotY = 0,
                                            rotZ = 0,
                                    }
                                },
                                new CustomModelPart() {
                                    boxDesc = new BoxDesc() {
                                            texX = 0,
                                                texY = 0,
                                                sizeX = 8,
                                                sizeY = 8,
                                                sizeZ = 8,
                                                x1 = 1.0f,
                                                y1 = 1.0f,
                                                z1 = 1.0f,
                                                x2 = 1.1f,
                                                y2 = 1.1f,
                                                z2 = 1.1f,
                                                rotX = 0,
                                                rotY = 0,
                                                rotZ = 0,
                                        },
                                        rotation = new Vec3F32() {
                                            X = 45.0f,
                                                Y = 0.0f,
                                                Z = 0.0f
                                        }
                                }
                        }
                });
            p.Send(data);
            p.Send(data);

        }

        class BlockBench {
            public class JsonRoot {
                public Meta meta;
                public string name;
                public Element[] elements;

                public string ToJson() {
                    return JsonConvert.SerializeObject(this);
                }

                public CustomModelPart[] ToCustomModelParts() {
                    var list = new List<CustomModelPart>();

                    if (!this.meta.box_uv) {
                        throw new Exception("unimplemented: not using box_uv");
                    }

                    foreach (Element e in this.elements) {
                        if (e.autouv != 0) {
                            throw new Exception("unimplemented: autouv not 0");
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

                        var part = new CustomModelPart {
                            boxDesc = new BoxDesc {
                            texX = texX,
                            texY = texY,
                            sizeX = (byte) Math.Abs(e.faces.up.uv[2] - e.faces.up.uv[0]),
                            sizeY = (byte) Math.Abs(e.faces.east.uv[3] - e.faces.east.uv[1]),
                            sizeZ = (byte) Math.Abs(e.faces.east.uv[2] - e.faces.east.uv[0]),
                            x1 = e.from[0] / 16.0f, y1 = e.from[1] / 16.0f, z1 = e.from[2] / 16.0f,
                            x2 = e.to[0] / 16.0f, y2 = e.to[1] / 16.0f, z2 = e.to[2] / 16.0f,
                            rotX = e.origin[0] / 16.0f, rotY = e.origin[1] / 16.0f, rotZ = e.origin[2] / 16.0f,
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

                public class Meta {
                    public bool box_uv;
                }
                public class Element {
                    public string name;
                    // 3 numbers
                    public float[] from;
                    // 3 numbers
                    public float[] to;

                    // so far only 0?
                    public UInt16 autouv;

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
                }
            }

            public static JsonRoot Parse(string json) {
                JsonRoot m = JsonConvert.DeserializeObject<JsonRoot>(json);
                return m;
            }
        }

    }

}
