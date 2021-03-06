// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// <auto-generated/>

#nullable disable

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SiteOfRefuge.API.Middleware;
using SiteOfRefuge.API.Models;

namespace SiteOfRefuge.API
{
    public class RefugeesApi
    {
        /// <summary> Initializes a new instance of RefugeesApi. </summary>
        public RefugeesApi() {}

        const string PARAM_REFUGEE_ID = "@Id";
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
        const string PARAM_REFUGEESUMMARYTOLANGUAGES_SUMMARYID = "@SummaryId";
        const string PARAM_REFUGEESUMMARYTORESTRICTIONS_SUMMARYID = "@SummaryId";     

        static JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        /// <summary> Get a summary list of refugees registered in the system. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        [FunctionAuthorize("admin")]
        [Function(nameof(GetRefugees))]
        public HttpResponseData GetRefugees([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "refugees")] HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(GetRefugees));
            logger.LogInformation("HTTP trigger function processed a request.");

            IEnumerable<RefugeeSummary> refugeeList = new List<RefugeeSummary> {
                new RefugeeSummary( Guid.NewGuid(), "PL-06", 1, DateTime.UtcNow ),
                new RefugeeSummary( Guid.NewGuid(), "PL-16", 4, DateTime.UtcNow )
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
        public async Task<HttpResponseData> AddRefugee([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "refugees")] HttpRequestData req, FunctionContext context) 
        {
            var logger = context.GetLogger(nameof(AddRefugee));
            logger.LogInformation("HTTP trigger function processed a request.");

            Refugee refugee = null;
            var response = req.CreateResponse(HttpStatusCode.OK);

            if (req.Body is not null)
            { 
                try
                {
                    var reader = new StreamReader(req.Body);
                    var respBody = await reader.ReadToEndAsync();
                    refugee = Newtonsoft.Json.JsonConvert.DeserializeObject<Refugee>(respBody);
                    //refugee.Summary.Possession_Date = DateTime.Parse(JObject.Parse(respBody)["summary"]["possession_date"].ToString());
                }
                catch(Exception exc)
                {
                    logger.LogInformation($"{context.InvocationId.ToString()} - Error deserializing Refugee object. Err: {exc.Message}");
                    response.StatusCode = HttpStatusCode.BadRequest;
                    return response;
                }
            }

            if(!Shared.ValidateUserIdMatchesToken(context, refugee.Id))
            {
                logger.LogInformation($"{context.InvocationId.ToString()} - Expected refugee Id does not match subject claim when creating a new refugee.");                    
                response.StatusCode = HttpStatusCode.Forbidden;
                return response;
            }
            
            using(SqlConnection sql = SqlShared.GetSqlConnection())
            {
                sql.Open();
                using(SqlCommand cmd = new SqlCommand($"select top 1 * from Refugee where Id = {PARAM_REFUGEE_ID}" , sql))
                {
                    cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_ID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_REFUGEE_ID].Value = refugee.Id;
                    using(SqlDataReader sdr = cmd.ExecuteReader())
                    {
                        if(sdr.Read())
                        {
                            response.StatusCode = HttpStatusCode.BadRequest;
                            await response.WriteStringAsync( $"Error: trying to create a refugee with Id '{refugee.Id.ToString()}' but a refugee with this Id already exists in the database.");
                            return response;
                        }
                    }
                }

                using(SqlTransaction transaction = sql.BeginTransaction())
                {
                    try
                    {
                        SqlShared.InsertContactModes(sql, transaction, refugee.Contact.Methods);
                        SqlShared.InsertContact(sql, transaction, refugee.Contact);
                        SqlShared.InsertContactToMethods(sql, transaction, refugee.Contact.Methods, refugee.Contact.Id);

                        using(SqlCommand cmd = new SqlCommand($@"insert into RefugeeSummary(Id, Region, People, Message, PossessionDate) values(
                            {PARAM_REFUGEESUMMARY_ID}, {PARAM_REFUGEESUMMARY_REGION}, {PARAM_REFUGEESUMMARY_PEOPLE}, {PARAM_REFUGEESUMMARY_MESSAGE}, {PARAM_REFUGEESUMMARY_POSSESSIONDATE});", sql, transaction))
                        {
                            cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_ID, System.Data.SqlDbType.UniqueIdentifier));
                            cmd.Parameters[PARAM_REFUGEESUMMARY_ID].Value = refugee.Summary.Id;
                            cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_REGION, System.Data.SqlDbType.NVarChar));
                            cmd.Parameters[PARAM_REFUGEESUMMARY_REGION].Value = refugee.Summary.Region;
                            cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_PEOPLE, System.Data.SqlDbType.Int));
                            cmd.Parameters[PARAM_REFUGEESUMMARY_PEOPLE].Value = refugee.Summary.People;
                            cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_MESSAGE, System.Data.SqlDbType.NVarChar));
                            cmd.Parameters[PARAM_REFUGEESUMMARY_MESSAGE].Value = refugee.Summary.Message;
                            cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARY_POSSESSIONDATE, System.Data.SqlDbType.SmallDateTime));
                            cmd.Parameters[PARAM_REFUGEESUMMARY_POSSESSIONDATE].Value = refugee.Summary.Possession_Date;
                            cmd.ExecuteNonQuery();
                        }

                        const string TABLE_NAME = "Refugee";
                        SqlShared.InsertCustomer(sql, transaction, refugee.Id, refugee.Summary.Id, refugee.Contact.Id, TABLE_NAME);

                        const string RESTRICTIONS_TABLE_NAME = "RefugeeSummaryToRestrictions";
                        const string RESTRICTIONS_ID_COLUMN_NAME = "RefugeeSummaryId";
                        SqlShared.InsertRestrictionsList(sql, transaction, refugee.Summary.Restrictions, RESTRICTIONS_TABLE_NAME, RESTRICTIONS_ID_COLUMN_NAME, refugee.Summary.Id);

                        const string LANGUAGES_TABLE_NAME = "RefugeeSummaryToLanguages";
                        const string LANGUAGES_ID_COLUMN_NAME = "RefugeeSummaryId";
                        SqlShared.InsertLanguageList(sql, transaction, refugee.Summary.Languages, LANGUAGES_TABLE_NAME, LANGUAGES_ID_COLUMN_NAME, refugee.Summary.Id);
                    }
                    catch (Exception exc)
                    {
                        transaction.Rollback();
                        response.StatusCode = HttpStatusCode.BadRequest;
                        logger.LogInformation($"{context.InvocationId.ToString()} - Error POSTing refugee Err: {exc.ToString()}");
                    
                        await response.WriteStringAsync(exc.Message);
                        return response;
                    }
                    transaction.Commit();
                }
                
                sql.Close();
            }

            response.StatusCode = HttpStatusCode.Created;                    
            return response;
        }

        /// <summary> Get information about a specific refugee. </summary>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        [FunctionAuthorize("subject")]
        [Function(nameof(GetRefugee))]
        public HttpResponseData GetRefugee([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "refugees/{id}")] HttpRequestData req, Guid id, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(GetRefugee));
            logger.LogInformation("HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);            

            try
            {
                using(SqlConnection sql = SqlShared.GetSqlConnection())
                {
                    sql.Open();

                    JObject json = new JObject();
                    Guid? contactId = null;
                    Guid? summaryId = null;

                    using(SqlCommand cmd = new SqlCommand($@"select r.id as Id,
                        rs.id as RefugeeSummaryId,
                        rs.Region as RefugeeSummaryRegion,
                        rs.People as RefugeeSummaryPeople,
                        rs.Message as RefugeeSummaryMessage,
                        rs.PossessionDate as RefugeePossessionDate,
                        c.Id as RefugeeContactId,
                        c.FirstName as RefugeeContactFirstName,
                        c.LastName as RefugeeContactLastName
                        from refugee r
                        join refugeesummary rs on r.summary = rs.id
                        join contact c on r.contact = c.id
                        where r.Id = {PARAM_REFUGEE_ID}", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_ID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_REFUGEE_ID].Value = id;
                        using(SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            if(!sdr.Read())
                            {
                                var resp2 = req.CreateResponse(HttpStatusCode.BadRequest);
                                resp2.WriteStringAsync("Error: no refugee with ID '" + id.ToString() + "'");
                                return resp2;
                                //return new BadRequestObjectResult("Error: no refugee with ID '" + id.ToString() + "'");
                            }
                            json["id"] = sdr.GetGuid(0).ToString();

                            //summary portion
                            JObject summary = new JObject();
                            summaryId = sdr.GetGuid(1);
                            summary["id"] = summaryId.ToString();
                            summary["region"] = sdr.GetString(2);
                            summary["people"] = sdr.GetInt32(3);
                            summary["message"] = sdr.GetString(4);
                            //restrictions
                            //languages
                            summary["possession_date"] = sdr.GetDateTime(5);
                            json["summary"] = summary;

                            //contact portion
                            JObject contact = new JObject();
                            contactId = sdr.GetGuid(6);
                            contact["id"] = contactId.ToString();
                            contact["firstname"] = sdr.GetString(7);
                            contact["lastname"] = sdr.GetString(8);
                            json["contact"] = contact;

                        }
                    }

                    const string PARAM_CONTACTTOMETHODS_CONTACTID = "@ContactId";
                    using(SqlCommand cmd = new SqlCommand($@"select cm.Id,
                        cmm.value as contactmethod,
                        cm.Value,
                        cm.verified
                        from contacttomethods ctm
                        join contactmode cm on ctm.contactmodeid = cm.id
                        join contactmodemethod cmm on cm.method = cmm.id
                        where ctm.contactid = {PARAM_CONTACTTOMETHODS_CONTACTID}", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTTOMETHODS_CONTACTID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_CONTACTTOMETHODS_CONTACTID].Value = contactId;
                        List<JObject> contactMethods = new List<JObject>();
                        using(SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            if(!sdr.Read())
                            {
                                var resp2 = req.CreateResponse(HttpStatusCode.BadRequest);
                                resp2.WriteStringAsync("Error: no contact with ID '" + contactId.ToString() + "'");
                                return resp2;
                                //return new BadRequestObjectResult("Error: no contact with ID '" + contactId.ToString() + "'");
                            }
                            JObject contactMethod = new JObject();
                            contactMethod["id"] = sdr.GetGuid(0).ToString();
                            contactMethod["method"] = sdr.GetString(1);
                            contactMethod["value"] = sdr.GetString(2);
                            bool verified = sdr.GetBoolean(3);
                            contactMethod["verified"] = verified;
                            contactMethods.Add(contactMethod);
                        }
                        json["contact"]["methods"] = JToken.FromObject(contactMethods);
                    }

                    using(SqlCommand cmd = new SqlCommand($@"select sl.value
                        from refugeesummarytolanguages rstl
                        join spokenlanguages sl on rstl.spokenlanguagesid = sl.id
                        where rstl.refugeesummaryid = {PARAM_REFUGEESUMMARYTOLANGUAGES_SUMMARYID}", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARYTOLANGUAGES_SUMMARYID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_REFUGEESUMMARYTOLANGUAGES_SUMMARYID].Value = summaryId;
                        List<string> languages = new List<string>();
                        using(SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            int found = 0;
                            while(sdr.Read())
                            {
                                found++;
                                languages.Add(sdr.GetString(0));
                            }
                            //QUESTION: it's okay not to restrict to a particular language, right? or need 1+?
                            //if(found < 1)
                            //    return new BadRequestObjectResult("Error: no contact with ID '" + contactId.ToString() + "'");
                        }
                        json["summary"]["languages"] = JToken.FromObject(languages);
                    }

                    using(SqlCommand cmd = new SqlCommand($@"select r.value
                        from refugeesummarytorestrictions rstr
                        join Restrictions r on rstr.restrictionsid = r.id
                        where rstr.refugeesummaryid = {PARAM_REFUGEESUMMARYTORESTRICTIONS_SUMMARYID}", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEESUMMARYTORESTRICTIONS_SUMMARYID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_REFUGEESUMMARYTORESTRICTIONS_SUMMARYID].Value = summaryId;
                        List<string> restrictions = new List<string>();
                        using(SqlDataReader sdr = cmd.ExecuteReader())
                        {
                            int found = 0;
                            while(sdr.Read())
                            {
                                found++;
                                restrictions.Add(sdr.GetString(0));
                            }
                            //QUESTION: it's okay not to restrict to a particular language, right? or need 1+?
                            //if(found < 1)
                            //    return new BadRequestObjectResult("Error: no contact with ID '" + contactId.ToString() + "'");
                        }
                        json["summary"]["restrictions"] = JToken.FromObject(restrictions);
                    }

                    sql.Close();

                    response.StatusCode = HttpStatusCode.OK;
                    string j = json.ToString();
                    j = Regex.Unescape(j);
                    response.WriteString(j);
                    return response;
                }
            }
            catch(Exception exc)
            {
                //return new BadRequestObjectResult(exc.ToString()); //TODO: DEBUG, not good for real site
                //return new StatusCodeResult(404);
                response.StatusCode = HttpStatusCode.NotFound;
                logger.LogInformation(exc.ToString());
                return response;
            }
            // Spec Defines: HTTP 200
            // Spec Defines: HTTP 404
        }

        /// <summary> Updates a refugee in the system. </summary>
        /// <param name="id"> Refugee id in UUID/GUID format. </param>
        /// <param name="body"> The Refugee to use. </param>
        /// <param name="req"> Raw HTTP Request. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="body"/> is null. </exception>
        [FunctionAuthorize("subject")]
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
        [FunctionAuthorize("subject")]
        [Function(nameof(DeleteRefugee))]
        public HttpResponseData DeleteRefugee([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "refugees/{id}")] HttpRequestData req, Guid id, FunctionContext context)
        {
            var logger = context.GetLogger(nameof(DeleteRefugee));
            logger.LogInformation("HTTP trigger function processed a request.");

            var response = req.CreateResponse(HttpStatusCode.OK);

            // TODO: Handle Documented Responses.
            try
            {
                logger.LogInformation("Guid: " + id.ToString());
                using(SqlConnection sql = SqlShared.GetSqlConnection())
                {
                    sql.Open();

                    using(SqlCommand cmd = new SqlCommand($@"exec DeleteRefugee @refugeeid = {PARAM_REFUGEE_ID}", sql))
                    {
                        cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_ID, System.Data.SqlDbType.UniqueIdentifier));
                        cmd.Parameters[PARAM_REFUGEE_ID].Value = id;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception exc)
            {
                //return new BadRequestObjectResult(exc.ToString()); //TODO: DEBUG, not good for real site
                logger.LogInformation("Guid: " + id.ToString() + "\r\n" + exc.ToString());
                response.StatusCode = HttpStatusCode.NotFound;
                return response;
            }
            // Spec Defines: HTTP 202
            // Spec Defines: HTTP 404

            response.StatusCode = HttpStatusCode.Accepted;
            return response;
        }
    }
}
