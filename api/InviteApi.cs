// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace SiteOfRefuge.API
{
    public class InviteApi
    {
        private ILogger<InviteApi> _logger;

        /// <summary> Initializes a new instance of InviteApi. </summary>
        /// <param name="logger"> Class logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger"/> is null. </exception>
        public InviteApi(ILogger<InviteApi> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        /// <summary> Lists any current invitation requests for this user. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        [FunctionName("GetInvitesAsync_get")]
        public async Task<IActionResult> GetInvitesAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invite")] HttpRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 200
            // Spec Defines: HTTP 403
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Invite a refugee to connect. </summary>
        /// <param name="body"> The Id to use. </param>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="body"/> is null. </exception>
        [FunctionName("InviteRefugeeAsync_post")]
        public async Task<IActionResult> InviteRefugeeAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "invite")] string body, HttpRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 204
            // Spec Defines: HTTP 403
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Show an invitation. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="id"> Invite id in UUID/GUID format. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="id"/> is null. </exception>
        [FunctionName("GetInviteAsync_get")]
        public async Task<IActionResult> GetInviteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "invite/{id}")] HttpRequest req, string id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 200
            // Spec Defines: HTTP 403
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Accept an invitation to connect. </summary>
        /// <param name="id"> Invite id in UUID/GUID format. </param>
        /// <param name="body"> The Id to use. </param>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="id"/> or <paramref name="body"/> is null. </exception>
        [FunctionName("AcceptInvitationAsync_put")]
        public async Task<IActionResult> AcceptInvitationAsync(string id, [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "invite/{id}")] string body, HttpRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 204
            // Spec Defines: HTTP 403
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Withdraw invitation request. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="id"> Invite id in UUID/GUID format. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="id"/> is null. </exception>
        [FunctionName("DeleteInviteAsync_delete")]
        public async Task<IActionResult> DeleteInviteAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "invite/{id}")] HttpRequest req, string id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 200
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }
    }
}