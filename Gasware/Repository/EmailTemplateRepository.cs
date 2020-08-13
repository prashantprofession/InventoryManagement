using Gasware.Database;
using Gasware.Models;
using Gasware.Repository.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace Gasware.Repository
{
    public class EmailTemplateRepository : IEmailTemplateRepository
    {
        private string _username;
        public EmailTemplateRepository(string username)
        {
            _username = username;            
        }
        public EmailTemplateModel Get(int id)
        {
            try
            {
                string sql = "SELECT * FROM emailtemplate where emailtemplateid=" + id.ToString();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                EmailTemplateModel emailTemplateModel = new EmailTemplateModel();
                while (rdr.Read())
                {
                    emailTemplateModel = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (emailTemplateModel.EmailTemplateId == 0)
                {
                    Utilities.ErrorMessage("No email template yet.Atleast one active email template is needed to use it.");
                    return null;
                }                
                return emailTemplateModel;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting email template details");
                return null;
            }
        }

        public EmailTemplateModel GetEmailTemplateByType(string type)
        {
            try
            {
                string sql = "SELECT * FROM emailtemplate where emailtype='" + type + "';";
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;
                EmailTemplateModel emailTemplateModel = new EmailTemplateModel();
                while (rdr.Read())
                {
                    emailTemplateModel = GetModelForRawData(rdr);
                }
                mySqlConnectionModel.MySqlConnection.Close();
                if (emailTemplateModel.EmailTemplateId == 0)
                {                    
                    return null;
                }               
                return emailTemplateModel;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while getting email template details");
                return null;
            }
        }
        public List<EmailTemplateModel> GetEmailTemplates()
        {
            try
            {
                string sql = "SELECT * FROM emailtemplate;";
                List<EmailTemplateModel> emailTemplateModels = new List<EmailTemplateModel>();
                MySqlConnectionModel mySqlConnectionModel = DbQueryExecuter.ReadDatabase(sql);
                MySqlDataReader rdr = mySqlConnectionModel.MySqlDataReader;

                EmailTemplateModel emailTemplateModel = new EmailTemplateModel();
                while (rdr.Read())
                {
                    emailTemplateModel = GetModelForRawData(rdr);
                    emailTemplateModels.Add(emailTemplateModel);
                }
                mySqlConnectionModel.MySqlConnection.Close();

                return emailTemplateModels;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while gettign delivery persons details");
                return null;
            }
        }

        private static EmailTemplateModel GetModelForRawData(MySqlDataReader rdr)
        {
            return new EmailTemplateModel()
            {
                EmailTemplateId = rdr.GetInt32(0),
                EmailId = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1),
                DisplayName = rdr.IsDBNull(2) ? string.Empty : rdr.GetString(2),
                Password = rdr.IsDBNull(3) ? string.Empty : Password.GetDecodedPassword(rdr.GetString(3)),
                Subject = rdr.IsDBNull(4) ? string.Empty : rdr.GetString(4),
                Body = rdr.IsDBNull(5) ? string.Empty : rdr.GetString(5),
                EmailType = rdr.IsDBNull(6) ? string.Empty : rdr.GetString(6)
            };
        }

        public void Update(EmailTemplateModel emailTemplate)
        {
            try
            {
                EmailTemplateModel templateModel = GetEmailTemplateByType(emailTemplate.EmailType);
                if (templateModel != null && 
                    templateModel.EmailTemplateId != emailTemplate.EmailTemplateId)
                {                    
                    return;
                }
                string updateEmailTemplateSql = GetUpdateQueryStatement(emailTemplate);
                DbQueryExecuter.WriteToDatabase(updateEmailTemplateSql);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while updating email template details");
            }
        }

        public int Create(EmailTemplateModel emailTemplate)
        {
            try
            {
                if (GetEmailTemplateByType(emailTemplate.EmailType) != null)
                {
                    //Utilities.ErrorMessage("Duplicate delivery person name.Retry with different name");
                    return 0;
                }
                string _nextemailtemplateIdQuery = "SELECT max(emailtemplateid) from emailtemplate";
                int nextId = StaticIdProvider.GetNextId(_nextemailtemplateIdQuery);                
                string insertEmailTemplateSql = GetInsertQueryStatement(emailTemplate, nextId);
                DbQueryExecuter.WriteToDatabase(insertEmailTemplateSql);
                return nextId;
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while adding email template details");
                return 0;
            }
        }


        private string GetInsertQueryStatement(EmailTemplateModel emailTemplate, int nextId)
        {
            return
                "INSERT INTO emailtemplate (emailtemplateid, emailid, displayname, password," +
                " subject, body, emailtype, createdate, createdby, modifieddate, modifiedby) VALUES(" +
                nextId +
                ",'" + emailTemplate.EmailId + "'," +
                "'" + emailTemplate.DisplayName + "'," +
                "'" + Password.GetEncodedPassword(emailTemplate.Password) + "'," +
                "'" + emailTemplate.Subject + "'," +
                "'" + emailTemplate.Body + "'," +
                "'" + emailTemplate.EmailType + "'," +
                "'" + Utilities.CurrentDBTimeStamp() + "'," +
                "'" + _username + "'," +
                "'" + Utilities.CurrentDBTimeStamp() + "'," +
                "'" + _username + "');";
        }

        private string GetUpdateQueryStatement(EmailTemplateModel emailTemplateModel)
        {
            return
               " UPDATE emailtemplate SET " +
               " emailtemplateid = " + emailTemplateModel.EmailTemplateId +
               ", emailid = '" + emailTemplateModel.EmailId +
               "', displayname = '" + emailTemplateModel.DisplayName +   
               "', password = '" + Password.GetEncodedPassword(emailTemplateModel.Password) +
               "', subject = '" + emailTemplateModel.Subject +
               "', body = '" + emailTemplateModel.Body +
               "', emailtype = '" + emailTemplateModel.EmailType +               
               "', modifieddate = '" +  Utilities.CurrentDBTimeStamp() +
               "', modifiedby = '" + _username +
               "' WHERE emailtemplateid = " + emailTemplateModel.EmailTemplateId.ToString() +
               ";";
        }

        public void Delete(EmailTemplateModel emailTemplateModel)
        {
            try
            {
                string deleteQuery = "delete from emailtemplate where emailtemplateid = " + 
                    emailTemplateModel.EmailTemplateId+ ";";
                DbQueryExecuter.WriteToDatabase(deleteQuery);
            }
            catch (Exception)
            {
                Utilities.ErrorMessage("Error occured while deleting email template");
            }

        }
    }
}
