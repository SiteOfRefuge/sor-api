// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;

namespace SiteOfRefuge.API
{
    /// <summary> The actual way to contact this person. </summary>
    public readonly partial struct ContactModeMethod : IEquatable<ContactModeMethod>
    {
        private readonly string _value;
        public string Value { get { return this._value; } } //need to read this to query for the ID

        /// <summary> Determines if two <see cref="ContactModeMethod"/> values are the same. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is null. </exception>
        public ContactModeMethod(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private const string PhoneValue = "Phone";
        private const string EmailValue = "Email";
        private const string SMSValue = "SMS";

        /// <summary> Phone. </summary>
        public static ContactModeMethod Phone { get; } = new ContactModeMethod(PhoneValue);
        /// <summary> Email. </summary>
        public static ContactModeMethod Email { get; } = new ContactModeMethod(EmailValue);
        /// <summary> SMS. </summary>
        public static ContactModeMethod SMS { get; } = new ContactModeMethod(SMSValue);
        /// <summary> Determines if two <see cref="ContactModeMethod"/> values are the same. </summary>
        public static bool operator ==(ContactModeMethod left, ContactModeMethod right) => left.Equals(right);
        /// <summary> Determines if two <see cref="ContactModeMethod"/> values are not the same. </summary>
        public static bool operator !=(ContactModeMethod left, ContactModeMethod right) => !left.Equals(right);
        /// <summary> Converts a string to a <see cref="ContactModeMethod"/>. </summary>
        public static implicit operator ContactModeMethod(string value) => new ContactModeMethod(value);

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is ContactModeMethod other && Equals(other);
        /// <inheritdoc />
        public bool Equals(ContactModeMethod other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        /// <inheritdoc />
        public override string ToString() => _value;
    }
}
