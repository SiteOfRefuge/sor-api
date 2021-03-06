#nullable disable

using System;
using System.Collections.Generic;
using System.Data.Sql;
using System.Data.SqlClient;
using SiteOfRefuge.API.Models;
using System.Net;
using System.Threading.Tasks;

namespace SiteOfRefuge.API
{
    internal class SqlShared
    {
        private static string SQL_CONNECTION_STRING = Environment.GetEnvironmentVariable("KVDBCONN");
    
        const string PARAM_ID = "@Id";
        const string PARAM_SUMMARY_ID = "@SummaryId";
        const string PARAM_REFUGEE_ID = "@RefugeeId";
        const string PARAM_HOST_ID = "@HostId";
        const string PARAM_CONTACTMODE_ID = "@ContactModeId";
        const string PARAM_CONTACTMODE_METHOD = "@Method2";
        const string PARAM_CONTACTMODE_VALUE = "@Value";
        const string PARAM_CONTACTMODE_VERIFIED = "@Verified";
        const string PARAM_CONTACTTOMETHODS_CONTACTID = "@ContactId";
        const string PARAM_CONTACTTOMETHODS_CONTACTMODEID = "@ContactModeId";
        const string PARAM_CONTACT_ID = "@ContactId";
        const string PARAM_CONTACT_FIRSTNAME = "@FirstName";
        const string PARAM_CONTACT_LASTNAME = "@LastName";

        internal static SqlConnection GetSqlConnection()
        {
            return new SqlConnection(SQL_CONNECTION_STRING);
        }

        internal static void InsertContactModes(SqlConnection sql, SqlTransaction transaction, IList<ContactMode> contactModes) 
        {
            foreach(ContactMode cm in contactModes)
            {
                InsertContactMode(sql, transaction, cm);
            }            
        }

