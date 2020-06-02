using System;
using System.Collections.Generic;
using MCGalaxy.Events.PlayerEvents;
using MCGalaxy.Maths;
using MCGalaxy.Network;

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

            var ag = new List<CustomModelPart>();

            var r = new Random();
            for (int i = 0; i < 32; i++) {

                var x = (((float) r.NextDouble()) * 2) - 1.0f;
                var y = (((float) r.NextDouble()) * 2) - 1.0f;
                var z = (((float) r.NextDouble()) * 2) - 1.0f;

                ag.Add(new CustomModelPart() {
                    boxDesc = new BoxDesc() {
                            texX = 0,
                                texY = 0,
                                sizeX = 8,
                                sizeY = 8,
                                sizeZ = 8,
                                x1 = x - 0.1f,
                                y1 = y - 0.1f,
                                z1 = z - 0.1f,
                                x2 = x + 0.1f,
                                y2 = y + 0.1f,
                                z2 = z + 0.1f,
                                rotX = 0,
                                rotY = 0,
                                rotZ = 0,
                        },
                        rotation = new Vec3F32() {
                            X = 45.0f,
                                Y = 0.0f,
                                Z = 0.0f
                        }
                });
            }

            var data = Packet.DefineModel(
                "cat",
                32.5f / 16.0f,
                26.0f / 16.0f,
                new Vec3F32 {
                    X = (8.6f) / 16.0f,
                        Y = (28.1f) / 16.0f,
                        Z = (8.6f) / 16.0f
                },
                new AABBF32 {
                    Min = new Vec3F32 {
                            X = (-8) / 16.0f,
                                Y = (0) / 16.0f,
                                Z = (-4) / 16.0f
                        },
                        Max = new Vec3F32 {
                            X = (8) / 16.0f,
                                Y = (32) / 16.0f,
                                Z = (4) / 16.0f
                        }
                },
                ag.ToArray()
            );
            p.Send(data);
        }

        static void OnPlayerFinishConnecting(Player p) {
            if (!p.Supports(CpeExt.DefineModel)) {
                return;
            }

            Logger.Log(LogType.Warning, "Sending DefineModel");
            var data = Packet.DefineModel(
                "cat",
                0.0f,
                0.0f,
                new Vec3F32 {
                    X = (8.6f) / 16.0f,
                        Y = (28.1f) / 16.0f,
                        Z = (8.6f) / 16.0f
                },
                new AABBF32 {
                    Min = new Vec3F32 {
                            X = (-8) / 16.0f,
                                Y = (0) / 16.0f,
                                Z = (-4) / 16.0f
                        },
                        Max = new Vec3F32 {
                            X = (8) / 16.0f,
                                Y = (32) / 16.0f,
                                Z = (4) / 16.0f
                        }
                },
                new CustomModelPart[] {
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
                });
            p.Send(data);
            p.Send(data);
        }
    }
}
