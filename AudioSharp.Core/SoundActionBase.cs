// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SoundActionBase.cs" company="AudioSharp">
//     Copyright (c) AudioSharp. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace AudioSharp.Core
{
    using System;

    using NAudio.CoreAudioApi;

    using Newtonsoft.Json;

    public abstract class SoundActionBase : ISoundAction, IEquatable<SoundActionBase>
    {
        protected SoundActionBase([NotNull] string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.Name = name;
        }

        [JsonIgnore]
        public string Name { get; }

        public static bool operator ==(SoundActionBase left, SoundActionBase right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(SoundActionBase left, SoundActionBase right)
        {
            return !Equals(left, right);
        }

        public abstract void Callback([NotNull] MMDevice device, [CanBeNull] string parameter = null);

        public bool Equals(SoundActionBase other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = obj as SoundActionBase;
            return (other != null) && this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.Name?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}