using HarmonyLib;
using VRage.Utils;
using VRageMath;
using Sandbox.Game.Multiplayer;
using Sandbox.ModAPI;
using System;
using System.Reflection;
using System.Threading;
using ClientPlugin.Settings.Elements;
using System.Linq;


[HarmonyPatch]
public static class GPSPatch
{
    public const string TG = "[GPSPlugin]";
    static MethodBase TargetMethod()
    {
        Type collectionType = typeof(Sandbox.Game.Multiplayer.MyGpsCollection);
        Type addMsgType = AccessTools.Inner(collectionType, "AddMsg");
        Type myGpsType = AccessTools.TypeByName("Sandbox.Game.Screens.Helpers.MyGps");

        if (addMsgType == null || myGpsType == null)
        {
            MyLog.Default.WriteLineAndConsole($"{TG} ERROR: Could not find types.");
            return null;
        }

        MethodBase method = AccessTools.Method(collectionType, "AddGpsLocal", new[] { addMsgType, myGpsType });

        if (method == null)
            MyLog.Default.WriteLineAndConsole($"{TG} ERROR: Could not find AddGpsLocal");

        return method;
    }

    public static void Postfix(object msg, object gps)
    {
        if (msg == null)
        {
            MyLog.Default.WriteLineAndConsole($"{TG}  Postfix called with null msg");
            return;
        }

        try
        {
            Type msgType = msg.GetType();

            FieldInfo nameField = AccessTools.Field(msgType, "Name");
            FieldInfo coordsField = AccessTools.Field(msgType, "Coords");
            FieldInfo identityField = AccessTools.Field(msgType, "IdentityId");


            if (nameField == null || coordsField == null || identityField == null)
            {
                MyLog.Default.WriteLineAndConsole($"{TG} One or more properties not found on AddMsg");
                return;
            }

            string name = nameField.GetValue(msg)?.ToString() ?? "Unnamed";
            Vector3D coords = (Vector3D)(coordsField.GetValue(msg) ?? Vector3D.Zero);
            long identityId = (long)(identityField.GetValue(msg) ?? -1L);


            if (MyAPIGateway.Session?.Player?.IdentityId == identityId)
            {
                string gpsString = $"GPS:{name}:{coords.X:F2}:{coords.Y:F2}:{coords.Z:F2}:";
                new Thread(() =>
                {
                    try
                    {
                        System.Windows.Forms.Clipboard.SetText(gpsString);
                        MyLog.Default.WriteLineAndConsole($"{TG} GPS Copied: {gpsString}");
                    }
                    catch (Exception ex)
                    {
                        MyLog.Default.WriteLineAndConsole($"{TG} Clipboard Error: {ex.Message}");
                    }
                })
                {
                    Name = "GpsClipboardThread",
                    IsBackground = true,
                    ApartmentState = ApartmentState.STA
                }.Start();

            }
            if (MyAPIGateway.Session?.GPS != null)
            {
                var gpsList = MyAPIGateway.Session.GPS.GetGpsList(identityId);
                var matching = gpsList?.FirstOrDefault(g => g.Name == name);

                if (matching != null)
                {
                    var rand = new Random();
                    var randomColor = new Color(
                        (byte)rand.Next(50, 256),
                        (byte)rand.Next(50, 256),
                        (byte)rand.Next(50, 256)
                    );

                    matching.GPSColor = randomColor;
                    MyAPIGateway.Session.GPS.ModifyGps(identityId, matching); // <- this forces a UI refresh
                    MyLog.Default.WriteLineAndConsole($"[GPSPlugin] Colorized GPS: {name} => {randomColor}");
                }
                else
                {
                    MyLog.Default.WriteLineAndConsole($"[GPSPlugin] Could not find GPS '{name}' to colorize.");
                }
            }

        }
        catch (Exception ex)
        {
            MyLog.Default.WriteLineAndConsole($"{TG} Exception in Postfix: {ex}");
        }
    }

}

