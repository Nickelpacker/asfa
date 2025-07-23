using System;
using System.IO;
using System.Reflection;
using ClientPlugin.Settings;
using ClientPlugin.Settings.Layouts;
using HarmonyLib;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using VRage.Plugins;
using VRage.Utils;

namespace ClientPlugin
{
    // ReSharper disable once UnusedType.Global
    public class Plugin : IPlugin, IDisposable
    {
        public const string Name = "GpsPlugin";
        public static readonly string TG = "[GPSPlugin]";
        public static Plugin Instance { get; private set; }
        public static GpsClipboardConfig Config;
        private SettingsGenerator settingsGenerator;

        [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
        public void Init(object gameInstance)
        {
            Instance = this;
            Instance.settingsGenerator = new SettingsGenerator();

            Harmony harmony = new Harmony(Name);
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            Config = ConfigStorage.Load();
            MyLog.Default.WriteLineAndConsole("[GPSPlugin] Init completed.");
        }



        public void Dispose()
        {
            // TODO: Save state and close resources here, called when the game exits (not guaranteed!)
            // IMPORTANT: Do NOT call harmony.UnpatchAll() here! It may break other plugins.
            MyAPIGateway.Utilities.MessageEntered -= OnMessageEntered;
            Instance = null;
        }

        private void OnMessageEntered(string messageText, ref bool sendToOthers)
        {
            if (!messageText.StartsWith("/")) return; // only handle commands
            sendToOthers = false; // hide from other players

            ChatCmdPatch.HandleCommand(messageText);
        }
        private bool _messageHooked = false;

        public void Update()
        {
            if (!_messageHooked && MyAPIGateway.Utilities != null)
            {
                MyAPIGateway.Utilities.MessageEntered += OnMessageEntered;
                _messageHooked = true;
            }

        }

        // ReSharper disable once UnusedMember.Global
        public void OpenConfigDialog()
        {
            Instance.settingsGenerator.SetLayout<Simple>();
            MyGuiSandbox.AddScreen(Instance.settingsGenerator.Dialog);
        }

        //TODO: Uncomment and use this method to load asset files
        /*public void LoadAssets(string folder)
        {

        }*/
    }
}