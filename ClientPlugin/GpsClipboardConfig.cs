using ClientPlugin.Settings;
using ClientPlugin.Settings.Elements;
using Sandbox.Graphics.GUI;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Forms;
using VRage.Input;
using VRageMath;


namespace ClientPlugin
{

    public class GpsClipboardConfig : INotifyPropertyChanged
    {
        #region Options

        // TODO: Define your configuration options and their default values
        private bool enableColorRandomizer = true;
        private bool ignoreSignals = true;

        #endregion

        #region User interface

        // TODO: Settings dialog title
        public readonly string Title = "Config";

        // TODO: Settings dialog controls, one property for each configuration option

        [Checkbox(description: "Enable Color Randomizer")]
        public bool EnableColorRandomizer
        {
            get => enableColorRandomizer;
            set => SetField(ref enableColorRandomizer, value);
        }
        [Checkbox(description: "Ignore Signals")]
        public bool IgnoreSignals
        {
            get => ignoreSignals;
            set => SetField(ref ignoreSignals, value);
        }

        #endregion

        #region Property change notification bilerplate

        public static readonly GpsClipboardConfig Default = new GpsClipboardConfig();
        public static readonly GpsClipboardConfig Current = ConfigStorage.Load();

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}