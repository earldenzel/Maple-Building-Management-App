using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.DataAccess;
using DataLibrary.Models;

namespace DataLibrary.Logic
{
    public static class AccountProcessor
    {
        public static int CreateAccount(string firstName, string lastName, string emailAddress, string password, bool tenant, string propertyCode)
        {
            AccountModel data = new AccountModel
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                Password = password,
                Tenant = tenant,
                PropertyCode = propertyCode

            };

            string sql = @"INSERT INTO dbo.Account (FirstName, LastName, EmailAddress, Password, Tenant, PropertyCode)
                               VALUES (@FirstName, @LastName, @EmailAddress, @Password, @Tenant, @PropertyCode);";
            return SqlDataAccess.SaveData(sql, data);
        }

        public static List<AccountModel> LoadAccounts()
        {
            string sql = "SELECT Id, FirstName, LastName, EmailAddress, Tenant, PropertyCode from dbo.Account;";
            return SqlDataAccess.LoadData<AccountModel>(sql);
        }

        //public static bool SearchAccount(string emailAddress, string password)
        public static List<AccountModel> SearchAccount(string emailAddress, string password)
        {
            //string sql = "SELECT EmailAddress, Password from dbo.Account;";
            string sql = "SELECT * from dbo.Account WHERE EmailAddress = '" + emailAddress + "' AND Password = '" + password + "';";
            List<AccountModel> model = SqlDataAccess.LoadData<AccountModel>(sql);

            //return (model.Find(m => m.EmailAddress == emailAddress).Password == password);
            //return (model.Count > 0);
            return model;
        }
    }
}
