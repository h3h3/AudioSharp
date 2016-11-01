// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeVolumeAction.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SoundSharp.Core
{
    using System;
    using System.Windows;

    using NAudio.CoreAudioApi;

    public class ChangeVolumeAction : SoundActionBase
    {
        public ChangeVolumeAction()
            : base("Change Volume")
        {
        }

        public override void Callback(MMDevice device, string parameter = null)
        {
            try
            {
                if (parameter == null)
                {
                    throw new ArgumentNullException(nameof(parameter));
                }

                var volume = float.Parse(parameter) / 100f;
                device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
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