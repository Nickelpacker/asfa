using Sandbox;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using ClientPlugin.Settings.Elements;
using VRageMath;
using EmptyKeys.UserInterface.Generated.RespawnScreen_Bindings;
using VRage.Game;
using VRage.Utils;
using System.Text;

namespace ClientPlugin.Settings
{
    internal class SettingsScreen : MyGuiScreenBase
    {
        public readonly string FriendlyName;
        public Func<List<MyGuiControlBase>> GetControls;

        public override string GetFriendlyName() => FriendlyName;

        public SettingsScreen(
            string friendlyName,
            Func<List<MyGuiControlBase>> getControls,
            Vector2? position = null,
            Vector2? size = null

            ) : base(
                position ?? new Vector2(0.5f, 0.5f),
                MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                size ?? new Vector2(0.3f, 0.42f),
                false,
                null,
                MySandboxGame.Config.UIBkOpacity,
                MySandboxGame.Config.UIOpacity)
        {
            FriendlyName = friendlyName;
            GetControls = getControls;

            EnabledBackgroundFade = true;
            m_closeOnEsc = true;
            m_drawEvenWithoutFocus = true;
            CanHideOthers = true;
            CanBeHidden = true;
            CloseButtonEnabled = true;
        }
        public void UpdateSize(Vector2 screenSize)
        {
            Size = screenSize;
            CloseButtonEnabled = CloseButtonEnabled; // Force close button to update
        }

        public override void LoadContent()
        {
            base.LoadContent();
            RecreateControls(true);
        }

        public override void OnRemoved()
        {
            ConfigStorage.Save(GpsClipboardConfig.Current);
            base.OnRemoved();
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);
            AddCaption(FriendlyName); // use FriendlyName, not Name

            foreach (var item in GetControls())
            {
                Controls.Add(item);
            }

            // Create and add the close button
            var closeButton = new MyGuiControlButton
            {
                Text = "Close",
                Position = new Vector2(0f, Size.Value.Y / 2f - 0.05f), // adjust to bottom
                OriginAlign = VRage.Utils.MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                VisualStyle = VRage.Game.MyGuiControlButtonStyleEnum.Default,
                
            };
            StringBuilder stringBuilder = new StringBuilder("");
            stringBuilder.AppendLine("Available Chat Commands:");
            stringBuilder.AppendLine("/gpsreload - Reload the config file");
            stringBuilder.AppendLine("/gpshelp   - List all available GPS commands");
            var helpBox = new MyGuiControlMultilineText
            {
                
                Position = new Vector2(0f, 0f),
                Size = new Vector2(0.25f, 0.2f),
                OriginAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                TextAlign = MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_TOP,
                Font = MyFontEnum.White,
                Text = stringBuilder,
            };
            closeButton.ButtonClicked += _ =>
            {
                CloseScreen();
                ConfigStorage.Save(GpsClipboardConfig.Current);
                Plugin.Config = ConfigStorage.Load();
            };
            Controls.Add(closeButton);
            Controls.Add(helpBox);
        }

    }
}
