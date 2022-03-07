// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace SiteOfRefuge.API.Models
{
    /// <summary> Detailed information about a refugee and their family. </summary>
    public partial class Refugee
    {
        /// <summary> Initializes a new instance of Refugee. </summary>
        /// <param name="summary"> A summary of a refugee. </param>
        /// <param name="contact"> Contact information of a person. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="summary"/> or <paramref name="contact"/> is null. </exception>
        public Refugee(RefugeeSummary summary, Contact contact)
        {
            if (summary == null)
            {
                throw new ArgumentNullException(nameof(summary));
            }
            if (contact == null)
            {
                throw new ArgumentNullException(nameof(contact));
            }

            Summary = summary;
            Contact = contact;
        }

        /// <summary> Initializes a new instance of Refugee. </summary>
        /// <param name="id"> Unique identifier in UUID/GUID format. </param>
        /// <param name="summary"> A summary of a refugee. </param>
        /// <param name="contact"> Contact information of a person. </param>
        internal Refugee(Guid id, RefugeeSummary summary, Contact contact)
        {
            Id = id;
            Summary = summary;
            Contact = contact;
        }

        /// <summary> Unique identifier in UUID/GUID format. </summary>
        public Guid Id { get; set; }
        /// <summary> A summary of a refugee. </summary>
        public RefugeeSummary Summary { get; set; }
        /// <summary> Contact information of a person. </summary>
        public Contact Contact { get; set; }
    }
}
