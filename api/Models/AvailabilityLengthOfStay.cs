// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.ComponentModel;

namespace SiteOfRefuge.API
{
    /// <summary> How long a refugee can stay. </summary>
    public readonly partial struct AvailabilityLengthOfStay : IEquatable<AvailabilityLengthOfStay>
    {
        private readonly string _value;
        public string Value { get { return this._value; } }

        public AvailabilityLengthOfStay() { this._value = null; }

        /// <summary> Determines if two <see cref="AvailabilityLengthOfStay"/> values are the same. </summary>
        /// <exception cref="ArgumentNullException"> <paramref name="value"/> is null. </exception>
        public AvailabilityLengthOfStay(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        private const string AFewDaysUpToAWeekValue = "A few days, up to a week";
        private const string UpToAMonthValue = "Up to a month";
        private const string AtLeast2To4MonthsValue = "At least 2 to 4 months";
        private const string Over4MonthsValue = "Over 4 months";

        /// <summary> A few days, up to a week. </summary>
        public static AvailabilityLengthOfStay AFewDaysUpToAWeek { get; set; } = new AvailabilityLengthOfStay(AFewDaysUpToAWeekValue);
        /// <summary> Up to a month. </summary>
        public static AvailabilityLengthOfStay UpToAMonth { get; set; } = new AvailabilityLengthOfStay(UpToAMonthValue);
        /// <summary> At least 2 to 4 months. </summary>
        public static AvailabilityLengthOfStay AtLeast2To4Months { get; set; } = new AvailabilityLengthOfStay(AtLeast2To4MonthsValue);
        /// <summary> Over 4 months. </summary>
        public static AvailabilityLengthOfStay Over4Months { get; set; } = new AvailabilityLengthOfStay(Over4MonthsValue);
        /// <summary> Determines if two <see cref="AvailabilityLengthOfStay"/> values are the same. </summary>
        public static bool operator ==(AvailabilityLengthOfStay left, AvailabilityLengthOfStay right) => left.Equals(right);
        /// <summary> Determines if two <see cref="AvailabilityLengthOfStay"/> values are not the same. </summary>
        public static bool operator !=(AvailabilityLengthOfStay left, AvailabilityLengthOfStay right) => !left.Equals(right);
        /// <summary> Converts a string to a <see cref="AvailabilityLengthOfStay"/>. </summary>
        public static implicit operator AvailabilityLengthOfStay(string value) => new AvailabilityLengthOfStay(value);

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object obj) => obj is AvailabilityLengthOfStay other && Equals(other);
        /// <inheritdoc />
        public bool Equals(AvailabilityLengthOfStay other) => string.Equals(_value, other._value, StringComparison.InvariantCultureIgnoreCase);

        /// <inheritdoc />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => _value?.GetHashCode() ?? 0;
        /// <inheritdoc />
        public override string ToString() => _value;
    }
}
