// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using SiteOfRefuge.API;
using static SiteOfRefuge.API.Shared;

namespace SiteOfRefuge.API.Models
{
    /// <summary> A summary of a refugee. </summary>
    public partial class RefugeeSummary
    {
        public RefugeeSummary() {}

        /// <summary> Initializes a new instance of RefugeeSummary. </summary>
        /// <param name="id"> Unique identifier in UUID/GUID format. </param>
        /// <param name="region"> The region where the refugee is located. </param>
        /// <param name="people"> . </param>
        /// <param name="possession_Date"> Date when shelter is needed by. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="region"/> is null. </exception>
        public RefugeeSummary(Guid id, string region, int people, DateTime possession_Date)
        {
            if (region == null)
            {
                throw new ArgumentNullException(nameof(region));
            }

            Id = id;
            Region = region;
            People = people;
            Restrictions = new List<Restrictions>();
            Languages = new List<SpokenLanguages>();
            Possession_Date = possession_Date;
        }

        /// <summary> Initializes a new instance of RefugeeSummary. </summary>
        /// <param name="id"> Unique identifier in UUID/GUID format. </param>
        /// <param name="region"> The region where the refugee is located. </param>
        /// <param name="people"> . </param>
        /// <param name="message"> A freeform text field that allows for a personalized message. </param>
        /// <param name="restrictions"> Any restrictions that might impact placement. </param>
        /// <param name="languages"> . </param>
        /// <param name="possession_Date"> Date when shelter is needed by. </param>
        internal RefugeeSummary(Guid id, string region, int people, string message, IList<Restrictions> restrictions, IList<SpokenLanguages> languages, DateTime possession_Date)
        {
            Id = id;
            Region = region;
            People = people;
            Message = message;
            Restrictions = restrictions;
            Languages = languages;
            Possession_Date = possession_Date;
        }

        /// <summary> Unique identifier in UUID/GUID format. </summary>
        public Guid Id { get; set; }
        /// <summary> The region where the refugee is located. </summary>
        public string Region { get; set; }
        public int People { get; set; }
        /// <summary> A freeform text field that allows for a personalized message. </summary>
        public string Message { get; set; }
        /// <summary> Any restrictions that might impact placement. </summary>
        public IList<Restrictions> Restrictions { get; set; }
        public IList<SpokenLanguages> Languages { get; set; }
        /// <summary> Date when shelter is needed by. </summary>
        public DateTime Possession_Date { get; set; }
    }
}
