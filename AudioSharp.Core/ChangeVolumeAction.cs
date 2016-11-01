// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChangeVolumeAction.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp.Core
{
    using System;

    using NAudio.CoreAudioApi;

    public class ChangeVolumeAction : SoundActionBase
    {
        public ChangeVolumeAction()
            : base("Change Volume")
        {
        }

        public override void Callback(MMDevice device, string parameter = null)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            var volume = float.Parse(parameter) / 100f;
            device.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
        }
    }
}