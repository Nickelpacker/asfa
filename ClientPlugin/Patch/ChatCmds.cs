using ClientPlugin.Settings;
using ClientPlugin;
using Sandbox.ModAPI;
using System;
using System.Collections.Generic;
using System.Text;
using VRage.Utils;
using HarmonyLib;
using Sandbox.Game.GameSystems.Chat;
using System.Linq;

public static class ChatCmdPatch
{
    private static readonly List<string> _commands = new List<string>
    {
        { "/gpsreload"},
        { "/gpshelp" }
    };

    public static void HandleCommand(string rawCommand)
    {
        string[] split = rawCommand.Trim().Split(' ');
        string command = split[0].ToLowerInvariant();
        string[] args = split.Skip(1).ToArray();

        switch (command)
        {
            case "/gpsreload":
                ReloadConfig();
                break;

            case "/gpshelp":
                HelpCmd();
                break;

            default:
                MyAPIGateway.Utilities.ShowMessage("Plugin", $"Unknown command: {command}");
                break;
        }
    }
    private static void ReloadConfig()
    {
        MyLog.Default.WriteLineAndConsole($"{Plugin.TG} Reloading config...");

        try
        {
            Plugin.Config = ConfigStorage.Load();
            MyAPIGateway.Utilities.ShowMessage(Plugin.TG, "✅ Config successfully reloaded.");
        }
        catch (Exception ex)
        {
            MyAPIGateway.Utilities.ShowMessage(Plugin.TG, "❌ Config reload failed.");
            MyLog.Default.WriteLineAndConsole($"{Plugin.TG} Config reload error: {ex}");
        }
    }
    private static void HelpCmd()
    {
        MyLog.Default.WriteLineAndConsole($"{Plugin.TG} Showing help...");

        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("Available GPSPlugin commands:");

            foreach (var command in _commands)
                sb.AppendLine($"  {command}");

            MyAPIGateway.Utilities.ShowMessage(Plugin.TG, sb.ToString());
        }
        catch (Exception ex)
        {
            MyAPIGateway.Utilities.ShowMessage(Plugin.TG, "❌ Failed to show help.");
            MyLog.Default.WriteLineAndConsole($"{Plugin.TG} Help command error: {ex}");
        }
    }
}
