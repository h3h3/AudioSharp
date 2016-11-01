// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISoundAction.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace SoundSharp.Core
{
    using System.ComponentModel.Composition;

    using NAudio.CoreAudioApi;

    [InheritedExport(typeof(ISoundAction))]
    public interface ISoundAction
    {
        string Name { get; }

        void Callback([NotNull] MMDevice device, [CanBeNull] string parameter = null);
    }
}