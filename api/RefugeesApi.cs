// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using SiteOfRefuge.API.Models;
using Newtonsoft.Json.Linq;

namespace SiteOfRefuge.API
{
    public class RefugeesApi
    {
        private ILogger<RefugeesApi> _logger;
        private const string SQL_CONNECTION_STRING = "Server=localhost;Database=SiteOfRefugeAPI;Trusted_Connection=True;";

        /// <summary> Initializes a new instance of RefugeesApi. </summary>
        /// <param name="logger"> Class logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger"/> is null. </exception>
        public RefugeesApi(ILogger<RefugeesApi> logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _logger = logger;
        }

        /// <summary> Get a summary list of refugees registered in the system. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        [FunctionName("GetRefugeesAsync_get")]
        public async Task<IActionResult> GetRefugeesAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "refugees")] HttpRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 200

            IEnumerable<RefugeeSummary> refugeeList = new List<RefugeeSummary> {
                new RefugeeSummary( new Guid("3F2504E0-4F89-41D3-9A0C-0305E82C3301"), "PL-06", 1, new DateTimeOffset(DateTime.UtcNow) ),
                new RefugeeSummary( new Guid("2D2503E0-4D89-41C6-2D3E-1263EF2B1829"), "PL-16", 4, new DateTimeOffset(DateTime.UtcNow) )
            };
            
            return new OkObjectResult(refugeeList);
        }


        //have to repeat the parameters 3 times (query, create param, set param value), so helps avoid typos and increases maintainability
        const string PARAM_REFUGEE_ID = "@ID";
        const string PARAM_CONTACTMODE_ID = "@ID";
        const string PARAM_CONTACTMODE_METHOD = "@Method";
        const string PARAM_CONTACTMODE_VALUE = "@Value";
        const string PARAM_CONTACTMODE_VERIFIED = "@Verified";
        const string PARAM_CONTACTTOMETHODS_CONTACTID = "@ContactId";
        const string PARAM_CONTACTTOMETHODS_CONTACTMODEID = "@ContactModeId";
        const string PARAM_CONTACT_ID = "@Id";
        const string PARAM_CONTACT_NAME = "@Name";
        const string PARAM_REFUGEESUMMARY_ID = "@Id";
        const string PARAM_REFUGEESUMMARY_REGION = "@Region";
        const string PARAM_REFUGEESUMMARY_PEOPLE = "@People";
        const string PARAM_REFUGEESUMMARY_MESSAGE = "@Message";
        const string PARAM_REFUGEESUMMARY_POSSESSIONDATE = "@PossessionDate";
        const string PARAM_REFUGEE_SUMMARY = "@Summary";
        const string PARAM_REFUGEE_CONTACT = "@Contact";
        const string PARAM_REFUGEESUMMARYTORESTRICTIONS_REFUGEESUMMARYID = "@RefugeeSummaryId";
        const string PARAM_REFUGEESUMMARYTORESTRICTIONS_RESTRICTIONVALUE = "@RestrictionValue";
        const string PARAM_REFUGEESUMMARYTOLANGUAGES_REFUGEESUMMARYID = "@RefugeeSummaryId";
        const string PARAM_REFUGEESUMMARYTOLANGUAGES_LANGUAGEVALUE = "@LanguageValue";


        /// <summary> Registers a new refugee in the system. </summary>
        /// <param name="body"> The Refugee to use. </param>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="body"/> is null. </exception>
        [FunctionName("AddRefugeeAsync_post")]
        public async Task<IActionResult> AddRefugeeAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "refugees")] Refugee body, HttpRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            if(body == null)
                throw new ArgumentNullException();
            
            //WARNING: trusting Id in body.Id (is this passed in from the request?) -- if we need to use an auth thing will need some code to update this

            using(SqlConnection sql = new SqlConnection(SQL_CONNECTION_STRING))
            {
                sql.Open();
                using(SqlCommand cmd = new SqlCommand("select top 1 * from Refugee where Id = " + PARAM_REFUGEE_ID, sql))
                {
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_ID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_REFUGEE_ID].Value = body.Id;
                    using(SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if(sdr.Read())
                            return new BadRequestObjectResult("Error: trying to create a refugee with Id '" + body.Id.ToString() + "' but a refugee with this Id already exists in the database.");
                    }
                }

                foreach(ContactMode cm in body.Contact.Methods)
                {
                    using(SqlCommand cmd = new SqlCommand("insert into ContactMode(Id, Method, Value, Verified) values(" + PARAM_CONTACTMODE_ID + ", (select top 1 Id from ContactModeMethod where value = " + PARAM_CONTACTMODE_METHOD + "), " + PARAM_CONTACTMODE_VALUE + ", " + PARAM_CONTACTMODE_VERIFIED + ");", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_ID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_CONTACTMODE_ID].Value = cm.Id;
                        cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_METHOD, System.Data.SqlDbType.VarChar));
                        cmd.Parameters[PARAM_CONTACTMODE_METHOD].Value = cm.Method.Value;
                        cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_VALUE, System.Data.SqlDbType.NVarChar));
                        cmd.Parameters[PARAM_CONTACTMODE_VALUE].Value = cm.Value;
                        cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_VERIFIED, System.Data.SqlDbType.Bit));
                        cmd.Parameters[PARAM_CONTACTMODE_VERIFIED].Value = (cm.Verified.HasValue && cm.Verified.Value) ? 1 : 0;
                        cmd.ExecuteNonQuery();
                    }
                }

                using(SqlCommand cmd = new SqlCommand("insert into Contact(Id, Name) values(" + PARAM_CONTACT_ID + ",  " + PARAM_CONTACT_NAME + ");", sql))
                {
                    cmd.Parameters.Add(new SqlParameter(PARAM_CONTACT_ID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_CONTACT_ID].Value = body.Contact.Id;
                    cmd.Parameters.Add(new SqlParameter(PARAM_CONTACT_NAME, System.Data.SqlDbType.NVarChar));
                    cmd.Parameters[PARAM_CONTACT_NAME].Value = body.Contact.Name;
                    cmd.ExecuteNonQuery();
                }

                //now that Contact and ContactMode(s) are inserted, can insert ContactToMethod links
                foreach(ContactMode cm in body.Contact.Methods)
                {
                    using(SqlCommand cmd = new SqlCommand("insert into ContactToMethods(ContactId, ContactModeId) values(" + PARAM_CONTACTTOMETHODS_CONTACTID + ",  " + PARAM_CONTACTTOMETHODS_CONTACTMODEID + ");", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTTOMETHODS_CONTACTID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_CONTACTTOMETHODS_CONTACTID].Value = body.Contact.Id;
                        cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTTOMETHODS_CONTACTMODEID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_CONTACTTOMETHODS_CONTACTMODEID].Value = cm.Id;
                        cmd.ExecuteNonQuery();
                    }
                }

                using(SqlCommand cmd = new SqlCommand("insert into RefugeeSummary(Id, Region, People, Message, PossessionDate) values(" + PARAM_REFUGEESUMMARY_ID + ", " + PARAM_REFUGEESUMMARY_REGION + ", " + PARAM_REFUGEESUMMARY_PEOPLE + ", " + PARAM_REFUGEESUMMARY_MESSAGE + ", " + PARAM_REFUGEESUMMARY_POSSESSIONDATE + ");", sql))
                {
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_ID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_REFUGEESUMMARY_ID].Value = body.Summary.Id;
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_REGION, System.Data.SqlDbType.NVarChar));
                    cmd.Parameters[PARAM_REFUGEESUMMARY_REGION].Value = body.Summary.Region;
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_PEOPLE, System.Data.SqlDbType.Int));
                    cmd.Parameters[PARAM_REFUGEESUMMARY_PEOPLE].Value = body.Summary.People;
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_MESSAGE, System.Data.SqlDbType.NVarChar));
                    cmd.Parameters[PARAM_REFUGEESUMMARY_MESSAGE].Value = body.Summary.Message;
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_POSSESSIONDATE, System.Data.SqlDbType.DateTimeOffset));
                    cmd.Parameters[PARAM_REFUGEESUMMARY_POSSESSIONDATE].Value = body.Summary.PossessionDate;
                    cmd.ExecuteNonQuery();
                }

                using(SqlCommand cmd = new SqlCommand("insert into Refugee(Id, Summary, Contact) values(" + PARAM_REFUGEE_ID + ", " + PARAM_REFUGEE_SUMMARY + ", " + PARAM_REFUGEE_CONTACT + ");", sql))
                {
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_ID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_REFUGEE_ID].Value = body.Id;
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_SUMMARY, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_REFUGEE_SUMMARY].Value = body.Summary.Id;
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_CONTACT, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_REFUGEE_CONTACT].Value = body.Contact.Id;
                    cmd.ExecuteNonQuery();
                }   

                foreach(Restrictions r in body.Summary.Restrictions)
                {
                    using(SqlCommand cmd = new SqlCommand("insert into RefugeeSummaryToRestrictions(RefugeeSummaryId, RestrictionsId) values(" + PARAM_REFUGEESUMMARYTORESTRICTIONS_REFUGEESUMMARYID + ", (select top 1 id from restrictions where value = " + PARAM_REFUGEESUMMARYTORESTRICTIONS_RESTRICTIONVALUE + "));", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARYTORESTRICTIONS_REFUGEESUMMARYID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_REFUGEESUMMARYTORESTRICTIONS_REFUGEESUMMARYID].Value = body.Summary.Id;
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARYTORESTRICTIONS_RESTRICTIONVALUE, System.Data.SqlDbType.NVarChar));
                        cmd.Parameters[PARAM_REFUGEESUMMARYTORESTRICTIONS_RESTRICTIONVALUE].Value = r.Value;
                        cmd.ExecuteNonQuery();
                    }      
                }

                foreach(SpokenLanguages l in body.Summary.Languages)
                {
                    using(SqlCommand cmd = new SqlCommand("insert into RefugeeSummaryToLanguages(RefugeeSummaryId, SpokenLanguagesId) values(" + PARAM_REFUGEESUMMARYTOLANGUAGES_REFUGEESUMMARYID + ", (select top 1 id from spokenlanguages where value = " + PARAM_REFUGEESUMMARYTOLANGUAGES_LANGUAGEVALUE + "));", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARYTOLANGUAGES_REFUGEESUMMARYID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_REFUGEESUMMARYTOLANGUAGES_REFUGEESUMMARYID].Value = body.Summary.Id;
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARYTOLANGUAGES_LANGUAGEVALUE, System.Data.SqlDbType.NVarChar));
                        cmd.Parameters[PARAM_REFUGEESUMMARYTOLANGUAGES_LANGUAGEVALUE].Value = l.Value;
                        cmd.ExecuteNonQuery();
                    }      
                }

                sql.Close();
            }
            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 201
            return new OkObjectResult("Success");
        }

        /// <summary> Get information about a specific refugee. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        [FunctionName("GetRefugeeAsync_get")]
        public async Task<IActionResult> GetRefugeeAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "refugees/{id}")] HttpRequest req, Guid id, CancellationToken cancellationToken = default)
        {
            //WARNING: incomplete and UNTESTED (route issue / likely user error)
            _logger.LogInformation("HTTP trigger function processed a request.");


            using(SqlConnection sql = new SqlConnection(SQL_CONNECTION_STRING))
            {
                sql.Open();
                using(SqlCommand cmd = new SqlCommand("select r.id as Id, rs.id as RefugeeSummaryId, rs.Region as RefugeeSummaryRegion, rs.People as RefugeeSummaryPeople, rs.Message as RefugeeSummaryMessage, rs.PossessionDate as RefugeePossessionDate, c.Id as RefugeeContactId, c.Name as RefugeeContactName from refugee r join refugeesummary rs on r.summary = rs.id join contact c on r.contact = c.id where r.Id = " + PARAM_REFUGEE_ID, sql))
                {
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_ID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_REFUGEE_ID].Value = id;
                    using(SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if(sdr.Read())
                            return new BadRequestObjectResult("Error: trying to create a refugee with Id '" + id.ToString() + "' but a refugee with this Id already exists in the database.");
                        JObject json = new JObject();
                        json["id"] = sdr.GetGuid(0).ToString();

                        //summary portion
                        JObject summary = new JObject();
                        summary["id"] = sdr.GetGuid(1).ToString();
                        summary["region"] = sdr.GetString(2);
                        summary["people"] = sdr.GetInt32(3);
                        summary["message"] = sdr.GetString(4);
                        //restrictions
                        //languages
                        summary["possession_date"] = sdr.GetDateTimeOffset(5);
                        json["summary"] = summary;

                        //contact portion
                        JObject contact = new JObject();
                        contact["id"] = sdr.GetGuid(6).ToString();
                        contact["name"] = sdr.GetString(7);
                        json["contact"] = contact;

                        return new OkObjectResult(json.ToString());
                    }
                }
                sql.Close();
            }


            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 200
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Updates a refugee in the system. </summary>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        /// <param name="body"> The Refugee to use. </param>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="body"/> is null. </exception>
        [FunctionName("UpdateRefugeeAsync_put")]
        public async Task<IActionResult> UpdateRefugeeAsync(Guid id, [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "refugees/{id}")] Refugee body, HttpRequest req, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 204
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }

        /// <summary> Schedules a refugee to be deleted from the system (after 7 days archival). </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        /// <param name="cancellationToken"> The cancellation token provided on Function shutdown. </param>
        [FunctionName("DeleteRefugeeAsync_delete")]
        public async Task<IActionResult> DeleteRefugeeAsync([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "refugees/{id}")] HttpRequest req, Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("HTTP trigger function processed a request.");

            // TODO: Handle Documented Responses.
            // Spec Defines: HTTP 202
            // Spec Defines: HTTP 404

            throw new NotImplementedException();
        }
    }
}
