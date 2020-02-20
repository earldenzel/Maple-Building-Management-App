using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.DataAccess;
using DataLibrary.Models;

namespace DataLibrary.Logic
{
    public static class ComplaintProcessor
    {
        public static int CreateComplaint(string firstName, string lastName, string emailAddress, bool tenant, string propertyCode)
        {
            ComplaintModel data = new ComplaintModel
            {


            };

            string sql = @"INSERT INTO dbo.Account (FirstName, LastName, EmailAddress, Tenant, PropertyCode)
                               VALUES (@FirstName, @LastName, @EmailAddress, @Tenant, @PropertyCode);";
            return SqlDataAccess.SaveData(sql, data);
        }

        public static List<AccountModel> LoadAccounts()
        {
            string sql = "SELECT Id, FirstName, LastName, EmailAddress, Tenant, PropertyCode from dbo.Account;";
            return SqlDataAccess.LoadData<AccountModel>(sql);
        }
    }
}
