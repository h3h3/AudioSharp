// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.ComponentModel.Composition;
    using System.ComponentModel.Composition.Hosting;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Windows;
    using System.Windows.Forms;
    using System.Windows.Input;

    using AudioSharp.Core;
    using AudioSharp.Hotkeys;

    using NAudio.CoreAudioApi;

    using Newtonsoft.Json;

    using Application = System.Windows.Application;

    public partial class MainWindow : INotifyPropertyChanged
    {
        private MMDevice device;

        private ObservableCollection<MMDevice> devices = new ObservableCollection<MMDevice>();

        private HotKey hotKey;

        private ObservableCollection<HotKey> hotKeys = new ObservableCollection<HotKey>();

        static MainWindow()
        {
            JsonConvert.DefaultSettings =
                () =>
                    new JsonSerializerSettings()
                    {
                        TypeNameHandling = TypeNameHandling.Auto,
                        NullValueHandling = NullValueHandling.Ignore,
                        DefaultValueHandling = DefaultValueHandling.Ignore
                    };
        }

        public MainWindow()
        {
            Instance = this;

            this.InitializeComponent();
            this.DataContext = this;
            this.Loaded += this.OnLoaded;
            this.Closing += this.OnClosing;

            this.InitSystray();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public static MainWindow Instance { get; private set; }

        public MMDevice Device
        {
            get
            {
                return this.device;
            }

            set
            {
                if (Equals(value, this.device))
                {
                    return;
                }

                this.device = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<MMDevice> Devices
        {
            get
            {
                return this.devices;
            }

            set
            {
                if (Equals(value, this.devices))
                {
                    return;
                }

                this.devices = value;
                this.OnPropertyChanged();
            }
        }

        public HotKey HotKey
        {
            get
            {
                return this.hotKey;
            }

            set
            {
                if (Equals(value, this.hotKey))
                {
                    return;
                }

                this.hotKey = value;
                this.OnPropertyChanged();
            }
        }

        public ObservableCollection<HotKey> HotKeys
        {
            get
            {
                return this.hotKeys;
            }

            set
            {
                if (Equals(value, this.hotKeys))
                {
                    return;
                }

                this.hotKeys = value;
                this.OnPropertyChanged();
            }
        }

        [ImportMany(typeof(ISoundAction))]
        public IEnumerable<ISoundAction> ImportedActions { get; set; }

        public NotifyIcon NotifyIcon { get; set; }

        public static MMDevice GetDefaultDevice()
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            return deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
        }

        public static IEnumerable<MMDevice> GetDevices()
        {
            var deviceEnumerator = new MMDeviceEnumerator();
            var devices = deviceEnumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);

            foreach (var dev in devices)
            {
                yield return dev;
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (this.WindowState == WindowState.Minimized)
            {
                this.Hide();
            }

            base.OnStateChanged(e);
        }

        private void Delete_OnClick(object sender, RoutedEventArgs e)
        {
            if (this.HotKey == null)
            {
                return;
            }

            this.HotKeys.Remove(this.HotKey);
            this.HotKey = null;
        }

        private void InitSystray()
        {
            this.NotifyIcon = new NotifyIcon();
            var iconStream =
                Application.GetResourceStream(new Uri("pack://application:,,,/AudioSharp;component/Resources/icon.ico"))
                           .Stream;

            this.NotifyIcon.Icon = new Icon(iconStream);
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.DoubleClick +=
                (sender, args) =>
                {
                    this.Show();
                    this.WindowState = WindowState.Normal;
                };
        }

        private void New_OnClick(object sender, RoutedEventArgs e)
        {
            var hotkey = new HotKey(string.Empty, Key.A, KeyModifier.None);

            this.HotKeys.Add(hotkey);
            this.HotKey = hotkey;
        }

        private void OnClosing(object sender, CancelEventArgs args)
        {
            var data = JsonConvert.SerializeObject(this.HotKeys, Formatting.Indented);
            File.WriteAllText("config.json", data);

            this.NotifyIcon.Dispose();
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!Directory.Exists("Plugins"))
            {
                Directory.CreateDirectory("Plugins");
            }

            var catalog = new AggregateCatalog();
            catalog.Catalogs.Add(new ApplicationCatalog());
            catalog.Catalogs.Add(new DirectoryCatalog("Plugins"));

            var container = new CompositionContainer(
                catalog,
                CompositionOptions.DisableSilentRejection | CompositionOptions.IsThreadSafe);

            container.ComposeParts(this);

            try
            {
                if (File.Exists("config.json"))
                {
                    var data = File.ReadAllText("config.json");
                    this.HotKeys = new ObservableCollection<HotKey>(JsonConvert.DeserializeObject<HotKey[]>(data));
                }
            }
            catch
            {
                File.Delete("config.json");
                this.HotKeys = new ObservableCollection<HotKey>();
            }

            this.Devices = new ObservableCollection<MMDevice>(GetDevices());
            this.Keys.ItemsSource = new ObservableCollection<Key>((Key[])Enum.GetValues(typeof(Key)));
            this.Modifiers.ItemsSource = new ObservableCollection<KeyModifier>((KeyModifier[])Enum.GetValues(typeof(KeyModifier)));
            this.Actions.ItemsSource = new ObservableCollection<ISoundAction>(this.ImportedActions);

            this.Device = this.Devices.FirstOrDefault(d => d.ID == GetDefaultDevice()?.ID);

            var cmd = Environment.GetCommandLineArgs();
            if (cmd.Length == 2 && cmd[1] == "/hide")
            {
                this.Hide();
            }
        }
    }
}