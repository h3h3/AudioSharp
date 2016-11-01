// --------------------------------------------------------------------------------------------------------------------
// <copyright file="KeyModifier.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp.Hotkeys
{
    using System;

    [Flags]
    public enum KeyModifier
    {
        None = 0x0000,

        Alt = 0x0001,

        Ctrl = 0x0002,

        NoRepeat = 0x4000,

        Shift = 0x0004,

        Win = 0x0008
    }
}