        private static void InsertContactMode(SqlConnection sql, SqlTransaction transaction, ContactMode cm)
        {
            using(SqlCommand cmd = new SqlCommand($@"insert into ContactMode(Id, Method, Value, Verified) values(
                {PARAM_CONTACTMODE_ID},
                (select top 1 Id from ContactModeMethod where value = {PARAM_CONTACTMODE_METHOD}),
                {PARAM_CONTACTMODE_VALUE},
                {PARAM_CONTACTMODE_VERIFIED});", sql, transaction))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_CONTACTMODE_ID].Value = cm.Id;
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_METHOD, System.Data.SqlDbType.NVarChar));
                cmd.Parameters[PARAM_CONTACTMODE_METHOD].Value = cm.Method.Value;
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_VALUE, System.Data.SqlDbType.NVarChar));
                cmd.Parameters[PARAM_CONTACTMODE_VALUE].Value = cm.Value;
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTMODE_VERIFIED, System.Data.SqlDbType.Bit));
                cmd.Parameters[PARAM_CONTACTMODE_VERIFIED].Value = (cm.Verified.HasValue && cm.Verified.Value) ? 1 : 0;
                cmd.ExecuteNonQuery();
            }

        }

        private const string PARAM_COLUMN_MATCH_ID = "@MatchColumnId";
        internal static HashSet<Guid> GetListOfIDs(SqlConnection sql, string TABLENAME, string ID_TO_MATCH, string ID_TO_RETURN, Guid matchId)
        {
            HashSet<Guid> ids = new HashSet<Guid>();
            using(SqlCommand cmd = new SqlCommand($@"select distinct {ID_TO_RETURN} from {TABLENAME} where {ID_TO_MATCH} = {PARAM_COLUMN_MATCH_ID}", sql))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_COLUMN_MATCH_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_COLUMN_MATCH_ID].Value = matchId;
                using(SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while(sdr.Read())
                    {
                        Guid id = sdr.GetGuid(0);
                        if(ids.Contains(id))
                            continue;
                        ids.Add(id);
                    }
                }
            }
            return ids;
        }

        internal static void UpdateContactModes(SqlConnection sql, SqlTransaction transaction, IList<ContactMode> contactModes, Guid contactId)
        {
            HashSet<Guid> existingIDs = GetListOfIDs(sql, "ContactToMethods", "ContactId", "ContactModeId", contactId);
            Dictionary<Guid, ContactMode> newIDs = new Dictionary<Guid, ContactMode>();

            foreach(ContactMode cm in contactModes)
            {
                if(cm.Id is null) continue;
                Guid id = cm.Id.Value;
                if(!newIDs.ContainsKey(id))
                    newIDs.Add(id, cm);
                if(existingIDs.Contains(id))
                   ; //update
                else
                    InsertContactMode(sql, transaction, cm);
            }
            foreach(Guid id in existingIDs)
            {
                if(!newIDs.ContainsKey(id))
                    ; //delete
            }
        }

        internal static void InsertContact(SqlConnection sql, SqlTransaction transaction, Contact contact)
        {
            using(SqlCommand cmd = new SqlCommand($"insert into Contact(Id, FirstName, LastName) values({PARAM_CONTACT_ID}, {PARAM_CONTACT_FIRSTNAME}, {PARAM_CONTACT_LASTNAME});", sql, transaction))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACT_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_CONTACT_ID].Value = contact.Id;
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACT_FIRSTNAME, System.Data.SqlDbType.NVarChar));
                cmd.Parameters[PARAM_CONTACT_FIRSTNAME].Value = contact.FirstName;
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACT_LASTNAME, System.Data.SqlDbType.NVarChar));
                cmd.Parameters[PARAM_CONTACT_LASTNAME].Value = contact.LastName;
                cmd.ExecuteNonQuery();
            }
        }

        internal static void InsertContactToMethods(SqlConnection sql, SqlTransaction transaction, IList<ContactMode> contactModes, Guid? contactId)
        {
            foreach(ContactMode cm in contactModes)
            {
                using(SqlCommand cmd = new SqlCommand($@"insert into ContactToMethods(ContactId, ContactModeId)
                    values({PARAM_CONTACTTOMETHODS_CONTACTID},  {PARAM_CONTACTTOMETHODS_CONTACTMODEID});", sql, transaction))
                {
                    cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTTOMETHODS_CONTACTID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_CONTACTTOMETHODS_CONTACTID].Value = contactId;
                    cmd.Parameters.Add(new SqlParameter(PARAM_CONTACTTOMETHODS_CONTACTMODEID, System.Data.SqlDbType.UniqueIdentifier));
                    cmd.Parameters[PARAM_CONTACTTOMETHODS_CONTACTMODEID].Value = cm.Id;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        internal static void InsertCustomer(SqlConnection sql, SqlTransaction transaction, Guid? id, Guid? summaryId, Guid? contactId, string TABLENAME)
        {
            using(SqlCommand cmd = new SqlCommand($"insert into {TABLENAME}(Id, Summary, Contact) values({PARAM_ID}, {PARAM_SUMMARY_ID}, {PARAM_CONTACT_ID});", sql, transaction))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_ID].Value = id;
                cmd.Parameters.Add(new SqlParameter(PARAM_SUMMARY_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_SUMMARY_ID].Value = summaryId;
                cmd.Parameters.Add(new SqlParameter(PARAM_CONTACT_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_CONTACT_ID].Value = contactId;
                cmd.ExecuteNonQuery();
            }   
        }

        const string PARAM_ID_1 = "@Id1";
        const string PARAM_VALUE_2 = "@Value2";

        private static void InsertIDAndValueToIDLookup(SqlConnection sql, SqlTransaction transaction, string COLUMNNAME1, Guid? id1, string COLUMNNAME2, string TABLENAME, string ID_TABLENAME, string value2, string VALUE_COLUMNNAME)
        {
            using(SqlCommand cmd = new SqlCommand($@"insert into {TABLENAME}({COLUMNNAME1}, {COLUMNNAME2}) values(
                {PARAM_ID_1},
                (select top 1 id from {ID_TABLENAME} where {VALUE_COLUMNNAME} = {PARAM_VALUE_2}));", sql, transaction))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_ID_1, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_ID_1].Value = id1;
                cmd.Parameters.Add(new SqlParameter(PARAM_VALUE_2, System.Data.SqlDbType.NVarChar));
                cmd.Parameters[PARAM_VALUE_2].Value = value2;
                cmd.ExecuteNonQuery();
            }      
        }

        private const string PARAM_RESTRICTIONID_COLUMNMAME = "RestrictionsId";
        private const string PARAM_RESTRICTIONS_TABLENAME = "Restrictions";
        private const string PARAM_RESTRICTIONS_VALUECOLUMNNAME = "value";
        internal static void InsertRestrictionsList(SqlConnection sql, SqlTransaction transaction, IList<Restrictions> restrictions, string TABLENAME, string IDCOLUMNNAME, Guid? summaryId)
        {
            foreach(Restrictions restriction in restrictions)
            {
                InsertIDAndValueToIDLookup(sql, transaction, IDCOLUMNNAME, summaryId, PARAM_RESTRICTIONID_COLUMNMAME, TABLENAME, PARAM_RESTRICTIONS_TABLENAME, restriction.Value, PARAM_RESTRICTIONS_VALUECOLUMNNAME);
            }   
        }

        private const string PARAM_LANGUAGEID_COLUMNMAME = "SpokenLanguagesId";
        private const string PARAM_LANGUAGE_TABLENAME = "SpokenLanguages";
        private const string PARAM_LANGUAGE_VALUECOLUMNNAME = "value";
        internal static void InsertLanguageList(SqlConnection sql, SqlTransaction transaction, IList<SpokenLanguages> languages, string TABLENAME, string IDCOLUMNNAME, Guid? summaryId)
        {
            foreach(SpokenLanguages language in languages)
            {
                InsertIDAndValueToIDLookup(sql, transaction, IDCOLUMNNAME, summaryId, PARAM_LANGUAGEID_COLUMNMAME, TABLENAME, PARAM_LANGUAGE_TABLENAME, language.Value, PARAM_LANGUAGE_VALUECOLUMNNAME);
            }   
        }

        internal static void GetContactInfo(SqlConnection sql, Guid id, bool IsRefugee, out string sms, out string email, out string firstname, out string lastname)
        {
            sms = "";
            email = "";
            firstname = "";
            lastname = "";

            string TABLENAME = IsRefugee ? "Refugees" : "Hosts";
            using(SqlCommand cmd = new SqlCommand($@"select top 1 SMSContactValue, EmailContactValue, RefugeeContactFirstName, RefugeeContactLastName  
                from {TABLENAME} where Id = {PARAM_ID}", sql))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_ID].Value = id;

                using(SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while(sdr.Read())
                    {
                        sms = sdr.GetString(0);
                        email = sdr.GetString(1);
                        firstname = sdr.GetString(2);
                        lastname = sdr.GetString(3);
                    }
                }
            }
        }

        internal enum AccountStatus
        {
            Active,
            Archived,
            NotFound
        }

        internal static void UpdateStatusForAccount(SqlConnection sql, Guid id)
        {
            using(SqlCommand cmd = new SqlCommand($"exec UpdateStatus {PARAM_ID};" , sql))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_ID].Value = id;
                cmd.ExecuteNonQuery();
            }
        }

        internal static async Task<AccountStatus> GetAccountStatus(SqlConnection sql, Guid id)
        {
            using(SqlCommand cmd = new SqlCommand($@"select top 1 IsEnabled from (
                    select Id, IsEnabled from Refugee where Id = {PARAM_ID}
                    union
                    select Id, IsEnabled from Host where Id = {PARAM_ID}
                ) x" , sql))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_ID].Value = id;
                using(SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                {
                    if(sdr.Read())
                    {
                        bool isEnabled = sdr.GetBoolean(0);
                        return isEnabled ? AccountStatus.Active : AccountStatus.Archived;
                    }
                    return AccountStatus.NotFound;
                }
            }
        }

        internal static async Task<bool> CanInviteBeSent(SqlConnection sql, Guid HostId, Guid RefugeeId)
        {
            AccountStatus hostStatus = await GetAccountStatus(sql, HostId);
            AccountStatus refugeeStatus = await GetAccountStatus(sql, RefugeeId);
            if(hostStatus == AccountStatus.Archived) return false;
            if(refugeeStatus == AccountStatus.Archived) return false;
            //then check status of current invites...

            using(SqlCommand cmd = new SqlCommand($@"
                select top 1 RefugeeId, HostId from InviteWithHostAndRefugeeSummary
                where 
                    (
                        (RefugeeId = {PARAM_REFUGEE_ID} or HostId = {PARAM_HOST_ID})
                        and InviteStatus in ('Active', 'Completed')
                    )
                    or (RefugeeId = {PARAM_REFUGEE_ID} and HostId = {PARAM_HOST_ID})
                ;", sql))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_HOST_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_HOST_ID].Value = HostId;
                cmd.Parameters.Add(new SqlParameter(PARAM_REFUGEE_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_REFUGEE_ID].Value = RefugeeId;
                using(SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                {
                    if(sdr.Read()) return false; //found a reason not to be able to send an invite (past invite between them, current )
                    return true;
                }
            }
        }


        internal enum AccountType
        {
            Refugee,
            Host,
            NotFound
        }

        internal static async Task<AccountType> GetAccountType(SqlConnection sql, Guid id)
        {
            using(SqlCommand cmd = new SqlCommand($@"select top 1 IsRefugee, IsHost from UserToRefugeeOrHostMapping where id = {PARAM_ID};" , sql))
            {
                cmd.Parameters.Add(new SqlParameter(PARAM_ID, System.Data.SqlDbType.UniqueIdentifier));
                cmd.Parameters[PARAM_ID].Value = id;
                using(SqlDataReader sdr = await cmd.ExecuteReaderAsync())
                {
                    if(sdr.Read())
                    {
                        int isRefugee = sdr.GetInt32(0);
                        int isHost = sdr.GetInt32(1);
                        if(isRefugee == 1 && isHost == 1) throw new Exception($"User is refugee and host: {id}");
                        if(isRefugee == 1) return AccountType.Refugee;
                        if(isHost == 1) return AccountType.Host;
                        return AccountType.NotFound;
                    }
                    return AccountType.NotFound;
                }
            }
        }        
    }
}
