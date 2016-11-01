// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MuteAction.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SoundSharp.Core
{
    using System;
    using System.Windows;

    using NAudio.CoreAudioApi;

    public class MuteAction : SoundActionBase
    {
        public MuteAction()
            : base("Toggle Mute")
        {
        }

        public override void Callback(MMDevice device, string parameter = null)
        {
            try
            {
                device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
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
        }
    }
}