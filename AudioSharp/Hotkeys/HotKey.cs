// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HotKey.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp.Hotkeys
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;

    using AudioSharp.Core;

    using Newtonsoft.Json;

    public class HotKey : NotifyBase, IDisposable
    {
        public const int WmHotKey = 0x0312;

        private static Dictionary<int, HotKey> dictHotKeyToCalBackProc;

        private SoundActionBase action;

        private bool disposed = false;

        private Key key;

        private KeyModifier keyModifiers;

        private string name;

        private string parameter;

        public HotKey()
        {
        }

        public HotKey(string name, Key k, KeyModifier keyModifiers)
        {
            this.Name = name;
            this.Key = k;
            this.KeyModifiers = keyModifiers;

            this.Register();
        }

        public SoundActionBase Action
        {
            get
            {
                return this.action;
            }

            set
            {
                if (Equals(value, this.action))
                {
                    return;
                }

                this.action = value;
                this.OnPropertyChanged();
            }
        }

        [JsonIgnore]
        public int Id
        {
            get
            {
                return (int)this.VirtualKey + (int)this.KeyModifiers * 0x10000;
            }
        }

        public Key Key
        {
            get
            {
                return this.key;
            }

            set
            {
                this.Unregister();
                this.key = value;
                this.Register();

                this.OnPropertyChanged();
            }
        }

        public KeyModifier KeyModifiers
        {
            get
            {
                return this.keyModifiers;
            }

            set
            {
                this.Unregister();
                this.keyModifiers = value;
                this.Register();

                this.OnPropertyChanged();
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
                this.OnPropertyChanged();
            }
        }

        public string Parameter
        {
            get
            {
                return this.parameter;
            }

            set
            {
                if (Equals(value, this.parameter))
                {
                    return;
                }

                this.parameter = value;
                this.OnPropertyChanged();
            }
        }

        public uint VirtualKey
        {
            get
            {
                return (uint)KeyInterop.VirtualKeyFromKey(this.Key);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool Register()
        {
            var result = RegisterHotKey(IntPtr.Zero, this.Id, (uint)this.KeyModifiers, this.VirtualKey);

            if (dictHotKeyToCalBackProc == null)
            {
                dictHotKeyToCalBackProc = new Dictionary<int, HotKey>();
                ComponentDispatcher.ThreadFilterMessage += ComponentDispatcherThreadFilterMessage;
            }

            dictHotKeyToCalBackProc[this.Id] = this;

            return result;
        }

        public void Unregister()
        {
            if (this.Id == 0)
            {
                return;
            }

            HotKey hotKey;
            if (dictHotKeyToCalBackProc.TryGetValue(this.Id, out hotKey))
            {
                UnregisterHotKey(IntPtr.Zero, this.Id);
            }
        }

        [OnDeserialized]
        internal void OnDeserializedMethod(StreamingContext context)
        {
            this.Register();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    this.Unregister();
                }

                this.disposed = true;
            }
        }

        private static void ComponentDispatcherThreadFilterMessage(ref MSG msg, ref bool handled)
        {
            if (!handled)
            {
                if (msg.message == WmHotKey)
                {
                    HotKey hotKey;

                    if (dictHotKeyToCalBackProc.TryGetValue((int)msg.wParam, out hotKey))
                    {
                        try
                        {
                            hotKey.Action?.Callback(MainWindow.Instance.Device, hotKey.Parameter);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(
                                e.Message,
                                "Error",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error,
                                MessageBoxResult.OK,
                                MessageBoxOptions.DefaultDesktopOnly);
                        }

                        handled = true;
                    }
                }
            }
        }

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vlc);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);
    }
}