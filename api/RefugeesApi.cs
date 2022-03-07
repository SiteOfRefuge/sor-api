// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using SiteOfRefuge.API.Models;

namespace SiteOfRefuge.API
{
    public class RefugeesApi
    {
        /// <summary> Initializes a new instance of RefugeesApi. </summary>
        public RefugeesApi() {}

        /// <summary> Get a summary list of refugees registered in the system. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        [Function(nameof(GetRefugees))]
        public HttpResponseData GetRefugees([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "refugees")] HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(GetRefugees));
            logger.LogInformation("HTTP trigger function processed a request.");

            IEnumerable<RefugeeSummary> refugeeList = new List<RefugeeSummary> {
                new RefugeeSummary( Guid.NewGuid(), "PL-06", 1, new DateTimeOffset(DateTime.UtcNow) ),
                new RefugeeSummary( Guid.NewGuid(), "PL-16", 4, new DateTimeOffset(DateTime.UtcNow) )
            };

            var okResponse = req.CreateResponse(HttpStatusCode.OK);
            okResponse.WriteAsJsonAsync(refugeeList);
            
            return okResponse;
        }

        /// <summary> Registers a new refugee in the system. </summary>
        /// <param name="body"> The Refugee to use. </param>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="body"/> is null. </exception>
        [Function(nameof(AddRefugee))]
        public HttpResponseData AddRefugee([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "refugees")] Refugee body, HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(AddRefugee));
            logger.LogInformation("HTTP trigger function processed a request.");


            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 201

            throw new NotImplementedException();
        }

        /// <summary> Get information about a specific refugee. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        [Function(nameof(GetRefugee))]
        public HttpResponseData GetRefugee([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "refugees/{id}")] HttpRequestData req, Guid id, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(GetRefugee));
            logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 200
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Updates a refugee in the system. </summary>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        /// <param name="body"> The Refugee to use. </param>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="body"/> is null. </exception>
        [Function(nameof(UpdateRefugee))]
        public HttpResponseData UpdateRefugee(Guid id, [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "refugees/{id}")] Refugee body, HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(UpdateRefugee));
            logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 204
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Schedules a refugee to be deleted from the system (after 7 days archival). </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        [Function(nameof(DeleteRefugee))]
        public HttpResponseData DeleteRefugee([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "refugees/{id}")] HttpRequestData req, Guid id, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(DeleteRefugee));
            logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 202
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }
    }
}
