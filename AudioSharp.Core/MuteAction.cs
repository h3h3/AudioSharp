// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MuteAction.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp.Core
{
    using NAudio.CoreAudioApi;

    public class MuteAction : SoundActionBase
    {
        public MuteAction()
            : base("Toggle Mute")
        {
        }

        public override void Callback(MMDevice device, string parameter = null)
        {
            device.AudioEndpointVolume.Mute = !device.AudioEndpointVolume.Mute;
        }
    }
}