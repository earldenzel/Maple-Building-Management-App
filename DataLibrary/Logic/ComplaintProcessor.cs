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
        public static int CreateComplaint(int tenantId, 
                                          int propertyManagerId, 
                                          DateTime incidentDate, 
                                          string details, 
                                          int complaintTypeId, 
                                          int complaintStatusId)
        {
            ComplaintModel data = new ComplaintModel
            {
                TenantId = tenantId,
                PropertyManagerId = propertyManagerId,
                IncidentDate = incidentDate,
                Details = details,
                ComplaintStatusId = complaintStatusId,
                ComplaintTypeId = complaintTypeId
            };

        string sql = @"INSERT INTO dbo.Complaint (TenantId, PropertyManagerId, IncidentDate, Details, ComplaintStatusId, ComplaintTypeId)
                               VALUES (@TenantId, @PropertyManagerId, @IncidentDate, @Details, @ComplaintStatusId, @ComplaintTypeId);";
            return SqlDataAccess.SaveData(sql, data);
        }
        public static int UpdateComplaint(
                                          DateTime incidentDate,
                                          string details,
                                          int complaintTypeId,
                                          int complaintStatusId)
        {
            ComplaintModel data = new ComplaintModel
            {
                IncidentDate = incidentDate,
                Details = details,
                ComplaintStatusId = complaintStatusId,
                ComplaintTypeId = complaintTypeId
            };

            string sql = @"UPDATE dbo.Complaint (IncidentDate, Details, ComplaintStatusId, ComplaintTypeId)
                               VALUES (@IncidentDate, @Details, @ComplaintStatusId, @ComplaintTypeId);";
            return SqlDataAccess.SaveData(sql, data);
        }

        public static List<ComplaintModel> LoadComplaints()
        {
            string sql = "SELECT TenantId, PropertyManagerId, IncidentDate, Details, ComplaintStatusId, ComplaintTypeId, ReportedDate from dbo.Complaint;";
            return SqlDataAccess.LoadData<ComplaintModel>(sql);
        }

        public static List<ComplaintModel> LoadComplaint(int id)
        {
            string sql = "SELECT * from dbo.Complaint WHERE Id = '"+id+"';";
            return SqlDataAccess.LoadData<ComplaintModel>(sql);
        }
    }
}
