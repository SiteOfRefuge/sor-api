// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;

namespace SiteOfRefuge.API.Models
{
    /// <summary> Detailed information about a host. </summary>
    public partial class Host
    {
        public Host() {}
        
        /// <summary> Initializes a new instance of Host. </summary>
        /// <param name="summary"> Summary of a Host. </param>
        /// <param name="contact"> Contact information of a person. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="summary"/> or <paramref name="contact"/> is null. </exception>
        public Host(HostSummary summary, Contact contact)
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

        /// <summary> Initializes a new instance of Host. </summary>
        /// <param name="id"> Unique identifier in UUID/GUID format. </param>
        /// <param name="summary"> Summary of a Host. </param>
        /// <param name="contact"> Contact information of a person. </param>
        internal Host(Guid id, HostSummary summary, Contact contact)
        {
            Id = id;
            Summary = summary;
            Contact = contact;
        }

        /// <summary> Unique identifier in UUID/GUID format. </summary>
        public Guid Id { get; set; }
        /// <summary> Summary of a Host. </summary>
        public HostSummary Summary { get; set; }
        /// <summary> Contact information of a person. </summary>
        public Contact Contact { get; set; }
    }
}
